// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.PolicyNames
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  [GenerateAllConstants(null)]
  public static class PolicyNames
  {
    public const string Namespace = "TenantPolicy";
    private const string NamespaceSeparator = ".";
    private const string OrganizationCreationRestrictionToken = "OrganizationCreationRestriction";
    private const string RestrictGlobalPersonalAccessTokenSuffix = "RestrictGlobalPersonalAccessToken";
    private const string RestrictFullScopePersonalAccessTokenSuffix = "RestrictFullScopePersonalAccessToken";
    private const string RestrictPersonalAccessTokenLifespanSuffix = "RestrictPersonalAccessTokenLifespan";
    private const string EnableLeakedPersonalAccessTokenAutoRevocationSuffix = "EnableLeakedPersonalAccessTokenAutoRevocation";
    public const string OrganizationCreationRestriction = "TenantPolicy.OrganizationCreationRestriction";
    public const string RestrictGlobalPersonalAccessToken = "TenantPolicy.RestrictGlobalPersonalAccessToken";
    public const string RestrictFullScopePersonalAccessToken = "TenantPolicy.RestrictFullScopePersonalAccessToken";
    public const string RestrictPersonalAccessTokenLifespan = "TenantPolicy.RestrictPersonalAccessTokenLifespan";
    public const string EnableLeakedPersonalAccessTokenAutoRevocation = "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation";
    public static readonly IReadOnlyCollection<string> KnownTenantPolicyNames = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "TenantPolicy.OrganizationCreationRestriction",
      "TenantPolicy.RestrictGlobalPersonalAccessToken",
      "TenantPolicy.RestrictFullScopePersonalAccessToken",
      "TenantPolicy.RestrictPersonalAccessTokenLifespan",
      "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation"
    };
  }
}
