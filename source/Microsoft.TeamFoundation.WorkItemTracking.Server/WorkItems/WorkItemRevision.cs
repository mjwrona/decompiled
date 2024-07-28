// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemRevision
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItemRevision : WorkItemFieldData
  {
    private WorkItemRevisionDataset m_revisionDataset;

    internal WorkItemRevision(WorkItemRevisionDataset revisionDataset)
      : base((IDictionary<int, object>) revisionDataset.Fields, (IDictionary<int, Guid>) revisionDataset.IdentityFields, revisionDataset.WorkItemCommentVersion)
    {
      this.m_revisionDataset = revisionDataset;
    }

    public DateTime AuthorizedDate => this.m_revisionDataset.AuthorizedDate;

    public DateTime RevisedDate => this.m_revisionDataset.RevisedDate;

    public List<WorkItemResourceLinkInfo> ResourceLinks => this.m_revisionDataset.ResourceLinks;

    public IEnumerable<WorkItemLinkInfo> WorkItemLinks => this.m_revisionDataset.WorkItemLinks;

    public List<WorkItemResourceLinkInfo> AllResourceLinks => this.m_revisionDataset.AllResourceLinks;

    public List<WorkItemLinkInfo> AllLinks => this.m_revisionDataset.AllLinks;

    internal List<WorkItemResourceLinkInfo> AllHiddenResourceLinks => this.m_revisionDataset.AllHiddenResourceLinks;

    internal List<WorkItemResourceLinkInfo> HiddenResourceLinks => this.m_revisionDataset.HiddenResourceLinks;
  }
}
