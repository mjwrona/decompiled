// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitions2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "definitions", ResourceVersion = 2)]
  public sealed class BuildDefinitions2Controller : BuildApiController
  {
    private const int DefaultTop = 10000;

    [HttpPost]
    public BuildDefinition3_2 CreateDefinition(
      BuildDefinition3_2 definition,
      [FromUri] int? definitionToCloneId = null,
      [FromUri] int? definitionToCloneRevision = null)
    {
      this.CheckRequestContent((object) definition);
      this.CheckProject(definition);
      if (definition.Queue != null)
        definition.Queue.ResolveToProject(this.TfsRequestContext, this.ProjectId);
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
    public DefinitionReference GetDefinition(
      int definitionId,
      int? revision = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      if (((int) type ?? 2) == 2)
      {
        IEnumerable<string> propertyFilters1 = ArtifactPropertyKinds.AsPropertyFilters(propertyFilters);
        Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId, revision, propertyFilters1);
        if (definition != null)
        {
          this.FixOutgoingDefinition(definition);
          return (DefinitionReference) definition.ToBuildDefinition3_2(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
        }
      }
      if (((int) type ?? 1) == 1)
      {
        XamlBuildDefinition definition = this.TfsRequestContext.GetService<IXamlDefinitionProvider>().GetDefinition(this.TfsRequestContext, this.ProjectInfo, definitionId);
        if (definition != null)
        {
          definition.AddLinks(this.TfsRequestContext);
          return (DefinitionReference) definition;
        }
      }
      throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId));
    }

    [HttpGet]
    public List<DefinitionReference> GetDefinitions(
      string name = "*",
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null,
      string repositoryId = null,
      string repositoryType = null,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.None,
      [FromUri(Name = "$top")] int top = 10000)
    {
      if (!string.IsNullOrEmpty(repositoryId) && string.IsNullOrEmpty(repositoryType))
        throw new InvalidDefinitionQueryException(Resources.RepositoryTypeMissing());
      if (!string.IsNullOrEmpty(repositoryType))
        repositoryId = (this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType, false) ?? throw new InvalidDefinitionQueryException(Resources.BuildRepositoryTypeNotSupported((object) repositoryType))).NormalizeRepositoryId(this.TfsRequestContext, repositoryId);
      bool flag1 = !string.IsNullOrEmpty(repositoryType);
      List<DefinitionReference> source = new List<DefinitionReference>();
      if (((int) type ?? 2) == 2)
      {
        IdentityMap identityMap = new IdentityMap(this.TfsRequestContext.GetService<IdentityService>());
        if (flag1)
        {
          List<DefinitionReference> definitionReferenceList = source;
          IBuildDefinitionService definitionService = this.DefinitionService;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          Guid projectId = this.ProjectId;
          string repositoryType1 = repositoryType;
          string repositoryId1 = repositoryId;
          string name1 = name;
          int num = top + 1;
          int queryOrder1 = (int) queryOrder;
          int count = num;
          DateTime? minLastModifiedTime = new DateTime?();
          DateTime? maxLastModifiedTime = new DateTime?();
          DateTime? minMetricsTime = new DateTime?();
          DateTime? builtAfter = new DateTime?();
          DateTime? notBuiltAfter = new DateTime?();
          Guid? taskIdFilter = new Guid?();
          int? processType = new int?();
          IEnumerable<BuildDefinitionReference3_2> collection = definitionService.GetDefinitionsForRepository(tfsRequestContext, projectId, repositoryType1, repositoryId1, name1, queryOrder: (Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder) queryOrder1, count: count, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, minMetricsTime: minMetricsTime, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, options: DefinitionQueryOptions.None, taskIdFilter: taskIdFilter, processType: processType).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionReference3_2>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionReference3_2>) (d => d.ToBuildDefinitionReference3_2(this.TfsRequestContext, identityMap)));
          definitionReferenceList.AddRange((IEnumerable<DefinitionReference>) collection);
        }
        else
        {
          List<DefinitionReference> definitionReferenceList = source;
          IBuildDefinitionService definitionService = this.DefinitionService;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          Guid projectId = this.ProjectId;
          string name2 = name;
          int num = top + 1;
          int queryOrder2 = (int) queryOrder;
          int count = num;
          DateTime? minLastModifiedTime = new DateTime?();
          DateTime? maxLastModifiedTime = new DateTime?();
          DateTime? minMetricsTime = new DateTime?();
          DateTime? builtAfter = new DateTime?();
          DateTime? notBuiltAfter = new DateTime?();
          Guid? taskIdFilter = new Guid?();
          int? processType = new int?();
          IEnumerable<BuildDefinitionReference3_2> collection = definitionService.GetDefinitions(tfsRequestContext, projectId, name2, queryOrder: (Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder) queryOrder2, count: count, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, minMetricsTime: minMetricsTime, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, options: DefinitionQueryOptions.None, taskIdFilter: taskIdFilter, processType: processType).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionReference3_2>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionReference3_2>) (d => d.ToBuildDefinitionReference3_2(this.TfsRequestContext, identityMap)));
          definitionReferenceList.AddRange((IEnumerable<DefinitionReference>) collection);
        }
        if (this.Request.Headers.UserAgent.Count > 1)
        {
          ProductInfoHeaderValue productInfoHeaderValue = this.Request.Headers.UserAgent.ElementAt<ProductInfoHeaderValue>(1);
          if (!string.IsNullOrEmpty(productInfoHeaderValue.Comment) && productInfoHeaderValue.Comment.IndexOf("devenv.exe", StringComparison.InvariantCultureIgnoreCase) >= 0)
          {
            foreach (DefinitionReference definitionReference in source)
            {
              string str1 = definitionReference.Name;
              if (!string.Equals(definitionReference.Path, "\\"))
              {
                string str2 = definitionReference.Path;
                if (str2[0] == '\\')
                  str2 = str2.Substring(1);
                str1 = str2 + "\\" + str1;
              }
              definitionReference.Name = str1;
            }
          }
        }
      }
      bool flag2 = false;
      if (!flag1 && ((int) type ?? 1) == 1)
      {
        IXamlDefinitionProvider service = this.TfsRequestContext.GetService<IXamlDefinitionProvider>();
        source.AddRange(service.GetDefinitions(this.TfsRequestContext, this.ProjectInfo, name).Select<XamlBuildDefinition, DefinitionReference>((Func<XamlBuildDefinition, DefinitionReference>) (d => d.ToReference())));
        flag2 = true;
      }
      if (flag2)
      {
        switch (queryOrder)
        {
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedAscending:
            source.Sort((Comparison<DefinitionReference>) ((d1, d2) => DateTime.Compare(d1.CreatedDate, d2.CreatedDate)));
            break;
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedDescending:
            source.Sort((Comparison<DefinitionReference>) ((d1, d2) => DateTime.Compare(d1.CreatedDate, d2.CreatedDate)));
            break;
        }
      }
      return source.Take<DefinitionReference>(top).ToList<DefinitionReference>();
    }

    [HttpPut]
    public BuildDefinition3_2 UpdateDefinition(
      int definitionId,
      [FromBody] BuildDefinition3_2 definition,
      [FromUri] int? secretsSourceDefinitionId = null,
      [FromUri] int? secretsSourceDefinitionRevision = null)
    {
      this.CheckRequestContent((object) definition);
      this.CheckProject(definition);
      if (definition.Id != definitionId)
        throw new RouteIdConflictException(Resources.WrongIdSpecifiedForDefinition((object) definitionId, (object) definition.Id));
      if (definition.Queue != null)
        definition.Queue.ResolveToProject(this.TfsRequestContext, this.ProjectId);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition serverBuildDefinition = definition.ToBuildServerBuildDefinition(this.TfsRequestContext);
      this.FixIncomingDefinition(serverBuildDefinition, true);
      ArgumentUtility.CheckForNull<int>(serverBuildDefinition.Revision, "definition.Revision");
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
    public void DeleteDefinition(int definitionId)
    {
      try
      {
        this.DefinitionService.DeleteDefinition(this.TfsRequestContext, this.ProjectId, definitionId);
      }
      catch (DefinitionNotFoundException ex)
      {
        IXamlDefinitionProvider service = this.TfsRequestContext.GetService<IXamlDefinitionProvider>();
        XamlBuildDefinition definition = service.GetDefinition(this.TfsRequestContext, this.ProjectInfo, definitionId);
        if (definition != null)
          service.DeleteDefinition(this.TfsRequestContext, this.ProjectInfo, definition);
        else
          throw;
      }
    }

    private void CheckProject(BuildDefinition3_2 definition)
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
  }
}
