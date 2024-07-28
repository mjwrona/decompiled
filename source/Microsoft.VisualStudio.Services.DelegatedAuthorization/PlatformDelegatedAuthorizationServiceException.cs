// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationServiceException
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.VisualStudio.Services.Security;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class PlatformDelegatedAuthorizationServiceException : Exception
  {
    internal PlatformDelegatedAuthorizationServiceError Error { get; private set; }

    public PlatformDelegatedAuthorizationServiceException(
      PlatformDelegatedAuthorizationServiceError error)
    {
      this.Error = error;
    }

    public void ToAccessCheckException(string message)
    {
      if (this.Error == PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission)
        throw new AccessCheckException(message);
    }

    public void ToPlatformDelegatedAuthorizationException(string message)
    {
      if (this.Error == PlatformDelegatedAuthorizationServiceError.AccessDenied)
        throw new PlatformDelegatedAuthorizationException(message);
    }

    public AppSessionTokenResult ToAppSessionTokenResult()
    {
      switch (this.Error)
      {
        case PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission:
        case PlatformDelegatedAuthorizationServiceError.AccessDenied:
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.AccessDenied
          };
        case PlatformDelegatedAuthorizationServiceError.InvalidUserType:
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.InvalidUserType
          };
        case PlatformDelegatedAuthorizationServiceError.UserIdRequired:
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.UserIdRequired
          };
        case PlatformDelegatedAuthorizationServiceError.InvalidUserId:
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.InvalidUserId
          };
        default:
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.None
          };
      }
    }

    public SessionTokenResult ToSessionTokenResult()
    {
      switch (this.Error)
      {
        case PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission:
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.AccessDenied
          };
        case PlatformDelegatedAuthorizationServiceError.InvalidUserType:
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.InvalidUserType
          };
        case PlatformDelegatedAuthorizationServiceError.UserIdRequired:
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.UserIdRequired
          };
        case PlatformDelegatedAuthorizationServiceError.InvalidUserId:
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.InvalidUserId
          };
        default:
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.None
          };
      }
    }

    public AuthorizationDecision ToAuthorizationDecision()
    {
      switch (this.Error)
      {
        case PlatformDelegatedAuthorizationServiceError.InvalidUserType:
          return new AuthorizationDecision()
          {
            AuthorizationError = AuthorizationError.InvalidUserType
          };
        case PlatformDelegatedAuthorizationServiceError.InvalidUserId:
          return new AuthorizationDecision()
          {
            AuthorizationError = AuthorizationError.InvalidUserId
          };
        case PlatformDelegatedAuthorizationServiceError.AccessDenied:
          return new AuthorizationDecision()
          {
            AuthorizationError = AuthorizationError.AccessDenied
          };
        default:
          return new AuthorizationDecision()
          {
            AuthorizationError = AuthorizationError.None
          };
      }
    }

    public HostAuthorizationDecision ToHostAuthorizationDecision()
    {
      if (this.Error == PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission)
        return new HostAuthorizationDecision()
        {
          HostAuthorizationError = HostAuthorizationError.AccessDenied
        };
      return new HostAuthorizationDecision()
      {
        HostAuthorizationError = HostAuthorizationError.None
      };
    }
  }
}
