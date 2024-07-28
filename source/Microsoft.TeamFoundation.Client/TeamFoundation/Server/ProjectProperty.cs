// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ProjectProperty
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
  public class ProjectProperty
  {
    private string m_name;
    private string m_value;

    public ProjectProperty(string name, string value)
    {
      this.m_name = name;
      this.m_value = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    public ProjectProperty()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ProjectProperty FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ProjectProperty projectProperty = new ProjectProperty();
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
              projectProperty.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Value":
              projectProperty.m_value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return projectProperty;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ProjectProperty instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Value: " + this.m_value);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.m_value);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ProjectProperty obj) => obj.ToXml(writer, element);
  }
}
