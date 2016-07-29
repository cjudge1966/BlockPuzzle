using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_P_BruteForce : PanelCounter {
        public override string Brief () {
            return "Brute Force - Parallel";
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
        /// While this algorithm does add parallelism, it is still mostly brute force 
        /// because it does not yet limit the subsequent row to those masks known to 
        /// be compatible with the current row. 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public override long Count (double width, int height)
        {
            PanelRow pr = new PanelRow(width, height);
            uint[] masks = pr.mask;

            // Don't use more threads than there are masks.
            int THREAD_COUNT = Math.Min(masks.Count(), DEFAULT_THREADS);

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
                    int hm1 = height - 1;
                    int row;

                    // Test all masks in this thread's range.
                    int j;
                    while ((j = Interlocked.Increment(ref nextMask)) < masks.Count()) {
                        // Set row 0 to "point" to mask[j].
                        panel[0] = j;

                        // Set next row to "point" to mask[0].
                        row = 1;
                        panel[row] = 0;

                    Top:    // Entry point for our "recursion."
                        while (row < hm1) {
                            // While current row's current mask is not compatible with the previous row's mask,
                            while (0 != (masks[panel[row - 1]] & masks[panel[row]])) {
                                // "point" current row to next mask.
                                panel[row]++;

                                // Continue until we exhaust masks.
                                if (panel[row] < masks.Count()) {
                                    continue;
                                }

                                // This is the "return" from our iterative recursion.
                                goto UpOneRow;
                            }

                            // When we've found a compatible mask for this row, "recurse" to next row.
                            row++;
                            panel[row] = 0;
                        }

                        // At the last row, just count the compatible masks.
                        for (int k = 0; k < masks.Count(); k++) {
                            if (0 == (masks[panel[row - 1]] & masks[k])) {
                                c++;
                            }
                        }

                    UpOneRow:   // And "return" from recursion.
                        while (row > 1) {
                            // Return to previous row and
                            row--;
                            // point to next mask.
                            panel[row]++;
                            if (panel[row] < masks.Count()) {
                                goto Top;
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
