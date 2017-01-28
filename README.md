# CSharpCollectionMemory
Some simple tests to compare a few different ways of storing a large number of strings in C#.  
Can be run in 32-bit or 64-bit to see the impact.  

The strings created will pretty much all be unique. String interning should have an impact if lots of strings are the same - see the relevant test - though it doesn't seem to make much difference; clearly I'm missing something.
