// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IWorkItemMetadataFacadeService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DefaultServiceImplementation(typeof (WorkItemMetadataFacadeService))]
  public interface IWorkItemMetadataFacadeService : IVssFrameworkService
  {
    IReadOnlyCollection<string> GetWorkItemStates(
      IVssRequestContext requestContext,
      StateGroup stateGroup,
      bool includeUnmappedStatesWithDone = true);

    IReadOnlyCollection<string> GetWorkItemStates(
      IVssRequestContext requestContext,
      Guid projectId,
      StateGroup stateGroup,
      bool includeUnmappedStatesWithDone = true);

    IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColors(
      IVssRequestContext requestContext,
      Guid projectId);

    IReadOnlyCollection<WorkItemStateColor> GetWorkItemStateColors(
      IVssRequestContext requestContext,
      Guid projectId,
      string witName);

    IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>> GetWorkItemStateColorsByProjectName(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames);

    bool TryGetWorkItemStateColorsFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      out IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> workItemTypeStateColorMap);

    void OnProjectSettingsChanged(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> migratedProjects);

    string GetWorkItemStateColorETag(IVssRequestContext requestContext, Guid projectId);

    IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemColor>> GetWorkItemTypeColorsByProjectNames(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames);

    IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>> GetWorkItemTypeColorsByProjectIds(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> projectIds);

    IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemTypeColorAndIcon>> GetWorkItemTypeColorAndIconsByProjectNames(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames);

    IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>> GetWorkItemTypeColorAndIconsByProjectIds(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> projectIds);

    IReadOnlyCollection<WorkItemTypeColorAndIcon> GetWorkItemTypeColorAndIconsByProjectId(
      IVssRequestContext requestContext,
      Guid projectId);
  }
}
