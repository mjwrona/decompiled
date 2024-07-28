// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleases4Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releases", ResourceVersion = 4)]
  public class RmReleases4Controller : RmReleases3Controller
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class should be refactored into smaller classes.")]
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>), null, null)]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public override HttpResponseMessage GetReleases(
      int definitionId = 0,
      int definitionEnvironmentId = 0,
      string searchText = "",
      string createdBy = null,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus statusFilter = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Undefined,
      int environmentStatusFilter = 0,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder queryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Descending,
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
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmReleasesController.GetReleases", 1971001, 8, true))
      {
        ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus releaseStatus = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus) statusFilter;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder releaseQueryOrder = queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdAscending : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending;
        bool flag1 = (expands & ReleaseExpands.Artifacts) == ReleaseExpands.Artifacts;
        bool flag2 = (expands & ReleaseExpands.Approvals) == ReleaseExpands.Approvals;
        bool flag3 = (expands & ReleaseExpands.ManualInterventions) == ReleaseExpands.ManualInterventions;
        bool flag4 = (expands & ReleaseExpands.Variables) == ReleaseExpands.Variables;
        bool flag5 = (expands & ReleaseExpands.Environments) == ReleaseExpands.Environments;
        bool flag6 = (expands & ReleaseExpands.Tags) == ReleaseExpands.Tags;
        top = top > 100 || top < 0 ? 100 : top;
        IList<string> stringList1 = ServerModelUtility.ToStringList(tagFilter);
        IList<string> stringList2 = ServerModelUtility.ToStringList(propertyFilters);
        IEnumerable<int> intList = ServerModelUtility.ToIntList(releaseIdFilter);
        ReleaseEnvironmentStatus environmentStatus = (ReleaseEnvironmentStatus) environmentStatusFilter;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        int definitionId1 = definitionId;
        int definitionEnvironmentId1 = definitionEnvironmentId;
        string searchText1 = searchText;
        string createdBy1 = createdBy;
        int statusFilter1 = (int) releaseStatus;
        int environmentStatusFilter1 = (int) environmentStatus;
        DateTime? minCreatedTime1 = minCreatedTime;
        DateTime? maxCreatedTime1 = maxCreatedTime;
        DateTime? maxModifiedTime = new DateTime?();
        int queryOrder1 = (int) releaseQueryOrder;
        int top1 = top + 1;
        int releaseContinuationToken = continuationToken;
        int num1 = flag1 ? 1 : 0;
        int num2 = flag2 ? 1 : 0;
        int num3 = flag3 ? 1 : 0;
        int num4 = flag4 ? 1 : 0;
        int num5 = flag5 ? 1 : 0;
        int num6 = flag6 ? 1 : 0;
        string artifactTypeId1 = artifactTypeId;
        string sourceId1 = sourceId;
        string artifactVersionId1 = artifactVersionId;
        string sourceBranchFilter1 = sourceBranchFilter;
        int num7 = isDeleted ? 1 : 0;
        IList<string> propertyFilters1 = stringList2;
        IList<string> tagFilter1 = stringList1;
        IEnumerable<int> releaseIdFilter1 = intList;
        string path1 = path;
        List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> list1 = service.ListReleases(tfsRequestContext, projectId, definitionId1, definitionEnvironmentId1, searchText1, createdBy1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus) statusFilter1, (ReleaseEnvironmentStatus) environmentStatusFilter1, minCreatedTime1, maxCreatedTime1, maxModifiedTime, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder) queryOrder1, top1, releaseContinuationToken, num1 != 0, num2 != 0, num3 != 0, num4 != 0, num5 != 0, num6 != 0, artifactTypeId1, sourceId1, artifactVersionId1, sourceBranchFilter1, num7 != 0, false, (IEnumerable<string>) propertyFilters1, (IEnumerable<string>) tagFilter1, releaseIdFilter1, path1).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>().ToContract(this.TfsRequestContext, this.ProjectId).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>().Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) (r => this.LatestToIncoming(r))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
        list1.NormalizeWebApiReleases(expands);
        if (queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending)
          list1.Sort((Comparison<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) ((r1, r2) => r1.Id.CompareTo(r2.Id)));
        else
          list1.Sort((Comparison<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) ((r1, r2) => r2.Id.CompareTo(r1.Id)));
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> filteredReleases = this.GetFilteredReleases(list1, top);
        HttpResponseMessage responseMessage = (HttpResponseMessage) null;
        List<ProjectInfo> list2 = this.TfsRequestContext.GetService<IProjectService>().GetProjects(this.TfsRequestContext, ProjectState.WellFormed).ToList<ProjectInfo>();
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) filteredReleases)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release = release1;
          if (release.ProjectReference != null && !release.ProjectReference.Id.Equals(Guid.Empty))
          {
            ProjectInfo projectInfo = list2.FirstOrDefault<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.Id.Equals(release.ProjectReference.Id)));
            if (projectInfo != null)
              release.ProjectReference.Name = projectInfo.Name;
          }
        }
        try
        {
          responseMessage = this.Request.CreateResponse<IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>>(HttpStatusCode.OK, filteredReleases);
          if (list1.Count > top && list1[top] != null)
            ReleaseManagementProjectControllerBase.SetContinuationToken(responseMessage, list1[top].Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          return responseMessage;
        }
        catch (Exception ex)
        {
          responseMessage?.Dispose();
          this.TfsRequestContext.TraceException(1971001, "ReleaseManagementService", "Service", ex);
          throw;
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    [ClientExample("GET__GetARelease.json", null, null, null)]
    public override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release GetRelease(
      int releaseId,
      [ClientIgnore] bool includeAllApprovals = true,
      ApprovalFilters approvalFilters = ApprovalFilters.All,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      [FromUri(Name = "$expand")] SingleReleaseExpands expands = SingleReleaseExpands.Tasks,
      [FromUri(Name = "$topGateRecords")] int topGateRecords = 5)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmReleases2Controller.GetRelease", 1971003, 3, true))
      {
        this.PublishGetReleaseCustomerIntelligenceData(this.TfsRequestContext, releaseId);
        IList<string> stringList = ServerModelUtility.ToStringList(propertyFilters);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.TfsRequestContext.GetService<ReleasesService>().GetRelease(this.TfsRequestContext, this.ProjectId, releaseId, propertyFilters: (IEnumerable<string>) stringList);
        if (!includeAllApprovals)
          approvalFilters = ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals;
        topGateRecords = topGateRecords > 10 || topGateRecords <= 0 ? 10 : topGateRecords;
        bool flag = (expands & SingleReleaseExpands.Tasks) == SingleReleaseExpands.Tasks;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        int num1 = flag ? 1 : 0;
        int num2 = (int) approvalFilters;
        int numberOfGateRecords = topGateRecords;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = release.ToContract(tfsRequestContext, projectId, num1 != 0, (ApprovalFilters) num2, numberOfGateRecords);
        this.TfsRequestContext.SetSecuredObject<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(contract);
        return this.LatestToIncoming(contract);
      }
    }

    [HttpPut]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.None)]
    public void UndeleteRelease(int releaseId, string comment)
    {
      ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = service.GetRelease(this.TfsRequestContext, this.ProjectId, releaseId, true);
      if (release == null)
        throw new ReleaseNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ReleaseNotFound, (object) releaseId));
      this.CheckForReleaseDefinitionSecurityPermission(ReleaseManagementSecurityProcessor.GetFolderPath(this.TfsRequestContext, this.ProjectId, release.ReleaseDefinitionId), release.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ManageReleases);
      service.UndeleteRelease(this.TfsRequestContext, this.ProjectId, release, comment);
    }

    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (release.Reason == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.PullRequest)
        release.Reason = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.ContinuousIntegration;
      if (release.Environments == null)
        return release;
      release.Environments.HandleCancelingStateBackCompatibility();
      release.Environments.HandleGateCanceledStateBackCompatibility();
      return release;
    }

    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release IncomingToLatest(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (release == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseObjectNotProvidedInBody);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment) null;
        if (serverRelease != null)
          serverEnvironment = serverRelease.GetEnvironment(environment.Id);
        environment.HandleDeploymentGatesCompatibility(serverEnvironment);
      }
      return release;
    }

    private IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> GetFilteredReleases(
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> webApiReleases,
      int top)
    {
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releaseList1 = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
      foreach (IGrouping<Guid, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> grouping in webApiReleases.GroupBy<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Guid>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Guid>) (i => i.ProjectReference.Id)))
      {
        Guid projectId = grouping.First<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>().ProjectReference.Id;
        IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, projectId, (IList<int>) grouping.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, int>) (r => r.ReleaseDefinitionReference.Id)).ToList<int>());
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releaseList2 = ReleaseManagementSecurityProcessor.FilterComponents<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(this.TfsRequestContext, (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) grouping, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, ReleaseManagementSecurityInfo>) (a => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, folderPaths[a.ReleaseDefinitionReference.Id], a.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), true);
        if (releaseList2.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>())
          releaseList1.AddRange<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>>((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) releaseList2);
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) releaseList1.Take<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(top).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
    }
  }
}
