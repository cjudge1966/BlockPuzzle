using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_P_Nested_Compact :PanelCounter {
        public override string Brief () {
            return "Nested Loops - Parallel, Compact";
        }

        /// <summary>
        /// Because the Nested loops ran so much faster than anticipated,
        /// I recoded it to use parallelism.  
        /// 
        /// Update: While I knew this would slow it down and make it even
        /// uglier, this algorithm uses a single set of nested loops for all heights.
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

            int THREAD_COUNT = Math.Min(masks.Length, DEFAULT_THREADS);

            Thread[] t = new Thread[THREAD_COUNT];
            long total = 0;
            int nextMask = -1;

            if (1 == height) {
                return masks.Length;
            }

            for (int i = 0; i < THREAD_COUNT; i++) {
                t[i] = new Thread(() => {
                    long c = 0;
                    int i0;

                    while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                        if (2 == height) {
                            c += lengths[i0];
                        }
                        else {
                            for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                if (3 == height) {
                                    c += lengths[cmasks[i0][i1]];
                                }
                                else {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        if (4 == height) {
                                            c += lengths[cmasks[cmasks[i0][i1]][i2]];
                                        }
                                        else {
                                            for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                                if (5 == height) {
                                                    c += lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]];
                                                }
                                                else {
                                                    for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                                        if (6 == height) {
                                                            c += lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]];
                                                        }
                                                        else {
                                                            for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                                                if (7 == height) {
                                                                    c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]];
                                                                }
                                                                else {
                                                                    for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                                        if (8 == height) {
                                                                            c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]];
                                                                        }
                                                                        else {
                                                                            for (int i7 = 0; i7 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]]; i7++) {
                                                                                if (9 == height) {
                                                                                    c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]];
                                                                                }
                                                                                else {
                                                                                    for (int i8 = 0; i8 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]]; i8++) {
                                                                                        c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]][i8]];
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Interlocked.Add(ref total, c);
                });

                t[i].Start();
            }

            for (int i = 0; i < THREAD_COUNT; i++) {
                t[i].Join();
            }

            return total;
        }
    }
}
