// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.OnPremInheritedAccessMappingProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class OnPremInheritedAccessMappingProvider : IAccessMappingProvider
  {
    public void AddAccessMappings(
      IVssRequestContext requestContext,
      Dictionary<string, AccessMapping> accessMappings,
      string webApplicationRelativeDirectory)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
      IEnumerable<AccessMapping> accessMappings1 = vssRequestContext.GetService<ILocationService>().GetAccessMappings(vssRequestContext);
      OnPremInheritedAccessMappingProvider.InheritAccessMappings(accessMappings, accessMappings1, webApplicationRelativeDirectory);
    }

    private static void InheritAccessMappings(
      Dictionary<string, AccessMapping> mappings,
      IEnumerable<AccessMapping> parentMappings,
      string webApplicationRelativeDirectory)
    {
      AccessMapping[] array = mappings.Values.ToArray<AccessMapping>();
      foreach (AccessMapping parentMapping in parentMappings)
      {
        if (!mappings.ContainsKey(parentMapping.Moniker))
        {
          bool flag = false;
          foreach (AccessMapping accessMapping in array)
          {
            if (VssStringComparer.ServerUrl.StartsWith(accessMapping.AccessPoint, parentMapping.AccessPoint) || VssStringComparer.ServerUrl.StartsWith(parentMapping.AccessPoint, accessMapping.AccessPoint))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            AccessMapping accessMapping = parentMapping.Clone();
            accessMapping.VirtualDirectory = LocationServiceHelper.ComputeVirtualDirectory(webApplicationRelativeDirectory);
            mappings[accessMapping.Moniker] = accessMapping;
          }
        }
      }
    }
  }
}
