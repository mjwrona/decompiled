// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.MetricsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "metrics")]
  public class MetricsController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    public IEnumerable<Metric> GetMetrics(DateTime? minMetricsTime = null)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "MetricsController.GetMetrics", 1900020, 8, true))
      {
        minMetricsTime = new DateTime?(minMetricsTime ?? DateTime.UtcNow.AddDays(-7.0));
        IList<Metric> contract = this.TfsRequestContext.GetService<MetricsService>().GetMetrics(this.TfsRequestContext, this.ProjectId, minMetricsTime.Value).ToContract();
        if (contract != null)
          contract.ForEach<Metric>((Action<Metric>) (d => d.SetSecuredObject(this.ProjectId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture), 32)));
        return (IEnumerable<Metric>) contract;
      }
    }
  }
}
