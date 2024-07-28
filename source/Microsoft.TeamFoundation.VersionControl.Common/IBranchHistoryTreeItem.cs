// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.IBranchHistoryTreeItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System.Collections;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IBranchHistoryTreeItem
  {
    void AddChild(IBranchHistoryTreeItem newChild);

    IBranchRelative Relative { get; set; }

    int Level { get; set; }

    IBranchHistoryTreeItem Parent { get; set; }

    ICollection Children { get; }

    IBranchHistoryTreeItem GetRequestedItem();
  }
}
