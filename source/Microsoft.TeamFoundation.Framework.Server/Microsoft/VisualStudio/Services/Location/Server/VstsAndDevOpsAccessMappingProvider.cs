// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.VstsAndDevOpsAccessMappingProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ExtensionPriority(300)]
  internal class VstsAndDevOpsAccessMappingProvider : IAccessMappingProvider
  {
    private const string c_area = "LocationService";
    private const string c_layer = "VstsAndDevOpsAccessMappingProvider";

    public void AddAccessMappings(
      IVssRequestContext requestContext,
      Dictionary<string, AccessMapping> accessMappings,
      string webApplicationRelativeDirectory)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IList<NameResolutionEntry> nameResolutionEntryList = vssRequestContext.GetService<NameResolutionStore>().QueryEntriesForValue(vssRequestContext, requestContext.ServiceHost.InstanceId);
      AccessMapping accessMapping1 = VstsDomainHostResolver.GetAccessMapping(vssRequestContext, nameResolutionEntryList);
      if (accessMapping1 != null)
        accessMappings[accessMapping1.Moniker] = accessMapping1;
      AccessMapping accessMapping2 = DevOpsOrgAndCollectionHostResolver.GetAccessMapping(vssRequestContext, nameResolutionEntryList);
      if (accessMapping2 != null)
        accessMappings[accessMapping2.Moniker] = accessMapping2;
      if (accessMapping1 != null && accessMapping2 != null || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || ServiceHostNameHelper.IsPGuid(requestContext.ServiceHost.Name) || ServiceHostNameHelper.IsDGuid(requestContext.ServiceHost.Name))
        return;
      requestContext.Trace(1521913335, TraceLevel.Error, "LocationService", nameof (VstsAndDevOpsAccessMappingProvider), string.Format("Malformed name resolution records: vstsExists={0}, azuredevopsExists={1}, entries={2}", (object) (accessMapping1 != null), (object) (accessMapping2 != null), (object) string.Join<NameResolutionEntry>(", ", (IEnumerable<NameResolutionEntry>) nameResolutionEntryList)));
    }
  }
}
