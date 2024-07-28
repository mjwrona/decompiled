// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementParser`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlElementParser<TResult> : XmlElementParser
  {
    private readonly Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc;

    internal XmlElementParser(
      string elementName,
      Dictionary<string, XmlElementParser> children,
      Func<XmlElementInfo, XmlElementValueCollection, TResult> parser)
      : base(elementName, children)
    {
      this.parserFunc = parser;
    }

    internal override XmlElementValue Parse(XmlElementInfo element, IList<XmlElementValue> children)
    {
      TResult newValue = this.parserFunc(element, XmlElementValueCollection.FromList(children));
      return (XmlElementValue) new XmlElementValue<TResult>(element.Name, element.Location, newValue);
    }
  }
}
