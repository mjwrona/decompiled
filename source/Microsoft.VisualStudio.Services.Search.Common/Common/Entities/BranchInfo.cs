// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.BranchInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [DataContract(Namespace = "")]
  public class BranchInfo
  {
    [DataMember(Name = "BranchName")]
    public string BranchName { get; set; }

    [DataMember(Name = "ChangeId")]
    public string ChangeId { get; set; }

    [DataMember(Name = "ChangeTime")]
    public DateTime ChangeTime { get; set; }

    public BranchInfo()
    {
    }

    public BranchInfo(string branchName, string changeId, DateTime changeTime)
    {
      this.BranchName = branchName;
      this.ChangeId = changeId;
      this.ChangeTime = changeTime;
    }

    public class BranchInfoComparer : IEqualityComparer<BranchInfo>
    {
      public bool Equals(BranchInfo x, BranchInfo y)
      {
        if (x == y)
          return true;
        return x != null && y != null && x.BranchName.Equals(y.BranchName, StringComparison.Ordinal) && x.ChangeId == y.ChangeId && x.ChangeTime == y.ChangeTime;
      }

      public int GetHashCode(BranchInfo obj) => obj == null || obj.BranchName == null ? 0 : obj.BranchName.GetHashCode();
    }
  }
}
