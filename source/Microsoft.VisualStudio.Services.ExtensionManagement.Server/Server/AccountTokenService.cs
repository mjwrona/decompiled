// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.AccountTokenService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class AccountTokenService : IAccountTokenService, IVssFrameworkService
  {
    private int m_expirationSeconds = 86400;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_expirationSeconds = systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) "/Configuration/ExtensionService/TokenTimeoutSeconds", this.m_expirationSeconds);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetToken(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return (string) null;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      IKeyManagementService service = context.GetService<IKeyManagementService>();
      string token = (string) null;
      IVssRequestContext requestContext1 = context;
      byte[] signingKey = service.GetSigningKey(requestContext1);
      if (signingKey != null)
      {
        IVssRequestContext vssRequestContext = requestContext;
        token = TokenManagement.GenerateJwtToken(new JwtClaims()
        {
          Expiration = new DateTime?(DateTime.UtcNow.AddSeconds((double) this.m_expirationSeconds)),
          ExtraClaims = new Dictionary<string, string>()
          {
            {
              "aid",
              vssRequestContext.ServiceHost.InstanceId.ToString()
            }
          }
        }, signingKey);
      }
      return token;
    }
  }
}
