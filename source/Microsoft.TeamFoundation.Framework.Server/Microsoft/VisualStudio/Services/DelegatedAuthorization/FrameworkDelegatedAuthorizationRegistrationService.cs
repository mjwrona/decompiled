// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.FrameworkDelegatedAuthorizationRegistrationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkDelegatedAuthorizationRegistrationService : 
    IDelegatedAuthorizationRegistrationService,
    IVssFrameworkService
  {
    private static readonly string s_Area = "DelegatedAuthorizationRegistrationService";
    private static readonly string s_Layer = nameof (FrameworkDelegatedAuthorizationRegistrationService);

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public Registration Create(IVssRequestContext requestContext, Registration registration) => this.Create(requestContext, registration, false);

    public Registration Create(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      requestContext.TraceEnter(1057000, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Create));
      try
      {
        this.TryResolveMasterId(requestContext, registration);
        return this.GetHttpClient(requestContext).CreateRegistrationAsync(registration, includeSecret).SyncResult<Registration>();
      }
      finally
      {
        requestContext.TraceLeave(1057009, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Create));
      }
    }

    public Registration Update(IVssRequestContext requestContext, Registration registration) => this.Update(requestContext, registration, false);

    public Registration Update(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      requestContext.TraceEnter(1056000, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Update));
      try
      {
        this.TryResolveMasterId(requestContext, registration);
        return this.GetHttpClient(requestContext).UpdateRegistrationAsync(registration, includeSecret).SyncResult<Registration>();
      }
      finally
      {
        requestContext.TraceLeave(1056009, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Update));
      }
    }

    public void Delete(IVssRequestContext requestContext, Guid registrationId)
    {
      requestContext.TraceEnter(1048160, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Delete));
      try
      {
        this.GetHttpClient(requestContext).DeleteRegistrationAsync(registrationId).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(1048169, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Delete));
      }
    }

    public JsonWebToken GetSecret(IVssRequestContext requestContext, Registration registration)
    {
      requestContext.TraceEnter(1048140, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (GetSecret));
      try
      {
        this.TryResolveMasterId(requestContext, registration);
        return this.GetHttpClient(requestContext).GetSecretAsync(registration.RegistrationId).SyncResult<JsonWebToken>();
      }
      finally
      {
        requestContext.TraceLeave(1048149, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (GetSecret));
      }
    }

    public Registration Get(IVssRequestContext requestContext, Guid registrationId) => this.Get(requestContext, registrationId, false);

    public Registration Get(
      IVssRequestContext requestContext,
      Guid registrationId,
      bool includeSecret)
    {
      requestContext.TraceEnter(1055000, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Get));
      try
      {
        return this.GetHttpClient(requestContext).GetRegistrationAsync(registrationId, includeSecret).SyncResult<Registration>();
      }
      finally
      {
        requestContext.TraceLeave(1055009, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (Get));
      }
    }

    public IList<Registration> List(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1048150, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (List));
      try
      {
        return (IList<Registration>) this.GetHttpClient(requestContext).GetRegistrationsAsync().SyncResult<System.Collections.Generic.List<Registration>>();
      }
      finally
      {
        requestContext.TraceLeave(1048159, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (List));
      }
    }

    private void TryResolveMasterId(IVssRequestContext requestContext, Registration registration)
    {
      Guid masterId;
      if (!IdentityHelper.TryResolveMasterId(requestContext, registration.IdentityId, out masterId))
        return;
      registration.IdentityId = masterId;
    }

    public Registration RotateSecret(IVssRequestContext requestContext, Guid registrationId)
    {
      requestContext.TraceEnter(1048161, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (RotateSecret));
      try
      {
        return this.GetHttpClient(requestContext).RotateSecretAsync(registrationId).SyncResult<Registration>();
      }
      finally
      {
        requestContext.TraceLeave(1048162, FrameworkDelegatedAuthorizationRegistrationService.s_Area, FrameworkDelegatedAuthorizationRegistrationService.s_Layer, nameof (RotateSecret));
      }
    }

    internal virtual DelegatedAuthorizationHttpClient GetHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<DelegatedAuthorizationHttpClient>();
    }
  }
}
