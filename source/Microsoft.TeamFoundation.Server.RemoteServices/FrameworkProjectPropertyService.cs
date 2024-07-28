// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RemoteServices.FrameworkProjectPropertyService
// Assembly: Microsoft.TeamFoundation.Server.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 836F660A-A756-49A7-82F0-68378533B43C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.RemoteServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Server.RemoteServices
{
  internal class FrameworkProjectPropertyService : BaseProjectPropertyService
  {
    private const string c_area = "Project";
    private const string c_layer = "FrameworkProjectPropertyService";

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", FrameworkProjectNotification.ProjectUpdated, new SqlNotificationHandler(this.OnProjectPropertiesChanged), true);
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", FrameworkProjectNotification.ProjectUpdated, new SqlNotificationHandler(this.OnProjectPropertiesChanged), false);
      base.ServiceEnd(systemRequestContext);
    }

    protected override IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> ReadFromSource(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters)
    {
      ProjectHttpClient client = requestContext.GetClient<ProjectHttpClient>();
      HttpRequestHeaders defaultRequestHeaders = client.DefaultRequestHeaders;
      if (!defaultRequestHeaders.Contains("X-TFS-BypassProjectPropertyCache"))
        defaultRequestHeaders.Add("X-TFS-BypassProjectPropertyCache", bool.TrueString);
      return (IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>>) client.GetProjectsPropertiesAsync(projectIds, filters).SyncResult<List<ProjectProperties>>().ToDictionary<ProjectProperties, Guid, IEnumerable<ProjectProperty>>((Func<ProjectProperties, Guid>) (r => r.ProjectId), (Func<ProjectProperties, IEnumerable<ProjectProperty>>) (r => r.Properties));
    }

    protected override void WriteToSource(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> properties)
    {
      requestContext.GetClient<ProjectHttpClient>().SetProjectPropertiesAsync(projectId, ProjectProperty.CreateJsonPatchDocument(properties)).SyncResult();
    }

    private void OnProjectPropertiesChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      ProjectInfo project;
      if (FrameworkProjectUtilities.TryDeserialize(args.Data, out project))
        this.RemoveCache(requestContext, project.Id, this.GetCacheEntriesToRemove(requestContext, project));
      else
        requestContext.Trace(5500290, TraceLevel.Error, "Project", nameof (FrameworkProjectPropertyService), "Invalid notification data: '{0}'.", (object) args.Data);
    }

    private IEnumerable<string> GetCacheEntriesToRemove(
      IVssRequestContext requestContext,
      ProjectInfo project)
    {
      if (project.State == ProjectState.Deleted)
        return requestContext.GetService<ProjectPropertyCacheService>().GetCachedPropertyNames(project.Id);
      return project.Properties != null ? project.Properties.Select<ProjectProperty, string>((Func<ProjectProperty, string>) (p => p.Name)) : (IEnumerable<string>) Array.Empty<string>();
    }

    private class FrameworkProjectPropertyCacheSubscriber : FrameworkProjectEventSubscriberBase
    {
      public override string Name => nameof (FrameworkProjectPropertyCacheSubscriber);

      public override SubscriberPriority Priority => SubscriberPriority.High;

      protected override void ProcessEvent(
        IVssRequestContext requestContext,
        ProjectInfo projectInfo)
      {
        FrameworkProjectPropertyService service = requestContext.GetService<FrameworkProjectPropertyService>();
        service.RemoveCache(requestContext, projectInfo.Id, service.GetCacheEntriesToRemove(requestContext, projectInfo));
      }
    }
  }
}
