// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FrameworkVstsAadOAuthService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class FrameworkVstsAadOAuthService : IVstsAadOAuthService2, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string CreateAadOAuthRequest(
      IVssRequestContext requestContext,
      string tenantId,
      string redirectUri,
      AadLoginPromptOption promptOption = AadLoginPromptOption.SelectAccount,
      string completeCallbackPayload = null,
      bool completeCallbackByAuthCode = false)
    {
      throw new NotImplementedException();
    }

    public string GetVstsAadTenantId(IVssRequestContext requestContext) => throw new NotImplementedException();
  }
}
