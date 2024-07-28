// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessControlListDetails
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class AccessControlListDetails : AccessControlList
  {
    internal AccessControlEntryDetails[] m_entries = Helper.ZeroLengthArrayOfAccessControlEntryDetails;
    private bool m_includeExtendedInfo;
    private bool m_inheritPermissions;
    private string m_token;

    internal AccessControlListDetails()
    {
    }

    internal AccessControlEntryDetails[] Entries
    {
      get => (AccessControlEntryDetails[]) this.m_entries.Clone();
      set => this.m_entries = value;
    }

    internal bool IncludeExtendedInfo
    {
      get => this.m_includeExtendedInfo;
      set => this.m_includeExtendedInfo = value;
    }

    internal static AccessControlListDetails FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      AccessControlListDetails controlListDetails = new AccessControlListDetails();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "IncludeExtendedInfo":
              controlListDetails.m_includeExtendedInfo = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "InheritPermissions":
              controlListDetails.m_inheritPermissions = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "Token":
              controlListDetails.m_token = XmlUtility.StringFromXmlAttribute(reader);
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
          if (reader.Name == "AccessControlEntries")
            controlListDetails.m_entries = Helper.ArrayOfAccessControlEntryDetailsFromXml(serviceProvider, reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      controlListDetails.InitializeFromWebService();
      return controlListDetails;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessControlListDetails instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Entries: " + Helper.ArrayToString<AccessControlEntryDetails>(this.m_entries));
      stringBuilder.AppendLine("  IncludeExtendedInfo: " + this.m_includeExtendedInfo.ToString());
      stringBuilder.AppendLine("  InheritPermissions: " + this.m_inheritPermissions.ToString());
      stringBuilder.AppendLine("  Token: " + this.m_token);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_includeExtendedInfo)
        XmlUtility.ToXmlAttribute(writer, "IncludeExtendedInfo", this.m_includeExtendedInfo);
      if (this.m_inheritPermissions)
        XmlUtility.ToXmlAttribute(writer, "InheritPermissions", this.m_inheritPermissions);
      if (this.m_token != null)
        XmlUtility.ToXmlAttribute(writer, "Token", this.m_token);
      Helper.ToXml(writer, "AccessControlEntries", this.m_entries, false, false);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, AccessControlListDetails obj) => obj.ToXml(writer, element);

    internal void InitializeFromWebService()
    {
      this.Token = this.m_token;
      this.InheritPermissions = this.m_inheritPermissions;
      this.IncludeExtendedInfoForAces = this.m_includeExtendedInfo;
      foreach (AccessControlEntry entry in this.m_entries)
        this.LoadAce(entry);
    }

    internal static AccessControlListDetails PrepareForWebServiceSerialization(
      AccessControlList accessControlList)
    {
      if (accessControlList == null)
        return (AccessControlListDetails) null;
      if (!(accessControlList is AccessControlListDetails controlListDetails))
        controlListDetails = new AccessControlListDetails();
      controlListDetails.m_token = accessControlList.Token;
      controlListDetails.m_inheritPermissions = accessControlList.InheritPermissions;
      controlListDetails.m_entries = AccessControlEntryDetails.PrepareForWebServiceSerialization(accessControlList.Token, accessControlList.AccessControlEntries);
      return controlListDetails;
    }
  }
}
