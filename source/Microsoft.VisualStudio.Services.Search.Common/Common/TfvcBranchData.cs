// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcBranchData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class TfvcBranchData
  {
    [DataMember]
    public int CurrentGroupInProgress { get; set; }

    [DataMember]
    public List<TfvcBranchGroup> AllBranchGroups { get; set; }

    public TfvcBranchData()
    {
      this.CurrentGroupInProgress = -1;
      this.AllBranchGroups = new List<TfvcBranchGroup>();
    }

    public TfvcBranchData Clone()
    {
      TfvcBranchData tfvcBranchData = (TfvcBranchData) this.MemberwiseClone();
      if (this.AllBranchGroups != null)
      {
        tfvcBranchData.AllBranchGroups = new List<TfvcBranchGroup>();
        foreach (TfvcBranchGroup allBranchGroup in this.AllBranchGroups)
          tfvcBranchData.AllBranchGroups.Add(allBranchGroup.Clone());
      }
      return tfvcBranchData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[CurrentGroupInProgress: ");
      stringBuilder.Append(this.CurrentGroupInProgress);
      stringBuilder.Append(", AllBranchGroups Count: ");
      stringBuilder.Append((object) this.AllBranchGroups?.Count);
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
