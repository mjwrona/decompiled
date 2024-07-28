// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.NodeInfo
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class NodeInfo
  {
    public string Uri;
    public string Name;
    public string Path;
    public string StructureType;
    public Property[] Properties;
    public string ParentUri;
    public string ProjectUri;
    public DateTime? StartDate;
    public DateTime? FinishDate;

    public NodeInfo(
      string uri,
      string name,
      string structureType,
      Property[] properties,
      string parentUri,
      string projectUri,
      string path)
    {
      this.Uri = uri;
      this.Name = name;
      this.StructureType = structureType;
      this.Properties = (Property[]) properties.Clone();
      this.Path = path;
      this.ParentUri = parentUri;
      this.ProjectUri = projectUri;
    }

    public NodeInfo(
      string uri,
      string name,
      string structureType,
      Property[] properties,
      string parentUri,
      string projectUri,
      string path,
      DateTime? startDate,
      DateTime? finishDate)
      : this(uri, name, structureType, properties, parentUri, projectUri, path)
    {
      this.StartDate = startDate;
      this.FinishDate = finishDate;
    }

    public NodeInfo()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static NodeInfo FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      NodeInfo nodeInfo = new NodeInfo();
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
              case 3:
                if (name2 == "Uri")
                {
                  nodeInfo.Uri = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 4:
                switch (name2[0])
                {
                  case 'N':
                    if (name2 == "Name")
                    {
                      nodeInfo.Name = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'P':
                    if (name2 == "Path")
                    {
                      nodeInfo.Path = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                }
                break;
              case 9:
                switch (name2[0])
                {
                  case 'P':
                    if (name2 == "ParentUri")
                    {
                      nodeInfo.ParentUri = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'S':
                    if (name2 == "StartDate")
                    {
                      nodeInfo.StartDate = Helper.NullableOfDateTimeFromXml(serviceProvider, reader);
                      continue;
                    }
                    break;
                }
                break;
              case 10:
                switch (name2[3])
                {
                  case 'i':
                    if (name2 == "FinishDate")
                    {
                      nodeInfo.FinishDate = Helper.NullableOfDateTimeFromXml(serviceProvider, reader);
                      continue;
                    }
                    break;
                  case 'j':
                    if (name2 == "ProjectUri")
                    {
                      nodeInfo.ProjectUri = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'p':
                    if (name2 == "Properties")
                    {
                      nodeInfo.Properties = Helper.ArrayOfPropertyFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                }
                break;
              case 13:
                if (name2 == "StructureType")
                {
                  nodeInfo.StructureType = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
            }
          }
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return nodeInfo;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("NodeInfo instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  FinishDate: " + this.FinishDate.ToString());
      stringBuilder.AppendLine("  Name: " + this.Name);
      stringBuilder.AppendLine("  ParentUri: " + this.ParentUri);
      stringBuilder.AppendLine("  Path: " + this.Path);
      stringBuilder.AppendLine("  ProjectUri: " + this.ProjectUri);
      stringBuilder.AppendLine("  Properties: " + Helper.ArrayToString<Property>(this.Properties));
      stringBuilder.AppendLine("  StartDate: " + this.StartDate.ToString());
      stringBuilder.AppendLine("  StructureType: " + this.StructureType);
      stringBuilder.AppendLine("  Uri: " + this.Uri);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.FinishDate.HasValue)
        XmlUtility.DateToXmlElement(writer, "FinishDate", this.FinishDate.Value);
      if (this.Name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.Name);
      if (this.ParentUri != null)
        XmlUtility.ToXmlElement(writer, "ParentUri", this.ParentUri);
      if (this.Path != null)
        XmlUtility.ToXmlElement(writer, "Path", this.Path);
      if (this.ProjectUri != null)
        XmlUtility.ToXmlElement(writer, "ProjectUri", this.ProjectUri);
      Helper.ToXml(writer, "Properties", this.Properties, false, false);
      if (this.StartDate.HasValue)
        XmlUtility.DateToXmlElement(writer, "StartDate", this.StartDate.Value);
      if (this.StructureType != null)
        XmlUtility.ToXmlElement(writer, "StructureType", this.StructureType);
      if (this.Uri != null)
        XmlUtility.ToXmlElement(writer, "Uri", this.Uri);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, NodeInfo obj) => obj.ToXml(writer, element);
  }
}
