using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Utils
{
    public class QuadTreeNode<TData>
    {
        public TData Data;
        public QuadTreeNode<TData>[] Children;
        public bool IsLeaf = true;


        public QuadTreeNode()
        {
        }

        public QuadTreeNode(TData data)
        {
            this.Data = data;
        }


        public void Expand(TData[] data)
        {
            this.IsLeaf = false;
            this.Children = new[]
            {
                new QuadTreeNode<TData>(data[0]), 
                new QuadTreeNode<TData>(data[1]), 
                new QuadTreeNode<TData>(data[2]), 
                new QuadTreeNode<TData>(data[3]), 
            };
        }

        public TData[] Collapse()
        {
            this.IsLeaf = true;
            var childrenData = this.Children.Select(c => c.Data).ToArray();
            this.Children = null;

            return childrenData;
        }
    }
}
