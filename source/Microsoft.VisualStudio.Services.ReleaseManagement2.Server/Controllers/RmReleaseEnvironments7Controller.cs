// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleaseEnvironments7Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid it")]
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environments", ResourceVersion = 7)]
  public class RmReleaseEnvironments7Controller : RmReleaseEnvironments6Controller
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment GetReleaseEnvironment(
      int releaseId,
      int environmentId,
      [FromUri(Name = "$expand")] ReleaseEnvironmentExpands expands)
    {
      using (PerformanceTelemetryService.Measure(this.TfsRequestContext, "Service", "RmReleaseEnvironmentsController.GetReleaseEnvironment", 50, true))
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.TfsRequestContext.GetService<ReleasesService>().GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
        if (release1.GetEnvironment(environmentId) == null)
        {
          IList<int> environmentIdList = release1.GetEnvironmentIdList();
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EnvironmentIdIsInvalid, (object) environmentId, (object) string.Join(",", environmentIdList.Select<int, string>((Func<int, string>) (id => id.ToString((IFormatProvider) CultureInfo.CurrentCulture))).ToArray<string>())));
        }
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironmentById = release1.GetReleaseEnvironmentById(environmentId);
        bool flag = (expands & ReleaseEnvironmentExpands.Tasks) == ReleaseEnvironmentExpands.Tasks;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release2 = release1;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        int num = flag ? 1 : 0;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment incoming = this.LatestToIncoming(releaseEnvironmentById.ToContract(release2, tfsRequestContext, projectId, num != 0));
        this.TfsRequestContext.SetSecuredObject<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>(incoming);
        return incoming;
      }
    }
  }
}
