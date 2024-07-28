// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitions3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "definitions", ResourceVersion = 2)]
  public class BuildDefinitions3Controller : BuildApiController
  {
    private const int DefaultTop = 10000;

    [HttpPost]
    public BuildDefinition3_2 CreateDefinition(
      BuildDefinition3_2 definition,
      [FromUri] int? definitionToCloneId = null,
      [FromUri] int? definitionToCloneRevision = null)
    {
      this.CheckRequestContent((object) definition);
      this.ValidateDefinition(definition);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition serverBuildDefinition = definition.ToBuildServerBuildDefinition(this.TfsRequestContext);
      this.FixIncomingDefinition(serverBuildDefinition, false);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition secretSource = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      if (definitionToCloneId.HasValue)
      {
        secretSource = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionToCloneId.Value, definitionToCloneRevision);
        if (secretSource == null)
          throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionToCloneId.Value));
      }
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition = this.DefinitionService.AddDefinition(this.TfsRequestContext, serverBuildDefinition, secretSource);
      this.FixOutgoingDefinition(buildDefinition);
      return buildDefinition.ToBuildDefinition3_2(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    [HttpGet]
    public BuildDefinition3_2 GetDefinition(
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      bool includeLatestBuilds = false)
    {
      IEnumerable<string> propertyFilters1 = ArtifactPropertyKinds.AsPropertyFilters(propertyFilters);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId, revision, propertyFilters1, minMetricsTime: minMetricsTime, includeLatestBuilds: includeLatestBuilds);
      if (definition == null)
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId));
      this.FixOutgoingDefinition(definition);
      return definition.ToBuildDefinition3_2(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<BuildDefinitionReference>), null, null)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.Protected)]
    public HttpResponseMessage GetDefinitions(
      string name = "*",
      string repositoryId = null,
      string repositoryType = null,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.None,
      [FromUri(Name = "$top")] int top = 10000,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool includeAllProperties = false,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null)
    {
      if (builtAfter.HasValue && notBuiltAfter.HasValue)
        throw new InvalidDefinitionQueryException(Resources.AmbigiousBuiltAfterFilter());
      bool flag = false;
      IList<int> source1 = RestHelpers.ToInt32List(definitionIds) ?? (IList<int>) new List<int>();
      if (source1 != null && source1.Any<int>())
      {
        if (!string.IsNullOrEmpty(continuationToken))
          throw new InvalidDefinitionQueryException(Resources.ContinuationTokenNotSupportedForDefinitionIds());
        if (!string.IsNullOrEmpty(repositoryId) || !string.IsNullOrEmpty(repositoryType))
          throw new InvalidDefinitionQueryException(Resources.RepositoryInfoNotSupportedForDefinitionIds());
        if (builtAfter.HasValue || notBuiltAfter.HasValue)
          throw new InvalidDefinitionQueryException(Resources.BuiltAfterFilterNotSupportedForDefinitionIds());
        flag = true;
      }
      DateTime? nullable1 = new DateTime?();
      DateTime? nullable2 = new DateTime?();
      string str = (string) null;
      if (!string.IsNullOrEmpty(continuationToken))
      {
        if (queryOrder == Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.None)
          throw new InvalidDefinitionQueryException(Resources.DefinitionContinuationTokenNoQueryOrder());
        DefinitionsContinuationToken token;
        if (!DefinitionsContinuationToken.TryParse(continuationToken, queryOrder, out token))
          throw new InvalidContinuationTokenException(Resources.InvalidContinuationToken());
        if (token.LastModifiedTime.HasValue)
        {
          switch (queryOrder)
          {
            case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedAscending:
              nullable1 = token.LastModifiedTime;
              break;
            case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedDescending:
              nullable2 = token.LastModifiedTime;
              break;
            case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.DefinitionNameAscending:
              str = token.Name;
              break;
          }
        }
        else
          str = token.Name;
      }
      if (!string.IsNullOrEmpty(repositoryId) && string.IsNullOrEmpty(repositoryType))
        throw new InvalidDefinitionQueryException(Resources.RepositoryTypeMissing());
      if (!string.IsNullOrEmpty(repositoryType))
        repositoryId = (this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType, false) ?? throw new InvalidDefinitionQueryException(Resources.BuildRepositoryTypeNotSupported((object) repositoryType))).NormalizeRepositoryId(this.TfsRequestContext, repositoryId);
      DefinitionQueryOptions definitionQueryOptions = includeAllProperties ? DefinitionQueryOptions.All : DefinitionQueryOptions.None;
      List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> list;
      if (flag)
      {
        IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> source2 = (IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>) this.DefinitionService.GetDefinitionsByIds(this.TfsRequestContext, this.ProjectId, (IEnumerable<int>) source1.ToList<int>(), minMetricsTime: minMetricsTime, includeLatestBuilds: includeLatestBuilds);
        switch (queryOrder)
        {
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedAscending:
            source2 = (IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>) source2.OrderBy<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DateTime>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DateTime>) (d => d.CreatedDate));
            break;
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedDescending:
            source2 = (IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>) source2.OrderByDescending<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DateTime>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DateTime>) (d => d.CreatedDate));
            break;
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.DefinitionNameAscending:
            source2 = (IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>) source2.OrderBy<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, string>) (d => d.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase);
            break;
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.DefinitionNameDescending:
            source2 = (IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>) source2.OrderByDescending<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, string>) (d => d.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase);
            break;
        }
        list = source2.ToList<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      }
      else if (!string.IsNullOrEmpty(repositoryType))
      {
        IBuildDefinitionService definitionService = this.DefinitionService;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        string repositoryType1 = repositoryType;
        string repositoryId1 = repositoryId;
        string name1 = name;
        int num1 = top + 1;
        int queryOrder1 = (int) queryOrder;
        int count = num1;
        DateTime? minLastModifiedTime = nullable1;
        DateTime? maxLastModifiedTime = nullable2;
        string lastDefinitionName = str;
        DateTime? minMetricsTime1 = minMetricsTime;
        string path1 = path;
        DateTime? builtAfter1 = builtAfter;
        DateTime? notBuiltAfter1 = notBuiltAfter;
        int options = (int) definitionQueryOptions;
        int num2 = includeLatestBuilds ? 1 : 0;
        Guid? taskIdFilter1 = taskIdFilter;
        int? processType = new int?();
        list = definitionService.GetDefinitionsForRepository(tfsRequestContext, projectId, repositoryType1, repositoryId1, name1, queryOrder: (Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder) queryOrder1, count: count, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, lastDefinitionName: lastDefinitionName, minMetricsTime: minMetricsTime1, path: path1, builtAfter: builtAfter1, notBuiltAfter: notBuiltAfter1, options: (DefinitionQueryOptions) options, includeLatestBuilds: num2 != 0, taskIdFilter: taskIdFilter1, processType: processType).ToList<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      }
      else
      {
        IBuildDefinitionService definitionService = this.DefinitionService;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        string name2 = name;
        int num3 = top + 1;
        int queryOrder2 = (int) queryOrder;
        int count = num3;
        DateTime? minLastModifiedTime = nullable1;
        DateTime? maxLastModifiedTime = nullable2;
        string lastDefinitionName = str;
        DateTime? minMetricsTime2 = minMetricsTime;
        string path2 = path;
        DateTime? builtAfter2 = builtAfter;
        DateTime? notBuiltAfter2 = notBuiltAfter;
        int options = (int) definitionQueryOptions;
        int num4 = includeLatestBuilds ? 1 : 0;
        Guid? taskIdFilter2 = taskIdFilter;
        int? processType = new int?();
        list = definitionService.GetDefinitions(tfsRequestContext, projectId, name2, queryOrder: (Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder) queryOrder2, count: count, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, lastDefinitionName: lastDefinitionName, minMetricsTime: minMetricsTime2, path: path2, builtAfter: builtAfter2, notBuiltAfter: notBuiltAfter2, options: (DefinitionQueryOptions) options, includeLatestBuilds: num4 != 0, taskIdFilter: taskIdFilter2, processType: processType).ToList<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      }
      IdentityMap identityMap = new IdentityMap(this.TfsRequestContext.GetService<IdentityService>());
      HttpResponseMessage responseMessage = includeAllProperties ? this.Request.CreateResponse<List<BuildDefinition3_2>>(HttpStatusCode.OK, list.Take<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(top).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinition3_2>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinition3_2>) (d =>
      {
        this.FixOutgoingDefinition(d);
        return d.ToBuildDefinition3_2(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion, identityMap);
      })).ToList<BuildDefinition3_2>()) : this.Request.CreateResponse<List<BuildDefinitionReference3_2>>(HttpStatusCode.OK, list.Take<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(top).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionReference3_2>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionReference3_2>) (d =>
      {
        this.FixOutgoingDefinition(d);
        return d.ToBuildDefinitionReference3_2(this.TfsRequestContext, identityMap);
      })).ToList<BuildDefinitionReference3_2>());
      if (!flag && list.Count > top)
        this.SetContinuationToken(responseMessage, list[top], (Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder) queryOrder);
      return responseMessage;
    }

    [HttpPut]
    public BuildDefinition3_2 UpdateDefinition(
      int definitionId,
      [FromBody] BuildDefinition3_2 definition,
      [FromUri] int? secretsSourceDefinitionId = null,
      [FromUri] int? secretsSourceDefinitionRevision = null)
    {
      this.CheckRequestContent((object) definition);
      this.ValidateDefinition(definition);
      if (definition.Id != definitionId)
        throw new RouteIdConflictException(Resources.WrongIdSpecifiedForDefinition((object) definitionId, (object) definition.Id));
      ArgumentUtility.CheckForNull<int>(definition.Revision, "definition.Revision");
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition serverBuildDefinition = definition.ToBuildServerBuildDefinition(this.TfsRequestContext);
      this.FixIncomingDefinition(serverBuildDefinition, true);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition secretSource = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      if (secretsSourceDefinitionId.HasValue)
      {
        secretSource = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, secretsSourceDefinitionId.Value, secretsSourceDefinitionRevision);
        if (secretSource == null)
          throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) secretsSourceDefinitionId.Value));
      }
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition = this.DefinitionService.UpdateDefinition(this.TfsRequestContext, serverBuildDefinition, secretSource: secretSource);
      this.FixOutgoingDefinition(buildDefinition);
      return buildDefinition.ToBuildDefinition3_2(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    [HttpDelete]
    public void DeleteDefinition(int definitionId) => this.DefinitionService.DeleteDefinition(this.TfsRequestContext, this.ProjectId, definitionId);

    private void ValidateDefinition(BuildDefinition3_2 definition)
    {
      ArgumentUtility.CheckForNull<BuildDefinition3_2>(definition, nameof (definition));
      if (definition.Type != Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Build)
        throw new NotSupportedOnXamlBuildException(Resources.NotSupportedOnXamlBuildDefinition());
      if (definition.Project == null)
        definition.Project = new TeamProjectReference()
        {
          Id = this.ProjectId
        };
      else if (definition.Project.Id != this.ProjectId)
        throw new RouteIdConflictException(Resources.WrongProjectSpecifiedForDefinition((object) this.ProjectId, (object) definition.Project.Id));
    }

    private void SetContinuationToken(
      HttpResponseMessage responseMessage,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition nextDefinition,
      Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder queryOrder)
    {
      if (nextDefinition == null)
        return;
      string str = new DefinitionsContinuationToken(nextDefinition.Name, new DateTime?(nextDefinition.CreatedDate), queryOrder).ToString();
      if (string.IsNullOrEmpty(str))
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", str);
    }
  }
}
