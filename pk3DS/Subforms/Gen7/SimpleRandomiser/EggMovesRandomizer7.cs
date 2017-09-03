using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using pk3DS.Core.Structures;
using pk3DS.Core;
using pk3DS.Core.Structures.Gen6;

namespace pk3DS.Subforms.Gen7.SimpleRandomiser
{
    class EggMovesRandomizer7
    {
        public EggMovesRandomizer7(byte[][] infiles)
        {
            files = infiles;
            string[] species = Main.Config.getText(TextName.SpeciesNames);
            string[][] AltForms = Main.Config.Personal.getFormList(species, Main.Config.MaxSpeciesID);
            string[] specieslist = Main.Config.Personal.getPersonalEntryList(AltForms, species, Main.Config.MaxSpeciesID, out baseForms, out formVal);
            specieslist[0] = movelist[0] = "";

            var newlist = new List<WinFormsUtil.cbItem>();
            for (int i = 0; i < species.Length; i++) // add all species & forms
                newlist.Add(new WinFormsUtil.cbItem { Text = species[i] + $" ({i})", Value = i });
            newlist = newlist.OrderBy(item => item.Text).ToList();
            for (int i = species.Length; i < files.Length; i++)
                newlist.Add(new WinFormsUtil.cbItem { Text = $"{i.ToString("0000")} - Extra", Value = i });

            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = newlist;


            CB_Species.SelectedIndex = 0;
        }

        private readonly string[] movelist = Main.Config.getText(TextName.MoveNames);
        private readonly byte[][] files;
        private int entry = -1;
        private EggMoves pkm = new EggMoves7(new byte[0]);

        private readonly ComboBox CB_Species = new ComboBox();
        private readonly int[] baseForms, formVal;

        private int getPkmnEggMovesCount()
        {
            entry = WinFormsUtil.getIndex(CB_Species);
            byte[] input = files[entry];
            pkm = new EggMoves7(input);
            if (pkm.Count < 1) { files[entry] = new byte[0]; return 0; }
            return pkm.Count;
        }

        public void randomizeEggMoves(Random rnd)
        {
            /*
             * 3111 Egg Moves Learned by 290 Species (10.73 avg)
             * 18 is the most
             * 1000 moves learned were STAB (32.1%)
             */

            int[] banned = new[] { 165, 621, 464 }.Concat(Legal.Z_Moves).ToArray(); // Struggle, Hyperspace Fury, Dark Void

            // Move Stats
            Move[] moveTypes = Main.Config.Moves;

            // Set up Randomized Moves
            int[] randomMoves = Enumerable.Range(1, movelist.Length - 1).Select(i => i).ToArray();
            Util.Shuffle(randomMoves);
            int ctr = 0;

            for (int i = 0; i < CB_Species.Items.Count; i++)
            {
                CB_Species.SelectedIndex = i; // Get new Species
                int count = getPkmnEggMovesCount() - 1;
                int species = WinFormsUtil.getIndex(CB_Species);
                List<int> moves = new List<int>();

                for (int j = 1; j < count; j++)
                {
                    // Assign New Moves
                    int move = Randomizer.getRandomSpecies(ref randomMoves, ref ctr);

                    while (banned.Contains(move))/* Invalid */
                        move = Randomizer.getRandomSpecies(ref randomMoves, ref ctr);

                    // Assign Move
                    if (move > 0 && !moves.Contains((ushort)move)) moves.Add(move);
                }

                pkm.Moves = moves.ToArray();

                files[entry] = pkm.Write();
            }
            CB_Species.SelectedIndex = 0;
            WinFormsUtil.Alert("All Pokemon's Egg Up Moves have been randomized!");
        }
    }
}
