// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.CommonRegistration
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
  [KnownType(typeof (ApplicationClientRegistration))]
  [KnownType(typeof (ConfidentialClientRegistration))]
  [KnownType(typeof (FullTrustClientRegistration))]
  [KnownType(typeof (HighTrustClientRegistration))]
  [KnownType(typeof (MediumTrustClientRegistration))]
  [KnownType(typeof (PublicClientRegistration))]
  public abstract class CommonRegistration
  {
    public CommonRegistration(Registration registration)
    {
      this.RegistrationId = registration.RegistrationId;
      this.IdentityId = registration.IdentityId;
      this.OrganizationName = registration.OrganizationName;
      this.OrganizationLocation = registration.OrganizationLocation;
      this.RegistrationName = registration.RegistrationName;
      this.RegistrationDescription = registration.RegistrationDescription;
      this.RegistrationLogoSecureLocation = registration.RegistrationLogoSecureLocation;
      this.RegistrationTermsOfServiceLocation = registration.RegistrationTermsOfServiceLocation;
      this.RegistrationPrivacyStatementLocation = registration.RegistrationPrivacyStatementLocation;
      this.ResponseTypes = registration.ResponseTypes;
      this.Scopes = registration.Scopes;
      this.SecretVersionId = registration.SecretVersionId;
      this.IsValid = registration.IsValid;
      this.RedirectUris = registration.RedirectUris;
      this.ClientType = registration.ClientType;
      this.SecretValidTo = registration.SecretValidTo;
      this.Secret = registration.Secret;
      this.Issuer = registration.Issuer;
      this.ValidFrom = registration.ValidFrom;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid RegistrationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid IdentityId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RegistrationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RegistrationDescription { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OrganizationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri OrganizationLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResponseTypes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<Uri> RedirectUris { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Scopes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ClientType ClientType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal string Issuer { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationLogoSecureLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationTermsOfServiceLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationPrivacyStatementLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsValid { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid SecretVersionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal DateTimeOffset? ValidFrom { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal DateTimeOffset? SecretValidTo { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Secret { get; set; }

    public virtual Registration ToRegistration()
    {
      if (this == null)
        return (Registration) null;
      return new Registration()
      {
        RegistrationId = this.RegistrationId,
        IdentityId = this.IdentityId,
        OrganizationName = this.OrganizationName,
        OrganizationLocation = this.OrganizationLocation,
        RegistrationName = this.RegistrationName,
        RegistrationDescription = this.RegistrationDescription,
        RegistrationLogoSecureLocation = this.RegistrationLogoSecureLocation,
        RegistrationTermsOfServiceLocation = this.RegistrationTermsOfServiceLocation,
        RegistrationPrivacyStatementLocation = this.RegistrationPrivacyStatementLocation,
        ResponseTypes = this.ResponseTypes,
        Scopes = this.Scopes,
        SecretVersionId = this.SecretVersionId,
        IsValid = this.IsValid,
        RedirectUris = this.RedirectUris,
        ClientType = this.ClientType,
        SecretValidTo = this.SecretValidTo,
        Secret = this.Secret,
        Issuer = this.Issuer,
        ValidFrom = this.ValidFrom
      };
    }
  }
}
