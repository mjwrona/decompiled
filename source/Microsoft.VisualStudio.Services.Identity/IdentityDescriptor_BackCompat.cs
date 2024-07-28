// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptor_BackCompat
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityDescriptor_BackCompat
  {
    private string m_identityType;
    private string m_identifier;
    private const int MaxIdLength = 256;
    private const int MaxTypeLength = 128;

    public IdentityDescriptor_BackCompat()
    {
    }

    internal IdentityDescriptor_BackCompat(string identityType, string identifier)
    {
      IdentityDescriptor_BackCompat.ValidateIdentityType(identityType);
      this.m_identityType = identityType;
      IdentityDescriptor_BackCompat.ValidateIdentifier(identifier);
      this.m_identifier = identifier;
    }

    public IdentityDescriptor_BackCompat(IdentityDescriptor_BackCompat clone)
    {
      this.m_identityType = clone.m_identityType;
      this.m_identifier = clone.m_identifier;
    }

    [XmlAttribute("identityType")]
    public virtual string IdentityType
    {
      get => this.m_identityType;
      set => this.m_identityType = value;
    }

    [XmlAttribute("identifier")]
    public virtual string Identifier
    {
      get => this.m_identifier;
      set => this.m_identifier = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IdentityDescriptor (IdentityType: {0}; Identifier: {1})", (object) this.m_identityType, (object) this.m_identifier);

    private static void ValidateIdentityType(string identityType)
    {
      if (string.IsNullOrEmpty(identityType))
        throw new ArgumentNullException(nameof (identityType));
      if (identityType.Length > 128)
        throw new ArgumentOutOfRangeException(nameof (identityType));
    }

    private static void ValidateIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        throw new ArgumentNullException(nameof (identifier));
      if (identifier.Length > 256)
        throw new ArgumentOutOfRangeException(nameof (identifier));
    }
  }
}
