// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionClaimService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionClaimService : IVssFrameworkService, IContributionClaimService
  {
    private const string c_userContributionClaimsKey = "user-contribution-claims";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public bool HasClaim(IVssRequestContext requestContext, string claim) => !string.IsNullOrEmpty(claim) && this.GetClaimsInternal(requestContext).Contains(claim);

    public HashSet<string> GetClaims(IVssRequestContext requestContext) => this.GetClaimsInternal(requestContext);

    internal static HashSet<string> ComputeUserIdentityClaims(IVssRequestContext requestContext)
    {
      HashSet<string> userIdentityClaims = new HashSet<string>();
      if (requestContext.IsAnonymousPrincipal())
        userIdentityClaims.Add("anonymous");
      else if (requestContext.IsPublicUser())
        userIdentityClaims.Add("public");
      else if (!requestContext.GetUserId().Equals(Guid.Empty))
        userIdentityClaims.Add("member");
      return userIdentityClaims;
    }

    private HashSet<string> GetClaimsInternal(IVssRequestContext requestContext)
    {
      object obj;
      if (!requestContext.RootContext.Items.TryGetValue("user-contribution-claims", out obj) || !(obj is HashSet<string> claimsInternal))
      {
        claimsInternal = new HashSet<string>(requestContext.GetService<IContributedFeatureService>().GetFeatureClaims(requestContext), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        claimsInternal.UnionWith((IEnumerable<string>) ContributionClaimService.ComputeUserIdentityClaims(requestContext));
        requestContext.RootContext.Items["user-contribution-claims"] = (object) claimsInternal;
      }
      return claimsInternal;
    }
  }
}
