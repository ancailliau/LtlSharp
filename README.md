# LtlSharp

* [Project Source](http://github.com/ancailliau/ltlsharp)

LtlSharp aims at providing model checking feature as a library for easy integration with external tools.

The current implementation does not focus on efficiency (both in time and in space) but rather on providing basic features for proof-of-concept prototypes.

Any contribution is welcome.

## Features
	
* Transformations from LTL to Generalized Buchï Automata
  * GPVW Algorithm (Gerth, R., Peled, D., Vardi, M. Y., & Wolper, P. (1995). Simple on-the-fly automatic verification of linear temporal logic. In International Symposium on Protocol Specification, Testing and Verification. IFIP.)
  * LTL2Buchï Algorithm (Giannakopoulou, D., & Lerda, F. (2001). Efficient translation of LTL formulae into Büchi automata. Research Institute for Advanced Computer Science (RIACS), Technical Report, 1.)
* Transformation from Generalized Buchi Automata to Buchi Automata
* Generation of LTL Monitors (Bauer, A., Leucker, M., & Schallhart, C. (2011). Runtime verification for LTL and TLTL. ACM Transactions on Software Engineering and Methodology (TOSEM), 20(4), 14.)
* Emptiness Checkers
  * (not on-the-fly) Emptiness checker for BA and GBA
  * On-the-fly emptiness checker for BA and GBA

## Requirements

* Mono/.NET >= 4.0

## Licence

    (The MIT License)

    Copyright (c) 2012 UCLouvain

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    'Software'), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
    CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
