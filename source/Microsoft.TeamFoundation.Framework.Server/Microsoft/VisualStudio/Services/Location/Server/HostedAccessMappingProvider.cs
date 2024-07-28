// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.HostedAccessMappingProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ExtensionPriority(100)]
  internal class HostedAccessMappingProvider : IAccessMappingProvider
  {
    public void AddAccessMappings(
      IVssRequestContext requestContext,
      Dictionary<string, AccessMapping> accessMappings,
      string webApplicationRelativeDirectory)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        AccessMapping guidAccessMapping;
        if (!accessMappings.TryGetValue(AccessMappingConstants.HostGuidAccessMappingMoniker, out guidAccessMapping))
        {
          guidAccessMapping = LocationServiceHelper.GetHostGuidAccessMapping(requestContext, requestContext.ServiceHost.InstanceId);
          accessMappings[AccessMappingConstants.HostGuidAccessMappingMoniker] = guidAccessMapping;
        }
        if (accessMappings.ContainsKey(AccessMappingConstants.PublicAccessMappingMoniker))
          return;
        AccessMapping accessMapping = LocationServiceHelper.GetHostPublicAccessMapping(requestContext, requestContext.ServiceHost.InstanceId);
        if (accessMapping == null)
        {
          accessMapping = guidAccessMapping.Clone();
          accessMapping.Moniker = AccessMappingConstants.PublicAccessMappingMoniker;
          accessMapping.DisplayName = TFCommonResources.PublicAccessMappingDisplayName();
        }
        accessMappings[AccessMappingConstants.PublicAccessMappingMoniker] = accessMapping;
      }
      else
      {
        if (accessMappings.ContainsKey(AccessMappingConstants.HostGuidAccessMappingMoniker))
          return;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Location.UseDevOpsDomainForS2S"))
        {
          AccessMapping accessMapping;
          if (!accessMappings.TryGetValue(AccessMappingConstants.ServiceDomainMappingMoniker, out accessMapping))
            return;
          AccessMapping guidAccessMapping = LocationServiceHelper.GetHostGuidAccessMapping(requestContext, requestContext.ServiceHost.InstanceId, accessMapping.ToLocationMapping());
          accessMappings[AccessMappingConstants.HostGuidAccessMappingMoniker] = guidAccessMapping;
        }
        else
        {
          AccessMapping accessMapping1;
          if (!accessMappings.TryGetValue(AccessMappingConstants.PublicAccessMappingMoniker, out accessMapping1))
            return;
          AccessMapping accessMapping2 = accessMapping1.Clone();
          accessMapping2.Moniker = AccessMappingConstants.HostGuidAccessMappingMoniker;
          accessMappings[AccessMappingConstants.HostGuidAccessMappingMoniker] = accessMapping2;
        }
      }
    }
  }
}
