// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementParser
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal abstract class XmlElementParser
  {
    private readonly Dictionary<string, XmlElementParser> childParsers;

    protected XmlElementParser(string elementName, Dictionary<string, XmlElementParser> children)
    {
      this.ElementName = elementName;
      this.childParsers = children;
    }

    internal string ElementName { get; private set; }

    public void AddChildParser(XmlElementParser child) => this.childParsers[child.ElementName] = child;

    internal static XmlElementParser<TResult> Create<TResult>(
      string elementName,
      Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc,
      IEnumerable<XmlElementParser> childParsers,
      IEnumerable<XmlElementParser> descendantParsers)
    {
      Dictionary<string, XmlElementParser> children = (Dictionary<string, XmlElementParser>) null;
      if (childParsers != null)
        children = childParsers.ToDictionary<XmlElementParser, string>((Func<XmlElementParser, string>) (p => p.ElementName));
      return new XmlElementParser<TResult>(elementName, children, parserFunc);
    }

    internal abstract XmlElementValue Parse(XmlElementInfo element, IList<XmlElementValue> children);

    internal bool TryGetChildElementParser(string elementName, out XmlElementParser elementParser)
    {
      elementParser = (XmlElementParser) null;
      return this.childParsers != null && this.childParsers.TryGetValue(elementName, out elementParser);
    }
  }
}
