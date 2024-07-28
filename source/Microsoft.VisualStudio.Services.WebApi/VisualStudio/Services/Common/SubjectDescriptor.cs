// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SubjectDescriptor
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
  [TypeConverter(typeof (SubjectDescriptorConverter))]
  public struct SubjectDescriptor : IEquatable<SubjectDescriptor>, IXmlSerializable
  {
    public SubjectDescriptor(string subjectType, string identifier)
    {
      SubjectDescriptor.ValidateSubjectType(subjectType);
      SubjectDescriptor.ValidateIdentifier(identifier);
      this.SubjectType = SubjectDescriptor.NormalizeSubjectType(subjectType);
      this.Identifier = identifier;
    }

    [DataMember]
    public string SubjectType { get; private set; }

    [DataMember]
    public string Identifier { get; private set; }

    public override string ToString() => this == new SubjectDescriptor() ? (string) null : this.SubjectType + (object) '.' + PrimitiveExtensions.ToBase64StringNoPaddingFromString(this.Identifier);

    public static SubjectDescriptor FromString(string subjectDescriptorString)
    {
      if (string.IsNullOrEmpty(subjectDescriptorString))
        return new SubjectDescriptor();
      if (subjectDescriptorString.Length < 6)
        return new SubjectDescriptor("ukn", subjectDescriptorString);
      int length = subjectDescriptorString.IndexOf('.', 3, 3);
      if (length < 3 || length == subjectDescriptorString.Length - 1)
        return new SubjectDescriptor("ukn", subjectDescriptorString);
      string subjectType = subjectDescriptorString.Substring(0, length);
      string base64String = subjectDescriptorString.Substring(length + 1);
      try
      {
        return new SubjectDescriptor(subjectType, PrimitiveExtensions.FromBase64StringNoPaddingToString(base64String));
      }
      catch
      {
      }
      return new SubjectDescriptor("ukn", subjectDescriptorString);
    }

    public static IEnumerable<SubjectDescriptor> FromCommaSeperatedStrings(string descriptors)
    {
      if (string.IsNullOrEmpty(descriptors))
        return Enumerable.Empty<SubjectDescriptor>();
      return ((IEnumerable<string>) descriptors.Split(',')).Where<string>((Func<string, bool>) (descriptor => !string.IsNullOrEmpty(descriptor))).Select<string, SubjectDescriptor>((Func<string, SubjectDescriptor>) (descriptor => SubjectDescriptor.FromString(descriptor)));
    }

    public bool Equals(SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(this.SubjectType, subjectDescriptor.SubjectType) && StringComparer.OrdinalIgnoreCase.Equals(this.Identifier, subjectDescriptor.Identifier);

    public override bool Equals(object obj) => obj is SubjectDescriptor subjectDescriptor && this == subjectDescriptor;

    public override int GetHashCode()
    {
      if (this == new SubjectDescriptor())
        return 0;
      int num1 = 7443;
      int num2 = (num1 << 19) - num1 + StringComparer.OrdinalIgnoreCase.GetHashCode(this.SubjectType);
      return (num2 << 19) - num2 + StringComparer.OrdinalIgnoreCase.GetHashCode(this.Identifier);
    }

    public static bool operator ==(SubjectDescriptor left, SubjectDescriptor right) => left.Equals(right);

    public static bool operator !=(SubjectDescriptor left, SubjectDescriptor right) => !left.Equals(right);

    public static implicit operator string(SubjectDescriptor subjectDescriptor) => subjectDescriptor.ToString();

    internal static int Compare(SubjectDescriptor left, SubjectDescriptor right)
    {
      int num = StringComparer.OrdinalIgnoreCase.Compare(left.SubjectType, right.SubjectType);
      if (num == 0)
        num = StringComparer.OrdinalIgnoreCase.Compare(left.Identifier, right.Identifier);
      return num;
    }

    private static string NormalizeSubjectType(string subjectType)
    {
      string str;
      if (!Constants.SubjectTypeMap.TryGetValue(subjectType, out str))
        str = subjectType;
      return str;
    }

    private static void ValidateSubjectType(string subjectType)
    {
      if (string.IsNullOrEmpty(subjectType))
        throw new ArgumentNullException(nameof (subjectType));
      if (subjectType.Length < 3 || subjectType.Length > 5)
        throw new ArgumentOutOfRangeException(nameof (subjectType), (object) subjectType, GraphResources.SubjectTypeLengthOutOfRange());
    }

    private static void ValidateIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        throw new ArgumentNullException(nameof (identifier));
      if (identifier.Length > 256)
        throw new ArgumentOutOfRangeException(nameof (identifier), (object) identifier, GraphResources.IdentifierLengthOutOfRange());
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
        SubjectDescriptor subjectDescriptor = SubjectDescriptor.FromString(reader.ReadContentAsString());
        this.SubjectType = subjectDescriptor.SubjectType;
        this.Identifier = subjectDescriptor.Identifier;
      }
      else
      {
        while (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "SubjectType":
              string subjectType = reader.ReadElementContentAsString();
              SubjectDescriptor.ValidateSubjectType(subjectType);
              this.SubjectType = subjectType;
              continue;
            case "Identifier":
              string identifier = reader.ReadElementContentAsString();
              SubjectDescriptor.ValidateIdentifier(identifier);
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
      if (this.Equals(new SubjectDescriptor()))
        return;
      writer.WriteElementString("SubjectType", this.SubjectType);
      writer.WriteElementString("Identifier", this.Identifier);
    }
  }
}
