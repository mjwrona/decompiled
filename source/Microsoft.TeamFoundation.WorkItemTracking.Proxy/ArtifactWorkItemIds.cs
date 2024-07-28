// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ArtifactWorkItemIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class ArtifactWorkItemIds : ITfsXmlSerializable
  {
    private int m_uriListOffset;
    private int[] m_workItemIds;
    private string m_uri;

    private static int IntFromXml(XmlReader reader)
    {
      reader.Read();
      int num = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      reader.Read();
      reader.ReadEndElement();
      return num;
    }

    private static string StringFromXml(XmlReader reader)
    {
      reader.Read();
      string str = reader.Value;
      reader.Read();
      reader.ReadEndElement();
      return str;
    }

    private static int[] IntArrayFromXml(XmlReader reader)
    {
      List<int> intList = new List<int>();
      reader.ReadStartElement();
      while (reader.NodeType == XmlNodeType.Element)
        intList.Add(ArtifactWorkItemIds.IntFromXml(reader));
      reader.ReadEndElement();
      return intList.ToArray();
    }

    public ArtifactWorkItemIds() => this.m_workItemIds = Array.Empty<int>();

    public int[] GetWorkItemIds() => this.m_workItemIds;

    public int UriListOffset
    {
      get => this.m_uriListOffset;
      set => this.m_uriListOffset = value;
    }

    public string Uri => this.m_uri;

    public void ReadXml(XmlReader reader, string xmlElement)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num != 0)
        return;
      while (reader.NodeType == XmlNodeType.Element)
      {
        switch (reader.Name)
        {
          case "WorkItemIds":
            this.m_workItemIds = ArtifactWorkItemIds.IntArrayFromXml(reader);
            continue;
          case "UriListOffset":
            this.m_uriListOffset = ArtifactWorkItemIds.IntFromXml(reader);
            continue;
          case "Uri":
            this.m_uri = ArtifactWorkItemIds.StringFromXml(reader);
            continue;
          default:
            reader.Read();
            continue;
        }
      }
      reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer, string xmlElement)
    {
    }
  }
}
