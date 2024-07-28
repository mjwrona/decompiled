// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessControlEntryDetails
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AccessControlEntryDetails : AccessControlEntry
  {
    private int m_allow;
    private int m_deny;
    private IdentityDescriptor m_descriptor;
    private AceExtendedInformation m_extendedInformation;
    private string m_token;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessControlEntryDetails FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      AccessControlEntryDetails controlEntryDetails = new AccessControlEntryDetails();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "Allow":
              controlEntryDetails.m_allow = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "Deny":
              controlEntryDetails.m_deny = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "Token":
              controlEntryDetails.m_token = XmlUtility.StringFromXmlAttribute(reader);
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
          switch (reader.Name)
          {
            case "SerializableDescriptor":
              controlEntryDetails.m_descriptor = IdentityDescriptor.FromXml(serviceProvider, reader);
              continue;
            case "ExtendedInformation":
              controlEntryDetails.m_extendedInformation = AceExtendedInformation.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      controlEntryDetails.InitializeFromWebService();
      return controlEntryDetails;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessControlEntryDetails instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Allow: " + this.m_allow.ToString());
      stringBuilder.AppendLine("  Deny: " + this.m_deny.ToString());
      stringBuilder.AppendLine("  Descriptor: " + this.m_descriptor?.ToString());
      stringBuilder.AppendLine("  ExtendedInformation: " + this.m_extendedInformation?.ToString());
      stringBuilder.AppendLine("  Token: " + this.m_token);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_allow != 0)
        XmlUtility.ToXmlAttribute(writer, "Allow", this.m_allow);
      if (this.m_deny != 0)
        XmlUtility.ToXmlAttribute(writer, "Deny", this.m_deny);
      if (this.m_token != null)
        XmlUtility.ToXmlAttribute(writer, "Token", this.m_token);
      if (this.m_descriptor != null)
        IdentityDescriptor.ToXml(writer, "SerializableDescriptor", this.m_descriptor);
      if (this.m_extendedInformation != null)
        AceExtendedInformation.ToXml(writer, "ExtendedInformation", this.m_extendedInformation);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AccessControlEntryDetails obj) => obj.ToXml(writer, element);

    internal void InitializeFromWebService()
    {
      this.ExtendedInfo = this.m_extendedInformation;
      this.Descriptor = this.m_descriptor;
      this.Allow = this.m_allow;
      this.Deny = this.m_deny;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessControlEntryDetails[] PrepareForWebServiceSerialization(
      string token,
      IEnumerable<AccessControlEntry> entries)
    {
      if (entries == null)
        return (AccessControlEntryDetails[]) null;
      List<AccessControlEntryDetails> controlEntryDetailsList = new List<AccessControlEntryDetails>();
      foreach (AccessControlEntry entry in entries)
        controlEntryDetailsList.Add(AccessControlEntryDetails.PrepareForWebServiceSerialization(token, entry));
      return controlEntryDetailsList.ToArray();
    }

    internal static AccessControlEntryDetails PrepareForWebServiceSerialization(
      string token,
      AccessControlEntry entry)
    {
      if (entry == null)
        return (AccessControlEntryDetails) null;
      if (!(entry is AccessControlEntryDetails controlEntryDetails))
        controlEntryDetails = new AccessControlEntryDetails();
      controlEntryDetails.m_allow = entry.Allow;
      controlEntryDetails.m_deny = entry.Deny;
      controlEntryDetails.m_descriptor = entry.Descriptor;
      controlEntryDetails.m_token = token;
      return controlEntryDetails;
    }
  }
}
