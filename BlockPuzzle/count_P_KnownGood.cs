using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_P_KnownGood : PanelCounter {
        public override string Brief () {
            return "Known Good - Parallel";
        }

        /// <summary>
        /// Multithreaded algorithm that counts panels by dividing masks[] into
        /// sections. Each thread permutes and counts the tree of panels that can 
        /// be created with each member of its section of masks[] as row 0 of the
        /// panel.  
        /// 
        /// Actual permutation of masks is an iterative coding of a recursive 
        /// permutation to avoid the performance costs of recursion.
        /// 
        /// Optimization: masks "pointed" to by row n+1 are limited to those masks 
        /// known to be compatible with the mask on row n.  Therefore, no testing
        /// is performed, panels are simply built (using known good masks) and
        /// then known good masks on last row are counted.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public override long Count (double width, int height)
        {
            PanelRow pr = new PanelRow(width, height);
            uint[] masks = pr.mask;
            int[][] cmasks = pr.cmask;
            int[] lengths = pr.length;

            // Don't use more threads than there are masks.
            int THREAD_COUNT = Math.Min(masks.Length, DEFAULT_THREADS);

            // How many masks will each thread test in row 0.
            //int BLOCK_SIZE = 1 + masks.Length / THREAD_COUNT;
            int hm1 = height - 1;

            Thread[] t = new Thread[THREAD_COUNT];
            long[] count = new long[THREAD_COUNT];
            long total = 0;
            int nextMask = -1;

            for (int i = 0; i < THREAD_COUNT; i++) {
                // Need a thread local instance of the closure on i.
                int localI = i;
                t[i] = new Thread(() => {
                    long c = 0;
                    int[] panel = new int[height];
                    int[] compatIndex = new int[height];
                    //int startMask = localI * BLOCK_SIZE;
                    //int endMask = Math.Min(startMask + BLOCK_SIZE, masks.Length);
                    int thisRow, prevRow;
                    int j;

                    while ((j = Interlocked.Increment(ref nextMask)) < masks.Length) {
                        // If mask[j] has compatible masks.
                        if (0 != lengths[j]) {
                            thisRow = 1;
                            prevRow = 0;

                            panel[prevRow] = j;
                            compatIndex[thisRow] = 0;

                        Top:
                            while (thisRow < hm1) {
                                // Here's our "pointer to a pointer" second level of indirection
                                // that allows us to check build a panel with known compatible masks.
                                panel[thisRow] = cmasks[panel[prevRow]][compatIndex[thisRow]];
                                thisRow++;
                                prevRow++;
                                compatIndex[thisRow] = 0;
                            }

                            // At the last row, just add the number of known good masks.
                            c += lengths[panel[prevRow]];

                            // And "return" from "recursion".
                            while (thisRow > 1) {
                                thisRow--;
                                prevRow--;
                                compatIndex[thisRow]++;
                                if (compatIndex[thisRow] < lengths[panel[prevRow]]) {
                                    goto Top;
                                }
                            }
                        }
                    }

                    // Hoist our local counter to the outer scope.
                    count[localI] = c;
                });

                t[i].Start();
            }

            for (int i = 0; i < THREAD_COUNT; i++) {
                t[i].Join();
                total += count[i];
            }

            return total;
        }
    }
}
