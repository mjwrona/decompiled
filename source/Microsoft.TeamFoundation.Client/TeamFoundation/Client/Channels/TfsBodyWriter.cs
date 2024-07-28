// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsBodyWriter
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TfsBodyWriter
  {
    private string m_name;
    private string m_namespace;
    private object[] m_parameters;
    private Action<XmlDictionaryWriter, object[]> m_writeParameters;

    public TfsBodyWriter(
      string name,
      string ns,
      object[] parameters,
      Action<XmlDictionaryWriter, object[]> writeParameters)
    {
      this.m_name = name;
      this.m_namespace = ns;
      this.m_parameters = parameters;
      this.m_writeParameters = writeParameters;
    }

    public void Close()
    {
      this.m_parameters = (object[]) null;
      this.m_writeParameters = (Action<XmlDictionaryWriter, object[]>) null;
    }

    public void WriteBodyContents(XmlDictionaryWriter writer)
    {
      writer.WriteStartElement(this.m_name, this.m_namespace);
      this.m_writeParameters(writer, this.m_parameters);
      writer.WriteEndElement();
    }
  }
}
