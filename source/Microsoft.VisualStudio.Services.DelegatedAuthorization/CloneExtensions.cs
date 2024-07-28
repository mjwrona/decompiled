// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.CloneExtensions
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal static class CloneExtensions
  {
    internal static Registration Clone(this Registration registration) => new Registration()
    {
      ClientType = registration.ClientType,
      IdentityId = registration.IdentityId,
      IsValid = registration.IsValid,
      IsWellKnown = registration.IsWellKnown,
      OrganizationLocation = registration.OrganizationLocation,
      OrganizationName = registration.OrganizationName,
      PublicKey = registration.PublicKey,
      RedirectUris = registration.RedirectUris,
      RegistrationDescription = registration.RegistrationDescription,
      RegistrationId = registration.RegistrationId,
      RegistrationLocation = registration.RegistrationLocation,
      RegistrationLogoSecureLocation = registration.RegistrationLogoSecureLocation,
      RegistrationName = registration.RegistrationName,
      RegistrationPrivacyStatementLocation = registration.RegistrationPrivacyStatementLocation,
      RegistrationTermsOfServiceLocation = registration.RegistrationTermsOfServiceLocation,
      ResponseTypes = registration.ResponseTypes,
      Scopes = registration.Scopes,
      Secret = registration.Secret,
      SecretValidTo = registration.SecretValidTo,
      SecretVersionId = registration.SecretVersionId,
      ValidFrom = registration.ValidFrom,
      Issuer = registration.Issuer
    };
  }
}
