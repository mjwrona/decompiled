// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ServiceEndpointHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ServiceEndpointHelper
  {
    public const string EndpointOboAuthorizationStrongBoxDrawerFormat = "/Service/ReleaseManagement/{0}/Releases/{1}/Environment/{2}";
    public const string AccessTokenKey = "accesstoken";
    public const string RefreshTokenKey = "RefreshToken";

    public static bool IsAzureActiveDirectoryAccount(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost != null && requestContext.ServiceHost.OrganizationServiceHost != null && requestContext.IsOrganizationAadBacked();
    }

    public static List<DeploymentAuthorizationInfo> GetDeploymentAuthorizationInfoForRevalidateApproverByTenantId(
      IEnumerable<DeploymentAuthorizationInfo> deploymentAuthorizationInfoList,
      string tenantId)
    {
      return !deploymentAuthorizationInfoList.IsNullOrEmpty<DeploymentAuthorizationInfo>() ? deploymentAuthorizationInfoList.Where<DeploymentAuthorizationInfo>((Func<DeploymentAuthorizationInfo, bool>) (d => string.Equals(d.TenantId, tenantId, StringComparison.OrdinalIgnoreCase) && d.AuthorizationHeaderFor == AuthorizationHeaderFor.RevalidateApproverIdentity)).ToList<DeploymentAuthorizationInfo>() : (List<DeploymentAuthorizationInfo>) null;
    }
  }
}
