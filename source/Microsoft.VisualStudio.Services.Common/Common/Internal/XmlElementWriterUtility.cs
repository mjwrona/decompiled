// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.XmlElementWriterUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class XmlElementWriterUtility : IDisposable
  {
    private XmlWriter m_xmlWriter;

    public XmlElementWriterUtility(string elementName, XmlWriter xmlWriter)
    {
      this.m_xmlWriter = xmlWriter;
      this.m_xmlWriter.WriteStartElement(elementName);
    }

    public void Dispose() => this.m_xmlWriter.WriteEndElement();
  }
}
