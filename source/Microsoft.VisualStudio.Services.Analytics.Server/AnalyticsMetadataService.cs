// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsMetadataService : IAnalyticsMetadataService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual MetadataModel GetModel(IVssRequestContext requestContext, string projectIdOrName = null)
    {
      requestContext.Trace(12011001, TraceLevel.Verbose, "AnalyticsModel", nameof (AnalyticsMetadataService), "GetContentType for projectIdOrName '" + projectIdOrName + "'");
      ProjectInfo projectInfo = this.GetProjectInfo(requestContext, projectIdOrName);
      return this.GetModel(requestContext, projectInfo);
    }

    public virtual MetadataModel GetModel(IVssRequestContext requestContext, ProjectInfo project = null)
    {
      requestContext.Trace(12011001, TraceLevel.Verbose, "AnalyticsModel", nameof (AnalyticsMetadataService), string.Format("GetContentType for project '{0}'", (object) project?.Id));
      Guid? processId = this.GetProcessId(requestContext, project);
      return this.GetModel(requestContext, processId);
    }

    private MetadataModel GetModel(IVssRequestContext requestContext, Guid? processId)
    {
      requestContext.Trace(12011001, TraceLevel.Verbose, "AnalyticsModel", nameof (AnalyticsMetadataService), string.Format("GetContentType for process id '{0}'", (object) processId));
      return requestContext.GetService<AnalyticsContextTypesCacheService>().GetModel(requestContext, processId);
    }

    protected virtual void SaveProcessId(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid processId)
    {
      requestContext.GetService<AnalyticsMetadataService.AnalyticsProcessIdsCacheService>().Set(requestContext, projectId, processId);
    }

    protected virtual Guid? GetCachedProcessId(IVssRequestContext requestContext, Guid projectId)
    {
      Guid guid;
      return requestContext.GetService<AnalyticsMetadataService.AnalyticsProcessIdsCacheService>().TryGetValue(requestContext, projectId, out guid) ? new Guid?(guid) : new Guid?();
    }

    private Guid? GetProcessId(IVssRequestContext requestContext, ProjectInfo projectInfo)
    {
      if (projectInfo == null)
        return new Guid?();
      Guid? nullable = this.GetCachedProcessId(requestContext, projectInfo.Id);
      if (!nullable.HasValue)
      {
        using (AnalyticsMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<AnalyticsMetadataComponent>("Analytics"))
        {
          nullable = new Guid?(replicaAwareComponent.GetProjectProcessId(projectInfo.Id));
          this.SaveProcessId(requestContext, projectInfo.Id, nullable.Value);
        }
      }
      return new Guid?(nullable.Value);
    }

    private ProjectInfo GetProjectInfo(IVssRequestContext requestContext, string projFilter)
    {
      if (string.IsNullOrWhiteSpace(projFilter))
        return (ProjectInfo) null;
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid result;
      return !Guid.TryParse(projFilter, out result) ? service.GetProject(requestContext, projFilter) : service.GetProject(requestContext, result);
    }

    public virtual ModelCreationProcessFields GetModelCreationProcessFields(
      IVssRequestContext requestContext,
      Guid? processId = null)
    {
      using (AnalyticsMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<AnalyticsMetadataComponent>("Analytics"))
        return replicaAwareComponent.GetModelCreationProcessFields(processId);
    }

    internal bool IsCustomFieldInModelMarkedAsDeleted(
      IVssRequestContext requestContext,
      string customFieldName,
      string modelName,
      ProjectInfo projectInfo = null)
    {
      ModelCreationProcessFields creationProcessFields = (ModelCreationProcessFields) null;
      using (AnalyticsMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<AnalyticsMetadataComponent>("Analytics"))
      {
        Guid? processId = this.GetProcessId(requestContext, projectInfo);
        creationProcessFields = replicaAwareComponent.GetModelCreationProcessFields(processId, true);
      }
      return creationProcessFields.CustomProcessFields.Any<ProcessFieldInfo>((Func<ProcessFieldInfo, bool>) (y => y.PropertyName == customFieldName));
    }

    public ICollection<Guid> GetModelProjectSKs(IVssRequestContext requestContext)
    {
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
        return (ICollection<Guid>) component.GetModelProjectGuids();
    }

    internal class AnalyticsProcessIdsCacheService : VssMemoryCacheService<Guid, Guid>
    {
      private static readonly TimeSpan _cacheCleanupInterval = TimeSpan.FromMinutes(15.0);

      public AnalyticsProcessIdsCacheService()
        : base(AnalyticsMetadataService.AnalyticsProcessIdsCacheService._cacheCleanupInterval)
      {
        this.ExpiryInterval.Value = AnalyticsMetadataService.AnalyticsProcessIdsCacheService._cacheCleanupInterval;
      }
    }

    public static class TraceConstants
    {
      public const string Area = "AnalyticsModel";
      public const string Layer = "AnalyticsMetadataService";
    }
  }
}
