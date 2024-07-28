// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitionsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
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
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be reviewed.")]
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "definitions")]
  public class RmDefinitionsController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    public static HttpResponseMessage GetReleaseDefinitionRevision(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      HttpRequestMessage httpRequest,
      ReleaseDefinitionHistoryService releaseDefinitionHistoryService,
      int definitionId,
      int revision)
    {
      if (releaseDefinitionHistoryService == null)
        throw new ArgumentNullException(nameof (releaseDefinitionHistoryService));
      Stream revision1 = releaseDefinitionHistoryService.GetRevision(tfsRequestContext, projectId, definitionId, revision);
      HttpResponseMessage response = httpRequest.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(revision1);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      return response;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>), null, null)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None, true)]
    [ClientExample("GET__ListAllReleaseDefinitions.json", null, null, null)]
    [ClientExample("GET__ListAllReleaseDefinitionsWithEnvironmentsExpanded.json", "With environments details expanded", null, null)]
    [ClientExample("GET_ListAllReleaseDefinitionsWithArtifactsExpanded.json", "With artifacts details expanded", null, null)]
    public virtual HttpResponseMessage GetReleaseDefinitions(
      string searchText = "",
      [FromUri(Name = "$expand")] ReleaseDefinitionExpands expands = ReleaseDefinitionExpands.None,
      string artifactType = "",
      string artifactSourceId = null,
      [FromUri(Name = "$top")] int top = 10000,
      string continuationToken = "",
      ReleaseDefinitionQueryOrder queryOrder = ReleaseDefinitionQueryOrder.IdAscending,
      string path = null,
      bool isExactNameMatch = false,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tagFilter = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string definitionIdFilter = null,
      bool isDeleted = false,
      bool searchTextContainsFolderName = false)
    {
      return this.CreateReleaseDefinitionsResponse(this.GetReleaseDefinitionsList(searchText, expands, artifactType, artifactSourceId, top, continuationToken, queryOrder, path, isExactNameMatch, tagFilter, propertyFilters, definitionIdFilter, isDeleted, searchTextContainsFolderName), top, queryOrder);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientExample("GET__GetReleaseDefinition.json", "Get a release definition", null, null)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition GetReleaseDefinition(
      int definitionId,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null)
    {
      IEnumerable<string> propertyFilters1 = ReleaseManagementArtifactPropertyKinds.AsPropertyFilters(propertyFilters);
      return this.LatestToIncoming(this.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(this.TfsRequestContext, this.ProjectId, definitionId, propertyFilters1, true));
    }

    [HttpPost]
    [ReleaseManagementSecurityPermission("releaseDefinition", ReleaseManagementSecurityArgumentType.ReleaseDefinition, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    [ClientExample("POST__CreateReleaseDefinition.json", null, null, null)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition CreateReleaseDefinition(
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseDefinitionCannotBeNull);
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RDController.CreateReleaseDefinition", 1961004, 1, true))
        return this.CreateReleaseDefinitionInternal(releaseDefinition);
    }

    [HttpPut]
    [ReleaseManagementSecurityPermission("releaseDefinition", ReleaseManagementSecurityArgumentType.ReleaseDefinition, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    [ClientExample("PUT__UpdateReleaseDefinition.json", "Update the release definition", null, null)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition UpdateReleaseDefinition(
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmDefinitionsController.UpdateReleaseDefinition", 1961013, 3, true))
        return this.UpdateReleaseDefinitionInternal(releaseDefinition);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Optional parameter to ensure back-compat")]
    [HttpDelete]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.DeleteReleaseDefinition)]
    [ClientExample("DELETE__DeleteReleaseDefinition.json", "Delete a release definition", null, null)]
    public void DeleteReleaseDefinition(int definitionId, string comment = null, bool forceDelete = false)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmDefinitionsController.DeleteReleaseDefinition", 1961031, 5, true))
        this.GetService<ReleaseDefinitionsService>().SoftDeleteReleaseDefinition(this.TfsRequestContext, this.ProjectId, definitionId, comment, forceDelete);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true)]
    [ClientExample("GET__GetReleaseDefinitionRevisions.json", "Get release definition revision", null, null)]
    public HttpResponseMessage GetReleaseDefinitionRevision(int definitionId, int revision) => RmDefinitionsController.GetReleaseDefinitionRevision(this.TfsRequestContext, this.ProjectId, this.Request, this.GetService<ReleaseDefinitionHistoryService>(), definitionId, revision);

    protected void SaveReleaseDefinitionRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AuditAction changeType)
    {
      this.GetService<ReleaseDefinitionHistoryService>().SaveRevision(requestContext, projectId, releaseDefinition, this.Request.GetApiResourceVersion().ToString(), changeType);
    }

    protected Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition CreateReleaseDefinitionInternal(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseDefinitionCannotBeNull);
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RDController.CreateReleaseDefinition", 1961004, 1, true))
      {
        this.FixIncomingDefinition(releaseDefinition, false);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition1 = releaseDefinition.FromContract(this.TfsRequestContext, this.ProjectId);
        releaseDefinition.ValidateBackCompatibilityChanges(this.TfsRequestContext);
        releaseManagementTimer.RecordLap("Service", "RDController.CreateReleaseDefinition.ObjectValidations", 1961008);
        if (releaseDefinition1.IsYamlDefinition(this.TfsRequestContext))
        {
          this.LoadYaml(this.TfsRequestContext, releaseDefinition1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition) null);
        }
        else
        {
          new ReleaseDefinitionVariableGroupValidations().ValidateVariableGroups(this.TfsRequestContext, this.ProjectId, releaseDefinition1, false);
          new ReleaseDefinitionsSecureFileValidations().ValidateSecureFiles(this.TfsRequestContext, this.ProjectId, releaseDefinition1);
          ReleaseDefinitionEnvironmentSecurityValidations securityValidations = new ReleaseDefinitionEnvironmentSecurityValidations(this.TfsRequestContext, this.ProjectId, releaseDefinition);
          securityValidations.CheckExistenceOfUsedResources();
          securityValidations.CheckUsePermissionOnQueuesUsed();
          securityValidations.CheckUsePermissionOnMachineGroupsUsed();
          releaseManagementTimer.RecordLap("Service", "RDController.CreateReleaseDefinition.QueuesPermissionsCheck", 1961206);
          securityValidations.CheckEndpointsPermissionOnEnvironmentsForCreate();
          releaseManagementTimer.RecordLap("Service", "RDController.CreateReleaseDefinition.EndpointsPermissionsCheck", 1961206);
          ArtifactValidations.ValidateEndpointPermissionsForArtifactsForCreate(this.TfsRequestContext, this.ProjectId, releaseDefinition1);
          releaseDefinition.SanitizeAndValidateNoDuplicateTaskRefNamesInPhase();
        }
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition contract = this.GetService<ReleaseDefinitionsService>().AddReleaseDefinition(this.TfsRequestContext, this.ProjectId, releaseDefinition1, releaseDefinition.Comment).ToContract(this.TfsRequestContext, this.ProjectId);
        contract.Comment = releaseDefinition.Comment;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition incoming = this.LatestToIncoming(contract);
        this.SaveReleaseDefinitionRevision(this.TfsRequestContext, this.ProjectId, incoming.ToWebApiDefinitionV2(), Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AuditAction.Add);
        return incoming;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506", Justification = "Each code path have tests")]
    protected Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition UpdateReleaseDefinitionInternal(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseDefinitionCannotBeNull);
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmDefinitionsController.UpdateReleaseDefinition", 1961013, 3, true))
      {
        this.FixIncomingDefinition(releaseDefinition);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition1 = releaseDefinition.FromContract(this.TfsRequestContext, this.ProjectId);
        releaseManagementTimer.RecordLap("Service", "RmDefinitionsController.UpdateReleaseDefinition.ObjectValidations", 1961008);
        ReleaseDefinitionsService service = this.GetService<ReleaseDefinitionsService>();
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition existingDefinition = service.GetReleaseDefinition(this.TfsRequestContext, this.ProjectId, releaseDefinition.Id);
        if (releaseDefinition.Revision != existingDefinition.Revision)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.DefinitionAlreadyUpdated));
        releaseDefinition.ValidateBackCompatibilityChanges(this.TfsRequestContext, existingDefinition);
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition existingWebApiDefinition = existingDefinition.ToContract(this.TfsRequestContext, this.ProjectId);
        ReleaseDefinitionEnvironmentSecurityValidations securityValidations = new ReleaseDefinitionEnvironmentSecurityValidations(this.TfsRequestContext, this.ProjectId, releaseDefinition, (Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) ((requestContext, projectId, releaseDefintionId) => existingWebApiDefinition));
        if (releaseDefinition1.IsYamlDefinition(this.TfsRequestContext))
        {
          this.LoadYaml(this.TfsRequestContext, releaseDefinition1, existingDefinition);
        }
        else
        {
          new ReleaseDefinitionVariableGroupValidations((Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>) ((requestContext, projectId, releaseDefintionId) => existingDefinition)).ValidateVariableGroups(this.TfsRequestContext, this.ProjectId, releaseDefinition1, true);
          new ReleaseDefinitionsSecureFileValidations((Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>) ((requestContext, projectId, releaseDefintionId) => existingDefinition)).ValidateSecureFiles(this.TfsRequestContext, this.ProjectId, releaseDefinition1, true);
          new ReleaseDefinitionApproverValidations((Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) ((requestContext, projectId, releaseDefintionId) => existingWebApiDefinition)).CheckManagePermission(this.TfsRequestContext, this.ProjectId, releaseDefinition);
          securityValidations.CheckExistenceOfUsedResourcesInChangedEnvironments();
          securityValidations.CheckSecurityPermissionForEdit();
          releaseManagementTimer.RecordLap("Service", "RmDefinitionsController.UpdateReleaseDefinition.SecurityPermissionsCheck", 1962010);
          securityValidations.CheckUsePermissionOnQueuesAddedOrChanged();
          securityValidations.CheckUsePermissionOnMachineGroupsAddedOrChanged();
          releaseManagementTimer.RecordLap("Service", "RmDefinitionsController.UpdateReleaseDefinition.QueuesPermissionCheck", 1962010);
          ArtifactValidations.ValidateEndpointPermissionsForArtifactsForUpdate(this.TfsRequestContext, this.ProjectId, releaseDefinition1);
          releaseDefinition.SanitizeAndValidateNoDuplicateTaskRefNamesInPhase();
        }
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition contract = service.UpdateReleaseDefinition(this.TfsRequestContext, this.ProjectId, releaseDefinition1, releaseDefinition.Comment).ToContract(this.TfsRequestContext, this.ProjectId);
        contract.Comment = releaseDefinition.Comment;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition incoming = this.LatestToIncoming(contract);
        this.SaveReleaseDefinitionRevision(this.TfsRequestContext, this.ProjectId, incoming.ToWebApiDefinitionV2(), Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AuditAction.Update);
        if (!releaseDefinition1.IsYamlDefinition(this.TfsRequestContext))
        {
          IEnumerable<ReleaseDefinitionEnvironment> deletedEnvironments = securityValidations.GetDeletedEnvironments();
          if (deletedEnvironments != null && deletedEnvironments.Any<ReleaseDefinitionEnvironment>())
          {
            List<string> tokens = new List<string>();
            foreach (ReleaseDefinitionEnvironment definitionEnvironment in deletedEnvironments)
            {
              string token = ReleaseManagementSecurityHelper.GetToken(this.ProjectId, releaseDefinition.Path, releaseDefinition.Id, definitionEnvironment.Id);
              tokens.Add(token);
            }
            ReleaseManagementSecurityProcessor.RemoveAccessControlLists(this.TfsRequestContext, (IEnumerable<string>) tokens);
          }
        }
        return incoming;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    protected virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition IncomingToLatest(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition incomingDefinition,
      bool isUpdateReleaseDefinition = true)
    {
      if (incomingDefinition == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseDefinitionCannotBeNull);
      if (incomingDefinition.Environments != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition) null;
        if (isUpdateReleaseDefinition)
          releaseDefinition = this.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(this.TfsRequestContext, this.ProjectId, incomingDefinition.Id);
        foreach (ReleaseDefinitionEnvironment environment1 in (IEnumerable<ReleaseDefinitionEnvironment>) incomingDefinition.Environments)
        {
          if (environment1 != null && environment1.DeployPhases != null && environment1.DeployPhases.Count > 0)
            throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidPropertyDeployPhaseInEnvironment, (object) environment1.Name));
          environment1.ToDeployPhasesFormat();
          if (releaseDefinition != null)
          {
            DefinitionEnvironment environment2 = releaseDefinition.GetEnvironment(environment1.Id);
            if (environment2 != null)
            {
              environment1.HandleDeploymentGatesCompatibility(environment2);
              environment1.HandleEnvironmentTriggersCompatibility(environment2);
            }
          }
        }
      }
      this.HandleRetentionBackCompat(incomingDefinition, this.ProjectId);
      incomingDefinition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DesignerPipelineProcess();
      return incomingDefinition;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> LatestToIncoming(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> definitions)
    {
      return definitions != null ? definitions.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) (d => this.LatestToIncoming(d))) : throw new ArgumentNullException(nameof (definitions));
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (definition.Environments != null)
      {
        foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
          environment.ToNoPhasesFormat();
      }
      definition.ToUndefinedReleaseDefinitionSource();
      definition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) null;
      return definition;
    }

    protected IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> GetReleaseDefinitionsList(
      string searchText = "",
      ReleaseDefinitionExpands expands = ReleaseDefinitionExpands.None,
      string artifactType = "",
      string artifactSourceId = null,
      int top = 10000,
      string continuationToken = "",
      ReleaseDefinitionQueryOrder queryOrder = ReleaseDefinitionQueryOrder.IdAscending,
      string path = null,
      bool isExactNameMatch = false,
      string tagFilter = null,
      string propertyFilters = null,
      string definitionIdFilter = null,
      bool isDeleted = false,
      bool searchTextContainsFolderName = false)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RDController.GetReleaseDefinitions", 1961208, 5, true))
      {
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> definitions;
        if ((!(this.ProjectId != Guid.Empty) || !string.IsNullOrEmpty(searchText) || expands != ReleaseDefinitionExpands.None || !string.IsNullOrEmpty(artifactType) || !string.IsNullOrEmpty(artifactSourceId) || top != 10000 || !string.IsNullOrEmpty(continuationToken) || !string.IsNullOrEmpty(path) || !string.IsNullOrEmpty(tagFilter) || !string.IsNullOrEmpty(propertyFilters) || isDeleted ? 0 : (string.IsNullOrEmpty(definitionIdFilter) ? 1 : 0)) != 0)
        {
          definitions = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) this.GetAllReleaseDefinitions();
        }
        else
        {
          RmDefinitionsController.ValidateContinutationToken(continuationToken, queryOrder);
          ReleaseDefinitionExpands expands1 = expands;
          if (!string.IsNullOrEmpty(artifactType) || !string.IsNullOrEmpty(artifactSourceId))
            expands1 = expands1 | ReleaseDefinitionExpands.Artifacts | ReleaseDefinitionExpands.Environments | ReleaseDefinitionExpands.Triggers;
          isExactNameMatch = !string.IsNullOrWhiteSpace(searchText) & isExactNameMatch;
          searchTextContainsFolderName = !string.IsNullOrWhiteSpace(searchText) & searchTextContainsFolderName;
          definitions = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) this.GetReleaseDefinitionsList(searchText, artifactType, artifactSourceId, expands1, continuationToken, top + 1, queryOrder, path, isExactNameMatch, tagFilter, propertyFilters, definitionIdFilter, isDeleted, searchTextContainsFolderName);
        }
        releaseManagementTimer.RecordLap("Service", "ReleaseDefinitionService.GetServerReleaseDefintionsList.DefinitionsFetchedAndConverted", 1961208);
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) this.LatestToIncoming((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) definitions).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>();
      }
    }

    protected HttpResponseMessage CreateReleaseDefinitionsResponse(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> definitions,
      int top,
      ReleaseDefinitionQueryOrder queryOrder)
    {
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> releaseDefinitions = top >= 0 ? definitions.Take<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>(top) : (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>) definitions;
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      try
      {
        responseMessage = this.Request.CreateResponse<IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>>(HttpStatusCode.OK, releaseDefinitions);
        if (top >= 0 && definitions.Count > top && definitions[top] != null)
        {
          string tokenValue = (string) null;
          switch (queryOrder)
          {
            case ReleaseDefinitionQueryOrder.IdAscending:
            case ReleaseDefinitionQueryOrder.IdDescending:
              tokenValue = definitions[top].Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              break;
            case ReleaseDefinitionQueryOrder.NameAscending:
            case ReleaseDefinitionQueryOrder.NameDescending:
              tokenValue = definitions[top].Name;
              break;
          }
          ReleaseManagementProjectControllerBase.SetContinuationToken(responseMessage, tokenValue);
        }
        return responseMessage;
      }
      catch (Exception ex)
      {
        responseMessage?.Dispose();
        this.TfsRequestContext.TraceException(1961208, "ReleaseManagementService", "Service", ex);
        throw;
      }
    }

    private static void ValidateContinutationToken(
      string continuationToken,
      ReleaseDefinitionQueryOrder queryOrder)
    {
      if (!string.IsNullOrWhiteSpace(continuationToken) && (queryOrder == ReleaseDefinitionQueryOrder.IdAscending || queryOrder == ReleaseDefinitionQueryOrder.IdDescending) && !int.TryParse(continuationToken, out int _))
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ContinuationTokenInvalidForIntQueryOrder);
    }

    private void LoadYaml(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition existingDefinition)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess pipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess) serverDefinition.PipelineProcess;
      YamlLoadResult yamlLoadResult = YamlHelper.Load(requestContext, this.ProjectId, serverDefinition.Id, pipelineProcess.FileSource, pipelineProcess.FileName, (YamlPipelineProcessResource) null, true);
      if (yamlLoadResult.PipelineResources != null)
        pipelineProcess.Resources = yamlLoadResult.PipelineResources.ToReleaseResources();
      if (yamlLoadResult.Errors.Any<string>())
      {
        pipelineProcess.Errors.Clear();
        pipelineProcess.Errors.AddRange<string, IList<string>>((IEnumerable<string>) yamlLoadResult.Errors);
      }
      else
        YamlHelper.UpdateDefinitionFromYaml(requestContext, yamlLoadResult.PipelineTemplate, existingDefinition, serverDefinition);
    }

    private void FixIncomingDefinition(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      bool isUpdateReleaseDefinition = true)
    {
      this.IncomingToLatest(releaseDefinition, isUpdateReleaseDefinition);
      this.FixLatestMajorTaskVersions(releaseDefinition);
    }

    private void FixLatestMajorTaskVersions(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition.Environments == null)
        return;
      IDistributedTaskPoolService service = this.TfsRequestContext.GetService<IDistributedTaskPoolService>();
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        if (environment.DeployPhases != null)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) environment.DeployPhases)
          {
            if (deployPhase.WorkflowTasks != null)
            {
              foreach (WorkflowTask workflowTask in (IEnumerable<WorkflowTask>) deployPhase.WorkflowTasks)
              {
                if (workflowTask.Version == "*")
                {
                  TaskDefinition latestMajorVersion = service.GetLatestMajorVersion(this.TfsRequestContext, workflowTask.TaskId);
                  if (latestMajorVersion != null)
                    workflowTask.Version = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.*", (object) latestMajorVersion.Version.Major);
                }
              }
            }
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Logging message")]
    private List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> GetAllReleaseDefinitions()
    {
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition> serverDefinitions;
      try
      {
        serverDefinitions = this.GetService<ReleaseDefinitionsService>().GetAllReleaseDefinitions(this.TfsRequestContext, this.ProjectId);
      }
      catch (DataspaceNotFoundException ex)
      {
        this.TfsRequestContext.Trace(1961208, TraceLevel.Warning, "ReleaseManagementService", "Service", "Dataspace not found exception for Get Definitions call");
        serverDefinitions = (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>();
      }
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> list = serverDefinitions.ToContract(this.TfsRequestContext, this.ProjectId).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>();
      list.ConvertToShallowContract(ReleaseDefinitionExpands.None);
      return list;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Logging message")]
    private List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> GetReleaseDefinitionsList(
      string searchText,
      string artifactType,
      string artifactSourceIds,
      ReleaseDefinitionExpands expands,
      string continuationToken,
      int top,
      ReleaseDefinitionQueryOrder queryOrder,
      string path,
      bool isExactNameMatch,
      string tagFilter,
      string propertyFilters,
      string definitionIdFilter,
      bool isDeleted,
      bool searchTextContainsFolderName)
    {
      IList<string> stringList1 = ServerModelUtility.ToStringList(artifactSourceIds);
      IList<string> stringList2 = ServerModelUtility.ToStringList(tagFilter);
      IList<string> stringList3 = ServerModelUtility.ToStringList(propertyFilters);
      IEnumerable<int> intList = ServerModelUtility.ToIntList(definitionIdFilter);
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition> serverDefinitions;
      try
      {
        serverDefinitions = this.GetService<ReleaseDefinitionsService>().GetReleaseDefinitions(this.TfsRequestContext, this.ProjectId, searchText, artifactType, (IEnumerable<string>) stringList1, isDeleted, expands, new DateTime?(), queryOrder.ToDataModelQueryOrder(), continuationToken, top, path, isExactNameMatch, (IEnumerable<string>) stringList2, (IEnumerable<string>) stringList3, intList, searchTextContainsFolderName);
      }
      catch (DataspaceNotFoundException ex)
      {
        this.TfsRequestContext.Trace(1961208, TraceLevel.Warning, "ReleaseManagementService", "Service", "Dataspace not found exception for Get Definitions call");
        serverDefinitions = (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>();
      }
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> list = serverDefinitions.ToContract(this.TfsRequestContext, this.ProjectId).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>();
      list.ConvertToShallowContract(expands);
      return list;
    }

    private void HandleRetentionBackCompat(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition, Guid projectId)
    {
      if (releaseDefinition.Environments == null || releaseDefinition.RetentionPolicy == null)
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy environmentRetentionPolicy = this.TfsRequestContext.GetService<ReleaseSettingsService>().GetReleaseSettings(this.TfsRequestContext, projectId).RetentionSettings.DefaultEnvironmentRetentionPolicy;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        if (environment.RetentionPolicy == null)
          environment.RetentionPolicy = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy()
          {
            DaysToKeep = releaseDefinition.RetentionPolicy.DaysToKeep,
            ReleasesToKeep = environmentRetentionPolicy.ReleasesToKeep,
            RetainBuild = environmentRetentionPolicy.RetainBuild
          };
      }
    }
  }
}
