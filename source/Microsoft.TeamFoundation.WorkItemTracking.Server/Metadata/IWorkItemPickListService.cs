// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IWorkItemPickListService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (WorkItemPickListService))]
  public interface IWorkItemPickListService : IVssFrameworkService
  {
    WorkItemPickList GetList(IVssRequestContext requestContext, Guid listId, bool bypassCache = false);

    bool TryGetList(IVssRequestContext requestContext, Guid listId, out WorkItemPickList picklist);

    IReadOnlyCollection<WorkItemPickListMetadata> GetListsMetadata(IVssRequestContext requestContext);

    WorkItemPickList CreateList(
      IVssRequestContext requestContext,
      string listName,
      WorkItemPickListType type,
      IReadOnlyList<string> items,
      bool isSuggested = false);

    WorkItemPickList UpdateList(
      IVssRequestContext requestContext,
      Guid listId,
      string listName,
      IReadOnlyList<string> items,
      bool isSuggested = false);

    void DeleteList(IVssRequestContext requestContext, Guid listId);
  }
}
