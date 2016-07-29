using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class PanelRow {
        /* Original problem uses 3 and 4.5 inch blocks, but the pattern works out the same 
         * as long as the ratio between short and long blocks is the same.  Therefore,
         * scale the blocks so that the sizes used are 2 and 3 inches - this makes mapping
         * the blocks to a pattern of 0s and 1s easier so that we can test for compatible
         * blocks by ANDing two rows together.  So a two-inch block is "mapped" as "10" and
         * a three-inch block is mapped as "100".  Because the "edge" (represented by the "1")
         * is on the left side, we should not map the left-most block (because the 1s on the 
         * left edge would always align and signal a false mismatch between rows).
         * 
         * So, a row consisting of a 3-inch, a 3-inch and a 2-inch block would be mapped as:
         *   10010
         * And a row consisting of a 2-inch, a 2-inch, a 2-inch and a 2-inch block would be:
         *  101010
         * ANDing these together results in "10" (non-zero) indicating that the rows, even
         * though they both total to 8 inches, are not compatible.
         */

        // Represent ratio as inverse (3/2) to avoid the repeating decimal in 2/3.
        const double SCALE_RATIO = 1.5;

        // The scaled width of the 3 inch block.
        const int SIZE_SHORT = (int) (3 / SCALE_RATIO);

        // The scaled width of the 4.5 inch block.
        const int SIZE_LONG = (int) (4.5 / SCALE_RATIO);
        
        public int width;
        public int height;
        public uint[] mask;
        
        /// <summary>
        /// Compatible mask: mask[x] is compatible with all mask[cmask[x][]].
        /// </summary>
        public int[][] cmask;

        /// <summary>
        /// Really just a cached lookup: length[x] == cmasks[x].Length.
        ///  - should do a timing test to see if the caching provides a boost.
        /// </summary>
        public int[] length;

        /// <summary>
        /// On instantiation, automatically get:
        ///     mask    the pattern masks that identify valid rows (that add up to the requested width).
        ///     cmask   compatible masks - cmask[0][] lists the indexes to the masks that can follow mask[0]
        ///     lengths lengths[x] = cmask[x].Length = the number of masks compatible with mask[x]
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PanelRow (double width, int height) {
            this.width = (int) (width / SCALE_RATIO);
            this.height = height;
            
            mask = getMasks(this.width);
            cmask = getCompatibleMasks(mask);
            length = getLengths(cmask);
        }

        // Got this from http://graphics.stanford.edu/~seander/bithacks.html#ZerosOnRightMultLookup
        // It is used to count trailing zeros (because .NET does not expose intrinsic functions).
        private int[] MultiplyDeBruijnBitPosition = {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };

        /// <summary>
        /// Translates a permutation of short and long blocks encoded in map
        /// (set bit = a short block, clear bit = a long block) into a
        /// mask in which a set bit represents the gap between blocks.  E.g.:
        ///     0111001 -> 10101010010010
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private uint getMask (uint map, int maxBits, int width) {
            uint bit = 1;
            uint mask = 0;

            // Transfer the "gaps" of map to a mask.  Don't code the edge as a gap.
            int count = 1;
            while (count < maxBits) {
                count++;
                bit <<= (2 - (int) (map & 1));
                mask |= bit;
                bit <<= 1;
                map >>= 1;
            }

            return mask;
        }

        /// <summary>
        /// Counts trailing zero bits.
        /// </summary>
        /// <returns>
        /// The number of trailing zero bits.
        /// </returns>
        /// <param name='i'></param>
        private int bitScanForward (uint i) {
            // Neither .NET nor Mono expose the intrinsic _BitScanForward 
            // (__builtin_ctz() under *nix), so I'll use another one I found at
            // http://graphics.stanford.edu/~seander/bithacks.html#ZerosOnRightMultLookup
            return MultiplyDeBruijnBitPosition[((uint) ((i & -i) * 0x077CB531U)) >> 27];
        }

        /// <summary>
        /// Given a permutation of n set bits, returns the next value lexicographical 
        /// permutation of n set bits.  Got this from 
        /// http://graphics.stanford.edu/~seander/bithacks.html#NextBitPermutation.
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        private uint nextPerm (uint perm) {
            // t gets v's least significant 0 bits set to 1
            uint t = perm | (perm - 1);

            // Next set to 1 the most significant bit to change, 
            // set to 0 the least significant ones, and add the necessary 1 bits.
            return (t + 1) | (uint) (((~t & -~t) - 1) >> (bitScanForward(perm) + 1));
        }


        /// <summary>
        /// Return masks representing the "gaps" for all permutations of 2 and 
        /// 3 inch blocks which total to width.
        /// </summary>
        /// <returns>
        /// The masks.
        /// </returns>
        /// <param name='panelWidth'>
        /// Width.
        /// </param>
        private uint[] getMasks (int panelWidth) {
            List<uint> masks = new List<uint>();
            int maxLong = panelWidth / SIZE_LONG;

            /* Find all combinations of short and long blocks that sum to panelWidth.
             * Because long blocks are odd width and short blocks are even width:
             * An odd panelWidth must include odd number (1, 3, 5, ...) of long blocks.
             * An even panelWidth must include even number (0, 2, 4, ...) of long blocks.
             * The loop counter, therefore, starts at panelWidth % 2.
             */
            for (int countLong = panelWidth % 2; countLong <= maxLong; countLong += 2) {
                /* If we use countLong long blocks, the remaining width of the panel - that
                 * must be made up of short blocks - is widthShort.
                 */
                int widthShort = panelWidth - (countLong * SIZE_LONG);

                // We can only make a valid row if widthShort is evenly divisible by SIZE_SHORT.
                if (0 == widthShort % SIZE_SHORT) {
                    // countShort is the number of short blocks for the current countLong.
                    int countShort = widthShort / SIZE_SHORT;
                    int totalBlocks = countShort + countLong;

                    /* We will now permute countShort 1s to find all patterns
                     * of countShort 1s that are less than 2^totalBlocks.
                     */
                    uint maxPerm = (uint) Math.Pow(2, totalBlocks);

                    // Set map to the initial permutation of countShort 1s -
                    // e.g. if countShort is 3, set perm to 0000...0000111.
                    uint map = (uint) Math.Pow(2, countShort) - 1;

                    // While the permutation is within range:
                    while (map < maxPerm) {
                        //Console.WriteLine("{0,16} -> {1,32}", Convert.ToString(map, 2).PadLeft(16, '0'), Convert.ToString(getMask(map, totalBlocks, panelWidth), 2).PadLeft(32, '0'));
                        
                        // Get the mask and add it to our collection.
                        masks.Add(getMask(map, totalBlocks, panelWidth));
                        
                        // Get the next permutation.
                        map = nextPerm(map);
                    }
                }
            }

            return masks.ToArray(); ;
        }

        /// <summary>
        /// Returns array of arrays containing indexes of the masks that are compatible 
        /// with each mask of the input array.
        /// </summary>
        /// <param name="masks"></param>
        /// <returns></returns>
        static int[][] getCompatibleMasks (uint[] masks) {
            int[][] cmasks = new int[masks.Length][];

            for (int i = 0; i < masks.Length; i++) {
                List<int> c = new List<int>();
                for (int j = 0; j < masks.Length; j++) {
                    if (0 == (masks[i] & masks[j])) {
                        c.Add(j);
                    }
                }

                cmasks[i] = c.ToArray();
            }

            return cmasks;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmasks"></param>
        /// <returns></returns>
        static int[] getLengths (int[][] cmasks) {
            int[] l = new int[cmasks.Length];
            for (int i = 0; i < cmasks.Length; i++) {
                l[i] = cmasks[i].Length;
            }

            return l;
        }

    }
}
