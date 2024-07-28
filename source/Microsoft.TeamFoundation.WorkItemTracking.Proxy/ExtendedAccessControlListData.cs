// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ExtendedAccessControlListData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public sealed class ExtendedAccessControlListData
  {
    private bool m_inheritPermissions;
    private bool m_isEditable;
    internal AccessControlListMetadata[] m_metadata = Helper.ZeroLengthArrayOfAccessControlListMetadata;
    internal AccessControlEntryData[] m_permissions = Helper.ZeroLengthArrayOfAccessControlEntryData;
    private string m_token;

    private ExtendedAccessControlListData()
    {
    }

    public bool InheritPermissions
    {
      get => this.m_inheritPermissions;
      set => this.m_inheritPermissions = value;
    }

    public bool IsEditable
    {
      get => this.m_isEditable;
      set => this.m_isEditable = value;
    }

    public AccessControlListMetadata[] Metadata
    {
      get => (AccessControlListMetadata[]) this.m_metadata.Clone();
      set => this.m_metadata = value;
    }

    public AccessControlEntryData[] Permissions
    {
      get => (AccessControlEntryData[]) this.m_permissions.Clone();
      set => this.m_permissions = value;
    }

    public string Token
    {
      get => this.m_token;
      set => this.m_token = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ExtendedAccessControlListData FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      ExtendedAccessControlListData accessControlListData = new ExtendedAccessControlListData();
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
            case "InheritPermissions":
              accessControlListData.m_inheritPermissions = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "IsEditable":
              accessControlListData.m_isEditable = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "Metadata":
              accessControlListData.m_metadata = Helper.ArrayOfAccessControlListMetadataFromXml(serviceProvider, reader, false);
              continue;
            case "Permissions":
              accessControlListData.m_permissions = Helper.ArrayOfAccessControlEntryDataFromXml(serviceProvider, reader, false);
              continue;
            case "Token":
              accessControlListData.m_token = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return accessControlListData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ExtendedAccessControlListData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  InheritPermissions: " + this.m_inheritPermissions.ToString());
      stringBuilder.AppendLine("  IsEditable: " + this.m_isEditable.ToString());
      stringBuilder.AppendLine("  Metadata: " + Helper.ArrayToString<AccessControlListMetadata>(this.m_metadata));
      stringBuilder.AppendLine("  Permissions: " + Helper.ArrayToString<AccessControlEntryData>(this.m_permissions));
      stringBuilder.AppendLine("  Token: " + this.m_token);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_inheritPermissions)
        XmlUtility.ToXmlElement(writer, "InheritPermissions", this.m_inheritPermissions);
      if (this.m_isEditable)
        XmlUtility.ToXmlElement(writer, "IsEditable", this.m_isEditable);
      Helper.ToXml(writer, "Metadata", this.m_metadata, false, false);
      Helper.ToXml(writer, "Permissions", this.m_permissions, false, false);
      if (this.m_token != null)
        XmlUtility.ToXmlElement(writer, "Token", this.m_token);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ExtendedAccessControlListData obj) => obj.ToXml(writer, element);
  }
}
