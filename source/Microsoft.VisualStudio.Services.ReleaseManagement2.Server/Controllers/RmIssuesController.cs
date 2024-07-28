// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmIssuesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid it")]
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "issues")]
  public class RmIssuesController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ClientLocationId("CD42261A-F5C6-41C8-9259-F078989B9F25")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<AutoTriggerIssue> GetIssues(int buildId, string sourceId = "") => this.TfsRequestContext.GetService<ReleasesService>().GetAutoTriggerIssues(this.TfsRequestContext, this.ProjectId, "Build", buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture), sourceId).ToAutoTriggerIssuesContract(this.TfsRequestContext);
  }
}
