// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IWorkItemHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IWorkItemHelper
  {
    List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetWorkItemReference(
      IVssRequestContext context,
      List<int> workItemIds,
      bool skipPermissionCheck);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetWorkItemReference(
      IVssRequestContext context,
      Guid projectId,
      List<int> workItemIds,
      bool skipPermissionCheck);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext context,
      List<int> workItemIds,
      bool includeResourceLinks = false,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext context,
      Guid projectId,
      List<int> workItemIds,
      bool includeResourceLinks = false,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail);

    bool BelongsToRequirementCategory(
      IVssRequestContext context,
      GuidAndString projectId,
      int workItemId,
      bool skipPermissionCheck);

    bool BelongsToCategory(
      IVssRequestContext context,
      GuidAndString projectId,
      string categoryName,
      string workItemTypeName);
  }
}
