// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleases9Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid it")]
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releases", ResourceVersion = 1)]
  public class RmReleases9Controller : RmReleases8Controller
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class should be refactored into smaller classes.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientResponseType(typeof (IEnumerable<Release>), null, null)]
    [ClientLocationId("6162082c-380f-4648-95d7-a72348c755f0")]
    [ClientForceCompatabilityLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientExample("Query_ListAllReleases.json", null, null, null)]
    [ClientExample("GET__ListAllReleasesForAReleaseDefinition.json", "For a release definition", null, null)]
    public override HttpResponseMessage GetReleases(
      int definitionId = 0,
      int definitionEnvironmentId = 0,
      string searchText = "",
      string createdBy = null,
      ReleaseStatus statusFilter = ReleaseStatus.Undefined,
      int environmentStatusFilter = 0,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder queryOrder = ReleaseQueryOrder.Descending,
      [FromUri(Name = "$top")] int top = 50,
      int continuationToken = 0,
      [FromUri(Name = "$expand")] ReleaseExpands expands = ReleaseExpands.None,
      string artifactTypeId = "",
      string sourceId = "",
      string artifactVersionId = "",
      string sourceBranchFilter = "",
      bool isDeleted = false,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tagFilter = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseIdFilter = null,
      string path = null)
    {
      if (environmentStatusFilter != 0 && !string.IsNullOrWhiteSpace(sourceBranchFilter))
        throw new InvalidRequestException(Resources.InvalidFilterUsage);
      return base.GetReleases(definitionId, definitionEnvironmentId, searchText, createdBy, statusFilter, environmentStatusFilter, minCreatedTime, maxCreatedTime, queryOrder, top, continuationToken, expands, artifactTypeId, sourceId, artifactVersionId, sourceBranchFilter, isDeleted, tagFilter, propertyFilters, releaseIdFilter, path);
    }
  }
}
