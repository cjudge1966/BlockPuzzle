using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_P_KnownGood_ptr : PanelCounter {
        public override string Brief () {
            return "Known Good - Parallel, Pointers";
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
        /// 
        /// Optimization: actually use pointers.
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

            Thread[] t = new Thread[THREAD_COUNT];
            long[] count = new long[THREAD_COUNT];
            long total = 0;
            int nextMask = -1;

            for (int i = 0; i < THREAD_COUNT; i++) {
                // Need a thread local instance of the closure on i.
                int localI = i;
                t[i] = new Thread(() => {
                    unsafe {
                        long c = 0;

                        int[] panel = new int[height];
                        int[] compatIndex = new int[height];
                        fixed (int* pWall = panel, pIndex = compatIndex) {
                            int j;

                            while ((j = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                // If mask[j] has compatible masks.
                                if (0 != lengths[j]) {
                                    int* pPrevRow = pWall, pThisRow = pWall + 1, pI = pIndex;
                                    int* hm1 = pWall + height - 1;
                                    int* pp1 = pThisRow;

                                    *pPrevRow = j;
                                    *pI = 0;

                                Top:
                                    while (pThisRow < hm1) {
                                        *pThisRow = cmasks[*pPrevRow][*pI];
                                        pThisRow++;
                                        pPrevRow++;
                                        pI++;
                                        *pI = 0;
                                    }

                                    c += lengths[*pPrevRow];

                                    while (pThisRow > pp1) {
                                        pThisRow--;
                                        pPrevRow--;
                                        pI--;
                                        (*pI)++;
                                        if (*pI < lengths[*pPrevRow]) {
                                            goto Top;
                                        }
                                        *pI = 0;
                                    }
                                }
                            }

                        }

                        count[localI] = c;
                    }
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
