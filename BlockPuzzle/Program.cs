using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPuzzle {
    class Program {
        /* Original Problem Statement:
         * Your niece was given a set of blocks for her birthday, 
         * and she has decided to build a panel using 3”×1” and 4.5”×1" blocks. 
         * For structural integrity, the spaces between the blocks must not 
         * line up in adjacent rows. 
         * 
         * There are 2 ways in which to build a 7.5”×1” panel, 
         * 2 ways to build a 7.5”×2” panel, 
         * 4 ways to build a 12”×3” panel, 
         * and 7958 ways to build a 27”×5” panel. 
         * 
         * How many different ways are there for your niece to build a 48”×10” panel? 
         * The answer will fit in a 64-bit integer. Write a program to calculate the answer.
         * 
         * The program should be non-interactive and run as a single-line command which 
         * takes two command-line arguments, width and height, in that order. 
         * Given any width between 3 and 48 that is a multiple of 0.5, inclusive, and 
         * any height that is an integer between 1 and 10, inclusive, your program should 
         * calculate the number of valid ways there are to build a wall of those dimensions. 
         * Your program’s output should simply be the solution as a number, with no 
         * line-breaks or white spaces.
         * 
         * Your program will be judged on how fast it runs and how clearly the code is 
         * written. We will be running your program as well as reading the source code, 
         * so anything you can do to make this process easier would be appreciated.
         * 
         * Send the source code and let us know the value that your program computes, 
         * your program’s running time, and the kind of machine on which you ran it.
         */

        /* Initial observations:
         * 1.   By scaling block widths to the smallest integral ratio (2/3) of 
         *      the "input" block sizes, the math pops out in an obvious way:
         *      a.  The 3" block becomes a 2" block.
         *      b.  The 4.5" block becomes a 3" block. No more fractional block width!
         *      c.  The 3" minimum panel width becomes 2".
         *      d.  The 48" maximum panel width becomes 32". We can now encode the 
         *          blocks in a row as a 32 bit int:
         *          i.  A 2" block is encoded as 10.
         *          ii. A 3" block is encoded as 100.
         *          iii.Testing adjacent rows can now be done by ANDing them.
         * 2.   Heights are not scaled.
         */

        /* Additional design decisions:
         * I started with the brute force solution in order to better understand the
         * problem space and soon wrote several optimizations... none of which would solve
         * the 48x10 panel in a reasonable amount of time.
         * 
         * The "known good" optimization showed me two things: 1) counting the arrangements
         * in the last row was a simple matter of adding knowngood[lastrow-1].Length and
         * 2) the algorithms thus far were slow because they were duplicating an enormous
         * amount of work.
         * 
         * The "nested loops" solutions started out as a joke to exemplify a bad programming
         * style (each wall height input (1 through 10) requiring a separate, hard-coded, 
         * nested loop).  It turned out, however, that performance was on par with the "known
         * good" algorithm.
         * 
         * I was pondering a modification to the "known good" algorithm which would detect 
         * when it was going to duplicate effort and would defer counting these subtrees and 
         * then assemble all deferred counts after each of the parallel tasks had completed.
         * It was during this effort that my observation about counting the last row started
         * tugging at my brain and led me to count from the leaves back instead of from the 
         * "trunk to the leaves." This was the realization that the "trunk to the leaves"
         * direction is what was causing the duplicated effort. I immediately went in and 
         * coded up the "Leaves" solution and it was able to solve the 48x10 problem in about 
         * 12 milliseconds.
         * 
         * I wanted to preserve this path I took from brute force to "leaves" so I wrote up 
         * this framework/solution that, when given the two inputs called for in the problem 
         * statement, produces the required output by using the "leaves" algorithm. However,
         * when provided with a third, optional command line parameter, this program does two things:
         * 
         * 1. Solves the problem using one or more of the other algorithms by using the third 
         * parameter as a bit-mapped algorithm selector, and
         * 2. Includes timing information in the output.
         * 
         * To make all of this easy, I wrote the abstract PanelCounter class requiring that
         * any algorithm to count panels be written as a concrete implementation of the 
         * PanelCounter class. 
         * 
         * So that I wouldn't have to remember the bitmap, running the program with a single
         * parameter - a question mark - will display the bitmap.
         */

        // MINs and MAXes as originally stated.
        const int MIN_WIDTH = 3;
        const int MAX_WIDTH = 48;
        const int MIN_HEIGHT = 1;
        const int MAX_HEIGHT = 10;

        // PanelCounters must be "registered" in order to be called by their bitmap value.
        // nextID is the bitvalue of the next PanelCounter to be registered.
        static int _nextID = 1;
        static Dictionary<int, PanelCounter> _counters = new Dictionary<int, PanelCounter>();
        
        public static void Main (string[] args) {
            /* Register the various methods.  First one is default if 
             * no method supplied on command line.
             */
            RegisterCounter(new count_Leaves());
            RegisterCounter(new count_BruteForce());
            RegisterCounter(new count_P_BruteForce());
            RegisterCounter(new count_P_KnownGood());
            RegisterCounter(new count_P_KnownGood_ptr());
            RegisterCounter(new count_NestedLoops());
            RegisterCounter(new count_P_NestedLoops());
            RegisterCounter(new count_P_Nested_Compact());
            
            // BlockPuzzle ? produces Usage statement.
            if (args.Length > 0 && args[0].Contains("?")) {
                Console.WriteLine("Usage: BlockPuzzle width height [method]");
                Console.WriteLine("\t{0} <= width <= {1}, width of wall", MIN_WIDTH, MAX_WIDTH);
                Console.WriteLine("\t{0} <= height <= {1}, height of wall", MIN_HEIGHT, MAX_HEIGHT);
                Console.WriteLine("\tmethod, bit-mapped method to use, combine any of the following:");
                foreach (KeyValuePair<int, PanelCounter> kvp in _counters) {
                    Console.WriteLine("\t\t{0,3} - {1}", kvp.Key, kvp.Key == 1 ? kvp.Value.Brief() + " (Default)" : kvp.Value.Brief());
                }
                return;
            }

            /* Problem description states that "output should simply
             * be the solution as a number, with no line-breaks or 
             * white space."  I would normally display a "usage"
             * message on bad input, but the instructions lead me to 
             * believe that the "solution" of 0 (zero) is expected 
             * when inputs do not match the criteria so all "failures"
             * output a zero as the solution.
             */
            if (args.Length < 2) {
                Console.WriteLine(0);
                return;
            }

            // Get width of panel.
            double width = 0;
            double.TryParse(args[0], out width);
            if (width < MIN_WIDTH || width > MAX_WIDTH || (width % 0.5) != 0) {
                Console.WriteLine(0);
                return;
            }

            // Get height of panel.
            int height = 0;
            int.TryParse(args[1], out height);
            if (height < MIN_HEIGHT || height > MAX_HEIGHT) {
                Console.WriteLine(0);
                return;
            }

            // args[2] is a bitmap to specify which algorithms to run.
            // Default method is 1.
            int method = 1;

            // If default is used, do not include extra info in output.
            bool verbose = false;
            if (args.Length > 2) {
                verbose = int.TryParse(args[2], out method);
            }

            // Run each method specified.
            foreach (KeyValuePair<int, PanelCounter> kvp in _counters) {
                if ((method & kvp.Key) == kvp.Key) { 
                    kvp.Value.CountPanels(width, height, verbose); 
                }
            }
        }

        /// <summary>
        /// Add PanelCounter class to collection and give it a bit-mapped ID.
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private static void RegisterCounter (PanelCounter pc) {
            _counters.Add(_nextID, pc);
            _nextID *= 2;
        }
    }
}
    
