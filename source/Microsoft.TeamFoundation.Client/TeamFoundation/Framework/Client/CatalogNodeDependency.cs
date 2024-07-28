// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogNodeDependency
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class CatalogNodeDependency
  {
    private string m_associationKey;
    private string m_fullPath;
    private bool m_isSingleton;
    private string m_requiredNodeFullPath;

    internal CatalogNodeDependency()
    {
    }

    public string AssociationKey
    {
      get => this.m_associationKey;
      set => this.m_associationKey = value;
    }

    public string FullPath
    {
      get => this.m_fullPath;
      set => this.m_fullPath = value;
    }

    public bool IsSingleton
    {
      get => this.m_isSingleton;
      set => this.m_isSingleton = value;
    }

    public string RequiredNodeFullPath
    {
      get => this.m_requiredNodeFullPath;
      set => this.m_requiredNodeFullPath = value;
    }

    internal static CatalogNodeDependency FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      CatalogNodeDependency catalogNodeDependency = new CatalogNodeDependency();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AssociationKey":
              catalogNodeDependency.m_associationKey = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "FullPath":
              catalogNodeDependency.m_fullPath = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "IsSingleton":
              catalogNodeDependency.m_isSingleton = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "RequiredNodeFullPath":
              catalogNodeDependency.m_requiredNodeFullPath = XmlUtility.StringFromXmlAttribute(reader);
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
      return catalogNodeDependency;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CatalogNodeDependency instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AssociationKey: " + this.m_associationKey);
      stringBuilder.AppendLine("  FullPath: " + this.m_fullPath);
      stringBuilder.AppendLine("  IsSingleton: " + this.m_isSingleton.ToString());
      stringBuilder.AppendLine("  RequiredNodeFullPath: " + this.m_requiredNodeFullPath);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_associationKey != null)
        XmlUtility.ToXmlAttribute(writer, "AssociationKey", this.m_associationKey);
      if (this.m_fullPath != null)
        XmlUtility.ToXmlAttribute(writer, "FullPath", this.m_fullPath);
      if (this.m_isSingleton)
        XmlUtility.ToXmlAttribute(writer, "IsSingleton", this.m_isSingleton);
      if (this.m_requiredNodeFullPath != null)
        XmlUtility.ToXmlAttribute(writer, "RequiredNodeFullPath", this.m_requiredNodeFullPath);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, CatalogNodeDependency obj) => obj.ToXml(writer, element);
  }
}
