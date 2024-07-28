// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal static class CustomSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public const int RegistrationNotFound = 1080001;
    public const int ResponseTypeNotSupported = 1080002;
    public const int InvalidRedirectUri = 1080003;
    public const int InvalidRegistration = 1080004;
    public const int AuthorizationNotFound = 1080005;
    public const int InvalidAuthorization = 1080006;
    public const int AccessAlreadyIssued = 1080007;
    public const int AuthorizationGrantExpired = 1080008;
    public const int AccessTokenNotFound = 1080009;
    public const int InvalidAccessToken = 1080010;
    public const int AccessTokenAlreadyRefreshed = 1080011;
    public const int InvalidClientSecret = 1080012;
    public const int RegistrationAlreadyExists = 1080013;
    public const int UnableToDeleteRegistration = 1080014;
    public const int UpdateRegistrationFail = 1080015;
    public const int InvalidScope = 1080016;
    public const int RedirectUriAlreadyExists = 1080017;
    public const int IssueAccessTokenFailed = 1080018;
    public const int InvalidAccessTokenKey = 1080019;
    public const int InvalidAccessId = 1080020;
    public const int InvalidClientType = 1080021;
    public const int UpdateDelegatedAuthorizationFail = 1080022;
    public const int HostAuthorizationNotFound = 1080023;
    public const int MAX_SQL_ERROR = 1080023;
  }
}
