using System;

namespace quarantine
{
    class Tree<T>
    {
        public Tree<T> Parent { get; set; }
        public T Data { get; set; }
        public int Index { get; set; }
        public Tree<T>[] Children { get; set; }

        public Tree(Tree<T> parent, int index, T data)
        {
            Parent = parent;
            Index = index;
            Data = data;
            Children = new Tree<T>[4];
        }
    }
}
