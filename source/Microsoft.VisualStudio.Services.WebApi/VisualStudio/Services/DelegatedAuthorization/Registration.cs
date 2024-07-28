// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DataContract]
  public class Registration
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid RegistrationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid IdentityId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OrganizationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri OrganizationLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RegistrationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RegistrationDescription { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationLogoSecureLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationTermsOfServiceLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationPrivacyStatementLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResponseTypes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Scopes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid SecretVersionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsValid { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<Uri> RedirectUris { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ClientType ClientType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsWellKnown { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal DateTimeOffset? SecretValidTo { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Secret { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri SetupUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal string Issuer { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal DateTimeOffset? ValidFrom { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal string AccessHash { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PublicKey { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Guid> TenantIds { get; set; }

    public Registration() => this.RedirectUris = (IList<Uri>) new List<Uri>(5);

    public CommonRegistration ToCommonRegistration()
    {
      if (this == null)
        return (CommonRegistration) null;
      switch (this.ClientType)
      {
        case ClientType.Confidential:
          return (CommonRegistration) new ConfidentialClientRegistration(this);
        case ClientType.Public:
          return (CommonRegistration) new PublicClientRegistration(this);
        case ClientType.MediumTrust:
          return (CommonRegistration) new MediumTrustClientRegistration(this);
        case ClientType.HighTrust:
          return (CommonRegistration) new HighTrustClientRegistration(this);
        case ClientType.FullTrust:
          return (CommonRegistration) new FullTrustClientRegistration(this);
        case ClientType.Application:
          return (CommonRegistration) new ApplicationClientRegistration(this);
        default:
          return (CommonRegistration) new ConfidentialClientRegistration(this);
      }
    }
  }
}
