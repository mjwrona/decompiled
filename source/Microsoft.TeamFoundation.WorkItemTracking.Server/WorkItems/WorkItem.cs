// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItem : WorkItemRevision
  {
    private Lazy<IReadOnlyList<WorkItemRevision>> m_lazyRevisions;

    internal WorkItem()
      : this((IDictionary<int, object>) null)
    {
    }

    internal WorkItem(IDictionary<int, object> latestData)
      : base(WorkItemDataset.CreateFrom(latestData))
    {
      this.m_lazyRevisions = new Lazy<IReadOnlyList<WorkItemRevision>>((Func<IReadOnlyList<WorkItemRevision>>) (() => (IReadOnlyList<WorkItemRevision>) new List<WorkItemRevision>().AsReadOnly()));
    }

    internal WorkItem(WorkItemDataset dataset)
      : base((WorkItemRevisionDataset) dataset)
    {
      this.m_lazyRevisions = new Lazy<IReadOnlyList<WorkItemRevision>>((Func<IReadOnlyList<WorkItemRevision>>) (() => (IReadOnlyList<WorkItemRevision>) dataset.Revisions.Select<WorkItemRevisionDataset, WorkItemRevision>((Func<WorkItemRevisionDataset, WorkItemRevision>) (rds => new WorkItemRevision(rds))).ToList<WorkItemRevision>().AsReadOnly()));
    }

    public IReadOnlyList<WorkItemRevision> Revisions => this.m_lazyRevisions.Value;

    public void SecureWorkItemWithRevisions(string token, int permissionsToCheck)
    {
      this.SetSecuredToken(token);
      this.SetRequiredPermissions(permissionsToCheck);
      foreach (WorkItemRevision workItemRevision in (IEnumerable<WorkItemRevision>) this.m_lazyRevisions.Value)
      {
        workItemRevision.SetSecuredToken(token);
        workItemRevision.SetRequiredPermissions(permissionsToCheck);
      }
    }
  }
}
