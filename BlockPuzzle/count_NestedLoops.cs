using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class count_NestedLoops : PanelCounter {
        public override string Brief () {
            return "Nested Loops";
        }

        /// <summary>
        /// I noticed that count_Parallel_Known_Good could be recoded as nested loops.
        /// Just for fun, here it is.
        /// 
        /// Update:  Turns out this ugly code runs really fast.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public override long Count (double width, int height)
        {
            long count = 0;
            PanelRow pr = new PanelRow(width, height);
            uint[] masks = pr.mask;
            int[][] cmasks = pr.cmask;
            int[] lengths = pr.length;

            switch (height) {
                case 1:
                    return masks.Length;

                case 2:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        count += lengths[i0];
                    }
                    return count;

                case 3:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            count += lengths[cmasks[i0][i1]];
                        }
                    }
                    return count;

                case 4:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                count += lengths[cmasks[cmasks[i0][i1]][i2]];
                            }
                        }
                    }
                    return count;

                case 5:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                    count += lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]];
                                }
                            }
                        }
                    }
                    return count;

                case 6:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                    for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                        count += lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]];
                                    }
                                }
                            }
                        }
                    }
                    return count;

                case 7:
                    // For 7 rows, I recoded to save intermediate values just to see if it makes the code easier to read.
                    // Here in the single-threaded algorithm, this seems to provide a substantial boost, but it slows down
                    // the multi-threaded version of this algorithm.
                    int b0 = masks.Length;
                    for (int i0 = 0; i0 < b0; i0++) {
                        int[] c0 = cmasks[i0];
                        int b1 = lengths[i0];
                        for (int i1 = 0; i1 < b1; i1++) {
                            int c1 = c0[i1];
                            int b2 = lengths[c1];
                            for (int i2 = 0; i2 < b2; i2++) {
                                int c2 = cmasks[c1][i2];
                                int b3 = lengths[c2];
                                for (int i3 = 0; i3 < b3; i3++) {
                                    int c3 = cmasks[c2][i3];
                                    int b4 = lengths[c3];
                                    for (int i4 = 0; i4 < b4; i4++) {
                                        int c4 = cmasks[c3][i4];
                                        int b5 = lengths[c4];
                                        for (int i5 = 0; i5 < b5; i5++) {
                                            count += lengths[cmasks[c4][i5]];
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return count;

                case 8:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                    for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                        for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                            for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                count += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return count;

                case 9:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                    for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                        for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                            for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                for (int i7 = 0; i7 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]]; i7++) {
                                                    count += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return count;

                case 10:
                    for (int i0 = 0; i0 < masks.Length; i0++) {
                        for (int i1 = 0; i1 < lengths[i0]; i1++) {
                            for (int i2 = 0; i2 < lengths[cmasks[i0][i1]]; i2++) {
                                for (int i3 = 0; i3 < lengths[cmasks[cmasks[i0][i1]][i2]]; i3++) {
                                    for (int i4 = 0; i4 < lengths[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]]; i4++) {
                                        for (int i5 = 0; i5 < lengths[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]]; i5++) {
                                            for (int i6 = 0; i6 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]]; i6++) {
                                                for (int i7 = 0; i7 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]]; i7++) {
                                                    for (int i8 = 0; i8 < lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]]; i8++) {
                                                        count += lengths[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[cmasks[i0][i1]][i2]][i3]][i4]][i5]][i6]][i7]][i8]];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return count;
            }

            return 0;
        }
    }
}
