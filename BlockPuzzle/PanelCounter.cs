using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzle {
    /// <summary>
    /// Base from which all panel counters inherit.
    /// </summary>
    abstract class PanelCounter {
        static protected int DEFAULT_THREADS;
                
        /// <summary>
        /// Calls a panel counter's Count method and displays the count. Optionally displays timing information.
        /// </summary>
        /// <param name="width">The width of the panel.</param>
        /// <param name="height">The height of the panel.</param>
        /// <param name="verbose">When true, display timing info.</param>
        public void CountPanels (double width, int height, bool verbose) {
            DEFAULT_THREADS = 2 * System.Environment.ProcessorCount;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Reset();
            sw.Start();

            long count = Count(width , height);

            sw.Stop();

            if (verbose) {
                Console.WriteLine("{0,25}: Milliseconds: {1,8}, Count: {2}.", this.GetType().Name, sw.ElapsedMilliseconds, count);
            }
            else {
                Console.WriteLine(count);
            }
        }

        /// <summary>
        /// The name of the algorithm.
        /// </summary>
        /// <returns></returns>
        public abstract string Brief ();

        /// <summary>
        /// Counts the possible arrangements given the width and height as per block puzzle problem statement.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public abstract long Count (double width, int height);
    }
}
