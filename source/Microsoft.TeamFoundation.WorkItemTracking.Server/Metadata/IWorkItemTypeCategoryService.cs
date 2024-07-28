// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IWorkItemTypeCategoryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (WorkItemTypeCategoryService))]
  public interface IWorkItemTypeCategoryService : IVssFrameworkService
  {
    IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId);

    IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      string projectName);

    WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      Guid projectId,
      string categoryNameOrRefName);

    WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      string projectName,
      string categoryNameOrRefName);

    bool TryGetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      Guid projectId,
      string categoryNameOrRefName,
      out WorkItemTypeCategory workItemTypeCategory);

    bool TryGetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      string projectName,
      string categoryNameOrRefName,
      out WorkItemTypeCategory workItemTypeCategory);

    IEnumerable<WorkItemTypeCategory> LegacyGetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeOobCategoriesNotInDb);

    IReadOnlyCollection<ProjectGuidChangedRecord> GetProjectsForChangedWorkItemTypeCategories(
      IVssRequestContext requestContext,
      long sinceWatermark);
  }
}
