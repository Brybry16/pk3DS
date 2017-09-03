using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pk3DS.Properties;
using pk3DS.Core.Structures;
using pk3DS.Core;
using pk3DS.Core.Structures.Gen6;

namespace pk3DS.Subforms.Gen7
{
    public partial class Randomizer7 : Form
    {
        public Randomizer7()
        {
            
        }

        

        private void Randomizer7_Load(object sender, EventArgs e)
        {

        }

        private void B_Randomize_Click(object sender, EventArgs e)
        {
            //Randomizer
            Random rnd = new Random();

            randomizeEggMoves(rnd);
        }

        private int pokemonLevelUpMovesCount()
        {
            entry = WinFormsUtil.getIndex(CB_Species);

            byte[] input = files[entry];
            if (input.Length <= 4) { files[entry] = BitConverter.GetBytes(-1); return; }
            pkm = new Learnset7(input);


        }

        private void randomizeLevelUpMoves(Random rnd)
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
                int count = dgv.Rows.Count - 1;
                int species = WinFormsUtil.getIndex(CB_Species);
                if (CHK_Expand.Checked && (int)NUD_Moves.Value > count)
                    dgv.Rows.AddCopies(count, (int)NUD_Moves.Value - count);

                // Default First Move
                dgv.Rows[0].Cells[0].Value = 1;
                dgv.Rows[0].Cells[1].Value = movelist[firstMoves[rnd.Next(0, firstMoves.Length)]];
                for (int j = 1; j < dgv.Rows.Count - 1; j++)
                {
                    // Assign New Moves
                    bool forceSTAB = CHK_STAB.Checked && rnd.Next(0, 99) < NUD_STAB.Value;
                    int move = Randomizer.getRandomSpecies(ref randomMoves, ref ctr);

                    while (banned.Contains(move) /* Invalid */
                        || forceSTAB && !Main.SpeciesStat[species].Types.Contains(moveTypes[move].Type)) // STAB is required
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
