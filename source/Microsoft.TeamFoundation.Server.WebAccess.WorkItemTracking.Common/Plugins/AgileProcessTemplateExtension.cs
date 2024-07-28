// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins.AgileProcessTemplateExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins
{
  internal class AgileProcessTemplateExtension : ProcessTemplateExtension
  {
    internal const char ProjectSettingsChangeUriDelimiter = ';';

    public override void OnMigrateProjectsProcess(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> migratedProjects)
    {
      requestContext.GetService<IProjectConfigurationService>().OnMigrateProjectsProcess(requestContext, migratedProjects);
      requestContext.GetService<IWorkItemMetadataFacadeService>().OnProjectSettingsChanged(requestContext, migratedProjects);
      requestContext.GetService<IWorkItemTypeService>().RemoveProjectWorkitemTypesFromCache(requestContext, migratedProjects);
      requestContext.GetService<WorkItemTypePropertiesService>().ClearCache();
      requestContext.GetService<WorkItemTrackingOutOfBoxRulesCache>().Clear(requestContext);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.ProjectsProcessMigrated, AgileProcessTemplateExtension.CreateProjectsProcessMigratedEventData(migratedProjects != null ? migratedProjects.Select<ProjectInfo, string>((Func<ProjectInfo, string>) (p => p.Uri)) : (IEnumerable<string>) null));
    }

    public static IEnumerable<string> GetProjectUrisFromProjectSettingsChanged(string eventData) => (IEnumerable<string>) (eventData ?? string.Empty).Split(new char[1]
    {
      ';'
    }, StringSplitOptions.RemoveEmptyEntries);

    public static string CreateProjectsProcessMigratedEventData(IEnumerable<string> projectUris) => projectUris == null || !projectUris.Any<string>() ? (string) null : string.Join(';'.ToString(), projectUris);
  }
}
