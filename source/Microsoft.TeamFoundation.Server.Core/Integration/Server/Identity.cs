// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.Identity
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [ClassNotSealed]
  [Serializable]
  public class Identity : IComparable
  {
    private IdentityType mType;
    private string mSid;
    private Guid mTeamFoundationId;
    private string mDisplayName;
    private string mDescription;
    private string mDomain;
    private string mAccountName;
    private string mDistinguishedName;
    private string mMailAddress;
    private ApplicationGroupSpecialType mSpecialType;
    private bool mDeleted;
    private bool mSecurityGroup;
    private string[] mMembers;
    private string[] mMemberOf;

    public Identity()
    {
      this.mType = IdentityType.InvalidIdentity;
      this.mSid = string.Empty;
      this.mTeamFoundationId = Guid.Empty;
      this.mDisplayName = string.Empty;
      this.mDescription = string.Empty;
      this.mDomain = string.Empty;
      this.mAccountName = string.Empty;
      this.mDistinguishedName = string.Empty;
      this.mMailAddress = string.Empty;
      this.mSpecialType = ApplicationGroupSpecialType.Generic;
      this.mDeleted = false;
      this.mSecurityGroup = false;
      this.mMembers = (string[]) null;
      this.mMemberOf = (string[]) null;
    }

    public static Microsoft.TeamFoundation.Integration.Server.Identity Convert(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity)
    {
      if (identity == null)
        return (Microsoft.TeamFoundation.Integration.Server.Identity) null;
      Microsoft.TeamFoundation.Integration.Server.Identity identity1 = new Microsoft.TeamFoundation.Integration.Server.Identity();
      identity1.Type = Microsoft.TeamFoundation.Integration.Server.Identity.GetType(identity.Descriptor.IdentityType, identity.IsContainer);
      identity1.SpecialType = (ApplicationGroupSpecialType) IdentityUtil.GetGroupSpecialType(identity);
      identity1.Sid = identity.Descriptor.Identifier;
      identity1.TeamFoundationId = identity.TeamFoundationId;
      identity1.Description = identity.GetAttribute("Description", string.Empty);
      identity1.Domain = identity.GetAttribute("Domain", string.Empty);
      if (!string.IsNullOrEmpty(identity.GetAttribute("GlobalScope", (string) null)) && requestContext.GetService<TeamFoundationIdentityService>().Domain.IsOwner(identity.Descriptor))
        identity1.Domain = string.Empty;
      identity1.AccountName = identity.GetAttribute("Account", string.Empty);
      identity1.DistinguishedName = identity.GetAttribute("DN", string.Empty);
      identity1.MailAddress = identity.GetAttribute("Mail", string.Empty);
      identity1.Deleted = !identity.IsActive;
      identity1.SecurityGroup = !string.IsNullOrEmpty(identity.GetAttribute("SecurityGroup", (string) null));
      identity1.DisplayName = identity1.Type != IdentityType.ApplicationGroup ? identity.DisplayName : identity1.AccountName;
      if (identity.Members != null)
      {
        identity1.Members = new string[identity.Members.Count];
        int num = 0;
        foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) identity.Members)
          identity1.Members[num++] = member.Identifier;
      }
      else
        identity1.Members = Array.Empty<string>();
      if (identity.MemberOf != null)
      {
        identity1.MemberOf = new string[identity.MemberOf.Count];
        int num = 0;
        foreach (IdentityDescriptor identityDescriptor in (IEnumerable<IdentityDescriptor>) identity.MemberOf)
          identity1.MemberOf[num++] = identityDescriptor.Identifier;
      }
      else
        identity1.MemberOf = Array.Empty<string>();
      return identity1;
    }

    public static IdentityType GetType(string identityType, bool isGroup)
    {
      if (string.Equals(identityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return IdentityType.ApplicationGroup;
      if (!string.Equals(identityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
        return IdentityType.UnknownIdentityType;
      return isGroup ? IdentityType.WindowsGroup : IdentityType.WindowsUser;
    }

    public int CompareTo(object obj) => obj is Microsoft.TeamFoundation.Integration.Server.Identity identity ? VssStringComparer.IdentityName.Compare(this.mDisplayName, identity.mDisplayName) : -1;

    public override bool Equals(object obj) => obj is Microsoft.TeamFoundation.Integration.Server.Identity identity && this.mSid == identity.Sid;

    public override int GetHashCode() => this.mSid.GetHashCode();

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public IdentityType Type
    {
      get => this.mType;
      set => this.mType = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Sid
    {
      get => this.mSid;
      set => this.mSid = value;
    }

    [XmlIgnore]
    public Guid TeamFoundationId
    {
      get => this.mTeamFoundationId;
      set => this.mTeamFoundationId = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string DisplayName
    {
      get => this.mDisplayName;
      set => this.mDisplayName = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Description
    {
      get => this.mDescription;
      set => this.mDescription = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Domain
    {
      get => this.mDomain;
      set => this.mDomain = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string AccountName
    {
      get => this.mAccountName;
      set => this.mAccountName = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string DistinguishedName
    {
      get => this.mDistinguishedName;
      set => this.mDistinguishedName = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string MailAddress
    {
      get => this.mMailAddress;
      set => this.mMailAddress = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public ApplicationGroupSpecialType SpecialType
    {
      get => this.mSpecialType;
      set => this.mSpecialType = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public bool Deleted
    {
      get => this.mDeleted;
      set => this.mDeleted = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string[] Members
    {
      get => this.mMembers;
      set => this.mMembers = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string[] MemberOf
    {
      get => this.mMemberOf;
      set => this.mMemberOf = value;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(256);
      if (this.Sid != null)
      {
        stringBuilder.Append('[');
        stringBuilder.Append(this.Sid);
        stringBuilder.Append("] ");
      }
      switch (this.Type)
      {
        case IdentityType.InvalidIdentity:
          return "(invalid)";
        case IdentityType.UnknownIdentityType:
          stringBuilder.Append("?: ");
          stringBuilder.Append(this.Domain);
          stringBuilder.Append("\\");
          stringBuilder.Append(this.AccountName);
          stringBuilder.Append(" (");
          stringBuilder.Append(this.DisplayName);
          stringBuilder.Append(")");
          break;
        case IdentityType.WindowsUser:
          stringBuilder.Append("U: ");
          stringBuilder.Append(this.Domain);
          stringBuilder.Append("\\");
          stringBuilder.Append(this.AccountName);
          stringBuilder.Append(" (");
          stringBuilder.Append(this.DisplayName);
          stringBuilder.Append(")");
          if (this.MemberOf != null)
          {
            stringBuilder.Append(" Mof:");
            stringBuilder.Append(this.MemberOf.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            break;
          }
          break;
        case IdentityType.WindowsGroup:
          stringBuilder.Append("G: ");
          stringBuilder.Append(this.Domain);
          stringBuilder.Append("\\");
          stringBuilder.Append(this.AccountName);
          stringBuilder.Append(" (");
          stringBuilder.Append(this.DisplayName);
          stringBuilder.Append(")");
          if (this.Members != null)
          {
            stringBuilder.Append(" M:");
            stringBuilder.Append(this.Members.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          if (this.MemberOf != null)
          {
            stringBuilder.Append(" Mof:");
            stringBuilder.Append(this.MemberOf.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            break;
          }
          break;
        case IdentityType.ApplicationGroup:
          stringBuilder.Append("A: ");
          stringBuilder.Append(this.DisplayName);
          if (this.Members != null)
          {
            stringBuilder.Append(" M:");
            stringBuilder.Append(this.Members.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          if (this.MemberOf != null)
          {
            stringBuilder.Append(" Mof:");
            stringBuilder.Append(this.MemberOf.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            break;
          }
          break;
      }
      return stringBuilder.ToString();
    }

    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public bool SecurityGroup
    {
      get => this.mSecurityGroup;
      set => this.mSecurityGroup = value;
    }

    public void ReadXml(XmlReader reader, string xmlElement)
    {
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name1 = reader.Name;
        }
      }
      reader.Read();
      if (isEmptyElement)
        return;
      while (reader.NodeType == XmlNodeType.Element)
      {
        string name2 = reader.Name;
        if (name2 != null)
        {
          switch (name2.Length)
          {
            case 3:
              if (name2 == "Sid")
              {
                this.mSid = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                continue;
              }
              break;
            case 4:
              if (name2 == "Type")
              {
                this.mType = (IdentityType) Microsoft.TeamFoundation.Integration.Server.Identity.EnumFromXmlElem(reader, typeof (IdentityType));
                continue;
              }
              break;
            case 6:
              if (name2 == "Domain")
              {
                this.mDomain = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                continue;
              }
              break;
            case 7:
              switch (name2[0])
              {
                case 'D':
                  if (name2 == "Deleted")
                  {
                    this.mDeleted = Microsoft.TeamFoundation.Integration.Server.Identity.BooleanFromXmlElem(reader);
                    continue;
                  }
                  break;
                case 'M':
                  if (name2 == "Members")
                  {
                    this.mMembers = Microsoft.TeamFoundation.Integration.Server.Identity.StringArrayFromXml(reader);
                    continue;
                  }
                  break;
              }
              break;
            case 8:
              if (name2 == "MemberOf")
              {
                this.mMemberOf = Microsoft.TeamFoundation.Integration.Server.Identity.StringArrayFromXml(reader);
                continue;
              }
              break;
            case 11:
              switch (name2[1])
              {
                case 'a':
                  if (name2 == "MailAddress")
                  {
                    this.mMailAddress = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'c':
                  if (name2 == "AccountName")
                  {
                    this.mAccountName = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'e':
                  if (name2 == "Description")
                  {
                    this.mDescription = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'i':
                  if (name2 == "DisplayName")
                  {
                    this.mDisplayName = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'p':
                  if (name2 == "SpecialType")
                  {
                    this.mSpecialType = (ApplicationGroupSpecialType) Microsoft.TeamFoundation.Integration.Server.Identity.EnumFromXmlElem(reader, typeof (ApplicationGroupSpecialType));
                    continue;
                  }
                  break;
              }
              break;
            case 13:
              if (name2 == "SecurityGroup")
              {
                this.mSecurityGroup = Microsoft.TeamFoundation.Integration.Server.Identity.BooleanFromXmlElem(reader);
                continue;
              }
              break;
            case 17:
              if (name2 == "DistinguishedName")
              {
                this.mDistinguishedName = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader);
                continue;
              }
              break;
          }
        }
        reader.Read();
      }
      reader.ReadEndElement();
    }

    private static string StringFromXml(XmlReader reader)
    {
      string empty = string.Empty;
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        if (reader.NodeType == XmlNodeType.Text)
        {
          empty = reader.Value;
          reader.Read();
        }
        reader.ReadEndElement();
      }
      return empty.Replace("\n", "\r\n");
    }

    private static object EnumFromXmlElem(XmlReader reader, System.Type type)
    {
      string str = Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader).Replace(' ', ',');
      return Enum.Parse(type, str, true);
    }

    private static bool BooleanFromXmlElem(XmlReader reader) => bool.Parse(Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader));

    private static string[] StringArrayFromXml(XmlReader reader)
    {
      List<string> stringList = new List<string>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("xsi:nil") == "true")
          {
            stringList.Add((string) null);
            reader.Read();
          }
          else
            stringList.Add(Microsoft.TeamFoundation.Integration.Server.Identity.StringFromXml(reader));
        }
        reader.ReadEndElement();
      }
      return stringList.ToArray();
    }

    public void WriteXml(XmlWriter writer, string xmlElement)
    {
      writer.WriteStartElement(xmlElement);
      if (this.mType != IdentityType.InvalidIdentity)
        Microsoft.TeamFoundation.Integration.Server.Identity.EnumToXmlElement(writer, "Type", (object) this.mType, typeof (IdentityType));
      if (this.mSid != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "Sid", this.mSid);
      if (this.mDisplayName != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "DisplayName", this.mDisplayName);
      if (this.mDescription != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "Description", this.mDescription);
      if (this.mDomain != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "Domain", this.mDomain);
      if (this.mAccountName != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "AccountName", this.mAccountName);
      if (this.mDistinguishedName != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "DistinguishedName", this.mDistinguishedName);
      if (this.mMailAddress != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "MailAddress", this.mMailAddress);
      if (this.mSpecialType != ApplicationGroupSpecialType.Generic)
        Microsoft.TeamFoundation.Integration.Server.Identity.EnumToXmlElement(writer, "SpecialType", (object) this.mSpecialType, typeof (ApplicationGroupSpecialType));
      if (this.mDeleted)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "Deleted", XmlConvert.ToString(this.mDeleted));
      if (this.mMembers != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringArrayToXmlElement(writer, "Members", this.mMembers);
      if (this.mMemberOf != null)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringArrayToXmlElement(writer, "MemberOf", this.mMemberOf);
      if (this.mSecurityGroup)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "SecurityGroup", XmlConvert.ToString(this.mSecurityGroup));
      writer.WriteEndElement();
    }

    private static void StringToXmlElement(XmlWriter writer, string element, string str)
    {
      if (str == null)
        return;
      writer.WriteElementString(element, str);
    }

    private static void EnumToXmlElement(
      XmlWriter writer,
      string element,
      object value,
      System.Type type)
    {
      string str = Enum.Format(type, value, "G").Replace(",", " ");
      writer.WriteElementString(element, str);
    }

    private static void StringArrayToXmlElement(XmlWriter writer, string element, string[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
        Microsoft.TeamFoundation.Integration.Server.Identity.StringToXmlElement(writer, "string", array[index]);
      writer.WriteEndElement();
    }
  }
}
