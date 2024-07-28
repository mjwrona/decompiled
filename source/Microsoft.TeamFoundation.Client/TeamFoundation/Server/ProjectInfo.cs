// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ProjectInfo
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class ProjectInfo
  {
    public string Uri;
    public string Name;
    public ProjectState Status;

    public ProjectInfo(string uri, string name, ProjectState status)
    {
      this.Uri = uri;
      this.Name = name;
      this.Status = status;
    }

    public override bool Equals(object that) => that != null && that is ProjectInfo && this.Uri == ((ProjectInfo) that).Uri;

    public override int GetHashCode() => this.Uri.GetHashCode();

    public ProjectInfo()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ProjectInfo FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ProjectInfo projectInfo = new ProjectInfo();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Name":
              projectInfo.Name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Status":
              projectInfo.Status = XmlUtility.EnumFromXmlElement<ProjectState>(reader);
              continue;
            case "Uri":
              projectInfo.Uri = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return projectInfo;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ProjectInfo instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.Name);
      stringBuilder.AppendLine("  Status: " + this.Status.ToString());
      stringBuilder.AppendLine("  Uri: " + this.Uri);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.Name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.Name);
      if (this.Status != ProjectState.New)
        XmlUtility.EnumToXmlElement<ProjectState>(writer, "Status", this.Status);
      if (this.Uri != null)
        XmlUtility.ToXmlElement(writer, "Uri", this.Uri);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ProjectInfo obj) => obj.ToXml(writer, element);
  }
}
