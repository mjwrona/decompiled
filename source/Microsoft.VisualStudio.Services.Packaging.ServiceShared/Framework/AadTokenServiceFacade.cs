// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.AadTokenServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public class AadTokenServiceFacade : IInjectableAadTokenService
  {
    private readonly IVssRequestContext requestContext;

    public AadTokenServiceFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public JwtSecurityToken AcquireAppToken(string resource, string tenantId) => this.requestContext.GetService<IAadTokenService>().AcquireAppToken(this.requestContext, resource, tenantId);
  }
}
