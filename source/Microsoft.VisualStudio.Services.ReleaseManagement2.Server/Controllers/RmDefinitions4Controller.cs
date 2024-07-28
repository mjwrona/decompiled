// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitions4Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "definitions", ResourceVersion = 3)]
  public class RmDefinitions4Controller : RmDefinitions3Controller
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition IncomingToLatest(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition incomingDefinition,
      bool isUpdateReleaseDefinition = true)
    {
      if (incomingDefinition == null)
        return incomingDefinition;
      if (incomingDefinition.Environments != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition) null;
        if (isUpdateReleaseDefinition && this.IsDev16PreviewVersion())
          releaseDefinition = this.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(this.TfsRequestContext, this.ProjectId, incomingDefinition.Id);
        if (releaseDefinition != null)
        {
          foreach (ReleaseDefinitionEnvironment environment1 in (IEnumerable<ReleaseDefinitionEnvironment>) incomingDefinition.Environments)
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
      this.ValidateObsoleteDeployStep(incomingDefinition);
      incomingDefinition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DesignerPipelineProcess();
      return incomingDefinition;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true)]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET__GetReleaseDefinition.json", "Get a release definition", null, null)]
    public override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition GetReleaseDefinition(
      int definitionId,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition = base.GetReleaseDefinition(definitionId, propertyFilters);
      this.TfsRequestContext.SetSecuredObject<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>(releaseDefinition);
      return releaseDefinition;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>), null, null)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None, true)]
    [ClientExample("GET__ListAllReleaseDefinitions.json", null, null, null)]
    [ClientExample("GET__ListAllReleaseDefinitionsWithEnvironmentsExpanded.json", "With environments details expanded", null, null)]
    [ClientExample("GET_ListAllReleaseDefinitionsWithArtifactsExpanded.json", "With artifacts details expanded", null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetReleaseDefinitions(
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
      return this.CreateReleaseDefinitionsResponse(ReleaseManagementSecurityProcessor.SecureComponents<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>(this.TfsRequestContext, this.GetReleaseDefinitionsList(searchText, expands, artifactType, artifactSourceId, top, continuationToken, queryOrder, path, isExactNameMatch, tagFilter, propertyFilters, definitionIdFilter, isDeleted, searchTextContainsFolderName), (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition, ReleaseManagementSecurityInfo>) (definition => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, definition.Path, definition.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition))), top, queryOrder);
    }

    [HttpPatch]
    [ClientInternalUseOnly(false)]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.DeleteReleaseDefinition)]
    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition UndeleteReleaseDefinition(
      int definitionId,
      [FromBody] ReleaseDefinitionUndeleteParameter releaseDefinitionUndeleteParameter)
    {
      if (releaseDefinitionUndeleteParameter == null)
        throw new InvalidRequestException(Resources.ReleaseDefinitionUndeleteMetadataCannotBeNull);
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmDefinitionsController.UndeleteReleaseDefinition", 1961228, 5, true))
        return this.GetService<ReleaseDefinitionsService>().UndeleteReleaseDefinition(this.TfsRequestContext, this.ProjectId, definitionId, releaseDefinitionUndeleteParameter.Comment).ToContract(this.TfsRequestContext, this.ProjectId);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition) null;
      definition.ToUndefinedDeployStepTasks();
      definition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) null;
      return definition;
    }

    private bool IsDev16PreviewVersion()
    {
      Version apiVersion = this.Request.GetApiVersion();
      return apiVersion != (Version) null && apiVersion.Major == 4 && apiVersion.Minor == 0;
    }
  }
}
