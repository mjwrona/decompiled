// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceAreaHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ResourceAreaHelper
  {
    private const string c_area = "LocationService";
    private const string c_layer = "ResourceAreaHelper";

    public static bool IsResourceRegistered(
      IVssRequestContext requestContext,
      Guid areaId,
      Guid resourceId)
    {
      requestContext.TraceEnter(1944028043, "LocationService", nameof (ResourceAreaHelper), nameof (IsResourceRegistered));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (vssRequestContext.GetService<IInheritedLocationDataService>().GetData(vssRequestContext, LocationServiceConstants.RootIdentifier, TeamFoundationHostTypeHelper.NormalizeHostType(requestContext.ServiceHost.HostType))?.FindServiceDefinition("LocationService2", areaId) == null)
      {
        requestContext.Trace(1760938632, TraceLevel.Info, "LocationService", nameof (ResourceAreaHelper), "Could not find area {0} at the deployment level.", (object) areaId);
        return false;
      }
      try
      {
        if (requestContext.GetClient<ResourceAreaHelper.ResourceAreaHelperClient>(areaId).HasResourceId(resourceId))
          return true;
        requestContext.Trace(1300056477, TraceLevel.Info, "LocationService", nameof (ResourceAreaHelper), "Could not find resource with resource id {0}", (object) resourceId);
        return false;
      }
      catch (ServiceOwnerNotFoundException ex)
      {
        requestContext.Trace(1160828368, TraceLevel.Info, "LocationService", nameof (ResourceAreaHelper), "Could not find area at this host level {0}", (object) areaId);
        return false;
      }
    }

    internal class ResourceAreaHelperClient : VssHttpClientBase
    {
      public ResourceAreaHelperClient(Uri baseUrl, VssCredentials credentials)
        : base(baseUrl, credentials)
      {
      }

      public ResourceAreaHelperClient(
        Uri baseUrl,
        HttpMessageHandler pipeline,
        bool disposeHandler)
        : base(baseUrl, pipeline, disposeHandler)
      {
      }

      public virtual bool HasResourceId(Guid resourceId) => this.HasResourceLocations && this.GetResourceLocationAsync(resourceId).SyncResult<ApiResourceLocation>() != null;
    }
  }
}
