# BlockPuzzle
How many unique walls can be built at a given height and width using two block sizes?

Initial problem statement is in Block Coding Puzzle.pdf.

Program includes several exploratory methods to solve the problem. Methods can be
selected via a third command line option which is a bit-mapped selector into the 
available methods. In other words, before I figured out how to solve it, I had to
write a brute-force solution in order to wrap my head around the problem. As
soon as I realized that the real problem is one of duplicated work, I tried a
few optimizations to speed it up. This exploration of the problem is what allowed
me to jump to what I call the "count the leaves" solution that solves the max-
sized wall (48 x 10) in milliseconds.

As always, I include a lot of comments because I sincerely hope that this can
be instructional for someone.

Running "BlockPuzzle.exe ?" produces usage statement including bat values for the
available methods. Example, "BlockPuzzle.exe 27 5 9" calculates the answer for
a 27 by 5 wall using methods 1 and 8.

Normal usage: run "BlockPuzzle.exe [width] [height]" to calculate answer for a
[width] by [height] wall using the default method 1.

Note that for large wall sizes - e.g. 48 by 10 - method 1 will calculate the answer
within 10 to 30 milliseconds (depending on your machine), the other methods
would require days, months, years, centuries to complete.  You've been warned.


