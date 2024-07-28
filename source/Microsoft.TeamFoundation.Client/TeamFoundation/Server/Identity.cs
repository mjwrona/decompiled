// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Identity
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03")]
  [Serializable]
  public class Identity : IComparable, ITfsXmlSerializable
  {
    private IdentityType mType;
    private string mSid = string.Empty;
    private Guid mTeamFoundationId = Guid.Empty;
    private string mDisplayName = string.Empty;
    private string mDescription = string.Empty;
    private string mDomain = string.Empty;
    private string mAccountName = string.Empty;
    private string mDistinguishedName = string.Empty;
    private string mMailAddress = string.Empty;
    private ApplicationGroupSpecialType mSpecialType;
    private bool mDeleted;
    private bool mSecurityGroup;
    private string[] mMembers;
    private string[] mMemberOf;

    public static IdentityType GetType(string identityType, bool isGroup)
    {
      if (string.Equals(identityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return IdentityType.ApplicationGroup;
      if (!string.Equals(identityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
        return IdentityType.UnknownIdentityType;
      return isGroup ? IdentityType.WindowsGroup : IdentityType.WindowsUser;
    }

    public int CompareTo(object obj) => obj is Identity identity ? VssStringComparer.IdentityName.Compare(this.mDisplayName, identity.mDisplayName) : -1;

    public override bool Equals(object obj) => obj is Identity identity && this.mSid == identity.Sid;

    public override int GetHashCode() => this.mSid.GetHashCode();

    public IdentityType Type
    {
      get => this.mType;
      set => this.mType = value;
    }

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

    public string DisplayName
    {
      get => this.mDisplayName;
      set => this.mDisplayName = value;
    }

    public string Description
    {
      get => this.mDescription;
      set => this.mDescription = value;
    }

    public string Domain
    {
      get => this.mDomain;
      set => this.mDomain = value;
    }

    public string AccountName
    {
      get => this.mAccountName;
      set => this.mAccountName = value;
    }

    public string DistinguishedName
    {
      get => this.mDistinguishedName;
      set => this.mDistinguishedName = value;
    }

    public string MailAddress
    {
      get => this.mMailAddress;
      set => this.mMailAddress = value;
    }

    public ApplicationGroupSpecialType SpecialType
    {
      get => this.mSpecialType;
      set => this.mSpecialType = value;
    }

    public bool Deleted
    {
      get => this.mDeleted;
      set => this.mDeleted = value;
    }

    public string[] Members
    {
      get => this.mMembers;
      set => this.mMembers = value;
    }

    public string[] MemberOf
    {
      get => this.mMemberOf;
      set => this.mMemberOf = value;
    }

    [DefaultValue(false)]
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
                this.mSid = Identity.StringFromXml(reader);
                continue;
              }
              break;
            case 4:
              if (name2 == "Type")
              {
                this.mType = (IdentityType) Identity.EnumFromXmlElem(reader, typeof (IdentityType));
                continue;
              }
              break;
            case 6:
              if (name2 == "Domain")
              {
                this.mDomain = Identity.StringFromXml(reader);
                continue;
              }
              break;
            case 7:
              switch (name2[0])
              {
                case 'D':
                  if (name2 == "Deleted")
                  {
                    this.mDeleted = Identity.BooleanFromXmlElem(reader);
                    continue;
                  }
                  break;
                case 'M':
                  if (name2 == "Members")
                  {
                    this.mMembers = Identity.StringArrayFromXml(reader);
                    continue;
                  }
                  break;
              }
              break;
            case 8:
              if (name2 == "MemberOf")
              {
                this.mMemberOf = Identity.StringArrayFromXml(reader);
                continue;
              }
              break;
            case 11:
              switch (name2[1])
              {
                case 'a':
                  if (name2 == "MailAddress")
                  {
                    this.mMailAddress = Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'c':
                  if (name2 == "AccountName")
                  {
                    this.mAccountName = Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'e':
                  if (name2 == "Description")
                  {
                    this.mDescription = Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'i':
                  if (name2 == "DisplayName")
                  {
                    this.mDisplayName = Identity.StringFromXml(reader);
                    continue;
                  }
                  break;
                case 'p':
                  if (name2 == "SpecialType")
                  {
                    this.mSpecialType = (ApplicationGroupSpecialType) Identity.EnumFromXmlElem(reader, typeof (ApplicationGroupSpecialType));
                    continue;
                  }
                  break;
              }
              break;
            case 13:
              if (name2 == "SecurityGroup")
              {
                this.mSecurityGroup = Identity.BooleanFromXmlElem(reader);
                continue;
              }
              break;
            case 17:
              if (name2 == "DistinguishedName")
              {
                this.mDistinguishedName = Identity.StringFromXml(reader);
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
      string str = Identity.StringFromXml(reader).Replace(' ', ',');
      return Enum.Parse(type, str, true);
    }

    private static bool BooleanFromXmlElem(XmlReader reader) => bool.Parse(Identity.StringFromXml(reader));

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
            stringList.Add(Identity.StringFromXml(reader));
        }
        reader.ReadEndElement();
      }
      return stringList.ToArray();
    }

    public void WriteXml(XmlWriter writer, string xmlElement)
    {
      writer.WriteStartElement(xmlElement);
      if (this.mType != IdentityType.InvalidIdentity)
        Identity.EnumToXmlElement(writer, "Type", (object) this.mType, typeof (IdentityType));
      if (this.mSid != null)
        Identity.StringToXmlElement(writer, "Sid", this.mSid);
      if (this.mDisplayName != null)
        Identity.StringToXmlElement(writer, "DisplayName", this.mDisplayName);
      if (this.mDescription != null)
        Identity.StringToXmlElement(writer, "Description", this.mDescription);
      if (this.mDomain != null)
        Identity.StringToXmlElement(writer, "Domain", this.mDomain);
      if (this.mAccountName != null)
        Identity.StringToXmlElement(writer, "AccountName", this.mAccountName);
      if (this.mDistinguishedName != null)
        Identity.StringToXmlElement(writer, "DistinguishedName", this.mDistinguishedName);
      if (this.mMailAddress != null)
        Identity.StringToXmlElement(writer, "MailAddress", this.mMailAddress);
      if (this.mSpecialType != ApplicationGroupSpecialType.Generic)
        Identity.EnumToXmlElement(writer, "SpecialType", (object) this.mSpecialType, typeof (ApplicationGroupSpecialType));
      if (this.mDeleted)
        Identity.StringToXmlElement(writer, "Deleted", XmlConvert.ToString(this.mDeleted));
      if (this.mMembers != null)
        Identity.StringArrayToXmlElement(writer, "Members", this.mMembers);
      if (this.mMemberOf != null)
        Identity.StringArrayToXmlElement(writer, "MemberOf", this.mMemberOf);
      if (this.mSecurityGroup)
        Identity.StringToXmlElement(writer, "SecurityGroup", XmlConvert.ToString(this.mSecurityGroup));
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
        Identity.StringToXmlElement(writer, "string", array[index]);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Identity FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      Identity identity = new Identity();
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
                if (name2 == "Sid")
                {
                  identity.Sid = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 4:
                if (name2 == "Type")
                {
                  identity.Type = XmlUtility.EnumFromXmlElement<IdentityType>(reader);
                  continue;
                }
                break;
              case 6:
                if (name2 == "Domain")
                {
                  identity.Domain = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 7:
                switch (name2[0])
                {
                  case 'D':
                    if (name2 == "Deleted")
                    {
                      identity.Deleted = XmlUtility.BooleanFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'M':
                    if (name2 == "Members")
                    {
                      identity.Members = Helper.ArrayOfStringFromXml(reader, false);
                      continue;
                    }
                    break;
                }
                break;
              case 8:
                if (name2 == "MemberOf")
                {
                  identity.MemberOf = Helper.ArrayOfStringFromXml(reader, false);
                  continue;
                }
                break;
              case 11:
                switch (name2[1])
                {
                  case 'a':
                    if (name2 == "MailAddress")
                    {
                      identity.MailAddress = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'c':
                    if (name2 == "AccountName")
                    {
                      identity.AccountName = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'e':
                    if (name2 == "Description")
                    {
                      identity.Description = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'i':
                    if (name2 == "DisplayName")
                    {
                      identity.DisplayName = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'p':
                    if (name2 == "SpecialType")
                    {
                      identity.SpecialType = XmlUtility.EnumFromXmlElement<ApplicationGroupSpecialType>(reader);
                      continue;
                    }
                    break;
                }
                break;
              case 13:
                if (name2 == "SecurityGroup")
                {
                  identity.SecurityGroup = XmlUtility.BooleanFromXmlElement(reader);
                  continue;
                }
                break;
              case 17:
                if (name2 == "DistinguishedName")
                {
                  identity.DistinguishedName = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
            }
          }
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return identity;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Identity instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AccountName: " + this.AccountName);
      stringBuilder.AppendLine("  Deleted: " + this.Deleted.ToString());
      stringBuilder.AppendLine("  Description: " + this.Description);
      stringBuilder.AppendLine("  DisplayName: " + this.DisplayName);
      stringBuilder.AppendLine("  DistinguishedName: " + this.DistinguishedName);
      stringBuilder.AppendLine("  Domain: " + this.Domain);
      stringBuilder.AppendLine("  MailAddress: " + this.MailAddress);
      stringBuilder.AppendLine("  MemberOf: " + Helper.ArrayToString<string>(this.MemberOf));
      stringBuilder.AppendLine("  Members: " + Helper.ArrayToString<string>(this.Members));
      stringBuilder.AppendLine("  SecurityGroup: " + this.SecurityGroup.ToString());
      stringBuilder.AppendLine("  Sid: " + this.Sid);
      stringBuilder.AppendLine("  SpecialType: " + this.SpecialType.ToString());
      stringBuilder.AppendLine("  Type: " + this.Type.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.AccountName != null)
        XmlUtility.ToXmlElement(writer, "AccountName", this.AccountName);
      if (this.Deleted)
        XmlUtility.ToXmlElement(writer, "Deleted", this.Deleted);
      if (this.Description != null)
        XmlUtility.ToXmlElement(writer, "Description", this.Description);
      if (this.DisplayName != null)
        XmlUtility.ToXmlElement(writer, "DisplayName", this.DisplayName);
      if (this.DistinguishedName != null)
        XmlUtility.ToXmlElement(writer, "DistinguishedName", this.DistinguishedName);
      if (this.Domain != null)
        XmlUtility.ToXmlElement(writer, "Domain", this.Domain);
      if (this.MailAddress != null)
        XmlUtility.ToXmlElement(writer, "MailAddress", this.MailAddress);
      Helper.ToXml(writer, "MemberOf", this.MemberOf, false, false);
      Helper.ToXml(writer, "Members", this.Members, false, false);
      if (this.SecurityGroup)
        XmlUtility.ToXmlElement(writer, "SecurityGroup", this.SecurityGroup);
      if (this.Sid != null)
        XmlUtility.ToXmlElement(writer, "Sid", this.Sid);
      if (this.SpecialType != ApplicationGroupSpecialType.Generic)
        XmlUtility.EnumToXmlElement<ApplicationGroupSpecialType>(writer, "SpecialType", this.SpecialType);
      if (this.Type != IdentityType.InvalidIdentity)
        XmlUtility.EnumToXmlElement<IdentityType>(writer, "Type", this.Type);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, Identity obj) => obj.ToXml(writer, element);
  }
}
