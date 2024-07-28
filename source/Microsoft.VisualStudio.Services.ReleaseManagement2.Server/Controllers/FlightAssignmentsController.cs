// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.FlightAssignmentsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "flightAssignments")]
  public class FlightAssignmentsController : TfsApiController
  {
    [HttpGet]
    [ClientLocationId("409D301F-3046-46F3-BEB9-4357FBCE0A8C")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientInternalUseOnly(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public IList<string> GetFlightAssignments([ClientQueryParameter] string flightName = null) => this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? (IList<string>) new List<string>() : FlightAssignmentsHelper.GetFlightAssignments(this.TfsRequestContext, new Uri("https://rm-vsts-tas.msedge.net/ab"), flightName);
  }
}
