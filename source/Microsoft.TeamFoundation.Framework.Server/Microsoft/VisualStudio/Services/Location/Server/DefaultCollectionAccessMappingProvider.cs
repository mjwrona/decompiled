// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.DefaultCollectionAccessMappingProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ExtensionPriority(200)]
  internal class DefaultCollectionAccessMappingProvider : IAccessMappingProvider
  {
    public void AddAccessMappings(
      IVssRequestContext requestContext,
      Dictionary<string, AccessMapping> accessMappings,
      string webApplicationRelativeDirectory)
    {
      AccessMapping accessMapping1;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || !(requestContext.ServiceInstanceType() == ServiceInstanceTypes.TFS) || !accessMappings.TryGetValue(AccessMappingConstants.VstsAccessMapping, out accessMapping1) || !string.IsNullOrEmpty(accessMapping1.VirtualDirectory))
        return;
      AccessMapping accessMapping2 = accessMapping1.Clone();
      accessMapping2.Moniker = "LegacyCollectionAccessMapping";
      accessMapping2.VirtualDirectory = "DefaultCollection";
      accessMappings[accessMapping2.Moniker] = accessMapping2;
    }
  }
}
