// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.RegistrationValidator
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class RegistrationValidator
  {
    public static void Validate(
      Registration registration,
      AuthorizationScopeConfiguration configuration,
      DelegatedAuthorizationSettings settings,
      bool allowDefaultValuesForRequiredFields = false)
    {
      if (!allowDefaultValuesForRequiredFields)
      {
        ArgumentUtility.CheckForEmptyGuid(registration.IdentityId, "userId");
        ArgumentUtility.CheckForEmptyGuid(registration.RegistrationId, "RegistrationId");
        ArgumentUtility.CheckStringForNullOrEmpty(registration.RegistrationName, "RegistrationName");
        ArgumentUtility.CheckStringForNullOrEmpty(registration.Scopes, "Scopes");
        ArgumentUtility.CheckStringForNullOrEmpty(registration.ResponseTypes, "ResponseTypes");
        if (registration.ClientType == ClientType.Confidential || registration.ClientType == ClientType.FullTrust)
        {
          ArgumentUtility.CheckForNull<Uri>(registration.RegistrationLocation, "RegistrationLocation");
          ArgumentUtility.CheckStringForNullOrEmpty(registration.OrganizationName, "OrganizationName");
          ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) registration.RedirectUris, "RedirectUris");
        }
        else if (registration.ClientType == ClientType.MediumTrust)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(registration.PublicKey, "PublicKey");
          int num = 0;
          try
          {
            using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider())
            {
              cryptoServiceProvider.FromXmlString(registration.PublicKey);
              num = cryptoServiceProvider.KeySize;
            }
          }
          catch (Exception ex)
          {
            throw new ArgumentException("The public key provided is not a valid RSA key", ex);
          }
          if (num < 2048)
            throw new ArgumentException("The public key provided must be at least 2048 bits in length");
        }
      }
      if (!string.IsNullOrWhiteSpace(registration.RegistrationName) && registration.RegistrationName.Length > 128)
        throw new ArgumentException("Registration name is not valid.");
      if (!string.IsNullOrWhiteSpace(registration.OrganizationName) && registration.OrganizationName.Length > 128)
        throw new ArgumentException("Organization name is not valid.");
      if (!string.IsNullOrWhiteSpace(registration.ResponseTypes) && registration.ClientType != ClientType.FullTrust && !string.Equals(registration.ResponseTypes, "Assertion", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Given response type not supported");
      foreach (Uri redirectUri in (IEnumerable<Uri>) registration.RedirectUris)
      {
        if (redirectUri == (Uri) null)
          throw new ArgumentException("Redirect URL is invalid or missing.", "redirectUris");
        if (!RegistrationValidator.ValidateUriScheme(redirectUri, true))
          throw new ArgumentException("Registration redirect URL must be wellformed Http/Https url.", "redirectUris");
      }
      if (registration.RegistrationLogoSecureLocation != (Uri) null && !string.Equals(registration.RegistrationLogoSecureLocation.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Registration Logo URL must be secure.", "registrationLogoSecureLocation");
      if (!configuration.IsEnabled || configuration == null || registration.ClientType == ClientType.FullTrust && registration.Scopes.ToLowerInvariant().Contains("app_token".ToLowerInvariant()))
        return;
      registration.Scopes = configuration.NormalizeScopes(registration.Scopes, out bool _);
    }

    public static bool ValidateUriScheme(Uri uri, bool allowHttp) => string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) || allowHttp && string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);
  }
}
