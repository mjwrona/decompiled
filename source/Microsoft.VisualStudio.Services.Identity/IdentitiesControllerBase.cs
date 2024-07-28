// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitiesControllerBase
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [RestrictDeploymentIdentitiesAccess]
  [ClientInclude(~RestClientLanguages.Swagger2)]
  public abstract class IdentitiesControllerBase : TfsApiController
  {
    private static readonly HashSet<string> ExcludedProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      "Issuer",
      "ExpiresOn",
      "Audience",
      "HMACSHA256",
      "CUID",
      "CUIDState"
    };
    private const string s_featureReturnEmptyGuidMasterId = "VisualStudio.Services.Identity.ReturnEmptyGuidMasterId";

    public override string ActivityLogArea => "Identities";

    protected void ScrubMasterId(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities, bool forUpdate = false)
    {
      if (identities.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      bool returnEmptyGuid = this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ReturnEmptyGuidMasterId");
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        IdentityHelper.ScrubMasterId(identity, returnEmptyGuid, forUpdate);
    }

    protected void ScrubIdentityPropertiesAndMasterId(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool forUpdate = false)
    {
      if (identities.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      bool returnEmptyGuid = this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ReturnEmptyGuidMasterId");
      bool flag = IdentitiesControllerBase.DeploymentAccessChecker.HasDeploymentAccess(this.TfsRequestContext) && !IdentityHelper.IsShardedFrameworkIdentity(this.TfsRequestContext, this.TfsRequestContext.UserContext);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
      {
        if (identity != null)
          IdentitiesControllerBase.ScrubIdentityProperties((IDictionary<string, object>) identity.Properties);
        if (forUpdate)
        {
          if (flag)
            identity.SetAllModifiedProperties();
          else
            identity.Properties.Clear();
        }
        IdentityHelper.ScrubMasterId(identity, returnEmptyGuid, forUpdate);
      }
    }

    protected static void ScrubIdentityProperties(IDictionary<string, object> properties)
    {
      if (properties == null || properties.Count == 0)
        return;
      foreach (string key in properties.Keys.ToArray<string>())
      {
        if (IdentitiesControllerBase.ExcludedProperties.Contains(key))
          properties.Remove(key);
      }
    }

    protected internal static class DeploymentAccessChecker
    {
      private const string AccessDeploymentIdentityDataInHostedToken = "DeploymentIdentityHosted";

      public static bool HasDeploymentAccess(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsHostedDeployment || IdentitiesControllerBase.DeploymentAccessChecker.HasDeploymentAccessHelper(requestContext.To(TeamFoundationHostType.Deployment), out IVssSecurityNamespace _);

      public static void CheckDeploymentAccess(IVssRequestContext requestContext)
      {
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IVssSecurityNamespace @namespace;
        if (IdentitiesControllerBase.DeploymentAccessChecker.HasDeploymentAccessHelper(requestContext, out @namespace))
          return;
        @namespace.ThrowAccessDeniedException(requestContext, "DeploymentIdentityHosted", 1, (EvaluationPrincipal) requestContext.GetAuthenticatedDescriptor());
      }

      private static bool HasDeploymentAccessHelper(
        IVssRequestContext deploymentRequestContext,
        out IVssSecurityNamespace @namespace)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        @namespace = deploymentRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(deploymentRequestContext, FrameworkSecurity.FrameworkNamespaceId);
        return (1 & @namespace.QueryEffectivePermissions(deploymentRequestContext, "DeploymentIdentityHosted", (EvaluationPrincipal) deploymentRequestContext.GetAuthenticatedDescriptor())) != 0;
      }
    }
  }
}
