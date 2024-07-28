// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IProjectConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.Tvps;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DefaultServiceImplementation(typeof (ProjectConfigurationService))]
  public interface IProjectConfigurationService : IVssFrameworkService
  {
    void SetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      ProjectProcessConfiguration settings);

    void SetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      ProjectProcessConfiguration settings,
      bool skipValidation);

    ProjectProcessConfiguration GetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      bool validateSettings);

    void DeleteProjectSettings(IVssRequestContext requestContext, string projectUri);

    IEnumerable<ProjectCategoryStateMap> GetCategoryStates(
      IVssRequestContext requestContext,
      WorkItemTypeEnum categoryType);

    void OnMigrateProjectsProcess(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> migratedProjects);

    IReadOnlyCollection<ProjectGuidWatermarkPair> GetChangedStateProjects(
      IVssRequestContext requestContext,
      int watermark);
  }
}
