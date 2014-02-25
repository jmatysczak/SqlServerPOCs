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

I had high hopes for the UDF. But it looks like there is signifcant overhead just to retrieve the
data. It looks like the best solution is to use top, which appears to have priority queue like
performance characteristics, and then use row_number to get the necessary page.

The UI only shows a fixed number of subsequent pages, so I don't need the total number of rows. I
can just increase the number of rows that top selects, then use row_number to pull off the 20 that
need to be displayed plus 1 row for each subsequent page in order to render links to the next N
pages.