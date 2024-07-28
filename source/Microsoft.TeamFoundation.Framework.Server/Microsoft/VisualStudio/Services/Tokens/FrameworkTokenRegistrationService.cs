// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.FrameworkTokenRegistrationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Tokens
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkTokenRegistrationService : 
    TokenServiceBase,
    ITokenRegistrationService,
    IDelegatedAuthorizationRegistrationService,
    IVssFrameworkService
  {
    private static readonly string s_Area = "TokenRegistrationService";
    private static readonly string s_Layer = nameof (FrameworkTokenRegistrationService);

    public Registration Create(IVssRequestContext requestContext, Registration registration) => this.Create(requestContext, registration, false);

    public Registration Create(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      Guid? userId = new Guid?(registration.IdentityId);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (registration.IsWellKnown && !IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity) && registration.ClientType == ClientType.Application)
        userId = new Guid?();
      return TokenServiceBase.ExecuteTokenServiceResultRequest<Registration>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, Registration>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1057000, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Create));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).CreateRegistrationAsync(registration, includeSecret).SyncResult<Registration>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1057009, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Create));
        }
      }), userId);
    }

    public Registration Update(IVssRequestContext requestContext, Registration registration) => this.Update(requestContext, registration, false);

    public Registration Update(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      Guid? userId = new Guid?(registration.IdentityId);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (registration.IsWellKnown && !IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity) && registration.ClientType == ClientType.Application)
        userId = new Guid?();
      return TokenServiceBase.ExecuteTokenServiceResultRequest<Registration>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, Registration>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1056000, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Update));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).UpdateRegistrationAsync(registration, includeSecret).SyncResult<Registration>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1056009, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Update));
        }
      }), userId);
    }

    public void Delete(IVssRequestContext requestContext, Guid registrationId) => TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Action<IVssRequestContext>) (context =>
    {
      context.TraceEnter(1048160, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Delete));
      try
      {
        this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).DeleteRegistrationAsync(registrationId).SyncResult();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048169, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Delete));
      }
    }));

    public JsonWebToken GetSecret(IVssRequestContext requestContext, Registration registration) => TokenServiceBase.ExecuteTokenServiceResultRequest<JsonWebToken>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, JsonWebToken>) ((context, isImpersonating) =>
    {
      requestContext.TraceEnter(1048140, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (GetSecret));
      try
      {
        return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).GetSecretAsync(registration.RegistrationId).SyncResult<JsonWebToken>();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048149, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (GetSecret));
      }
    }), new Guid?(registration.IdentityId));

    public JsonWebToken GetSecret(IVssRequestContext requestContext, Guid registrationId) => TokenServiceBase.ExecuteTokenServiceResultRequest<JsonWebToken>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, JsonWebToken>) ((context, isImpersonating) =>
    {
      context.TraceEnter(1048140, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (GetSecret));
      try
      {
        return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).GetSecretAsync(registrationId).SyncResult<JsonWebToken>();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048149, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (GetSecret));
      }
    }));

    public Registration Get(IVssRequestContext requestContext, Guid registrationId) => this.Get(requestContext, registrationId, false);

    public Registration Get(
      IVssRequestContext requestContext,
      Guid registrationId,
      bool includeSecret)
    {
      return TokenServiceBase.ExecuteTokenServiceResultRequest<Registration>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, Registration>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1055000, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Get));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).GetRegistrationAsync(registrationId, includeSecret).SyncResult<Registration>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1055009, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (Get));
        }
      }));
    }

    public IList<Registration> List(IVssRequestContext requestContext) => (IList<Registration>) TokenServiceBase.ExecuteTokenServiceResultRequest<System.Collections.Generic.List<Registration>>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, System.Collections.Generic.List<Registration>>) ((context, isImpersonating) =>
    {
      context.TraceEnter(1048150, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (List));
      try
      {
        return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).GetRegistrationsAsync().SyncResult<System.Collections.Generic.List<Registration>>();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048159, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (List));
      }
    }));

    public Registration RotateSecret(IVssRequestContext requestContext, Guid registrationId) => TokenServiceBase.ExecuteTokenServiceResultRequest<Registration>(requestContext, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, (Func<IVssRequestContext, bool, Registration>) ((context, isImpersonating) =>
    {
      context.TraceEnter(1048161, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (RotateSecret));
      try
      {
        return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).RotateSecretAsync(registrationId).SyncResult<Registration>();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048162, FrameworkTokenRegistrationService.s_Area, FrameworkTokenRegistrationService.s_Layer, nameof (RotateSecret));
      }
    }), elevateCall: true);
  }
}
