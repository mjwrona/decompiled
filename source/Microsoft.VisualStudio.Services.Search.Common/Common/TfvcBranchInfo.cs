// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcBranchInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class TfvcBranchInfo
  {
    [DataMember]
    public string BranchName { get; set; }

    [DataMember]
    public string ContinuationToken { get; set; }

    public TfvcBranchInfo()
    {
      this.BranchName = string.Empty;
      this.ContinuationToken = string.Empty;
    }

    public TfvcBranchInfo Clone() => (TfvcBranchInfo) this.MemberwiseClone();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[BranchName: ");
      stringBuilder.Append(this.BranchName);
      stringBuilder.Append(", ContinuationToken: ");
      stringBuilder.Append(this.ContinuationToken);
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
