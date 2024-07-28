// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleasesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releases")]
  public class RmReleasesController : ReleaseManagementProjectControllerBase
  {
    public const int DefaultTop = 50;
    public const int MaxAllowedTop = 100;

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class should be refactored into smaller classes.")]
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>), null, null)]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public virtual HttpResponseMessage GetReleases(
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
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus statusFilter1 = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus) statusFilter;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder1 = queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdAscending : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending;
        bool includeArtifacts = (expands & ReleaseExpands.Artifacts) == ReleaseExpands.Artifacts;
        bool includeApprovals = (expands & ReleaseExpands.Approvals) == ReleaseExpands.Approvals;
        bool includeManualInterventions = (expands & ReleaseExpands.ManualInterventions) == ReleaseExpands.ManualInterventions;
        bool includeVariables = (expands & ReleaseExpands.Variables) == ReleaseExpands.Variables;
        bool includeEnvironments = (expands & ReleaseExpands.Environments) == ReleaseExpands.Environments;
        bool includeTags = (expands & ReleaseExpands.Tags) == ReleaseExpands.Tags;
        top = top > 100 || top < 0 ? 100 : top;
        IList<string> stringList1 = ServerModelUtility.ToStringList(tagFilter);
        IList<string> stringList2 = ServerModelUtility.ToStringList(propertyFilters);
        IEnumerable<int> intList = ServerModelUtility.ToIntList(releaseIdFilter);
        ReleaseEnvironmentStatus environmentStatusFilter1 = (ReleaseEnvironmentStatus) environmentStatusFilter;
        expands &= ~ReleaseExpands.ManualInterventions;
        List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> list = service.ListReleases(this.TfsRequestContext, this.ProjectId, definitionId, definitionEnvironmentId, searchText, createdBy, statusFilter1, environmentStatusFilter1, minCreatedTime, maxCreatedTime, new DateTime?(), queryOrder1, top + 1, continuationToken, includeArtifacts, includeApprovals, includeManualInterventions, includeVariables, includeEnvironments, includeTags, artifactTypeId, sourceId, artifactVersionId, sourceBranchFilter, isDeleted, false, (IEnumerable<string>) stringList2, (IEnumerable<string>) stringList1, intList, path).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>().ToContract(this.TfsRequestContext, this.ProjectId).Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) (r => this.LatestToIncoming(r))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
        list.NormalizeWebApiReleases(expands);
        if (queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending)
          list.Sort((Comparison<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) ((r1, r2) => r1.Id.CompareTo(r2.Id)));
        else
          list.Sort((Comparison<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) ((r1, r2) => r2.Id.CompareTo(r1.Id)));
        HttpResponseMessage responseMessage = (HttpResponseMessage) null;
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases = service.FilterReleasesWithViewPermissions(this.TfsRequestContext, this.ProjectId, (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) list).Take<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(top);
        try
        {
          responseMessage = this.Request.CreateResponse<IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>>(HttpStatusCode.OK, releases);
          if (list.Count > top && list[top] != null)
            ReleaseManagementProjectControllerBase.SetContinuationToken(responseMessage, list[top].Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release GetRelease(
      int releaseId,
      [ClientIgnore] bool includeAllApprovals = true,
      ApprovalFilters approvalFilters = ApprovalFilters.All,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      [FromUri(Name = "$expand")] SingleReleaseExpands expands = SingleReleaseExpands.Tasks,
      [FromUri(Name = "$topGateRecords")] int topGateRecords = 5)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmReleasesController.GetRelease", 1971003, 3, true))
      {
        this.PublishGetReleaseCustomerIntelligenceData(this.TfsRequestContext, releaseId);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.TfsRequestContext.GetService<ReleasesService>().GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
        if (!includeAllApprovals)
          approvalFilters = ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals;
        topGateRecords = topGateRecords > 10 || topGateRecords <= 0 ? 10 : topGateRecords;
        bool flag = (expands & SingleReleaseExpands.Tasks) == SingleReleaseExpands.Tasks;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        int num1 = flag ? 1 : 0;
        int num2 = (int) approvalFilters;
        int numberOfGateRecords = topGateRecords;
        return this.LatestToIncoming(release.ToContract(tfsRequestContext, projectId, num1 != 0, (ApprovalFilters) num2, numberOfGateRecords).ToV1Release());
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    [HttpGet]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    [ClientExample("GET__GetReleaseRevisionOfRelease.json", null, null, null)]
    public HttpResponseMessage GetReleaseRevision(int releaseId, int definitionSnapshotRevision)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(this.GetService<ReleaseHistoryService>().GetRevision(this.TfsRequestContext, this.ProjectId, releaseId, definitionSnapshotRevision));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      return response;
    }

    [HttpPost]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseStartMetadata", ReleaseManagementSecurityArgumentType.ReleaseStartMetadataObject, ReleaseManagementSecurityPermissions.CreateReleases)]
    [ClientExample("POST__CreateARelease.json", null, null, null)]
    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release CreateRelease(
      [FromBody] ReleaseStartMetadata releaseStartMetadata)
    {
      if (releaseStartMetadata == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.NoRequestBodySpecifiedInPostMethod);
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmReleasesController.CreateRelease", 1971014, 1, true))
      {
        string str = this.ProjectInfo != null ? this.ProjectInfo.Name : throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.NoProjectInfoPresent);
        ReleaseProjectInfo releaseProjectInfo = new ReleaseProjectInfo()
        {
          Id = this.ProjectId,
          Name = str ?? string.Empty
        };
        CreateReleaseParameters releaseParameters = releaseStartMetadata.ToCreateReleaseParameters(this.TfsRequestContext);
        ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
        return this.LatestToIncoming((releaseStartMetadata.IsDraft ? service.CreateRelease(this.TfsRequestContext, releaseProjectInfo, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition) null, releaseParameters) : service.InitiateRelease(this.TfsRequestContext, releaseProjectInfo, releaseParameters)).ToContract(this.TfsRequestContext, this.ProjectId, true, ApprovalFilters.All));
      }
    }

    [HttpPatch]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageReleases)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release UpdateReleaseResource(
      int releaseId,
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      releaseUpdateMetadata.Status = releaseUpdateMetadata != null ? releaseUpdateMetadata.Status.ConvertToNewReleaseStatusWithUpdatedValue() : throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EmptyBodyIsNotAllowedInPatchRequest);
      return this.UpdateReleaseResourceImplementation(releaseId, releaseUpdateMetadata);
    }

    private static IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> GetEnvironmentsWithUpdatedConditions(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleasesService releasesService,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> updatedConditions = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>();
      if (releaseUpdateMetadata.ManualEnvironments.IsNullOrEmpty<string>())
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) updatedConditions;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = releasesService.GetRelease(requestContext, projectId, releaseId);
      if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft && releaseUpdateMetadata.Status == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Active)
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) ReleaseBuilder.ClearConditionsForSelectedEnvironments(requestContext, release, releaseUpdateMetadata.ManualEnvironments).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>();
      throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.CannotOverrideEnvironmentToManual);
    }

    [HttpPost]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageReleases)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release UpdateRelease(
      int releaseId,
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.NoRequestBodySpecifiedInPostMethod);
      ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = service.GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
      release = this.IncomingToLatest(release, release1);
      return this.LatestToIncoming(service.UpdateRelease(this.TfsRequestContext, this.ProjectId, releaseId, release, release1).ToContract(this.TfsRequestContext, this.ProjectId, true, ApprovalFilters.All));
    }

    [HttpDelete]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.None)]
    public void DeleteRelease(int releaseId, string comment = "")
    {
      ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = service.GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
      if (release == null)
        throw new ReleaseNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ReleaseNotFound, (object) releaseId));
      string folderPath = ReleaseManagementSecurityProcessor.GetFolderPath(this.TfsRequestContext, this.ProjectId, release.ReleaseDefinitionId);
      if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft)
        this.CheckForReleaseDefinitionSecurityPermission(folderPath, release.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ManageReleases);
      else
        this.CheckForReleaseDefinitionSecurityPermission(folderPath, release.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.DeleteReleases);
      if (release.KeepForever)
        throw new ReleaseDeletionNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ReleaseDeletionNotAllowedAsKeepForeverIsSet, (object) release.Name));
      service.DeleteRelease(this.TfsRequestContext, this.ProjectId, release, comment);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary GetReleaseDefinitionSummary(
      int definitionId,
      int releaseCount,
      bool includeArtifact = false,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string definitionEnvironmentIdsFilter = null)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary webApi = ReleaseDefinitionSummaryConverter.ConvertToWebApi(this.TfsRequestContext.GetService<ReleasesService>().GetReleaseDefinitionSummary(this.TfsRequestContext, this.ProjectId, definitionId, releaseCount, includeArtifact, definitionEnvironmentIdsFilter), this.TfsRequestContext, this.ProjectId);
      this.TfsRequestContext.SetSecuredObject<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary>(webApi);
      return webApi;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Analytics api shuld not throw")]
    protected virtual void PublishGetReleaseCustomerIntelligenceData(
      IVssRequestContext requestContext,
      int releaseId)
    {
      try
      {
        RmTelemetryFactory.GetLogger(requestContext).PublishReleaseGetByUser(requestContext, releaseId, requestContext.GetUserId(true), requestContext.GetUserCuid());
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972113, TraceLevel.Error, "ReleaseManagementService", "Analytics", "Failed to publish getRelease customer intellligence data. Exception {0}", (object) ex);
      }
    }

    protected Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release UpdateReleaseResourceImplementation(
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      if (releaseUpdateMetadata == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EmptyBodyIsNotAllowedInPatchRequest);
      using (PerformanceTelemetryService.Measure(this.TfsRequestContext, "Service", "RmReleasesController.UpdateReleaseResource", 5, true))
      {
        bool flag1 = false;
        ReleaseProjectInfo projectInfo = new ReleaseProjectInfo()
        {
          Id = this.ProjectId,
          Name = this.ProjectInfo.Name ?? string.Empty
        };
        ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release) null;
        bool isStatusChanged;
        bool isKeepForeverChanged;
        bool areManualEnvironmentsChanged;
        bool isNameChanged;
        RmReleasesController.GetUpdatedProperties(releaseUpdateMetadata, out isStatusChanged, out isKeepForeverChanged, out areManualEnvironmentsChanged, out isNameChanged);
        if (isNameChanged && !isStatusChanged && !isKeepForeverChanged && !areManualEnvironmentsChanged)
          flag1 = true;
        KeyValuePair<int, string> definitionFolderPathAndId = service.GetReleaseDefinitionFolderPathAndId(this.TfsRequestContext, this.ProjectId, releaseId);
        int key = definitionFolderPathAndId.Key;
        string folderPath = definitionFolderPathAndId.Value;
        bool flag2 = this.TfsRequestContext.HasPermission(this.ProjectId, folderPath, key, ReleaseManagementSecurityPermissions.ManageReleases);
        if (!flag2 & flag1)
          flag2 = this.TfsRequestContext.HasPermission(this.ProjectId, folderPath, key, ReleaseManagementSecurityPermissions.ManageTaskHubExtension);
        if (!flag2)
          throw new AccessCheckException(new ResourceAccessException(this.TfsRequestContext.GetUserId().ToString(), ReleaseManagementSecurityPermissions.ManageReleases).Message);
        if (isKeepForeverChanged | areManualEnvironmentsChanged)
        {
          IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> updatedConditions = RmReleasesController.GetEnvironmentsWithUpdatedConditions(this.TfsRequestContext, this.ProjectId, service, releaseId, releaseUpdateMetadata);
          release = service.PatchRelease(this.TfsRequestContext, this.ProjectId, releaseId, updatedConditions, releaseUpdateMetadata.KeepForever, releaseUpdateMetadata.Comment);
        }
        if (isStatusChanged | isNameChanged)
          release = service.UpdateReleaseNameAndStatus(this.TfsRequestContext, projectInfo, releaseId, releaseUpdateMetadata.FromContract());
        if (release == null)
          throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.InvalidPatchReleaseRequest);
        return this.LatestToIncoming(release.ToContract(this.TfsRequestContext, this.ProjectId, true, ApprovalFilters.All));
      }
    }

    protected void CheckForReleaseDefinitionSecurityPermission(
      string releaseDefinitionPath,
      int releaseDefinitionId,
      ReleaseManagementSecurityPermissions permission)
    {
      if (!this.TfsRequestContext.HasPermission(this.ProjectId, releaseDefinitionPath, releaseDefinitionId, permission))
        throw new AccessCheckException(new ResourceAccessException(this.TfsRequestContext.GetUserId().ToString(), permission).Message);
    }

    protected virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (release.Environments != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
        {
          environment.ToNoPhasesFormat();
          environment.HandleCancelingStateBackCompatibility();
          environment.HandleGateCanceledStateBackCompatibility();
          if (environment.Status == EnvironmentStatus.PartiallySucceeded)
            environment.Status = EnvironmentStatus.Rejected;
          if (environment.DeploySteps != null)
          {
            foreach (DeploymentAttempt deployStep in environment.DeploySteps)
            {
              if (deployStep.Status == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.PartiallySucceeded)
                deployStep.Status = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Failed;
            }
          }
        }
      }
      if (release.Reason == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.PullRequest)
        release.Reason = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.ContinuousIntegration;
      return release;
    }

    protected virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release IncomingToLatest(
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
        environment.ToDeployPhasesFormat(serverEnvironment);
        environment.HandleDeploymentGatesCompatibility(serverEnvironment);
      }
      return release;
    }

    private static void GetUpdatedProperties(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseUpdateMetadata releaseUpdateMetadata,
      out bool isStatusChanged,
      out bool isKeepForeverChanged,
      out bool areManualEnvironmentsChanged,
      out bool isNameChanged)
    {
      isStatusChanged = false;
      isKeepForeverChanged = false;
      areManualEnvironmentsChanged = false;
      isNameChanged = false;
      if (releaseUpdateMetadata.Status != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStatus.Undefined)
        isStatusChanged = true;
      if (releaseUpdateMetadata.KeepForever.HasValue)
        isKeepForeverChanged = true;
      if (!releaseUpdateMetadata.ManualEnvironments.IsNullOrEmpty<string>())
        areManualEnvironmentsChanged = true;
      if (releaseUpdateMetadata.Name.IsNullOrEmpty<char>())
        return;
      isNameChanged = true;
    }
  }
}
