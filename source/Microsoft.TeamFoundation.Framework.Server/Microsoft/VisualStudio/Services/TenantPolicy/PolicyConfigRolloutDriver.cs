// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.PolicyConfigRolloutDriver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  internal class PolicyConfigRolloutDriver
  {
    private static readonly IConfigPrototype<bool> configPrototypeRestrictFullScopePat = ConfigPrototype.Create<bool>("TenantPolicy.RestrictFullScopePersonalAccessToken", false);
    private static readonly IConfigPrototype<bool> configPrototypeRestrictGlobalPat = ConfigPrototype.Create<bool>("TenantPolicy.RestrictGlobalPersonalAccessToken", false);
    private static readonly IConfigPrototype<bool> configPrototypeRestrictPatLifespan = ConfigPrototype.Create<bool>("TenantPolicy.RestrictPersonalAccessTokenLifespan", false);
    private static readonly IConfigPrototype<bool> configPrototypeEnableLeakedPatAutoRevocation = ConfigPrototype.Create<bool>("TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation", false);
    private readonly IConfigQueryable<bool> configRestrictFullScopePat;
    private readonly IConfigQueryable<bool> configRestrictGlobalPat;
    private readonly IConfigQueryable<bool> configRestrictPatLifespan;
    private readonly IConfigQueryable<bool> configEnableLeakedPatAutoRevocation;

    public PolicyConfigRolloutDriver()
    {
      this.configRestrictFullScopePat = ConfigProxy.Create<bool>(PolicyConfigRolloutDriver.configPrototypeRestrictFullScopePat);
      this.configRestrictGlobalPat = ConfigProxy.Create<bool>(PolicyConfigRolloutDriver.configPrototypeRestrictGlobalPat);
      this.configRestrictPatLifespan = ConfigProxy.Create<bool>(PolicyConfigRolloutDriver.configPrototypeRestrictPatLifespan);
      this.configEnableLeakedPatAutoRevocation = ConfigProxy.Create<bool>(PolicyConfigRolloutDriver.configPrototypeEnableLeakedPatAutoRevocation);
    }

    public bool IsPolicyEnabledForTenant(
      IVssRequestContext requestContext,
      string policyName,
      Guid tenantId)
    {
      switch (policyName)
      {
        case "TenantPolicy.RestrictFullScopePersonalAccessToken":
          return this.configRestrictFullScopePat.QueryByTenantId<bool>(requestContext, tenantId);
        case "TenantPolicy.RestrictGlobalPersonalAccessToken":
          return this.configRestrictGlobalPat.QueryByTenantId<bool>(requestContext, tenantId);
        case "TenantPolicy.RestrictPersonalAccessTokenLifespan":
          return this.configRestrictPatLifespan.QueryByTenantId<bool>(requestContext, tenantId);
        case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation":
          return this.configEnableLeakedPatAutoRevocation.QueryByTenantId<bool>(requestContext, tenantId);
        default:
          return false;
      }
    }
  }
}
