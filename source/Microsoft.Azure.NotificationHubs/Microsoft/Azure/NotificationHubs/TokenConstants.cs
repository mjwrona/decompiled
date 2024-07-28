// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TokenConstants
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs
{
  public static class TokenConstants
  {
    public const char DefaultCompoundClaimDelimiter = ',';
    public const char HttpAuthParameterSeparator = ',';
    public const string HttpMethodPost = "POST";
    public const string HttpMethodGet = "GET";
    public const string HttpMethodHead = "HEAD";
    public const string HttpMethodTrace = "TRACE";
    public const string ManagementIssuerName = "owner";
    public const int MaxIssuerNameSize = 128;
    public const int MaxIssuerSecretSize = 128;
    public const string OutputClaimIssuerId = "ACS";
    public const string ServiceBusIssuerName = "owner";
    public const string WrapAppliesTo = "wrap_scope";
    public const string WrapRequestedLifetime = "requested_lifetime";
    public const string WrapAccessToken = "wrap_access_token";
    public const string WrapAssertion = "wrap_assertion";
    public const string WrapAssertionFormat = "wrap_assertion_format";
    public const string WrapAuthenticationType = "WRAP";
    public const string WrapAuthorizationHeaderKey = "access_token";
    public static readonly DateTime WrapBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    public const string WrapName = "wrap_name";
    public const string WrapPassword = "wrap_password";
    public const string WrapContentType = "application/x-www-form-urlencoded";
    public const string WrapSamlAssertionFormat = "SAML";
    public const string WrapSwtAssertionFormat = "SWT";
    public const string WrapTokenExpiresIn = "wrap_access_token_expires_in";
    public const string Saml11ConfirmationMethodBearerToken = "urn:oasis:names:tc:SAML:1.0:cm:bearer";
    public const string TokenAudience = "Audience";
    public const string TokenExpiresOn = "ExpiresOn";
    public const string TokenIssuer = "Issuer";
    public const string TokenDigest256 = "HMACSHA256";
    public const string TrackingIdHeaderName = "x-ms-request-id";
    public const char UrlParameterSeparator = '&';
    public const char KeyValueSeparator = '=';
    internal static readonly string TokenServiceRealmFormat = "https://{0}.accesscontrol.windows.net/";
    internal static readonly string[] WrapContentTypes = new string[3]
    {
      "*/*",
      "application/*",
      "application/x-www-form-urlencoded"
    };
  }
}
