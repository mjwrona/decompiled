// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SpsServiceDefinitionController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Location", ResourceName = "SpsServiceDefinition")]
  public class SpsServiceDefinitionController : TfsApiController
  {
    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateSpsServiceDefinition(Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      ITeamFoundationHostManagementService service = this.TfsRequestContext.GetService<ITeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(this.TfsRequestContext, hostId);
      if (serviceHostProperties != null)
      {
        using (IVssRequestContext vssRequestContext1 = service.BeginRequest(this.TfsRequestContext, hostId, RequestContextType.ServicingContext))
        {
          if (!vssRequestContext1.IsVirtualServiceHost())
          {
            LocationServiceHelper.UpdateSpsServiceLocationRaw(vssRequestContext1);
            vssRequestContext1.GetService<IInternalLocationService>().OnLocationDataChanged(vssRequestContext1, LocationDataKind.All);
            if (serviceHostProperties.HostType == TeamFoundationHostType.ProjectCollection)
            {
              IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Application);
              if (vssRequestContext2.IsVirtualServiceHost())
                vssRequestContext2.GetService<IInternalLocationService>().OnLocationDataChanged(vssRequestContext2, LocationDataKind.All);
            }
          }
        }
      }
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    public override string TraceArea => "LocationService";

    public override string ActivityLogArea => "Framework";
  }
}
