// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.BufferingXmlReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Metadata
{
  internal sealed class BufferingXmlReader : XmlReader, IXmlLineInfo
  {
    internal readonly string XmlNamespace;
    internal readonly string XmlBaseAttributeName;
    internal readonly string ODataMetadataNamespace;
    internal readonly string ODataNamespace;
    internal readonly string ODataErrorElementName;
    private readonly IXmlLineInfo lineInfo;
    private readonly XmlReader reader;
    private readonly LinkedList<BufferingXmlReader.BufferedNode> bufferedNodes;
    private readonly BufferingXmlReader.BufferedNode endOfInputBufferedNode;
    private readonly bool disableXmlBase;
    private readonly int maxInnerErrorDepth;
    private readonly Uri documentBaseUri;
    private LinkedListNode<BufferingXmlReader.BufferedNode> currentBufferedNode;
    private LinkedListNode<BufferingXmlReader.BufferedNode> currentAttributeNode;
    private LinkedListNode<BufferingXmlReader.BufferedNode> currentBufferedNodeToReport;
    private bool isBuffering;
    private bool removeOnNextRead;
    private bool disableInStreamErrorDetection;
    private Stack<BufferingXmlReader.XmlBaseDefinition> xmlBaseStack;
    private Stack<BufferingXmlReader.XmlBaseDefinition> bufferStartXmlBaseStack;

    internal BufferingXmlReader(
      XmlReader reader,
      Uri parentXmlBaseUri,
      Uri documentBaseUri,
      bool disableXmlBase,
      int maxInnerErrorDepth)
    {
      this.reader = reader;
      this.lineInfo = reader as IXmlLineInfo;
      this.documentBaseUri = documentBaseUri;
      this.disableXmlBase = disableXmlBase;
      this.maxInnerErrorDepth = maxInnerErrorDepth;
      XmlNameTable nameTable = this.reader.NameTable;
      this.XmlNamespace = nameTable.Add("http://www.w3.org/XML/1998/namespace");
      this.XmlBaseAttributeName = nameTable.Add("base");
      this.ODataMetadataNamespace = nameTable.Add("http://docs.oasis-open.org/odata/ns/metadata");
      this.ODataNamespace = nameTable.Add("http://docs.oasis-open.org/odata/ns/data");
      this.ODataErrorElementName = nameTable.Add("error");
      this.bufferedNodes = new LinkedList<BufferingXmlReader.BufferedNode>();
      this.currentBufferedNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
      this.endOfInputBufferedNode = BufferingXmlReader.BufferedNode.CreateEndOfInput(this.reader.NameTable);
      this.xmlBaseStack = new Stack<BufferingXmlReader.XmlBaseDefinition>();
      if (!(parentXmlBaseUri != (Uri) null))
        return;
      this.xmlBaseStack.Push(new BufferingXmlReader.XmlBaseDefinition(parentXmlBaseUri, 0));
    }

    public override XmlNodeType NodeType => this.currentBufferedNodeToReport == null ? this.reader.NodeType : this.currentBufferedNodeToReport.Value.NodeType;

    public override bool IsEmptyElement => this.currentBufferedNodeToReport == null ? this.reader.IsEmptyElement : this.currentBufferedNodeToReport.Value.IsEmptyElement;

    public override string LocalName => this.currentBufferedNodeToReport == null ? this.reader.LocalName : this.currentBufferedNodeToReport.Value.LocalName;

    public override string Prefix => this.currentBufferedNodeToReport == null ? this.reader.Prefix : this.currentBufferedNodeToReport.Value.Prefix;

    public override string NamespaceURI => this.currentBufferedNodeToReport == null ? this.reader.NamespaceURI : this.currentBufferedNodeToReport.Value.NamespaceUri;

    public override string Value => this.currentBufferedNodeToReport == null ? this.reader.Value : this.currentBufferedNodeToReport.Value.Value;

    public override int Depth => this.currentBufferedNodeToReport == null ? this.reader.Depth : this.currentBufferedNodeToReport.Value.Depth;

    public override bool EOF => this.currentBufferedNodeToReport == null ? this.reader.EOF : this.IsEndOfInputNode(this.currentBufferedNodeToReport.Value);

    public override ReadState ReadState
    {
      get
      {
        if (this.currentBufferedNodeToReport == null)
          return this.reader.ReadState;
        if (this.IsEndOfInputNode(this.currentBufferedNodeToReport.Value))
          return ReadState.EndOfFile;
        return this.currentBufferedNodeToReport.Value.NodeType != XmlNodeType.None ? ReadState.Interactive : ReadState.Initial;
      }
    }

    public override XmlNameTable NameTable => this.reader.NameTable;

    public override int AttributeCount
    {
      get
      {
        if (this.currentBufferedNodeToReport == null)
          return this.reader.AttributeCount;
        return this.currentBufferedNodeToReport.Value.AttributeNodes == null ? 0 : this.currentBufferedNodeToReport.Value.AttributeNodes.Count;
      }
    }

    public override string BaseURI => (string) null;

    public override bool HasValue
    {
      get
      {
        if (this.currentBufferedNodeToReport == null)
          return this.reader.HasValue;
        switch (this.NodeType)
        {
          case XmlNodeType.Attribute:
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.DocumentType:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          case XmlNodeType.XmlDeclaration:
            return true;
          default:
            return false;
        }
      }
    }

    int IXmlLineInfo.LineNumber => !this.HasLineInfo() ? 0 : this.lineInfo.LineNumber;

    int IXmlLineInfo.LinePosition => !this.HasLineInfo() ? 0 : this.lineInfo.LinePosition;

    internal Uri XmlBaseUri => this.xmlBaseStack.Count <= 0 ? (Uri) null : this.xmlBaseStack.Peek().BaseUri;

    internal Uri ParentXmlBaseUri
    {
      get
      {
        if (this.xmlBaseStack.Count == 0)
          return (Uri) null;
        BufferingXmlReader.XmlBaseDefinition xmlBaseDefinition = this.xmlBaseStack.Peek();
        if (xmlBaseDefinition.Depth == this.Depth)
        {
          if (this.xmlBaseStack.Count == 1)
            return (Uri) null;
          xmlBaseDefinition = this.xmlBaseStack.Skip<BufferingXmlReader.XmlBaseDefinition>(1).First<BufferingXmlReader.XmlBaseDefinition>();
        }
        return xmlBaseDefinition.BaseUri;
      }
    }

    internal bool DisableInStreamErrorDetection
    {
      get => this.disableInStreamErrorDetection;
      set => this.disableInStreamErrorDetection = value;
    }

    public override bool Read()
    {
      if (!this.disableXmlBase && this.xmlBaseStack.Count > 0)
      {
        XmlNodeType xmlNodeType = this.NodeType;
        if (xmlNodeType == XmlNodeType.Attribute)
        {
          this.MoveToElement();
          xmlNodeType = XmlNodeType.Element;
        }
        if (this.xmlBaseStack.Peek().Depth == this.Depth)
        {
          switch (xmlNodeType)
          {
            case XmlNodeType.Element:
              if (!this.IsEmptyElement)
                break;
              goto case XmlNodeType.EndElement;
            case XmlNodeType.EndElement:
              this.xmlBaseStack.Pop();
              break;
          }
        }
      }
      bool flag = this.ReadInternal(this.disableInStreamErrorDetection);
      if (flag && !this.disableXmlBase && this.NodeType == XmlNodeType.Element)
      {
        string withAtomizedName = this.GetAttributeWithAtomizedName(this.XmlBaseAttributeName, this.XmlNamespace);
        if (withAtomizedName != null)
        {
          Uri uri = new Uri(withAtomizedName, UriKind.RelativeOrAbsolute);
          if (!uri.IsAbsoluteUri)
          {
            if (this.xmlBaseStack.Count == 0)
              uri = !(this.documentBaseUri == (Uri) null) ? UriUtils.UriToAbsoluteUri(this.documentBaseUri, uri) : throw new ODataException(Strings.ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified((object) withAtomizedName));
            else
              uri = UriUtils.UriToAbsoluteUri(this.xmlBaseStack.Peek().BaseUri, uri);
          }
          this.xmlBaseStack.Push(new BufferingXmlReader.XmlBaseDefinition(uri, this.Depth));
        }
      }
      return flag;
    }

    public override bool MoveToElement()
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.MoveToElement();
      this.MoveFromAttributeValueNode();
      if (this.isBuffering)
      {
        if (this.currentBufferedNodeToReport.Value.NodeType != XmlNodeType.Attribute)
          return false;
        this.currentBufferedNodeToReport = this.currentBufferedNode;
        return true;
      }
      if (this.currentBufferedNodeToReport.Value.NodeType != XmlNodeType.Attribute)
        return false;
      this.currentBufferedNodeToReport = this.bufferedNodes.First;
      return true;
    }

    public override bool MoveToFirstAttribute()
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.MoveToFirstAttribute();
      BufferingXmlReader.BufferedNode currentElementNode = this.GetCurrentElementNode();
      if (currentElementNode.NodeType != XmlNodeType.Element || currentElementNode.AttributeNodes.Count <= 0)
        return false;
      this.currentAttributeNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
      this.currentBufferedNodeToReport = currentElementNode.AttributeNodes.First;
      return true;
    }

    public override bool MoveToNextAttribute()
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.MoveToNextAttribute();
      LinkedListNode<BufferingXmlReader.BufferedNode> linkedListNode = this.currentAttributeNode ?? this.currentBufferedNodeToReport;
      if (linkedListNode.Value.NodeType == XmlNodeType.Attribute)
      {
        if (linkedListNode.Next == null)
          return false;
        this.currentAttributeNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
        this.currentBufferedNodeToReport = linkedListNode.Next;
        return true;
      }
      if (this.currentBufferedNodeToReport.Value.NodeType != XmlNodeType.Element || this.currentBufferedNodeToReport.Value.AttributeNodes.Count <= 0)
        return false;
      this.currentBufferedNodeToReport = this.currentBufferedNodeToReport.Value.AttributeNodes.First;
      return true;
    }

    public override bool ReadAttributeValue()
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.ReadAttributeValue();
      if (this.currentBufferedNodeToReport.Value.NodeType != XmlNodeType.Attribute || this.currentAttributeNode != null)
        return false;
      LinkedListNode<BufferingXmlReader.BufferedNode> linkedListNode = new LinkedListNode<BufferingXmlReader.BufferedNode>(new BufferingXmlReader.BufferedNode(this.currentBufferedNodeToReport.Value.Value, this.currentBufferedNodeToReport.Value.Depth, this.NameTable));
      this.currentAttributeNode = this.currentBufferedNodeToReport;
      this.currentBufferedNodeToReport = linkedListNode;
      return true;
    }

    public override string GetAttribute(int i)
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.GetAttribute(i);
      if (i < 0 || i >= this.AttributeCount)
        throw new ArgumentOutOfRangeException(nameof (i));
      return this.FindAttributeBufferedNode(i)?.Value.Value;
    }

    public override string GetAttribute(string name, string namespaceURI)
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.GetAttribute(name, namespaceURI);
      return this.FindAttributeBufferedNode(name, namespaceURI)?.Value.Value;
    }

    public override string GetAttribute(string name)
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.GetAttribute(name);
      return this.FindAttributeBufferedNode(name)?.Value.Value;
    }

    public override string LookupNamespace(string prefix) => throw new NotSupportedException();

    public override bool MoveToAttribute(string name, string ns)
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.MoveToAttribute(name, ns);
      LinkedListNode<BufferingXmlReader.BufferedNode> attributeBufferedNode = this.FindAttributeBufferedNode(name, ns);
      if (attributeBufferedNode == null)
        return false;
      this.currentAttributeNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
      this.currentBufferedNodeToReport = attributeBufferedNode;
      return true;
    }

    public override bool MoveToAttribute(string name)
    {
      if (this.bufferedNodes.Count <= 0)
        return this.reader.MoveToAttribute(name);
      LinkedListNode<BufferingXmlReader.BufferedNode> attributeBufferedNode = this.FindAttributeBufferedNode(name);
      if (attributeBufferedNode == null)
        return false;
      this.currentAttributeNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
      this.currentBufferedNodeToReport = attributeBufferedNode;
      return true;
    }

    public override void ResolveEntity() => throw new InvalidOperationException(Strings.ODataException_GeneralError);

    bool IXmlLineInfo.HasLineInfo() => this.HasLineInfo();

    internal void StartBuffering()
    {
      if (this.bufferedNodes.Count == 0)
        this.bufferedNodes.AddFirst(this.BufferCurrentReaderNode());
      else
        this.removeOnNextRead = false;
      this.currentBufferedNode = this.bufferedNodes.First;
      this.currentBufferedNodeToReport = this.currentBufferedNode;
      int count = this.xmlBaseStack.Count;
      switch (count)
      {
        case 0:
          this.bufferStartXmlBaseStack = new Stack<BufferingXmlReader.XmlBaseDefinition>();
          break;
        case 1:
          this.bufferStartXmlBaseStack = new Stack<BufferingXmlReader.XmlBaseDefinition>();
          this.bufferStartXmlBaseStack.Push(this.xmlBaseStack.Peek());
          break;
        default:
          BufferingXmlReader.XmlBaseDefinition[] array = this.xmlBaseStack.ToArray();
          this.bufferStartXmlBaseStack = new Stack<BufferingXmlReader.XmlBaseDefinition>(count);
          for (int index = count - 1; index >= 0; --index)
            this.bufferStartXmlBaseStack.Push(array[index]);
          break;
      }
      this.isBuffering = true;
    }

    internal void StopBuffering()
    {
      this.isBuffering = false;
      this.removeOnNextRead = true;
      this.currentBufferedNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
      if (this.bufferedNodes.Count > 0)
        this.currentBufferedNodeToReport = this.bufferedNodes.First;
      this.xmlBaseStack = this.bufferStartXmlBaseStack;
      this.bufferStartXmlBaseStack = (Stack<BufferingXmlReader.XmlBaseDefinition>) null;
    }

    private bool ReadInternal(bool ignoreInStreamErrors)
    {
      if (this.removeOnNextRead)
      {
        this.currentBufferedNodeToReport = this.currentBufferedNodeToReport.Next;
        this.bufferedNodes.RemoveFirst();
        this.removeOnNextRead = false;
      }
      bool flag;
      if (this.isBuffering)
      {
        this.MoveFromAttributeValueNode();
        if (this.currentBufferedNode.Next != null)
        {
          this.currentBufferedNode = this.currentBufferedNode.Next;
          this.currentBufferedNodeToReport = this.currentBufferedNode;
          flag = true;
        }
        else if (ignoreInStreamErrors)
        {
          flag = this.reader.Read();
          this.bufferedNodes.AddLast(this.BufferCurrentReaderNode());
          this.currentBufferedNode = this.bufferedNodes.Last;
          this.currentBufferedNodeToReport = this.currentBufferedNode;
        }
        else
          flag = this.ReadNextAndCheckForInStreamError();
      }
      else if (this.bufferedNodes.Count == 0)
      {
        flag = ignoreInStreamErrors ? this.reader.Read() : this.ReadNextAndCheckForInStreamError();
      }
      else
      {
        this.currentBufferedNodeToReport = this.bufferedNodes.First;
        flag = !this.IsEndOfInputNode(this.currentBufferedNodeToReport.Value);
        this.removeOnNextRead = true;
      }
      return flag;
    }

    private bool ReadNextAndCheckForInStreamError()
    {
      bool flag = this.ReadInternal(true);
      if (!this.disableInStreamErrorDetection && this.NodeType == XmlNodeType.Element && this.LocalNameEquals(this.ODataErrorElementName) && this.NamespaceEquals(this.ODataMetadataNamespace))
        throw new ODataErrorException(ODataAtomErrorDeserializer.ReadErrorElement(this, this.maxInnerErrorDepth));
      return flag;
    }

    private bool IsEndOfInputNode(BufferingXmlReader.BufferedNode node) => node == this.endOfInputBufferedNode;

    private BufferingXmlReader.BufferedNode BufferCurrentReaderNode()
    {
      if (this.reader.EOF)
        return this.endOfInputBufferedNode;
      BufferingXmlReader.BufferedNode bufferedNode = new BufferingXmlReader.BufferedNode(this.reader);
      if (this.reader.NodeType == XmlNodeType.Element)
      {
        while (this.reader.MoveToNextAttribute())
          bufferedNode.AttributeNodes.AddLast(new BufferingXmlReader.BufferedNode(this.reader));
        this.reader.MoveToElement();
      }
      return bufferedNode;
    }

    private BufferingXmlReader.BufferedNode GetCurrentElementNode() => this.isBuffering ? this.currentBufferedNode.Value : this.bufferedNodes.First.Value;

    private LinkedListNode<BufferingXmlReader.BufferedNode> FindAttributeBufferedNode(int index)
    {
      BufferingXmlReader.BufferedNode currentElementNode = this.GetCurrentElementNode();
      if (currentElementNode.NodeType == XmlNodeType.Element && currentElementNode.AttributeNodes.Count > 0)
      {
        LinkedListNode<BufferingXmlReader.BufferedNode> attributeBufferedNode = currentElementNode.AttributeNodes.First;
        int num = 0;
        for (; attributeBufferedNode != null; attributeBufferedNode = attributeBufferedNode.Next)
        {
          if (num == index)
            return attributeBufferedNode;
          ++num;
        }
      }
      return (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
    }

    private LinkedListNode<BufferingXmlReader.BufferedNode> FindAttributeBufferedNode(
      string localName,
      string namespaceUri)
    {
      BufferingXmlReader.BufferedNode currentElementNode = this.GetCurrentElementNode();
      if (currentElementNode.NodeType == XmlNodeType.Element && currentElementNode.AttributeNodes.Count > 0)
      {
        for (LinkedListNode<BufferingXmlReader.BufferedNode> attributeBufferedNode = currentElementNode.AttributeNodes.First; attributeBufferedNode != null; attributeBufferedNode = attributeBufferedNode.Next)
        {
          BufferingXmlReader.BufferedNode bufferedNode = attributeBufferedNode.Value;
          if (string.CompareOrdinal(bufferedNode.NamespaceUri, namespaceUri) == 0 && string.CompareOrdinal(bufferedNode.LocalName, localName) == 0)
            return attributeBufferedNode;
        }
      }
      return (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
    }

    private LinkedListNode<BufferingXmlReader.BufferedNode> FindAttributeBufferedNode(
      string qualifiedName)
    {
      BufferingXmlReader.BufferedNode currentElementNode = this.GetCurrentElementNode();
      if (currentElementNode.NodeType == XmlNodeType.Element && currentElementNode.AttributeNodes.Count > 0)
      {
        for (LinkedListNode<BufferingXmlReader.BufferedNode> attributeBufferedNode = currentElementNode.AttributeNodes.First; attributeBufferedNode != null; attributeBufferedNode = attributeBufferedNode.Next)
        {
          BufferingXmlReader.BufferedNode bufferedNode = attributeBufferedNode.Value;
          bool flag = !string.IsNullOrEmpty(bufferedNode.Prefix);
          if (!flag && string.CompareOrdinal(bufferedNode.LocalName, qualifiedName) == 0 || flag && string.CompareOrdinal(bufferedNode.Prefix + ":" + bufferedNode.LocalName, qualifiedName) == 0)
            return attributeBufferedNode;
        }
      }
      return (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
    }

    private void MoveFromAttributeValueNode()
    {
      if (this.currentAttributeNode == null)
        return;
      this.currentBufferedNodeToReport = this.currentAttributeNode;
      this.currentAttributeNode = (LinkedListNode<BufferingXmlReader.BufferedNode>) null;
    }

    private string GetAttributeWithAtomizedName(string name, string namespaceURI)
    {
      if (this.bufferedNodes.Count > 0)
      {
        for (LinkedListNode<BufferingXmlReader.BufferedNode> linkedListNode = this.GetCurrentElementNode().AttributeNodes.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          BufferingXmlReader.BufferedNode bufferedNode = linkedListNode.Value;
          if ((object) namespaceURI == (object) bufferedNode.NamespaceUri && (object) name == (object) bufferedNode.LocalName)
            return linkedListNode.Value.Value;
        }
        return (string) null;
      }
      string withAtomizedName = (string) null;
      while (this.reader.MoveToNextAttribute())
      {
        if ((object) name == (object) this.reader.LocalName && (object) namespaceURI == (object) this.reader.NamespaceURI)
        {
          withAtomizedName = this.reader.Value;
          break;
        }
      }
      this.reader.MoveToElement();
      return withAtomizedName;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is a DEBUG only method.")]
    [Conditional("DEBUG")]
    private void ValidateInternalState()
    {
    }

    private bool HasLineInfo() => this.lineInfo != null && this.lineInfo.HasLineInfo();

    private sealed class BufferedNode
    {
      private LinkedList<BufferingXmlReader.BufferedNode> attributeNodes;

      internal BufferedNode(XmlReader reader)
      {
        this.NodeType = reader.NodeType;
        this.NamespaceUri = reader.NamespaceURI;
        this.LocalName = reader.LocalName;
        this.Prefix = reader.Prefix;
        this.Value = reader.Value;
        this.Depth = reader.Depth;
        this.IsEmptyElement = reader.IsEmptyElement;
      }

      internal BufferedNode(string value, int depth, XmlNameTable nametable)
      {
        string str = nametable.Add(string.Empty);
        this.NodeType = XmlNodeType.Text;
        this.NamespaceUri = str;
        this.LocalName = str;
        this.Prefix = str;
        this.Value = value;
        this.Depth = depth + 1;
        this.IsEmptyElement = false;
      }

      private BufferedNode(string emptyString)
      {
        this.NodeType = XmlNodeType.None;
        this.NamespaceUri = emptyString;
        this.LocalName = emptyString;
        this.Prefix = emptyString;
        this.Value = emptyString;
      }

      internal XmlNodeType NodeType { get; private set; }

      internal string NamespaceUri { get; private set; }

      internal string LocalName { get; private set; }

      internal string Prefix { get; private set; }

      internal string Value { get; private set; }

      internal int Depth { get; private set; }

      internal bool IsEmptyElement { get; private set; }

      internal LinkedList<BufferingXmlReader.BufferedNode> AttributeNodes
      {
        get
        {
          if (this.NodeType == XmlNodeType.Element && this.attributeNodes == null)
            this.attributeNodes = new LinkedList<BufferingXmlReader.BufferedNode>();
          return this.attributeNodes;
        }
      }

      internal static BufferingXmlReader.BufferedNode CreateEndOfInput(XmlNameTable nametable) => new BufferingXmlReader.BufferedNode(nametable.Add(string.Empty));
    }

    private sealed class XmlBaseDefinition
    {
      internal XmlBaseDefinition(Uri baseUri, int depth)
      {
        this.BaseUri = baseUri;
        this.Depth = depth;
      }

      internal Uri BaseUri { get; private set; }

      internal int Depth { get; private set; }
    }
  }
}
