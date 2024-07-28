// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptor
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [XmlInclude(typeof (ReadOnlyIdentityDescriptor))]
  [KnownType(typeof (ReadOnlyIdentityDescriptor))]
  [TypeConverter(typeof (IdentityDescriptorConverter))]
  [DataContract]
  public class IdentityDescriptor : 
    IEquatable<IdentityDescriptor>,
    IComparable<IdentityDescriptor>,
    IComparable
  {
    protected string m_identityType;
    private string m_identifier;
    private const int MaxIdLength = 256;
    private const int MaxTypeLength = 128;

    public IdentityDescriptor()
    {
    }

    public IdentityDescriptor(string identityType, string identifier, object data)
      : this(identityType, identifier)
    {
      this.Data = data;
    }

    public IdentityDescriptor(string identityType, string identifier)
    {
      this.IdentityType = identityType;
      this.Identifier = identifier;
    }

    public IdentityDescriptor(IdentityDescriptor clone)
    {
      this.IdentityType = clone.IdentityType;
      this.Identifier = clone.Identifier;
    }

    [XmlAttribute("identityType")]
    [DataMember]
    public virtual string IdentityType
    {
      get => this.m_identityType ?? "Microsoft.VisualStudio.Services.Identity.UnknownIdentity";
      set
      {
        IdentityDescriptor.ValidateIdentityType(value);
        this.m_identityType = IdentityDescriptor.NormalizeIdentityType(value);
        this.Data = (object) null;
      }
    }

    [XmlAttribute("identifier")]
    [DataMember]
    public virtual string Identifier
    {
      get => this.m_identifier;
      set
      {
        IdentityDescriptor.ValidateIdentifier(value);
        this.m_identifier = value;
        this.Data = (object) null;
      }
    }

    [XmlIgnore]
    public virtual object Data { get; set; }

    public override string ToString() => this.m_identityType + ";" + this.m_identifier;

    public static IdentityDescriptor FromString(string identityDescriptorString)
    {
      if (string.IsNullOrEmpty(identityDescriptorString))
        return (IdentityDescriptor) null;
      string[] strArray;
      try
      {
        strArray = identityDescriptorString.Split(new string[1]
        {
          ";"
        }, 2, StringSplitOptions.RemoveEmptyEntries);
      }
      catch
      {
        return new IdentityDescriptor("Microsoft.VisualStudio.Services.Identity.UnknownIdentity", identityDescriptorString);
      }
      return strArray.Length == 2 ? new IdentityDescriptor(strArray[0], strArray[1]) : new IdentityDescriptor("Microsoft.VisualStudio.Services.Identity.UnknownIdentity", identityDescriptorString);
    }

    private static void ValidateIdentityType(string identityType)
    {
      if (string.IsNullOrEmpty(identityType))
        throw new ArgumentNullException(nameof (identityType));
      if (identityType.Length > 128)
        throw new ArgumentOutOfRangeException(nameof (identityType));
    }

    private static string NormalizeIdentityType(string identityType)
    {
      string str;
      if (!IdentityConstants.IdentityTypeMap.TryGetValue(identityType, out str))
        str = identityType;
      return str;
    }

    private static void ValidateIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        throw new ArgumentNullException(nameof (identifier));
      if (identifier.Length > 256)
        throw new ArgumentOutOfRangeException(nameof (identifier));
    }

    internal static IdentityDescriptor FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "identifier":
              empty1 = reader.Value;
              continue;
            case "identityType":
              empty2 = reader.Value;
              continue;
            default:
              continue;
          }
        }
      }
      IdentityDescriptor identityDescriptor = new IdentityDescriptor(empty2, empty1);
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
      return identityDescriptor;
    }

    public bool Equals(IdentityDescriptor other) => IdentityDescriptorComparer.Instance.Equals(this, other);

    public int CompareTo(IdentityDescriptor other) => IdentityDescriptorComparer.Instance.Compare(this, other);

    public int CompareTo(object other) => IdentityDescriptorComparer.Instance.Compare(this, other as IdentityDescriptor);

    public override bool Equals(object obj) => this.Equals(obj as IdentityDescriptor);

    public override int GetHashCode() => IdentityDescriptorComparer.Instance.GetHashCode(this);

    public static bool operator ==(IdentityDescriptor x, IdentityDescriptor y) => IdentityDescriptorComparer.Instance.Equals(x, y);

    public static bool operator !=(IdentityDescriptor x, IdentityDescriptor y) => !IdentityDescriptorComparer.Instance.Equals(x, y);
  }
}
