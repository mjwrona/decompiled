// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationSecurityNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class LocationSecurityNamespaceExtension : DefaultSecurityNamespaceExtension
  {
    public override bool HasPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      int effectiveAllows,
      int effectiveDenys,
      bool preliminaryDecision)
    {
      Guid spId;
      if (!preliminaryDecision && token.Length > FrameworkSecurity.ServiceDefinitionsToken.Length && token.StartsWith(FrameworkSecurity.ServiceDefinitionsToken, StringComparison.OrdinalIgnoreCase) && ServicePrincipals.IsServicePrincipal(requestContext, principal.PrimaryDescriptor, false, out spId))
      {
        string[] strArray = token.Split(FrameworkSecurity.LocationPathSeparator);
        if (strArray.Length == 4)
        {
          string x = strArray[2];
          Guid serviceInstanceType = new Guid(strArray[3]);
          if ((VssStringComparer.ServiceType.Equals(x, "VsService") || VssStringComparer.ServiceType.Equals(x, "LocationService2")) && InstanceManagementHelper.ServicePrincipalFromServiceInstance(serviceInstanceType) == spId)
            return true;
        }
      }
      return preliminaryDecision;
    }
  }
}
