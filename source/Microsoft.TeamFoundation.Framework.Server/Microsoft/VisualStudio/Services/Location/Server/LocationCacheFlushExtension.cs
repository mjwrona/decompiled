// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationCacheFlushExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class LocationCacheFlushExtension : IHostManagementExtension
  {
    public const string c_area = "LocationService";
    public const string c_layer = "RemoteLocationCacheFlushExtension";

    public void Initialize(IVssRequestContext systemRequestContext)
    {
    }

    public void OnBeforeCreateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      ISqlConnectionInfo connectionInfo)
    {
    }

    public void OnBeforeDeleteServiceHost(
      IVssRequestContext requestContext,
      Guid hostId,
      HostDeletionReason deletionReason,
      DeleteHostResourceOptions deleteHostResourceOptions)
    {
    }

    public void OnBeforeUpdateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties newProperties,
      TeamFoundationServiceHostProperties oldProperties)
    {
    }

    public void OnAfterUpdateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties newProperties,
      TeamFoundationServiceHostProperties oldProperties)
    {
      if (StringComparer.OrdinalIgnoreCase.Equals(oldProperties.Name, newProperties.Name))
        return;
      this.ClearCache(requestContext, newProperties.Id);
    }

    private void ClearCache(IVssRequestContext requestContext, Guid hostId)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(704203553, "LocationService", "RemoteLocationCacheFlushExtension", nameof (ClearCache));
      try
      {
        if (hostId == requestContext.ServiceHost.InstanceId)
        {
          requestContext.Trace(1924373822, TraceLevel.Info, "LocationService", "RemoteLocationCacheFlushExtension", "Skipping {0} because it's the deployment host", (object) hostId);
        }
        else
        {
          ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
          try
          {
            using (IVssRequestContext vssRequestContext = service.BeginRequest(requestContext, hostId, RequestContextType.SystemContext, throwIfShutdown: false))
              vssRequestContext.GetService<IInternalLocationService>().OnLocationDataChanged(vssRequestContext, LocationDataKind.All);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1924373822, "LocationService", "RemoteLocationCacheFlushExtension", ex);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(704203554, "LocationService", "RemoteLocationCacheFlushExtension", nameof (ClearCache));
      }
    }
  }
}
