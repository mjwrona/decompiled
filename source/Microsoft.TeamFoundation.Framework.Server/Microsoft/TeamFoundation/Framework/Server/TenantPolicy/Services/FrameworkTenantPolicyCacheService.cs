// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TenantPolicy.Services.FrameworkTenantPolicyCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.TenantPolicy;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.TenantPolicy.Services
{
  internal class FrameworkTenantPolicyCacheService : VssMemoryCacheService<string, Policy>
  {
    public static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(15.0);
    public static readonly TimeSpan ExpiryIntervalDefault = TimeSpan.FromHours(1.0);
    public static readonly TimeSpan InactivityIntervalDefault = TimeSpan.FromMinutes(15.0);

    public FrameworkTenantPolicyCacheService()
      : base(FrameworkTenantPolicyCacheService.CleanupInterval)
    {
      this.ExpiryInterval.Value = FrameworkTenantPolicyCacheService.ExpiryIntervalDefault;
      this.InactivityInterval.Value = FrameworkTenantPolicyCacheService.InactivityIntervalDefault;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      base.ServiceStart(requestContext);
    }

    public bool TryGetValue(
      IVssRequestContext requestContext,
      Guid tenantId,
      string policyName,
      out Policy policy)
    {
      if (!PolicyNames.KnownTenantPolicyNames.Contains<string>(policyName))
        throw new TenantPolicyBadRequestException(FrameworkResources.TenantPolicyNotExist((object) policyName));
      return this.TryGetValue(requestContext, this.GetCacheKey(tenantId, policyName), out policy);
    }

    public void Set(
      IVssRequestContext requestContext,
      Guid tenantId,
      string policyName,
      Policy policy)
    {
      if (!PolicyNames.KnownTenantPolicyNames.Contains<string>(policyName))
        throw new TenantPolicyBadRequestException(FrameworkResources.TenantPolicyNotExist((object) policyName));
      this.Set(requestContext, this.GetCacheKey(tenantId, policyName), policy);
    }

    private string GetCacheKey(Guid tenantId, string policyName) => string.Format("{0}_{1}", (object) tenantId.ToString(), (object) policyName);
  }
}
