// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ActionDefinition
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ActionDefinition
  {
    private int m_bit;
    private string m_displayName;
    private string m_name;

    private ActionDefinition()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionDefinition FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ActionDefinition actionDefinition = new ActionDefinition();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "bit":
              actionDefinition.m_bit = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "displayName":
              actionDefinition.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "name":
              actionDefinition.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return actionDefinition;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ActionDefinition instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Bit: " + this.m_bit.ToString());
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  Name: " + this.m_name);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_bit != 0)
        XmlUtility.ToXmlAttribute(writer, "bit", this.m_bit);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "displayName", this.m_displayName);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ActionDefinition obj) => obj.ToXml(writer, element);

    public ActionDefinition(int bit, string name, string displayName)
    {
      this.m_bit = bit;
      this.m_name = name;
      this.m_displayName = displayName;
    }

    public int Bit => this.m_bit;

    public string Name => this.m_name;

    public string DisplayName => this.m_displayName;
  }
}
