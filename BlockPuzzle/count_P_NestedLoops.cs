using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_P_NestedLoops : PanelCounter {
        public override string Brief () {
            return "Nested Loops - Parallel";
        }

        /// <summary>
        /// Because the Nested loops ran so much faster than anticipated,
        /// I recoded it to use parallelism.  
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public override long Count (double width, int height) {
            PanelRow pr = new PanelRow(width, height);
            // Providing a local instance seems to give approx. a 1.11% boost in performance
            // when running the 48 x 8 wall.
            uint[] masks = pr.mask;
            int[][] cmasks = pr.cmask;
            int[] lengths = pr.length;

            int THREAD_COUNT = Math.Min(masks.Length, DEFAULT_THREADS);

            Thread[] t = new Thread[THREAD_COUNT];
            long total = 0;
            int nextMask = -1;

            switch (height) {
                // For less than 4 rows, don't even bother with parallelism.
                case 1:
                    return masks.Length;

                case 2:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        total += lengths[i0];
                    }
                    return total;

                case 3:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            total += lengths[cmasks[i0][i1]];
                        }
                    }
                    return total;

                case 4:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        c += lengths[cmasks[cmasks[i0][i1]][i2]];
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

                case 5:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                            c += lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]];
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

                case 6:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                            for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                                c += lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]];
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

                case 7:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                            for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                                for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                                    c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]];
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

                case 8:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                            for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                                for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                                    for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                        c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]];
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

                case 9:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                            for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                                for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                                    for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                        for (int i7 = 0; i7 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]]; i7++) {
                                                            c += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]];
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

                case 10:
                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i] = new Thread(() => {
                            long c = 0;
                            int i0;

                            while ((i0 = Interlocked.Increment(ref nextMask)) < masks.Length) {
                                for (int i1 = 0; i1 < lengths[i0]; i1++) {
                                    for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                        for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                            for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                                for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                                    for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                        for (int i7 = 0; i7 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]]; i7++) {
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

                            Interlocked.Add(ref total, c);
                        });

                        t[i].Start();
                    }

                    for (int i = 0; i < THREAD_COUNT; i++) {
                        t[i].Join();
                    }
                    return total;
            }

            return 0;
        }
    }
}
