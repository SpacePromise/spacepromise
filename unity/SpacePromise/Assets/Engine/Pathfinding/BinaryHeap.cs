using System;

namespace Assets.Engine.Pathfinding
{
    //  /** Binary heap implementation.
    //* Binary heaps are really fast for ordering nodes in a way that
    //* makes it possible to get the node with the lowest F score.
    //* Also known as a priority queue.
    //*
    //* This has actually been rewritten as a d-ary heap (by default a 4-ary heap)
    //* for performance, but it's the same principle.
    //*
    //* \see http://en.wikipedia.org/wiki/Binary_heap
    //* \see https://en.wikipedia.org/wiki/D-ary_heap
    //*/
    public class BinaryHeap
    {
        /** Number of items in the tree */
        public int NumberOfItems;

        /** The tree will grow by at least this factor every time it is expanded */
        const int GrowthFactor = 4;

        /**
   * Number of children of each node in the tree.
   * Different values have been tested and 4 has been empirically found to perform the best.
   * \see https://en.wikipedia.org/wiki/D-ary_heap
   */
        const int D = 4;

        /** Internal backing array for the tree */
        private Tuple[] binaryHeap;

        private struct Tuple
        {
            public readonly uint F;
            public readonly PathNode node;

            public Tuple(uint f, PathNode node)
            {
                this.F = f;
                this.node = node;
            }
        }

        public BinaryHeap(int numberOfElements)
        {
            binaryHeap = new Tuple[numberOfElements];
            NumberOfItems = 0;
        }

        public void Clear()
        {
            NumberOfItems = 0;
        }

        internal PathNode GetNode(int i)
        {
            return binaryHeap[i].node;
        }

        /** Adds a node to the heap */
        public void Add(PathNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            if (NumberOfItems == binaryHeap.Length)
            {
                int newSize = Math.Max(binaryHeap.Length + 4, binaryHeap.Length * GrowthFactor);
                if (newSize > 1 << 18)
                {
                    throw new Exception(
                        "Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. " +
                        "\nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
                }

                var tmp = new Tuple[newSize];

                for (int i = 0; i < binaryHeap.Length; i++)
                {
                    tmp[i] = binaryHeap[i];
                }

                binaryHeap = tmp;
            }

            var obj = new Tuple(node.FunctionCost, node);
            binaryHeap[NumberOfItems] = obj;

            int bubbleIndex = NumberOfItems;
            uint nodeF = node.FunctionCost;

            while (bubbleIndex != 0)
            {
                int parentIndex = (bubbleIndex - 1) / D;

                if (nodeF < binaryHeap[parentIndex].F || nodeF == binaryHeap[parentIndex].F)
                {
                    binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
                    binaryHeap[parentIndex] = obj;
                    bubbleIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }

            NumberOfItems++;
        }

        /** Returns the node with the lowest F score from the heap */
        public PathNode Remove()
        {
            NumberOfItems--;
            PathNode returnItem = binaryHeap[0].node;

            binaryHeap[0] = binaryHeap[NumberOfItems];

            int swapItem = 0;

            do
            {
                var parent = swapItem;
                var swapF = binaryHeap[swapItem].F;
                var pd = parent * D + 1;

                if (pd + 0 <= NumberOfItems &&
                    (binaryHeap[pd + 0].F < swapF ||
                     binaryHeap[pd + 0].F == swapF))
                {
                    swapF = binaryHeap[pd + 0].F;
                    swapItem = pd + 0;
                }

                if (pd + 1 <= NumberOfItems &&
                    (binaryHeap[pd + 1].F < swapF ||
                     binaryHeap[pd + 1].F == swapF))
                {
                    swapF = binaryHeap[pd + 1].F;
                    swapItem = pd + 1;
                }

                if (pd + 2 <= NumberOfItems &&
                    (binaryHeap[pd + 2].F < swapF ||
                     binaryHeap[pd + 2].F == swapF))
                {
                    swapF = binaryHeap[pd + 2].F;
                    swapItem = pd + 2;
                }

                if (pd + 3 <= NumberOfItems &&
                    (binaryHeap[pd + 3].F < swapF ||
                     binaryHeap[pd + 3].F == swapF))
                {
                    swapItem = pd + 3;
                }

                // One if the parent's children are smaller or equal, swap them
                if (parent != swapItem)
                {
                    var tmpIndex = binaryHeap[parent];
                    binaryHeap[parent] = binaryHeap[swapItem];
                    binaryHeap[swapItem] = tmpIndex;
                }
                else
                {
                    break;
                }
            } while (true);

            return returnItem;
        }
    }
}