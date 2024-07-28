// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AceExtendedInformation
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
  public sealed class AceExtendedInformation
  {
    private int m_effectiveAllow;
    private int m_effectiveDeny;
    private int m_inheritedAllow;
    private int m_inheritedDeny;

    private AceExtendedInformation()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AceExtendedInformation FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      AceExtendedInformation extendedInformation = new AceExtendedInformation();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "EffectiveAllow":
              extendedInformation.m_effectiveAllow = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "EffectiveDeny":
              extendedInformation.m_effectiveDeny = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "InheritedAllow":
              extendedInformation.m_inheritedAllow = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "InheritedDeny":
              extendedInformation.m_inheritedDeny = XmlUtility.Int32FromXmlAttribute(reader);
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
      return extendedInformation;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AceExtendedInformation instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  EffectiveAllow: " + this.m_effectiveAllow.ToString());
      stringBuilder.AppendLine("  EffectiveDeny: " + this.m_effectiveDeny.ToString());
      stringBuilder.AppendLine("  InheritedAllow: " + this.m_inheritedAllow.ToString());
      stringBuilder.AppendLine("  InheritedDeny: " + this.m_inheritedDeny.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_effectiveAllow != 0)
        XmlUtility.ToXmlAttribute(writer, "EffectiveAllow", this.m_effectiveAllow);
      if (this.m_effectiveDeny != 0)
        XmlUtility.ToXmlAttribute(writer, "EffectiveDeny", this.m_effectiveDeny);
      if (this.m_inheritedAllow != 0)
        XmlUtility.ToXmlAttribute(writer, "InheritedAllow", this.m_inheritedAllow);
      if (this.m_inheritedDeny != 0)
        XmlUtility.ToXmlAttribute(writer, "InheritedDeny", this.m_inheritedDeny);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AceExtendedInformation obj) => obj.ToXml(writer, element);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public AceExtendedInformation(
      int inheritedAllow,
      int inheritedDeny,
      int effectiveAllow,
      int effectiveDeny)
    {
      this.m_inheritedAllow = inheritedAllow;
      this.m_inheritedDeny = inheritedDeny;
      this.m_effectiveAllow = effectiveAllow;
      this.m_effectiveDeny = effectiveDeny;
    }

    public int InheritedAllow => this.m_inheritedAllow;

    public int InheritedDeny => this.m_inheritedDeny;

    public int EffectiveAllow => this.m_effectiveAllow;

    public int EffectiveDeny => this.m_effectiveDeny;
  }
}
