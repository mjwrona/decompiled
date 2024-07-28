// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.TraceXPathNavigator
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  [DebuggerDisplay("")]
  internal class TraceXPathNavigator : XPathNavigator
  {
    private const int UnlimitedSize = -1;
    private TraceXPathNavigator.ElementNode root;
    private TraceXPathNavigator.TraceNode current;
    private bool closed;
    private XPathNodeType state = XPathNodeType.Element;
    private int maxSize;
    private long currentSize;

    public TraceXPathNavigator(int maxSize) => this.maxSize = maxSize;

    internal void AddElement(string prefix, string name, string xmlns)
    {
      if (this.closed)
        throw new InvalidOperationException();
      TraceXPathNavigator.ElementNode node = new TraceXPathNavigator.ElementNode(name, prefix, this.CurrentElement, xmlns);
      if (this.current == null)
      {
        this.VerifySize((TraceXPathNavigator.IMeasurable) node);
        this.root = node;
        this.current = (TraceXPathNavigator.TraceNode) this.root;
      }
      else
      {
        if (this.closed)
          return;
        this.VerifySize((TraceXPathNavigator.IMeasurable) node);
        this.CurrentElement.Add((TraceXPathNavigator.TraceNode) node);
        this.current = (TraceXPathNavigator.TraceNode) node;
      }
    }

    internal void AddProcessingInstruction(string name, string text)
    {
      if (this.current == null)
        return;
      TraceXPathNavigator.ProcessingInstructionNode node = new TraceXPathNavigator.ProcessingInstructionNode(name, text, this.CurrentElement);
      this.VerifySize((TraceXPathNavigator.IMeasurable) node);
      this.CurrentElement.Add((TraceXPathNavigator.TraceNode) node);
    }

    internal void AddText(string value)
    {
      if (this.closed)
        throw new InvalidOperationException();
      if (this.current == null)
        return;
      if (this.CurrentElement.text == null)
      {
        TraceXPathNavigator.TextNode node = new TraceXPathNavigator.TextNode(value);
        this.VerifySize((TraceXPathNavigator.IMeasurable) node);
        this.CurrentElement.text = node;
      }
      else
      {
        if (string.IsNullOrEmpty(value))
          return;
        this.VerifySize(value);
        this.CurrentElement.text.nodeValue += value;
      }
    }

    internal void AddAttribute(string name, string value, string xmlns, string prefix)
    {
      if (this.closed)
        throw new InvalidOperationException();
      if (this.current == null)
        throw new InvalidOperationException();
      TraceXPathNavigator.AttributeNode node = new TraceXPathNavigator.AttributeNode(name, prefix, value, xmlns);
      this.VerifySize((TraceXPathNavigator.IMeasurable) node);
      this.CurrentElement.attributes.Add(node);
    }

    internal void AddComment(string text)
    {
      if (this.closed)
        throw new InvalidOperationException();
      if (this.current == null)
        throw new InvalidOperationException();
      TraceXPathNavigator.CommentNode node = new TraceXPathNavigator.CommentNode(text, this.CurrentElement);
      this.VerifySize((TraceXPathNavigator.IMeasurable) node);
      this.CurrentElement.Add((TraceXPathNavigator.TraceNode) node);
    }

    internal void CloseElement()
    {
      if (this.closed)
        throw new InvalidOperationException();
      this.current = (TraceXPathNavigator.TraceNode) this.CurrentElement.parent;
      if (this.current != null)
        return;
      this.closed = true;
    }

    public override string BaseURI => string.Empty;

    public override XPathNavigator Clone() => (XPathNavigator) this;

    public override bool IsEmptyElement
    {
      get
      {
        bool isEmptyElement = true;
        if (this.current != null)
          isEmptyElement = this.CurrentElement.text != null || this.CurrentElement.childNodes.Count > 0;
        return isEmptyElement;
      }
    }

    public override bool IsSamePosition(XPathNavigator other) => false;

    [DebuggerDisplay("")]
    public override string LocalName => this.Name;

    public override string LookupPrefix(string ns) => this.LookupPrefix(ns, this.CurrentElement);

    private string LookupPrefix(string ns, TraceXPathNavigator.ElementNode node)
    {
      string str = (string) null;
      if (string.Compare(ns, node.xmlns, StringComparison.Ordinal) == 0)
      {
        str = node.prefix;
      }
      else
      {
        foreach (TraceXPathNavigator.AttributeNode attribute in node.attributes)
        {
          if (string.Compare("xmlns", attribute.prefix, StringComparison.Ordinal) == 0 && string.Compare(ns, attribute.nodeValue, StringComparison.Ordinal) == 0)
          {
            str = attribute.name;
            break;
          }
        }
      }
      if (string.IsNullOrEmpty(str) && node.parent != null)
        str = this.LookupPrefix(ns, node.parent);
      return str;
    }

    public override bool MoveTo(XPathNavigator other) => false;

    public override bool MoveToFirstAttribute()
    {
      if (this.current == null)
        throw new InvalidOperationException();
      int num = this.CurrentElement.MoveToFirstAttribute() ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.state = XPathNodeType.Attribute;
      return num != 0;
    }

    public override bool MoveToFirstChild()
    {
      if (this.current == null)
        throw new InvalidOperationException();
      bool firstChild = false;
      if (this.CurrentElement.childNodes != null && this.CurrentElement.childNodes.Count > 0)
      {
        this.current = this.CurrentElement.childNodes[0];
        this.state = this.current.NodeType;
        firstChild = true;
      }
      else if ((this.CurrentElement.childNodes == null || this.CurrentElement.childNodes.Count == 0) && this.CurrentElement.text != null)
      {
        this.state = XPathNodeType.Text;
        this.CurrentElement.movedToText = true;
        firstChild = true;
      }
      return firstChild;
    }

    public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) => false;

    public override bool MoveToId(string id) => false;

    public override bool MoveToNext()
    {
      if (this.current == null)
        throw new InvalidOperationException();
      bool next1 = false;
      if (this.state != XPathNodeType.Text)
      {
        TraceXPathNavigator.ElementNode parent = this.current.parent;
        if (parent != null)
        {
          TraceXPathNavigator.TraceNode next2 = parent.MoveToNext();
          if (next2 == null && parent.text != null && !parent.movedToText)
          {
            this.state = XPathNodeType.Text;
            parent.movedToText = true;
            this.current = (TraceXPathNavigator.TraceNode) parent;
            next1 = true;
          }
          else if (next2 != null)
          {
            this.state = next2.NodeType;
            next1 = true;
            this.current = next2;
          }
        }
      }
      return next1;
    }

    public override bool MoveToNextAttribute()
    {
      if (this.current == null)
        throw new InvalidOperationException();
      int num = this.CurrentElement.MoveToNextAttribute() ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.state = XPathNodeType.Attribute;
      return num != 0;
    }

    public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) => false;

    public override bool MoveToParent()
    {
      if (this.current == null)
        throw new InvalidOperationException();
      bool parent = false;
      switch (this.state)
      {
        case XPathNodeType.Element:
        case XPathNodeType.ProcessingInstruction:
        case XPathNodeType.Comment:
          if (this.current.parent != null)
          {
            this.current = (TraceXPathNavigator.TraceNode) this.current.parent;
            this.state = this.current.NodeType;
            parent = true;
            break;
          }
          break;
        case XPathNodeType.Attribute:
          this.state = XPathNodeType.Element;
          parent = true;
          break;
        case XPathNodeType.Namespace:
          this.state = XPathNodeType.Element;
          parent = true;
          break;
        case XPathNodeType.Text:
          this.state = XPathNodeType.Element;
          parent = true;
          break;
      }
      return parent;
    }

    public override bool MoveToPrevious() => false;

    public override void MoveToRoot()
    {
      this.current = (TraceXPathNavigator.TraceNode) this.root;
      this.state = XPathNodeType.Element;
      this.root.Reset();
    }

    [DebuggerDisplay("")]
    public override string Name
    {
      get
      {
        string name = string.Empty;
        if (this.current != null)
        {
          switch (this.state)
          {
            case XPathNodeType.Element:
              name = this.CurrentElement.name;
              break;
            case XPathNodeType.Attribute:
              name = this.CurrentElement.CurrentAttribute.name;
              break;
            case XPathNodeType.ProcessingInstruction:
              name = this.CurrentProcessingInstruction.name;
              break;
          }
        }
        return name;
      }
    }

    public override XmlNameTable NameTable => (XmlNameTable) null;

    [DebuggerDisplay("")]
    public override string NamespaceURI
    {
      get
      {
        string namespaceUri = string.Empty;
        if (this.current != null)
        {
          switch (this.state)
          {
            case XPathNodeType.Element:
              namespaceUri = this.CurrentElement.xmlns;
              break;
            case XPathNodeType.Attribute:
              namespaceUri = this.CurrentElement.CurrentAttribute.xmlns;
              break;
            case XPathNodeType.Namespace:
              namespaceUri = (string) null;
              break;
          }
        }
        return namespaceUri;
      }
    }

    [DebuggerDisplay("")]
    public override XPathNodeType NodeType => this.state;

    [DebuggerDisplay("")]
    public override string Prefix
    {
      get
      {
        string prefix = string.Empty;
        if (this.current != null)
        {
          switch (this.state)
          {
            case XPathNodeType.Element:
              prefix = this.CurrentElement.prefix;
              break;
            case XPathNodeType.Attribute:
              prefix = this.CurrentElement.CurrentAttribute.prefix;
              break;
            case XPathNodeType.Namespace:
              prefix = (string) null;
              break;
          }
        }
        return prefix;
      }
    }

    private TraceXPathNavigator.CommentNode CurrentComment => this.current as TraceXPathNavigator.CommentNode;

    private TraceXPathNavigator.ElementNode CurrentElement => this.current as TraceXPathNavigator.ElementNode;

    private TraceXPathNavigator.ProcessingInstructionNode CurrentProcessingInstruction => this.current as TraceXPathNavigator.ProcessingInstructionNode;

    [DebuggerDisplay("")]
    public override string Value
    {
      get
      {
        string str = string.Empty;
        if (this.current != null)
        {
          switch (this.state)
          {
            case XPathNodeType.Attribute:
              str = this.CurrentElement.CurrentAttribute.nodeValue;
              break;
            case XPathNodeType.Text:
              str = this.CurrentElement.text.nodeValue;
              break;
            case XPathNodeType.ProcessingInstruction:
              str = this.CurrentProcessingInstruction.text;
              break;
            case XPathNodeType.Comment:
              str = this.CurrentComment.nodeValue;
              break;
          }
        }
        return str;
      }
    }

    internal WriteState WriteState
    {
      get
      {
        WriteState writeState = WriteState.Error;
        if (this.current == null)
          writeState = WriteState.Start;
        else if (this.closed)
        {
          writeState = WriteState.Closed;
        }
        else
        {
          switch (this.state)
          {
            case XPathNodeType.Element:
              writeState = WriteState.Element;
              break;
            case XPathNodeType.Attribute:
              writeState = WriteState.Attribute;
              break;
            case XPathNodeType.Text:
              writeState = WriteState.Content;
              break;
            case XPathNodeType.Comment:
              writeState = WriteState.Content;
              break;
          }
        }
        return writeState;
      }
    }

    public override string ToString()
    {
      this.MoveToRoot();
      StringBuilder sb = new StringBuilder();
      new XmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.CurrentCulture)).WriteNode((XPathNavigator) this, false);
      return sb.ToString();
    }

    private void VerifySize(TraceXPathNavigator.IMeasurable node) => this.VerifySize(node.Size);

    private void VerifySize(string node) => this.VerifySize(node.Length);

    private void VerifySize(int nodeSize)
    {
      if (this.maxSize != -1 && this.currentSize + (long) nodeSize > (long) this.maxSize)
        throw new PlainXmlWriter.MaxSizeExceededException();
      this.currentSize += (long) nodeSize;
    }

    public void RemovePii(string[][] paths)
    {
      if (paths == null)
        throw new ArgumentNullException(nameof (paths));
      foreach (string[] path in paths)
        this.RemovePii(path);
    }

    public void RemovePii(string[] path) => this.RemovePii(path, DiagnosticStrings.PiiList);

    public void RemovePii(string[] headersPath, string[] piiList)
    {
      if (this.root == null)
        throw new ArgumentNullException(SRClient.NullRoot);
      foreach (TraceXPathNavigator.ElementNode subnode in this.root.FindSubnodes(headersPath))
        TraceXPathNavigator.MaskSubnodes(subnode, piiList);
    }

    private static void MaskElement(TraceXPathNavigator.ElementNode element)
    {
      if (element == null)
        return;
      element.childNodes.Clear();
      element.Add((TraceXPathNavigator.TraceNode) new TraceXPathNavigator.CommentNode("Removed", element));
      element.text = (TraceXPathNavigator.TextNode) null;
      element.attributes = (List<TraceXPathNavigator.AttributeNode>) null;
    }

    private static void MaskSubnodes(TraceXPathNavigator.ElementNode element, string[] elementNames) => TraceXPathNavigator.MaskSubnodes(element, elementNames, false);

    private static void MaskSubnodes(
      TraceXPathNavigator.ElementNode element,
      string[] elementNames,
      bool processNodeItself)
    {
      if (elementNames == null)
        throw new ArgumentNullException(nameof (elementNames));
      if (element == null)
        return;
      bool flag = true;
      if (processNodeItself)
      {
        foreach (string elementName in elementNames)
        {
          if (string.CompareOrdinal(elementName, element.name) == 0)
          {
            TraceXPathNavigator.MaskElement(element);
            flag = false;
            break;
          }
        }
      }
      if (!flag || element.childNodes == null)
        return;
      foreach (TraceXPathNavigator.ElementNode childNode in element.childNodes)
        TraceXPathNavigator.MaskSubnodes(childNode, elementNames, true);
    }

    private interface IMeasurable
    {
      int Size { get; }
    }

    private class TraceNode
    {
      private XPathNodeType nodeType;
      internal TraceXPathNavigator.ElementNode parent;

      protected TraceNode(XPathNodeType nodeType, TraceXPathNavigator.ElementNode parent)
      {
        this.nodeType = nodeType;
        this.parent = parent;
      }

      internal XPathNodeType NodeType => this.nodeType;
    }

    private class CommentNode : TraceXPathNavigator.TraceNode, TraceXPathNavigator.IMeasurable
    {
      internal string nodeValue;

      internal CommentNode(string nodeValue, TraceXPathNavigator.ElementNode parent)
        : base(XPathNodeType.Comment, parent)
      {
        this.nodeValue = nodeValue;
      }

      public int Size => this.nodeValue.Length + 8;
    }

    private class ElementNode : TraceXPathNavigator.TraceNode, TraceXPathNavigator.IMeasurable
    {
      private int attributeIndex;
      private int elementIndex;
      internal string name;
      internal string prefix;
      internal string xmlns;
      internal List<TraceXPathNavigator.TraceNode> childNodes = new List<TraceXPathNavigator.TraceNode>();
      internal List<TraceXPathNavigator.AttributeNode> attributes = new List<TraceXPathNavigator.AttributeNode>();
      internal TraceXPathNavigator.TextNode text;
      internal bool movedToText;

      internal ElementNode(
        string name,
        string prefix,
        TraceXPathNavigator.ElementNode parent,
        string xmlns)
        : base(XPathNodeType.Element, parent)
      {
        this.name = name;
        this.prefix = prefix;
        this.xmlns = xmlns;
      }

      internal void Add(TraceXPathNavigator.TraceNode node) => this.childNodes.Add(node);

      internal IEnumerable<TraceXPathNavigator.ElementNode> FindSubnodes(string[] headersPath)
      {
        if (headersPath == null)
          throw new ArgumentNullException(nameof (headersPath));
        TraceXPathNavigator.ElementNode elementNode = this;
        if (string.CompareOrdinal(elementNode.name, headersPath[0]) != 0)
          elementNode = (TraceXPathNavigator.ElementNode) null;
        int i = 0;
        while (elementNode != null && ++i < headersPath.Length)
        {
          TraceXPathNavigator.ElementNode subNode = (TraceXPathNavigator.ElementNode) null;
          if (elementNode.childNodes != null)
          {
            foreach (TraceXPathNavigator.TraceNode childNode1 in elementNode.childNodes)
            {
              if (childNode1.NodeType == XPathNodeType.Element)
              {
                TraceXPathNavigator.ElementNode childNode = childNode1 as TraceXPathNavigator.ElementNode;
                if (childNode != null && string.CompareOrdinal(childNode.name, headersPath[i]) == 0)
                {
                  if (headersPath.Length == i + 1)
                  {
                    yield return childNode;
                  }
                  else
                  {
                    subNode = childNode;
                    break;
                  }
                }
                childNode = (TraceXPathNavigator.ElementNode) null;
              }
            }
          }
          elementNode = subNode;
          subNode = (TraceXPathNavigator.ElementNode) null;
        }
      }

      internal TraceXPathNavigator.TraceNode MoveToNext()
      {
        TraceXPathNavigator.TraceNode next = (TraceXPathNavigator.TraceNode) null;
        if (this.elementIndex + 1 < this.childNodes.Count)
        {
          ++this.elementIndex;
          next = this.childNodes[this.elementIndex];
        }
        return next;
      }

      internal bool MoveToFirstAttribute()
      {
        this.attributeIndex = 0;
        return this.attributes != null && this.attributes.Count > 0;
      }

      internal bool MoveToNextAttribute()
      {
        bool nextAttribute = false;
        if (this.attributeIndex + 1 < this.attributes.Count)
        {
          ++this.attributeIndex;
          nextAttribute = true;
        }
        return nextAttribute;
      }

      internal void Reset()
      {
        this.attributeIndex = 0;
        this.elementIndex = 0;
        this.movedToText = false;
        if (this.childNodes == null)
          return;
        foreach (TraceXPathNavigator.TraceNode childNode in this.childNodes)
        {
          if (childNode.NodeType == XPathNodeType.Element && childNode is TraceXPathNavigator.ElementNode elementNode)
            elementNode.Reset();
        }
      }

      internal TraceXPathNavigator.AttributeNode CurrentAttribute => this.attributes[this.attributeIndex];

      public int Size
      {
        get
        {
          int size = 2 * this.name.Length + 6;
          if (!string.IsNullOrEmpty(this.prefix))
            size += this.prefix.Length + 1;
          if (!string.IsNullOrEmpty(this.xmlns))
            size += this.xmlns.Length + 9;
          return size;
        }
      }
    }

    private class AttributeNode : TraceXPathNavigator.IMeasurable
    {
      internal string name;
      internal string nodeValue;
      internal string prefix;
      internal string xmlns;

      internal AttributeNode(string name, string prefix, string value, string xmlns)
      {
        this.name = name;
        this.prefix = prefix;
        this.nodeValue = value;
        this.xmlns = xmlns;
      }

      public int Size
      {
        get
        {
          int size = this.name.Length + this.nodeValue.Length + 5;
          if (!string.IsNullOrEmpty(this.prefix))
            size += this.prefix.Length + 1;
          if (!string.IsNullOrEmpty(this.xmlns))
            size += this.xmlns.Length + 9;
          return size;
        }
      }
    }

    private class ProcessingInstructionNode : 
      TraceXPathNavigator.TraceNode,
      TraceXPathNavigator.IMeasurable
    {
      internal string name;
      internal string text;

      internal ProcessingInstructionNode(
        string name,
        string text,
        TraceXPathNavigator.ElementNode parent)
        : base(XPathNodeType.ProcessingInstruction, parent)
      {
        this.name = name;
        this.text = text;
      }

      public int Size => this.name.Length + this.text.Length + 12;
    }

    private class TextNode : TraceXPathNavigator.IMeasurable
    {
      internal string nodeValue;

      internal TextNode(string value) => this.nodeValue = value;

      public int Size => this.nodeValue.Length;
    }
  }
}
