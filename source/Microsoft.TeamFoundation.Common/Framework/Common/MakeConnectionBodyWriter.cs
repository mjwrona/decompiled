// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MakeConnectionBodyWriter
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Common
{
  internal sealed class MakeConnectionBodyWriter : BodyWriter
  {
    private string m_id;

    public MakeConnectionBodyWriter(string id)
      : base(true)
    {
      this.m_id = id;
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
      writer.WriteStartElement("wsmc", "MakeConnection", "http://docs.oasis-open.org/ws-rx/wsmc/200702");
      writer.WriteElementString("wsmc", "Address", "http://docs.oasis-open.org/ws-rx/wsmc/200702", this.m_id);
      writer.WriteEndElement();
    }

    public static string Create(XmlDictionaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      string str = (string) null;
      if (reader.IsStartElement("MakeConnection", "http://docs.oasis-open.org/ws-rx/wsmc/200702"))
      {
        reader.ReadStartElement();
        int content = (int) reader.MoveToContent();
        while (reader.IsStartElement())
        {
          if (reader.IsStartElement("Address", "http://docs.oasis-open.org/ws-rx/wsmc/200702"))
          {
            str = string.IsNullOrEmpty(str) ? reader.ReadElementContentAsString() : throw new InvalidOperationException("Multiple addresses in WS-MakeConnection");
            if (string.IsNullOrEmpty(str))
              throw new InvalidOperationException("Invalid addresss in WS-MakeConnection");
          }
          else
            reader.Skip();
        }
        reader.ReadEndElement();
      }
      return str != null ? str : throw new InvalidOperationException("Address missing in WS-MakeConnection");
    }
  }
}
