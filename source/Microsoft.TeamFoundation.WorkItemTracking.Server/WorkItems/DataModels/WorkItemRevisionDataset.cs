// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemRevisionDataset
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemRevisionDataset : WorkItemFieldValues, IRevisedWorkItemEntity
  {
    public WorkItemRevisionDataset()
    {
      this.ResourceLinks = new List<WorkItemResourceLinkInfo>();
      this.AllResourceLinks = new List<WorkItemResourceLinkInfo>();
      this.AllLinks = new List<WorkItemLinkInfo>();
      this.HiddenResourceLinks = new List<WorkItemResourceLinkInfo>();
      this.AllHiddenResourceLinks = new List<WorkItemResourceLinkInfo>();
    }

    public List<WorkItemResourceLinkInfo> ResourceLinks { get; private set; }

    public List<WorkItemLinkInfo> AllLinks { get; private set; }

    public List<WorkItemResourceLinkInfo> AllResourceLinks { get; private set; }

    public List<WorkItemResourceLinkInfo> HiddenResourceLinks { get; private set; }

    public List<WorkItemResourceLinkInfo> AllHiddenResourceLinks { get; private set; }

    public IEnumerable<WorkItemLinkInfo> WorkItemLinks => this.AllLinks.Where<WorkItemLinkInfo>((Func<WorkItemLinkInfo, bool>) (l => l.RevisedDate >= this.RevisedDate));

    int IRevisedWorkItemEntity.Id => this.Id;

    DateTime IRevisedWorkItemEntity.AuthorizedDate
    {
      get => this.AuthorizedDate;
      set => this.AuthorizedDate = value;
    }

    DateTime IRevisedWorkItemEntity.RevisedDate => this.RevisedDate;

    public bool SpansMultipleRevisions => false;
  }
}
