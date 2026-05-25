### 3D Navigation space generation using Octrees!

## Introduction
I'm Yannick, a student at Howest following Game Development. For one of the assignments, we had to pick a topic and research/implement it.
The topic I've chosen is **3D Navigation Space Generation Using Octrees**. This repo is entirely dedicated to the research of this topic.

## What are Octrees
To make it easier to understand octrees, I'll first explain the origin. Enter **QuadTrees**! In definition, a quadtree is just a tree-structure where every node as either no or exactly 4 children. Now if you have never heard of a tree structure before, this is how it looks like.
<img width="651" height="550" alt="image" src="https://github.com/user-attachments/assets/338c1538-2858-49bf-939e-9b33e1fc0343" />

As you can see, every tree has **ONE** root node. It is the node where the tree starts. Each node inside a tree can also have any amount of children nodes. You can see in the example that Node A, the root node, has 2 children (node B and node C). Node E and node F are in their turn node C's children. Now who is/are node B's children... ? Yes that's correct, it's node D! There 2 last important thing to know about tree structures. A tree structure has leaf nodes. Leaf nodes are nodes that have no children at all. In our example being nodes D, E and F. And the last property is that each node has 1 parent node (except for the root node).

Now you can probably imagine how a quadtree looks, but I'll show you anyways.
<img width="641" height="389" alt="image" src="https://github.com/user-attachments/assets/6393e4b4-8caa-42f7-b8bc-2ff35acacd64" />



## Sources!

Image 1:
https://simplealgo.org/ 

Image2: 
https://www.mdpi.com/2076-3417/10/21/7636
