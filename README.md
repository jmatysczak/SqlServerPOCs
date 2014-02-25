### Proof of concepts/examples/investigations in SqlServer.

#### SortingAndPagingPerformance
Various implementations of sorting and paging on a non indexed column. I wanted to play around
with different implementations in order to get better performance out of the first couple of pages
of data versus doing a full sort. I also wanted to see if there was a way to get the total row
count.

To make a long story short, I have a grid based screen that can display "a lot" of rows. The user
can filter it down, but by default the screen displays the first "page" of rows according to a sort
on a non indexed column.

Additionally, the user can sort on any column. There are around 60 columns so not all of the
columns can be indexed.

Lucene.NET is already used for full text search. So, a better solution would be to augment the
indexes so the data can be retrieved from Lucene since the columns are stored sorted. I did this
for another screen that had the same use cases, but where the retrieval was much simpler.