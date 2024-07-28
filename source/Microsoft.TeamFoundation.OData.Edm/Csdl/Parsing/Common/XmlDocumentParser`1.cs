// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlDocumentParser`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal abstract class XmlDocumentParser<TResult> : XmlDocumentParser
  {
    internal XmlDocumentParser(XmlReader underlyingReader, string documentPath)
      : base(underlyingReader, documentPath)
    {
    }

    internal XmlElementValue<TResult> Result => base.Result != null ? (XmlElementValue<TResult>) base.Result : (XmlElementValue<TResult>) null;

    protected override sealed bool TryGetRootElementParser(
      Version artifactVersion,
      XmlElementInfo rootElement,
      out XmlElementParser parser)
    {
      XmlElementParser<TResult> parser1;
      if (this.TryGetDocumentElementParser(artifactVersion, rootElement, out parser1))
      {
        parser = (XmlElementParser) parser1;
        return true;
      }
      parser = (XmlElementParser) null;
      return false;
    }

    protected abstract bool TryGetDocumentElementParser(
      Version artifactVersion,
      XmlElementInfo rootElement,
      out XmlElementParser<TResult> parser);
  }
}
