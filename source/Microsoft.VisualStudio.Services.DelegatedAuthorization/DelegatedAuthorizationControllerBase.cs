// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DelegatedAuthorizationControllerBase
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public abstract class DelegatedAuthorizationControllerBase : TfsApiController
  {
    protected const string TokenServiceFeatureFlag = "VisualStudio.DelegatedAuthorizationService.TokenService";
    protected const string DelegatedAuthorizationFeatureFlag = "VisualStudio.DelegatedAuthorizationService.DelegatedAuthorizationService";
    protected const string TokenServiceReadFeatureFlag = "VisualStudio.DelegatedAuthorizationService.TokenServiceRead";
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "DelegatedAuthorizationControllerBase";

    public override string TraceArea => "DelegatedAuthorizationService";

    public override string ActivityLogArea => "DelegatedAuthorization";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions { get; } = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (SessionTokenCreateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RegistrationNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FailedToIssueAccessTokenException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (SessionTokenNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ExchangeAccessTokenKeyException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (HostAuthorizationCreateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TokenPairCreateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ExchangeAppTokenCreateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AppSessionTokenCreateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RegistrationCreateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AuthorizationNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (RegistrationAlreadyExistsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ExchangeAppTokenNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PlatformDelegatedAuthorizationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPublicKeyException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidPersonalAccessTokenException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (HostAuthorizationNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (AuthorizationIdNotFoundException),
        HttpStatusCode.NotFound
      }
    };

    protected class InternalServerErrorException : Exception
    {
      public InternalServerErrorException(string message)
        : base(message)
      {
      }
    }
  }
}
