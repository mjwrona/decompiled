// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.InternalCreateUserParameters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class InternalCreateUserParameters : CreateUserParameters
  {
    public InternalCreateUserParameters()
    {
      this.PrivateAttributes = new List<SetUserAttributeParameters>();
      this.ProviderProperties = new Dictionary<string, string>();
    }

    public InternalCreateUserParameters(CreateUserParameters copy)
      : base(copy)
    {
      this.PrivateAttributes = new List<SetUserAttributeParameters>();
      this.ProviderProperties = new Dictionary<string, string>();
    }

    public InternalCreateUserParameters(InternalCreateUserParameters copy)
      : base((CreateUserParameters) copy)
    {
      this.PrivateAttributes = new List<SetUserAttributeParameters>((IEnumerable<SetUserAttributeParameters>) copy.PrivateAttributes);
      this.ProviderProperties = new Dictionary<string, string>();
      this.StorageKey = copy.StorageKey;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityDescriptor IdentityDescriptor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public SocialDescriptor SocialDescriptor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid StorageKey { get; set; }

    [DataMember(IsRequired = false)]
    public List<SetUserAttributeParameters> PrivateAttributes { get; set; }

    [DataMember(IsRequired = false)]
    public Dictionary<string, string> ProviderProperties { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? SendConfirmationEmail { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public UserValidationOptions ValidationOptions { get; set; }

    internal InternalCreateUserParameters Clone() => new InternalCreateUserParameters(this);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "InternalCreateUserParameters\r\n[\r\nDescriptor:             {0}\r\nIdentityDescriptor:     {1}\r\nSocialDescriptor:       {2}\r\nStorageKey:             {3}\r\nDisplayName:            {4}\r\nMail:                   {5}\r\nRegion:                 {6}\r\nCountry:                {7}\r\nPendingProfileCreation: {8}\r\nSendConfirmationEmail:  {9}\r\n]", (object) this.Descriptor, (object) this.IdentityDescriptor, (object) this.SocialDescriptor, (object) this.StorageKey, (object) this.DisplayName, (object) this.Mail, (object) this.Region, (object) this.Country, (object) this.PendingProfileCreation, (object) this.SendConfirmationEmail);
  }
}
