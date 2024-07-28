// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SocialDescriptor
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Common
{
  [TypeConverter(typeof (SocialDescriptorConverter))]
  public struct SocialDescriptor : IEquatable<SocialDescriptor>, IXmlSerializable
  {
    public SocialDescriptor(string socialType, string identifier)
    {
      SocialDescriptor.ValidateSocialType(socialType);
      SocialDescriptor.ValidateIdentifier(identifier);
      this.SocialType = SocialDescriptor.NormalizeSocialType(socialType);
      this.Identifier = identifier;
    }

    [DataMember]
    public string SocialType { get; private set; }

    [DataMember]
    public string Identifier { get; private set; }

    public override string ToString()
    {
      if (this == new SocialDescriptor())
        return (string) null;
      return "@" + this.SocialType + (object) '.' + PrimitiveExtensions.ToBase64StringNoPaddingFromString(this.Identifier);
    }

    public static SocialDescriptor FromString(string socialDescriptorString)
    {
      if (string.IsNullOrEmpty(socialDescriptorString))
        return new SocialDescriptor();
      if (!socialDescriptorString.StartsWith("@"))
        return new SocialDescriptor("ukn", socialDescriptorString);
      if (socialDescriptorString.Length < 6)
        return new SocialDescriptor("ukn", socialDescriptorString);
      string[] strArray = socialDescriptorString.Split(new char[1]
      {
        '.'
      }, 3);
      if (strArray.Length != 2)
        return new SocialDescriptor("ukn", socialDescriptorString);
      string socialType = strArray[0].Substring(1);
      string base64String = strArray[1];
      try
      {
        return new SocialDescriptor(socialType, PrimitiveExtensions.FromBase64StringNoPaddingToString(base64String));
      }
      catch
      {
      }
      return new SocialDescriptor("ukn", socialDescriptorString);
    }

    public static IEnumerable<SocialDescriptor> FromCommaSeperatedStrings(string descriptors)
    {
      if (string.IsNullOrEmpty(descriptors))
        return Enumerable.Empty<SocialDescriptor>();
      return ((IEnumerable<string>) descriptors.Split(',')).Where<string>((Func<string, bool>) (descriptor => !string.IsNullOrEmpty(descriptor))).Select<string, SocialDescriptor>((Func<string, SocialDescriptor>) (descriptor => SocialDescriptor.FromString(descriptor)));
    }

    public bool Equals(SocialDescriptor socialDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(this.SocialType, socialDescriptor.SocialType) && StringComparer.Ordinal.Equals(this.Identifier, socialDescriptor.Identifier);

    public override bool Equals(object obj) => obj is SocialDescriptor socialDescriptor && this == socialDescriptor;

    public override int GetHashCode()
    {
      if (this == new SocialDescriptor())
        return 0;
      int num1 = 7443;
      int num2 = (num1 << 19) - num1 + StringComparer.OrdinalIgnoreCase.GetHashCode(this.SocialType);
      return (num2 << 19) - num2 + StringComparer.Ordinal.GetHashCode(this.Identifier);
    }

    public static bool operator ==(SocialDescriptor left, SocialDescriptor right) => left.Equals(right);

    public static bool operator !=(SocialDescriptor left, SocialDescriptor right) => !left.Equals(right);

    public static implicit operator string(SocialDescriptor socialDescriptor) => socialDescriptor.ToString();

    internal static int Compare(SocialDescriptor left, SocialDescriptor right)
    {
      int num = StringComparer.OrdinalIgnoreCase.Compare(left.SocialType, right.SocialType);
      if (num == 0)
        num = StringComparer.Ordinal.Compare(left.Identifier, right.Identifier);
      return num;
    }

    private static string NormalizeSocialType(string socialType)
    {
      string str;
      if (!Constants.SocialTypeMap.TryGetValue(socialType, out str))
        str = socialType;
      return str;
    }

    private static void ValidateSocialType(string socialType)
    {
      if (string.IsNullOrEmpty(socialType))
        throw new ArgumentNullException(nameof (socialType));
      if (socialType.Length < 3 || socialType.Length > 4)
        throw new ArgumentOutOfRangeException(nameof (socialType), (object) socialType, GraphResources.SubjectTypeLengthOutOfRange());
    }

    private static void ValidateIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        throw new ArgumentNullException(nameof (identifier));
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      ArgumentUtility.CheckForNull<XmlReader>(reader, nameof (reader));
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.ReadStartElement();
      if (num != 0)
        return;
      if (reader.NodeType == XmlNodeType.Text)
      {
        SocialDescriptor socialDescriptor = SocialDescriptor.FromString(reader.ReadContentAsString());
        this.SocialType = socialDescriptor.SocialType;
        this.Identifier = socialDescriptor.Identifier;
      }
      else
      {
        while (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "SocialType":
              string socialType = reader.ReadElementContentAsString();
              SocialDescriptor.ValidateSocialType(socialType);
              this.SocialType = socialType;
              continue;
            case "Identifier":
              string identifier = reader.ReadElementContentAsString();
              SocialDescriptor.ValidateIdentifier(identifier);
              this.Identifier = identifier;
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
      }
      reader.ReadEndElement();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      ArgumentUtility.CheckForNull<XmlWriter>(writer, nameof (writer));
      if (this.Equals(new SocialDescriptor()))
        return;
      writer.WriteElementString("SocialType", this.SocialType);
      writer.WriteElementString("Identifier", this.Identifier);
    }
  }
}
