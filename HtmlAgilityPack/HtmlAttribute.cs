// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

#endregion
// ReSharper disable InconsistentNaming

namespace HtmlAgilityPack
{
    /// <summary>
    /// Represents an HTML attribute.
    /// </summary>
    [DebuggerDisplay("Name: {OriginalName}, Value: {Value}")]
    public class HtmlAttribute : IComparable, IHtmlNode
    {
        #region Fields

        private int _line;
        internal int _lineposition;
        internal string _name;
        internal int _namelength;
        internal int _namestartindex;
        internal HtmlDocument _ownerdocument; // attribute can exists without a node
        internal HtmlNode _ownernode;
        private AttributeValueQuote _quoteType = AttributeValueQuote.DoubleQuote;
        internal int _streamposition;
        internal string _value;
        internal int _valuelength;
        internal int _valuestartindex;

        #endregion

        #region Constructors

        internal HtmlAttribute(HtmlDocument ownerdocument)
        {
            _ownerdocument = ownerdocument;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the line number of this attribute in the document.
        /// </summary>
        public int Line
        {
            get { return _line; }
            internal set { _line = value; }
        }

        /// <summary>
        /// Gets the column number of this attribute in the document.
        /// </summary>
        public int LinePosition
        {
            get { return _lineposition; }
        }

        /// <summary>
        /// Gets the qualified name of the attribute.
        /// </summary>
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _ownerdocument.Text.Substring(_namestartindex, _namelength);
                }
                return _name.ToLower();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _name = value;
                if (_ownernode != null)
                {
                    _ownernode.SetChanged();
                }
            }
        }

        /// <summary>
        /// Name of attribute with original case
        /// </summary>
        public string OriginalName
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the HTML document to which this attribute belongs.
        /// </summary>
        public HtmlDocument OwnerDocument
        {
            get { return _ownerdocument; }
        }

        /// <summary>
        /// Gets the HTML node to which this attribute belongs.
        /// </summary>
        public HtmlNode OwnerNode
        {
            get { return _ownernode; }
        }

        /// <summary>
        /// Specifies what type of quote the data should be wrapped in
        /// </summary>
        public AttributeValueQuote QuoteType
        {
            get { return _quoteType; }
            set { _quoteType = value; }
        }

        /// <summary>
        /// Gets the stream position of this attribute in the document, relative to the start of the document.
        /// </summary>
        public int StreamPosition
        {
            get { return _streamposition; }
        }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        public string Value
        {
            get
            {
                if (_value == null)
                {
                    if (_ownerdocument.Text == null)
                    {
                        _value = "";
                    }
                    else
                    {
                        _value = HtmlEntity.DeEntitize(_ownerdocument.Text.Substring(_valuestartindex, _valuelength));
                    }
                }
                return _value;
            }
            set
            {
                _value = value;
                if (_ownernode != null)
                {
                    _ownernode.SetChanged();
                }
            }
        }

        internal string XmlName
        {
            get { return HtmlDocument.GetXmlName(Name); }
        }

        internal string XmlValue
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets a valid XPath string that points to this Attribute
        /// </summary>
        public string XPath
        {
            get
            {
                string basePath = (OwnerNode == null) ? "/" : OwnerNode.XPath + "/";
                return basePath + GetRelativeXpath();
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another attribute. Comparison is based on attributes' name.
        /// </summary>
        /// <param name="obj">An attribute to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the names comparison.</returns>
        public int CompareTo(object obj)
        {
            HtmlAttribute att = obj as HtmlAttribute;
            if (att == null)
            {
                throw new ArgumentException("obj");
            }
            return Name.CompareTo(att.Name);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a duplicate of this attribute.
        /// </summary>
        /// <returns>The cloned attribute.</returns>
        public HtmlAttribute Clone()
        {
            HtmlAttribute att = new HtmlAttribute(_ownerdocument);
            att.Name = Name;
            att.Value = Value;
            return att;
        }

        /// <summary>
        /// Removes this attribute from it's parents collection
        /// </summary>
        public void Remove()
        {
            _ownernode.Attributes.Remove(this);
        }

        /// <summary>Writes this attribute to a stream</summary>
        public void WriteTo(TextWriter outText)
        {
            string name;
            string quote = this.QuoteType == AttributeValueQuote.DoubleQuote ? "\"" : "'";
            if (_ownerdocument.OptionOutputAsXml)
            {
                name = _ownerdocument.OptionOutputUpperCase ? this.XmlName.ToUpper() : this.XmlName;
                if (_ownerdocument.OptionOutputOriginalCase)
                    name = this.OriginalName;

                outText.Write(" " + name + "=" + quote + HtmlDocument.HtmlEncode(this.XmlValue) + quote);
            }
            else
            {
                name = _ownerdocument.OptionOutputUpperCase ? this.Name.ToUpper() : this.Name;
                if (_ownerdocument.OptionOutputOriginalCase)
                    name = this.OriginalName;
                if (this.Name.Length >= 4)
                {
                    if ((this.Name[0] == '<') && (this.Name[1] == '%') &&
                        (this.Name[this.Name.Length - 1] == '>') && (this.Name[this.Name.Length - 2] == '%'))
                    {
                        outText.Write(" " + name);
                        return;
                    }
                }
                if (_ownerdocument.OptionOutputOptimizeAttributeValues)
                    if (this.Value.IndexOfAny(new char[] { (char)10, (char)13, (char)9, ' ' }) < 0)
                        outText.Write(" " + name + "=" + this.Value);
                    else
                        outText.Write(" " + name + "=" + quote + this.Value + quote);
                else
                    outText.Write(" " + name + "=" + quote + this.Value + quote);
            }
        }

        public System.Xml.XPath.XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, this);
        }
        #endregion

        #region Private Methods

        private string GetRelativeXpath()
        {
            if (OwnerNode == null)
                return Name;

            int i = 1;
            foreach (HtmlAttribute node in OwnerNode.Attributes)
            {
                if (node.Name != Name) continue;

                if (node == this)
                    break;

                i++;
            }
            return "@" + Name + "[" + i + "]";
        }

        #endregion

        #region IHtmlNode members

        IEnumerable<HtmlNode> IHtmlNode.Elements(string name) { return HtmlNode.emptyNodes; }
        IEnumerable<HtmlNode> IHtmlNode.Descendants(string name) { return HtmlNode.emptyNodes; }
        IEnumerable<HtmlNode> IHtmlNode.Descendants() { return HtmlNode.emptyNodes; }
        IHtmlNode IHtmlNode.CloneNode(bool deep)
        {
            return new HtmlAttribute(OwnerDocument) { _name = _name, _ownerdocument = _ownerdocument, _quoteType = _quoteType, _value = _value };
        }
        string IHtmlNode.OuterHtml
        {
            get
            {
                StringBuilder b = new StringBuilder();
                using (StringWriter w = new StringWriter(b))
                {
                    WriteTo(w);
                    w.Flush();
                }
                return b.ToString();
            }
        }

        HtmlNodeCollection IHtmlNode.ChildNodes { get { return new HtmlNodeCollection(_ownernode); } }
        bool IHtmlNode.HasChildNodes { get { return false; } }
        HtmlNode IHtmlNode.ParentNode { get { return OwnerNode; } }
        
        #endregion
    }

    /// <summary>
    /// An Enum representing different types of Quotes used for surrounding attribute values
    /// </summary>
    public enum AttributeValueQuote
    {
        /// <summary>
        /// A single quote mark '
        /// </summary>
        SingleQuote,
        /// <summary>
        /// A double quote mark "
        /// </summary>
        DoubleQuote
    }
}