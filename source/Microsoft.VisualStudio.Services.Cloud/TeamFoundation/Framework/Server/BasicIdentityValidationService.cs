// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicIdentityValidationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class BasicIdentityValidationService : IIdentityValidationService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IdentityValidationResult ValidateRequestIdentity(IVssRequestContext requestContext)
    {
      requestContext.RequestContextInternal().IdentityValidationStatus |= IdentityValidationStatus.Validated;
      RequestRestrictions requestRestrictions = requestContext.RequestRestrictions();
      return (requestRestrictions != null ? (int) requestRestrictions.RequiredAuthentication : 1) > 1 && requestContext.IsAnonymous() ? requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().CompleteUnauthorizedRequest(requestContext, HttpContextFactory.Current.Response, IdentityValidationResult.Unauthorized(FrameworkResources.AuthenticationRequiredError()), (Uri) null) : IdentityValidationResult.Success;
    }
  }
}
