// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ReadOnlyIdentityDescriptor
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public sealed class ReadOnlyIdentityDescriptor : IdentityDescriptor
  {
    public ReadOnlyIdentityDescriptor()
    {
    }

    public ReadOnlyIdentityDescriptor(string identityType, string identifier, object data)
      : base(identityType, identifier, data)
    {
    }

    [XmlAttribute("identityType")]
    [DataMember]
    public override string IdentityType
    {
      get => base.IdentityType;
      set
      {
        if (this.m_identityType != null)
          throw new InvalidOperationException(IdentityResources.FieldReadOnly((object) nameof (IdentityType)));
        base.IdentityType = value;
      }
    }

    [XmlAttribute("identifier")]
    [DataMember]
    public override string Identifier
    {
      get => base.Identifier;
      set => base.Identifier = string.IsNullOrEmpty(base.Identifier) ? value : throw new InvalidOperationException(IdentityResources.FieldReadOnly((object) nameof (Identifier)));
    }

    [XmlIgnore]
    public override object Data
    {
      get => base.Data;
      set => base.Data = base.Data == null ? value : throw new InvalidOperationException(IdentityResources.FieldReadOnly((object) nameof (Data)));
    }
  }
}
