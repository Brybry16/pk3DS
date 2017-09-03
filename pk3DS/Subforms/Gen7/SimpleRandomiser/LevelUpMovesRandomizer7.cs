using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using pk3DS.Core.Structures;
using pk3DS.Core;
using pk3DS.Core.Structures.Gen6;

namespace pk3DS.Subforms.Gen7.SimpleRandomiser
{
    class LevelUpMovesRandomizer7
    {
        public LevelUpMovesRandomizer7(byte[][] infiles)
        {
            files = infiles;
            string[] species = Main.Config.getText(TextName.SpeciesNames);
            string[][] AltForms = Main.Config.Personal.getFormList(species, Main.Config.MaxSpeciesID);
            string[] specieslist = Main.Config.Personal.getPersonalEntryList(AltForms, species, Main.Config.MaxSpeciesID, out baseForms, out formVal);
            specieslist[0] = movelist[0] = "";

            string[] sortedspecies = (string[])specieslist.Clone();
            Array.Resize(ref sortedspecies, Main.Config.MaxSpeciesID + 1); Array.Sort(sortedspecies);

            var newlist = new List<WinFormsUtil.cbItem>();
            for (int i = 1; i <= Main.Config.MaxSpeciesID; i++) // add all species
                newlist.Add(new WinFormsUtil.cbItem { Text = sortedspecies[i], Value = Array.IndexOf(specieslist, sortedspecies[i]) });
            for (int i = Main.Config.MaxSpeciesID + 1; i < specieslist.Length; i++) // add all forms
                newlist.Add(new WinFormsUtil.cbItem { Text = specieslist[i], Value = i });

            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = newlist;
            CB_Species.SelectedIndex = 0;
        }

        private readonly string[] movelist = Main.Config.getText(TextName.MoveNames);
        private readonly byte[][] files;
        private int entry = -1;
        private Learnset pkm;

        private readonly ComboBox CB_Species = new ComboBox();
        private readonly int[] baseForms, formVal;

        private int getPkmnLevelUpMovesCount()
        {
            entry = WinFormsUtil.getIndex(CB_Species);
            byte[] input = files[entry];
            if (input.Length <= 4) { files[entry] = BitConverter.GetBytes(-1); return; }
            pkm = new Learnset7(input);
            return pkm.Count;
        }

        public void randomizeLevelUpMoves(Random rnd)
        {
            // ORAS: 10682 moves learned on levelup/birth. 
            // 5593 are STAB. 52.3% are STAB. 
            // Steelix learns the most @ 25 (so many level 1)!
            // Move relearner ingame glitch fixed (52 tested), but keep below 75:
            // https://twitter.com/Drayano60/status/807297858244411397

            int[] firstMoves = { 1, 40, 52, 55, 64, 71, 84, 98, 122, 141 };
            // Pound, Poison Sting, Ember, Water Gun, Peck, Absorb, Thunder Shock, Quick Attack, Lick, Leech Life

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
                int count = getPkmnLevelUpMovesCount() - 1;
                int species = WinFormsUtil.getIndex(CB_Species);

                // Default First Move
                dgv.Rows[0].Cells[0].Value = 1;
                dgv.Rows[0].Cells[1].Value = movelist[firstMoves[rnd.Next(0, firstMoves.Length)]];
                for (int j = 1; j < dgv.Rows.Count - 1; j++)
                {
                    int move = Randomizer.getRandomSpecies(ref randomMoves, ref ctr);

                    while (banned.Contains(move)) /* Invalid */
                        move = Randomizer.getRandomSpecies(ref randomMoves, ref ctr);

                    // Assign Move
                    dgv.Rows[j].Cells[1].Value = movelist[move];
                    // Assign Level
                    if (j >= count)
                    {
                        string level = (dgv.Rows[count - 1].Cells[0].Value ?? 0).ToString();
                        ushort lv;
                        UInt16.TryParse(level, out lv);
                        if (lv > 100) lv = 100;
                        dgv.Rows[j].Cells[0].Value = lv + (j - count) + 1;
                    }
                    if (CHK_Spread.Checked)
                        dgv.Rows[j].Cells[0].Value = ((int)(j * (NUD_Level.Value / (dgv.Rows.Count - 1)))).ToString();
                }
            }
            CB_Species.SelectedIndex = 0;
            WinFormsUtil.Alert("All Pokemon's Level Up Moves have been randomized!");
        }
    }
}
