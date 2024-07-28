// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseWorkItemsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseWorkItemsService : ReleaseCommitsWorkItemsServiceBase
  {
    private const int MaxWorkItemsToLink = 250;

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseWorkItemRef> GetReleaseWorkItemRefs(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      int top)
    {
      return this.GetReleaseWorkItemRefsForArtifactType(requestContext, projectInfo, startReleaseId, endReleaseId, (string) null, top);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseWorkItemRef> GetReleaseWorkItemRefs(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      int top,
      string artifactAlias)
    {
      return this.GetReleaseWorkItemRefsForArtifactType(requestContext, projectInfo, startReleaseId, endReleaseId, (string) null, top, artifactAlias);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual void LinkReleaseWorkItemRefs(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      IList<ArtifactSource> linkedArtifacts,
      int previousReleaseId,
      int currentReleaseId,
      LinkConfig linkConfig,
      DeploymentData deploymentData)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (linkedArtifacts == null)
        throw new ArgumentNullException(nameof (linkedArtifacts));
      if (linkConfig == null)
        throw new ArgumentNullException(nameof (linkConfig));
      if (deploymentData == null)
        throw new ArgumentNullException(nameof (deploymentData));
      linkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.SupportsWorkItemLinking)).ForEach<ArtifactSource>((Action<ArtifactSource>) (artifact =>
      {
        try
        {
          this.LinkArtifactWorkItems(requestContext, projectInfo, previousReleaseId, currentReleaseId, 250, artifact.ArtifactTypeId, artifact.Alias, linkConfig, deploymentData);
        }
        catch (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException ex)
        {
          requestContext.Trace(1976423, TraceLevel.Warning, "ReleaseManagementService", "Pipeline", "AutoLinkWorkItems: Failed to link work items for project: {0}, startReleaseId: {1}, endReleaseId: {2}. Exception: {3}", (object) projectInfo.Name, (object) previousReleaseId, (object) currentReleaseId, (object) ex);
        }
      }));
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "By design.")]
    private static GetConfig GetWorkItemsConfig(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      int key = requestContext.GetService<ReleasesService>().GetReleaseDefinitionFolderPathAndId(requestContext, projectId, releaseId).Key;
      IEnumerable<PropertyValue> properties = requestContext.GetService<ReleaseDefinitionsService>().GetProperties(requestContext, projectId, key);
      PropertyValue propertyValue1 = properties.Where<PropertyValue>((Func<PropertyValue, bool>) (x => x.PropertyName.Equals("IntegrateJiraWorkItems"))).FirstOrDefault<PropertyValue>();
      bool result = false;
      if (propertyValue1 != null)
        bool.TryParse((string) propertyValue1.Value, out result);
      PropertyValue propertyValue2 = properties.Where<PropertyValue>((Func<PropertyValue, bool>) (x => x.PropertyName.Equals("JiraServiceEndpointId"))).FirstOrDefault<PropertyValue>();
      string str = propertyValue2 != null ? (string) propertyValue2.Value : string.Empty;
      return new GetConfig()
      {
        IntegrateJiraWorkItems = result,
        JiraEndpointId = str
      };
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    private IEnumerable<ReleaseWorkItemRef> GetReleaseWorkItemRefsForArtifactType(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      string artifactTypeId,
      int top,
      string artifactAlias = null)
    {
      IEnumerable<ReleaseWorkItemRef> source = (IEnumerable<ReleaseWorkItemRef>) new List<ReleaseWorkItemRef>();
      GetConfig workItemsConfig = ReleaseWorkItemsService.GetWorkItemsConfig(requestContext, projectInfo.Id, endReleaseId);
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        source = this.GetArtifactItems<ReleaseWorkItemRef>(requestContext, projectInfo, startReleaseId, endReleaseId, top, artifactTypeId, ReleaseWorkItemsService.\u003C\u003EO.\u003C0\u003E__GetWorkItems ?? (ReleaseWorkItemsService.\u003C\u003EO.\u003C0\u003E__GetWorkItems = new Func<PipelineArtifactSource, IVssRequestContext, SortedList<int, PipelineArtifactSource>, ProjectInfo, int, int, GetConfig, IEnumerable<ReleaseWorkItemRef>>(ArtifactSourceExtensions.GetWorkItems)), artifactAlias, workItemsConfig);
      }
      catch (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException ex)
      {
        requestContext.Trace(1976423, TraceLevel.Warning, "ReleaseManagementService", "Pipeline", "AutoLinkWorkItems: Failed to get work items for project: {0}, startReleaseId: {1}, endReleaseId: {2}. Exception: {3}", (object) projectInfo.Name, (object) startReleaseId, (object) endReleaseId, (object) ex);
      }
      return source.Distinct<ReleaseWorkItemRef>((IEqualityComparer<ReleaseWorkItemRef>) new Microsoft.VisualStudio.Services.ReleaseManagement.Server.Comparer.ReleaseWorkItemComparer());
    }
  }
}
