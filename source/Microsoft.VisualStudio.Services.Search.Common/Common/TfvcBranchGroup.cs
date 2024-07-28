// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcBranchGroup
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class TfvcBranchGroup
  {
    [DataMember]
    public int GroupId { get; set; }

    [DataMember]
    public List<TfvcBranchInfo> BranchesInfo { get; set; }

    public TfvcBranchGroup()
    {
      this.GroupId = -1;
      this.BranchesInfo = new List<TfvcBranchInfo>();
    }

    public TfvcBranchGroup Clone()
    {
      TfvcBranchGroup tfvcBranchGroup = (TfvcBranchGroup) this.MemberwiseClone();
      if (this.BranchesInfo != null)
      {
        tfvcBranchGroup.BranchesInfo = new List<TfvcBranchInfo>();
        foreach (TfvcBranchInfo tfvcBranchInfo in this.BranchesInfo)
          tfvcBranchGroup.BranchesInfo.Add(tfvcBranchInfo.Clone());
      }
      return tfvcBranchGroup;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[GroupId: ");
      stringBuilder.Append(this.GroupId);
      stringBuilder.Append(", BranchesInfo Count: ");
      stringBuilder.Append((object) this.BranchesInfo?.Count);
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
