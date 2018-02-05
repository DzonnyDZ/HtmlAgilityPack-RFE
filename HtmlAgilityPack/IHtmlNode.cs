using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace HtmlAgilityPack
{
    /// <summary>Common interface for HTML nodes and attributes</summary>
    public interface IHtmlNode : IXPathNavigable
    {
        /// <summary>Gets the line number of this node in the document.</summary>
        int Line { get; }
        /// <summary>Gets the column number of this node in the document.</summary>
        int LinePosition { get; }
        /// <summary>Gets or sets this node's name.</summary>
        string Name { get; set; }
        /// <summary>The original unaltered name of the node</summary>
        string OriginalName { get; }
        /// <summary>Gets the <see cref="HtmlDocument"/> to which this node belongs.</summary>
        HtmlDocument OwnerDocument { get; }
        /// <summary>Gets the parent of this node (for nodes that can have parents).</summary>
        HtmlNode ParentNode { get; }
        /// <summary>Gets the stream position of this node in the document, relative to the start of the document.</summary>
        int StreamPosition { get; }
        /// <summary>Gets a valid XPath string that points to this node</summary>
        string XPath { get; }
        /// <summary>Gets a value indicating whether this node has any child nodes.</summary>
        bool HasChildNodes { get; }
        /// <summary>Gets all the children of the node.</summary>
        HtmlNodeCollection ChildNodes { get; }
        /// <summary>Gets or Sets the object and its content in HTML.</summary>
        string OuterHtml { get; }
        /// <summary>Creates a duplicate of the node.</summary>
        /// <param name="deep">true to recursively clone the subtree under the specified node; false to clone only the node itself.</param>
        /// <returns>The cloned node.</returns>
        IHtmlNode CloneNode(bool deep);
        /// <summary>Gets all Descendant nodes in enumerated list</summary>
        /// <returns>Descendatnt nodes of this node</returns>
        IEnumerable<HtmlNode> Descendants();
        /// <summary>Get all descendant nodes with matching name</summary>
        /// <param name="name">Name of elements to get</param>
        /// <returns>Descendatbnt elements with matching name</returns>
        IEnumerable<HtmlNode> Descendants(string name);
        /// <summary>Gets matching first generation child nodes matching name</summary>
        /// <param name="name">Name to get elements with</param>
        /// <returns>Immediate children of current node with matching name</returns>
        IEnumerable<HtmlNode> Elements(string name);
        /// <summary>Gets value of the node</summary>
        string Value { get; }
    }
}
