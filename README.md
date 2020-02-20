# TreeBalancer
Takes random tree structure and makes it balanced via find-center algorithm
[Center of tree](https://en.wikipedia.org/wiki/Centered_tree) is the node from which greatest way to all leaves is minimal, or center of diameter (greatest distance between nodes)

Center of tree could be found with 2 (or more, esp. as graph cases) algorithms:
* [2 Bfs searches](https://stackoverflow.com/a/33949846/10396077)
* [shrinking tree](https://stackoverflow.com/a/5056755/10396077)

Here we implement second one - by shrinking tree from leaf nodes

Example tree used in algorithm:
![](https://www.geeksforgeeks.org/wp-content/uploads/centroidDecompo2-660x293.png)

With this example tree algorithm finds node 10 as center
BUT there are 2 centers, 6 and 10, and the next approach would be analysing subtree sizes if 2 centers found and claim final center the one of two which has "heavier" subtree.
