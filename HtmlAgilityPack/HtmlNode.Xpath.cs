using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace HtmlAgilityPack
{
    public partial class HtmlNode : IHtmlNode
    {

        /// <summary>
        /// Creates a new XPathNavigator object for navigating this HTML node.
        /// </summary>
        /// <returns>An XPathNavigator object. The XPathNavigator is positioned on the node from which the method was called. It is not positioned on the root of the document.</returns>
        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, this);
        }

        /// <summary>
        /// Creates an XPathNavigator using the root of this document.
        /// </summary>
        /// <returns></returns>
        public XPathNavigator CreateRootNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, OwnerDocument.DocumentNode);
        }

        /// <summary>
        /// Selects a list of nodes matching the <see cref="XPath"/> expression.
        /// </summary>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>An <see cref="HtmlNodeCollection"/> containing a collection of nodes matching the <see cref="XPath"/> query, or empty collection if no node matched the XPath expression.</returns>
        /// <remarks>This method nevere returns attributes. If <paramref name="xpath"/> matches attributes returns parent elements instead. To return attributes use <see cref="Select"/>.</remarks>
        public HtmlCollection<HtmlNode> SelectNodes(string xpath)
        {
            var list = new HtmlCollection<HtmlNode>();

            HtmlNodeNavigator nav = new HtmlNodeNavigator(OwnerDocument, this);
            XPathNodeIterator it = nav.Select(xpath);
            while (it.MoveNext())
            {
                HtmlNodeNavigator n = (HtmlNodeNavigator)it.Current;
                list.Add(n.CurrentNode);
            }
            return list;
        }

        /// <summary>
        /// Selects a list of nodes or attributes matching the <see cref="XPath"/> expression.
        /// </summary>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>An <see cref="HtmlNodeCollection"/> containing a collection of nodes matching the <see cref="XPath"/> query, or empty collection if no node matched the XPath expression.</returns>
        public HtmlCollection Select(string xpath)
        {
            var list = new HtmlCollection();

            HtmlNodeNavigator nav = new HtmlNodeNavigator(OwnerDocument, this);
            XPathNodeIterator it = nav.Select(xpath);
            while (it.MoveNext())
            {
                HtmlNodeNavigator n = (HtmlNodeNavigator)it.Current;
                list.Add(n.Current);
            }
            return list;
        }

        /// <summary>
        /// Selects the first node that matches the XPath expression.
        /// </summary>
        /// <param name="xpath">The XPath expression. May not be null.</param>
        /// <returns>The first <see cref="HtmlNode"/> that matches the XPath query or a null reference if no matching node was found.</returns>
        /// <remarks>This method never returns attributes. If <paramref name="xpath"/> matches attribute this method returns parent element instead. To get attributes use <see cref="SelectSingle"/> instead.</remarks>
        public HtmlNode SelectSingleNode(string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }

            HtmlNodeNavigator nav = new HtmlNodeNavigator(OwnerDocument, this);
            XPathNodeIterator it = nav.Select(xpath);
            if (!it.MoveNext())
            {
                return null;
            }

            HtmlNodeNavigator node = (HtmlNodeNavigator)it.Current;
            return node.CurrentNode;
        }

        /// <summary>
        /// Selects the first node or attribute that matches the XPath expression.
        /// </summary>
        /// <param name="xpath">The XPath expression. May not be null.</param>
        /// <returns>The first <see cref="HtmlNode"/> that matches the XPath query or a null reference if no matching node was found.</returns>
        public IHtmlNode SelectSingle(string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }

            HtmlNodeNavigator nav = new HtmlNodeNavigator(OwnerDocument, this);
            XPathNodeIterator it = nav.Select(xpath);
            if (!it.MoveNext())
            {
                return null;
            }

            HtmlNodeNavigator node = (HtmlNodeNavigator)it.Current;
            return node.Current;
        }
    }
}
