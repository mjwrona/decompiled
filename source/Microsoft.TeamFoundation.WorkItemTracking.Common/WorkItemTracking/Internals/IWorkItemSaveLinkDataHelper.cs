// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.IWorkItemSaveLinkDataHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IWorkItemSaveLinkDataHelper
  {
    IEnumerable<LinkInfo> DeletedLinks { get; }

    IEnumerable<LinkInfo> AddedLinks { get; }

    IEnumerable<KeyValuePair<LinkInfo, LinkUpdate>> UpdatedLinks { get; }

    void ResetWorkItemLinkInfo();

    void AddWorkItemLinkInfo(WorkItemLinkInfo link);
  }
}
