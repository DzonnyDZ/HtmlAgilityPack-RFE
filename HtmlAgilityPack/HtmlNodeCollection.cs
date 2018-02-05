// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace HtmlAgilityPack
{
    /// <summary>Represents a combined list and collection of HTML nodes of specific type.</summary>
    /// <typeparam name="T">Type of nodes in collection</typeparam>
    /// <remarks>This class is intended mainly only as common base class for <see cref="HtmlCollection"/> and <see cref="HtmlNodeCollection"/> and is not intended for direct use.</remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public class HtmlCollection<T> : IList<T> where T : class, IHtmlNode
    {
        #region Fields

        private readonly List<T> _items = new List<T>();

        #endregion

        #region Properties

        /// <summary>Gets a given node or attribute from the list.</summary>
        public int this[T node]
        {
            get
            {
                int index = GetNodeIndex(node);
                if (index == -1)
                    throw new ArgumentOutOfRangeException("node",
                                                          "Node \"" + node.CloneNode(false).OuterHtml +
                                                          "\" was not found in the collection");
                return index;
            }
        }

        /// <summary>Gets node or attribute with given name</summary>
        /// <param name="nodeName">Name of node or attribute to get</param>
        /// <returns>First node or attribute in collection with matching name, null if no such node can be found</returns>
        public T this[string nodeName]
        {
            get
            {
                nodeName = nodeName.ToLower();
                for (int i = 0; i < _items.Count; i++)
                    if (_items[i].Name.Equals(nodeName))
                        return _items[i];

                return null;
            }
        }

        #endregion

        #region IList<IHtmlNode> Members

        /// <summary>
        /// Gets the number of elements and attributes actually contained in the list.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Is collection read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the node or attribute at the specified index.
        /// </summary>
        public T this[int index]
        {
            get { return _items[index]; }
            set { Replace(index, value); }
        }

        /// <summary>
        /// Add node or attribute to the collection
        /// </summary>
        /// <param name="node">A node or attribute to be added</param>
        public void Add(T node)
        {
            Append(node);
        }

        /// <summary>
        /// Clears out the collection of nodes and attributes.
        /// </summary>
        public virtual void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Gets existence of node or attribute in collection
        /// </summary>
        /// <param name="item">A node or attribute to test</param>
        /// <returns>True if <paramref name="item"/> is present in the collection, false otherwie</returns>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copy collection to array
        /// </summary>
        /// <param name="array">An array to copy collection to</param>
        /// <param name="arrayIndex">Index in <paramref name="array"/> to assign first item to. Subsequent items are assigned to subsequent indexes.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Type-safe enumerator that iterates over all nodes and attributes in this collection</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Get Explicit Enumerator
        /// </summary>
        /// <returns>Type-unsafe enumerator that iterates over all nodes and attributes in this collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Get index of node or attribute
        /// </summary>
        /// <param name="item">A node or attribute to get index of</param>
        /// <returns>Index of firts occurence of <paramref name="item"/> in this collection. -1 if <paramref name="item"/> is not present in this collection.</returns>
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Insert node or attribute at index
        /// </summary>
        /// <param name="index">Index to insert <paramref name="item"/> at</param>
        /// <param name="item">Node or attribute to be inserted</param>
        public virtual void Insert(int index, T item)
        {
            _items.Insert(index, item);
        }

        /// <summary>
        /// Removes first occurence of node or attribute from this collection
        /// </summary>
        /// <param name="item">Node or attribute to be removed from this collection</param>
        /// <returns>True if <paramref name="item"/> was present in the collection and was removed. False if <paramref name="item"/> was not present.</returns>
        public bool Remove(T item)
        {
            int i = _items.IndexOf(item);
            if (i < 0) return false;
            RemoveAt(i);
            return true;
        }

        /// <summary>
        /// Remove node or attribute at index
        /// </summary>
        /// <param name="index">Index to remove item at</param>
        public virtual void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get first instance of node or attribute in supplied collection
        /// </summary>
        /// <param name="items">The collection fo find first node in</param>
        /// <param name="name">Name of the node to find</param>
        /// <returns>First node or attribute with matching name</returns>
        public static IHtmlNode FindFirst(HtmlCollection<T> items, string name)
        {
            foreach (T node in items)
            {
                if (node.Name.ToLower().Contains(name))
                    return node;
                if (!node.HasChildNodes) continue;
                IHtmlNode returnNode = HtmlNodeCollection.FindFirst(node.ChildNodes, name);
                if (returnNode != null)
                    return returnNode;
            }
            return null;
        }

        /// <summary>
        /// Adds node or attribute to the end of the collection
        /// </summary>
        /// <param name="node">A node or attribute to be added</param>
        public virtual void Append(T node)
        {
            _items.Add(node);
        }

        /// <summary>
        /// Get first instance of node with name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHtmlNode FindFirst(string name)
        {
            return FindFirst(this, name);
        }

        /// <summary>
        /// Get index of node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int GetNodeIndex(T node)
        {
            // TODO: should we rewrite this? what would be the key of a node?
            for (int i = 0; i < _items.Count; i++)
                if (node == _items[i])
                    return i;
            return -1;
        }

        /// <summary>
        /// Adds node or attribute to the beginning of the collection
        /// </summary>
        /// <param name="node">A node or attribute to be added</param>
        public virtual void Prepend(T node)
        {
            _items.Insert(0, node);
        }

        /// <summary>
        /// Remove node or attribute at index
        /// </summary>
        /// <param name="index">Index to remove item at</param>
        /// <returns>True</returns>
        public bool Remove(int index)
        {
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Replace node or attribute at index
        /// </summary>
        /// <param name="index">Index to remove item at</param>
        /// <param name="node">An item to be removed</param>
        public virtual void Replace(int index, T node)
        {
            _items[index] = node;
        }

        #endregion

        #region LINQ Methods

        /// <summary>
        /// Get all node descended from this collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlNode> Descendants()
        {
            foreach (T item in _items)
                foreach (HtmlNode n in item.Descendants())
                    yield return n;
        }

        /// <summary>
        /// Get all node descended from this collection with matching name
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlNode> Descendants(string name)
        {
            foreach (T item in _items)
                foreach (HtmlNode n in item.Descendants(name))
                    yield return n;
        }

        /// <summary>
        /// Gets all first generation elements in collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlNode> Elements()
        {
            foreach (T item in _items)
                foreach (HtmlNode n in item.ChildNodes)
                    yield return n;
        }

        /// <summary>
        /// Gets all first generation elements matching name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> Elements(string name)
        {
            foreach (T item in _items)
                foreach (HtmlNode n in item.Elements(name))
                    yield return n;
        }

        /// <summary>
        /// All first generation nodes in collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlNode> Nodes()
        {
            foreach (T item in _items)
                foreach (HtmlNode n in item.ChildNodes)
                    yield return n;
        }

        #endregion
    }

    /// <summary>Represents a combined list and collection of HTML nodes and attributes</summary>
    public class HtmlCollection : HtmlCollection<IHtmlNode>
    {
        /// <summary>
        /// Initialize the <see cref="HtmlCollection"/> with the base parent node
        /// </summary>
        /// <param name="parentnode">The base node of the collection</param>
        public HtmlCollection() { }
    }

    /// <summary>Represents a combined list and collection of HTML nodes.</summary>
    public class HtmlNodeCollection : HtmlCollection<HtmlNode>
    {
        #region Fields

        private readonly HtmlNode _parentnode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the <see cref="HtmlCollection"/> with the base parent node
        /// </summary>
        /// <param name="parentnode">The base node of the collection</param>
        public HtmlNodeCollection(HtmlNode parentnode)
        {
            _parentnode = parentnode;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get first instance of node in supplied collection
        /// </summary>
        /// <param name="items"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HtmlNode FindFirst(HtmlNodeCollection items, string name)
        {
            foreach (HtmlNode node in items)
            {
                if (node.Name.ToLower().Contains(name))
                    return node;
                if (!node.HasChildNodes) continue;
                HtmlNode returnNode = FindFirst(node.ChildNodes, name);
                if (returnNode != null)
                    return returnNode;
            }
            return null;
        }

        /// <summary>
        /// Get first instance of node with name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HtmlNode FindFirst(string name)
        {
            return FindFirst(this, name);
        }

        /// <summary>
        /// Clears out the collection of HtmlNodes. Removes each nodes reference to parentnode, nextnode and prevnode
        /// </summary>
        public override void Clear()
        {
            foreach (HtmlNode node in this)
            {
                node.ParentNode = null;
                node.NextSibling = null;
                node.PreviousSibling = null;
            }
            base.Clear();
            if (_parentnode != null) _parentnode.SetChanged();
        }

        /// <summary>Inserts a node at index</summary>
        /// <param name="index">Index to insert <paramref name="node"/> at</param>
        /// <param name="node">A node to be inserted</param>
        public override void Insert(int index, HtmlNode node)
        {
            HtmlNode next = null;
            HtmlNode prev = null;

            if (index > 0)
                prev = this[index - 1];

            if (index < this.Count)
                next = this[index];

            base.Insert(index, node);

            if (prev != null)
            {
                if (node == prev)
                    throw new InvalidProgramException("Unexpected error.");

                prev._nextnode = node;
            }

            if (next != null)
                next._prevnode = node;

            node._prevnode = prev;
            if (next == node)
                throw new InvalidProgramException("Unexpected error.");

            node._nextnode = next;
            if (_parentnode != null)
                node._parentnode = _parentnode;
            if (_parentnode != null) _parentnode.SetChanged();
        }

        /// <summary>Remove <see cref="HtmlNode"/> at index</summary>
        /// <param name="index">Index to remove item at</param>
        public override void RemoveAt(int index)
        {
            HtmlNode next = null;
            HtmlNode prev = null;
            HtmlNode oldnode = this[index];

            if (index > 0)
                prev = this[index - 1];

            if (index < (this.Count - 1))
                next = this[index + 1];

            base.RemoveAt(index);

            if (prev != null)
            {
                if (next == prev)
                    throw new InvalidProgramException("Unexpected error.");
                prev._nextnode = next;
            }

            if (next != null)
                next._prevnode = prev;

            oldnode._prevnode = null;
            oldnode._nextnode = null;
            oldnode._parentnode = null;
            if (_parentnode != null) _parentnode.SetChanged();
        }

        /// <summary>
        /// Adds node to the end of the collection
        /// </summary>
        /// <param name="node">A node to be added</param>
        public override void Append(HtmlNode node)
        {

            HtmlNode last = null;
            if (this.Count > 0)
                last = this[this.Count - 1];

            base.Append(node);
            node._prevnode = last;
            node._nextnode = null;
            if (_parentnode != null)
                node._parentnode = _parentnode;
            if (last == node)
                throw new InvalidProgramException("Unexpected error.");

            if (last != null)
                last._nextnode = node;
            if (_parentnode != null) _parentnode.SetChanged();
        }

        /// <summary>Adds node to the beginning of the collection</summary>
        /// <param name="node">A node to be added</param>
        public override void Prepend(HtmlNode node)
        {
            HtmlNode first = null;
            if (this.Count > 0)
                first = this[0];

            base.Prepend(node);

            if (node == first)
                throw new InvalidProgramException("Unexpected error.");
            node._nextnode = first;
            node._prevnode = null;
            if (_parentnode != null)
                node._parentnode = _parentnode;

            if (first != null)
                first._prevnode = node;
            if (_parentnode != null) _parentnode.SetChanged();
        }

        public override void Replace(int index, HtmlNode node)
        {
            HtmlNode next = null;
            HtmlNode prev = null;
            HtmlNode oldnode = this[index];

            if (index > 0)
                prev = this[index - 1];

            if (index < (this.Count - 1))
                next = this[index + 1];

            base.Replace(index, node);

            if (prev != null)
            {
                if (node == prev)
                    throw new InvalidProgramException("Unexpected error.");
                prev._nextnode = node;
            }

            if (next != null)
                next._prevnode = node;

            node._prevnode = prev;

            if (next == node)
                throw new InvalidProgramException("Unexpected error.");

            node._nextnode = next;
            if (_parentnode != null)
                node._parentnode = _parentnode;

            oldnode._prevnode = null;
            oldnode._nextnode = null;
            oldnode._parentnode = null;
            if (_parentnode != null) _parentnode.SetChanged();
        }

        #endregion
    }
}