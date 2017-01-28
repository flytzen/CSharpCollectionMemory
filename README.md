# CSharpCollectionMemory
Some simple tests to compare a few different ways of storing a large number of strings in C#.  
Can be run in 32-bit or 64-bit to see the impact.  

The strings created will pretty much all be unique in the tests.  
There are some tests with interning where it tries with both unique strings and lots of identical strings. In the latter case, Interning makes a huge difference - but interning is fraught with dangers so must be used very cautiously.
