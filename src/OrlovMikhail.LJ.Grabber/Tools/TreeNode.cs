using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public sealed class TreeNode<T> : IEnumerable<TreeNode<T>>, IEquatable<TreeNode<T>>
    {
        public T Data { get; set; }
        public TreeNode<T> Parent { get; private set; }
        public ICollection<TreeNode<T>> Children { get { return _children; } }
        private readonly ICollection<TreeNode<T>> _children;

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return _children.Count == 0; }
        }

        public int Level
        {
            get
            {
                if (this.IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }

        public TreeNode(T data, TreeNode<T> parent = null)
        {
            this.Data = data;
            this.Parent = parent;
            this._children = new LinkedList<TreeNode<T>>();
        }

        public TreeNode<T> AddChild(T child)
        {
            TreeNode<T> childNode = new TreeNode<T>(child) { Parent = this };
            this._children.Add(childNode);

            return childNode;
        }

        public override string ToString()
        {
            return Data != null ? Data.ToString() : "[data null]";
        }

        #region iterating

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in this._children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }

        #endregion

        #region equality
        public bool Equals(TreeNode<T> other)
        {
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }
        #endregion
    }
}
