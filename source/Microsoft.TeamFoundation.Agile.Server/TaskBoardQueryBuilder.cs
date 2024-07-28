// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoardQueryBuilder
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public class TaskBoardQueryBuilder : AgileBaseQueryBuilder
  {
    private IterationQueryBuilder m_iterationQueryBuilder;

    public TaskBoardQueryBuilder(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      IEnumerable<string> fields,
      string iterationPath,
      string backlogIterationPath)
      : base(settings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fields, nameof (fields));
      ArgumentUtility.CheckStringForNullOrEmpty(iterationPath, "requestIteration");
      ArgumentUtility.CheckStringForNullOrEmpty(backlogIterationPath, nameof (backlogIterationPath));
      List<BacklogLevelConfiguration> sourceConfigurations = new List<BacklogLevelConfiguration>()
      {
        this.m_settings.BacklogConfiguration.TaskBacklog,
        this.m_settings.BacklogConfiguration.RequirementBacklog
      };
      List<BacklogLevelConfiguration> targetConfigurations = new List<BacklogLevelConfiguration>()
      {
        this.m_settings.BacklogConfiguration.TaskBacklog,
        this.m_settings.BacklogConfiguration.RequirementBacklog
      };
      WorkItemStateCategory[] stateTypes = new WorkItemStateCategory[4]
      {
        WorkItemStateCategory.Proposed,
        WorkItemStateCategory.InProgress,
        WorkItemStateCategory.Completed,
        WorkItemStateCategory.Resolved
      };
      this.m_iterationQueryBuilder = new IterationQueryBuilder(requestContext, settings, (IEnumerable<BacklogLevelConfiguration>) sourceConfigurations, (IEnumerable<BacklogLevelConfiguration>) targetConfigurations, stateTypes, fields, iterationPath, backlogIterationPath);
    }

    public override string GetQuery() => this.m_iterationQueryBuilder.GetQuery();
  }
}
