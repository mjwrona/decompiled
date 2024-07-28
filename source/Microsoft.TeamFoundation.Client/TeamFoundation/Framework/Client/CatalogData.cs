// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogData
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class CatalogData
  {
    internal CatalogNode[] m_catalogNodes = Helper.ZeroLengthArrayOfCatalogNode;
    internal CatalogResourceType[] m_catalogResourceTypes = Helper.ZeroLengthArrayOfCatalogResourceType;
    internal CatalogResource[] m_catalogResources = Helper.ZeroLengthArrayOfCatalogResource;
    internal CatalogResource[] m_deletedNodeResources = Helper.ZeroLengthArrayOfCatalogResource;
    internal CatalogNode[] m_deletedNodes = Helper.ZeroLengthArrayOfCatalogNode;
    internal CatalogResource[] m_deletedResources = Helper.ZeroLengthArrayOfCatalogResource;
    private int m_locationServiceLastChangeId;

    internal CatalogData()
    {
    }

    public CatalogNode[] CatalogNodes
    {
      get => (CatalogNode[]) this.m_catalogNodes.Clone();
      set => this.m_catalogNodes = value;
    }

    public CatalogResourceType[] CatalogResourceTypes
    {
      get => (CatalogResourceType[]) this.m_catalogResourceTypes.Clone();
      set => this.m_catalogResourceTypes = value;
    }

    public CatalogResource[] CatalogResources
    {
      get => (CatalogResource[]) this.m_catalogResources.Clone();
      set => this.m_catalogResources = value;
    }

    public CatalogResource[] DeletedNodeResources
    {
      get => (CatalogResource[]) this.m_deletedNodeResources.Clone();
      set => this.m_deletedNodeResources = value;
    }

    public CatalogNode[] DeletedNodes
    {
      get => (CatalogNode[]) this.m_deletedNodes.Clone();
      set => this.m_deletedNodes = value;
    }

    public CatalogResource[] DeletedResources
    {
      get => (CatalogResource[]) this.m_deletedResources.Clone();
      set => this.m_deletedResources = value;
    }

    public int LocationServiceLastChangeId
    {
      get => this.m_locationServiceLastChangeId;
      set => this.m_locationServiceLastChangeId = value;
    }

    internal static CatalogData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      CatalogData catalogData = new CatalogData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name1 = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name2 = reader.Name;
          if (name2 != null)
          {
            switch (name2.Length)
            {
              case 12:
                switch (name2[0])
                {
                  case 'C':
                    if (name2 == "CatalogNodes")
                    {
                      catalogData.m_catalogNodes = Helper.ArrayOfCatalogNodeFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                  case 'D':
                    if (name2 == "DeletedNodes")
                    {
                      catalogData.m_deletedNodes = Helper.ArrayOfCatalogNodeFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                }
                break;
              case 16:
                switch (name2[0])
                {
                  case 'C':
                    if (name2 == "CatalogResources")
                    {
                      catalogData.m_catalogResources = Helper.ArrayOfCatalogResourceFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                  case 'D':
                    if (name2 == "DeletedResources")
                    {
                      catalogData.m_deletedResources = Helper.ArrayOfCatalogResourceFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                }
                break;
              case 20:
                switch (name2[0])
                {
                  case 'C':
                    if (name2 == "CatalogResourceTypes")
                    {
                      catalogData.m_catalogResourceTypes = Helper.ArrayOfCatalogResourceTypeFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                  case 'D':
                    if (name2 == "DeletedNodeResources")
                    {
                      catalogData.m_deletedNodeResources = Helper.ArrayOfCatalogResourceFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                }
                break;
              case 27:
                if (name2 == "LocationServiceLastChangeId")
                {
                  catalogData.m_locationServiceLastChangeId = XmlUtility.Int32FromXmlElement(reader);
                  continue;
                }
                break;
            }
          }
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return catalogData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CatalogData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  CatalogNodes: " + Helper.ArrayToString<CatalogNode>(this.m_catalogNodes));
      stringBuilder.AppendLine("  CatalogResourceTypes: " + Helper.ArrayToString<CatalogResourceType>(this.m_catalogResourceTypes));
      stringBuilder.AppendLine("  CatalogResources: " + Helper.ArrayToString<CatalogResource>(this.m_catalogResources));
      stringBuilder.AppendLine("  DeletedNodeResources: " + Helper.ArrayToString<CatalogResource>(this.m_deletedNodeResources));
      stringBuilder.AppendLine("  DeletedNodes: " + Helper.ArrayToString<CatalogNode>(this.m_deletedNodes));
      stringBuilder.AppendLine("  DeletedResources: " + Helper.ArrayToString<CatalogResource>(this.m_deletedResources));
      stringBuilder.AppendLine("  LocationServiceLastChangeId: " + this.m_locationServiceLastChangeId.ToString());
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      Helper.ToXml(writer, "CatalogNodes", this.m_catalogNodes, false, false);
      Helper.ToXml(writer, "CatalogResourceTypes", this.m_catalogResourceTypes, false, false);
      Helper.ToXml(writer, "CatalogResources", this.m_catalogResources, false, false);
      Helper.ToXml(writer, "DeletedNodeResources", this.m_deletedNodeResources, false, false);
      Helper.ToXml(writer, "DeletedNodes", this.m_deletedNodes, false, false);
      Helper.ToXml(writer, "DeletedResources", this.m_deletedResources, false, false);
      if (this.m_locationServiceLastChangeId != 0)
        XmlUtility.ToXmlElement(writer, "LocationServiceLastChangeId", this.m_locationServiceLastChangeId);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, CatalogData obj) => obj.ToXml(writer, element);
  }
}
