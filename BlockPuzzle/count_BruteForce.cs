using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzle {
    /// <summary>
    /// Brute force algorithm to count panels. 
    /// </summary>
    class count_BruteForce : PanelCounter {
        public override string Brief () {
            return "Brute Force";
        }

        /// <summary>
        /// Treats the panel as an h digit number in base (masks.Length) and simply 
        /// "counts" through all permutations of h elements of masks[].
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public override long Count (double width, int height) {
            long count = 0;
            PanelRow pr = new PanelRow(width, height);
            uint[] masks = pr.mask;
            int b = masks.Length;
            int[] p = { -1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int i = 0;

            do {
                // Increment digit i.
                p[i]++;

                // If it's less than the base, then we have a new permutation to test.
                if (p[i] < b) {
                    // Compare each row to the following row to test for adjacent gaps.
                    for (int j = 0; j < height - 1; j++) {
                        if (0 != (masks[p[j]] & masks[p[j + 1]])) {
                            // Yeah, gotos are bad, but sometimes they provide the most 
                            // efficient way to code an algorithm without having to 
                            // introduce status variables.
                            i = 0;
                            continue;
                        }
                    }

                    // No gaps, increment our count.
                    count++;
                    i = 0;
                    continue;
                }

                // Otherwise, we have overflowed this digit.  
                // Set this digit to zero and continue with next digit.
                p[i] = 0;
                i++;

                // When i == h, we have overflowed the entire "number" in p[].
                // This means there are no more permutations to test.
            } while (i < height);

            return count;
        }
    }
}
