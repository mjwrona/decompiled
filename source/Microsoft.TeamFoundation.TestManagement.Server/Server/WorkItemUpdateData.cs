// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WorkItemUpdateData
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WorkItemUpdateData
  {
    public WorkItemUpdateData()
    {
    }

    public WorkItemUpdateData(WorkItemUpdateData source) => this.CopyProperties(source);

    public IdAndRev WitIdAndRev { get; set; }

    public int TemporaryId { get; set; }

    public bool IsNew { get; set; }

    public bool HasStateChanged { get; set; }

    public string WitTypeName { get; set; }

    public string WitCreationState { get; set; }

    public IList<TestExternalLink> ExternalLinks { get; set; }

    public IList<WorkItemLinkInfo> WitLinks { get; set; }

    public bool UpdateLinksOnly { get; set; }

    public WitOperationType WitOperationType { get; set; }

    public void CopyProperties(WorkItemUpdateData sourceData)
    {
      this.WitIdAndRev = sourceData.WitIdAndRev;
      this.IsNew = sourceData.IsNew;
      this.WitTypeName = sourceData.WitTypeName;
      this.WitCreationState = sourceData.WitCreationState;
      this.ExternalLinks = sourceData.ExternalLinks;
      this.WitLinks = sourceData.WitLinks;
      this.UpdateLinksOnly = sourceData.UpdateLinksOnly;
      this.WitOperationType = sourceData.WitOperationType;
      this.HasStateChanged = sourceData.HasStateChanged;
    }

    internal static WorkItemUpdateData CreateWorkItemUpdateData(
      string witTypeName,
      string state,
      bool isNew,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      bool witStateChanged,
      bool updateLinksOnly,
      WitOperationType witOperationType)
    {
      return new WorkItemUpdateData()
      {
        WitTypeName = witTypeName,
        WitCreationState = state,
        ExternalLinks = externalLinks,
        WitLinks = witLinks,
        WitOperationType = witOperationType,
        IsNew = isNew,
        UpdateLinksOnly = updateLinksOnly,
        HasStateChanged = witStateChanged
      };
    }
  }
}
