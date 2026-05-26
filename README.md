### 3D Navigation space generation using Octrees!

## Introduction
I'm Yannick, a student at Howest following Game Development. For one of the assignments, we had to pick a topic and research/implement it.
The topic I've chosen is **3D Navigation Space Generation Using Octrees**. This repo is entirely dedicated to the research of this topic.

## What are Octrees
To make it easier to understand octrees, I'll first explain the origin. Tree structures! If you've never seen a tree structure before, this is what it looks like.

<img width="300" alt="image" src="https://github.com/user-attachments/assets/338c1538-2858-49bf-939e-9b33e1fc0343" />

As you can see, every tree has **ONE** root node. It is the node where the tree starts. Each node inside a tree can also have any amount of children nodes. You can see in the example that Node A, the root node, has 2 children (node B and node C). Node E and node F are in their turn node C's children. Now who is/are node B's children... ? Yes that's correct, it's node D! There are 2 last important things to know about tree structures. A tree structure has leaf nodes. Leaf nodes are nodes that have no children at all. In our example being nodes D, E and F. The last property is that each node has 1 parent node (except for the root node).

Let's go 1 step closer to octrees. Enter **Quadtrees**!
Now you can probably already imagine what a quadtree looks if I tell you that it is a tree where each node has exactly 4 or no children, but I'll show you anyways.

<img width="300" alt="image" src="https://github.com/user-attachments/assets/6393e4b4-8caa-42f7-b8bc-2ff35acacd64" />

As you can see in this example, each node has exactly 4 children or none at all (=> meaning it's a leaf node).

Now to finally get to the octree! The octree is in definition as easy as a quadtree. It's a tree structure where every node has either no or exactly 8 children.
This is what it looks like

<img width="600" alt="image" src="https://github.com/user-attachments/assets/e33bf40b-75f5-45e3-a38b-0a2405d746dd" />


There you go! All theory done!

## Why would we use them?
"Why would this ever be useful at all?", you might be wondering. Well octrees are used in quite a few different topics. It's used for accelerating rendering, checking collisions, storing geographical data,... . What I currently will be using it for is spatial partitioning. 
Once again, I'll start with quadtrees to make it easier to understand. 
Let's say you're making some kind of algorithm that simulates a big crowd of people moving in a direction without ever making contact (or just flocking). What would you need for this? Let's start with 1 person, his name will be John. He's in a room with 100 people. You want to know the location of every person within a radius of 1 meter from John. We do this so John knows how to continue moving without hitting someone else.

<img width="300" alt="image" src="https://github.com/user-attachments/assets/b82b9697-2def-408e-b481-1f7153bca3ed" />

John is our red point here and we want to know who currently can be found in John's radius. Right now we would have to check every single person in that room and calculate the distance to John to see if they fall within John's radius. Doing this is stupid! Don't! You might be thinking, "I've done this before with 100 people and it worked perfectly fine". I sincerely hope you were not the one having that thought. First of all, this might work if it's the only thing you do. But imagine that there's an entire game running while you do this. It would calculate and compare 100 different distances each frame while the rest still has to work well on 60fps. Unless you want to make the requirements of the game scare off about half its fanbase, you will probably need to do some optimizations there. Luckily this is where spatial partitioning comes in play. Let's start by dividing our room into a couple of grids.

<img width="300" alt="image" src="https://github.com/user-attachments/assets/f603dac2-9162-4b10-a83b-841f44ec2265" />

Now some of you might already have noticed something seeing this image, and you're correct. The radius of John only goes into his current grid and the one next to it. This means we only need to take the people who are in neighboring grids into account. This is already a huge optimization! We went from checking every person in the room, to checking only the ones that are in the same or neighboring grids. 
"I'll just make each grid as small as possible so I have to check even less people!", is what at least someone reading this thought. And you're partially correct! The only issue is that before we start calculating with the people in the neighboring grids, we need to know who the neighboring grids are. If you decide to split up your room into 100 grids, your algorithm will actually be slower than the gridless one. You need to do the neighbor check for 100 grids and then also calculate distances to the people within those neighboring grids. Bad idea! "Well I'll just save each grid's neighbor so I don't have to calculate them every frame.", and this is not a bad idea and for some games will do the job. But right now the problem is that you're using a lot of memory and even still, some grids have no people inside them so it's wasted precision. We want to have a solution where you have as few people per grid but also as few grids as possible. This is where quadtrees come in. Let's start with a simple example!

<img width="300" alt="image" src="https://github.com/user-attachments/assets/007b1abe-144a-40a6-a14f-99069fbcd525" />

In this example, we put 6 players in a simple room. In a quadtree, each internal node has exactly 4 children (or none), so let's divide our room into 4!

<img width="300" alt="image" src="https://github.com/user-attachments/assets/e96e226c-d007-4ff1-aea4-05c56609016e" />

Right now, the grids on the right have 1 player per grid. This is what we want! The left grids on the other hand still have 2 players per grid. Let's subdivide again. Just like before, we divide each of those grids into 4 again.

<img width="300" alt="image" src="https://github.com/user-attachments/assets/73b65c3c-2234-44bf-88a2-4b7802e8b58a" />

Now we've succesfully divided our grid into smaller grids only where needed keeping the empty grids to a minimum! This is the base concept of using quadtrees for spatial partitioning. I'm leaving out some details on purpose like the fact that the players won't stay still and we would need some kind of optimized recalculation, but those are topics for another time. 
To get to the actual topic of this repo, **Octrees**! Now that you know what quadtrees are, it's really easy.
Octrees are just quadtrees but then in 3D!

<img width="300" alt="image" src="https://github.com/user-attachments/assets/6f43ffe9-2126-4a6f-b6b8-eb1a649ad077" />

Instead of checking if there is a player within a square, we check if there's a player within a cube. If there is a player within the cube, we subdivide that cube into 8 equal smaller cubes. It works exactly the same as quadtrees but instead it's a 3D version!




## Sources!
# Images
Image 1:
https://simplealgo.org/ 

Image2: 
https://www.mdpi.com/2076-3417/10/21/7636

Image 3:
https://kaolin.readthedocs.io/en/latest/notes/spc_summary.html


# General knowledge
https://www.numberanalytics.com/blog/octrees-ultimate-spatial-data-structure

https://en.wikipedia.org/wiki/Octree

https://en.wikipedia.org/wiki/Quadtree

https://kaolin.readthedocs.io/en/latest/notes/spc_summary.html

https://www.researchgate.net/publication/309526457_INDOOR_A_PATHFINDING_THROUGH_AN_OCTREE_REPRESENTATION_OF_A_POINT_CLOUD

https://www.academia.edu/54040490/OctoMap_An_Efficient_Probabilistic_3D_Mapping_Framework_Based_on_Octrees

https://tsapps.nist.gov/publication/get_pdf.cfm?pub_id=821308

