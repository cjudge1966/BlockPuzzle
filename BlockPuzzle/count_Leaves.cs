using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_Leaves : PanelCounter {
        public override string Brief () {
            return "Leaves";
        }

        /// <summary>
        /// Instead of building a tree that represents all possible arrangements (and
        /// then counting the leaves), start at the leaf level (actually height - 1)
        /// and save the count for each possible mask.  Now step through the "preceeding"
        /// rows of the wall and for each possible mask, accumulate the saved counts for
        /// each compatible masks's "next" row which was already calculated in the previous
        /// iteration.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public override long Count (double width, int height) {
            long count = 0;
            PanelRow pr = new PanelRow(width, height);
            int mCount = pr.mask.Length;
            int[][] cmasks = pr.cmask;
            int[] lengths = pr.length;

            long[,] rowCount = new long[height, mCount];

            // Seed row 1 with the count of known good masks for each mask.
            for (int i = 0; i < mCount; i++) {
                rowCount[1, i] = lengths[i];
            }

            // For the remaining rows:
            for (int row = 2; row < height; row++) {
                // For each possible mask:
                for (int i = 0; i < mCount; i++) {
                    // Accumulate the compatible rows' counts for the previous row.
                    for (int p = 0; p < cmasks[i].Length; p++) {
                        rowCount[row, i] += rowCount[row - 1, cmasks[i][p]];
                    }
                }

            }

            // Now accumulate the counts for each possible mask on the root-level of the tree.
            for (int i = 0; i < mCount; i++) {
                count += rowCount[height - 1, i];
            }

            return count;
        }
    }
}
