// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FileSplitGroupData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class FileSplitGroupData : ChangeEventData
  {
    private FileSplitGroupData()
    {
    }

    public FileSplitGroupData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember(Order = 0)]
    public long StartingId { get; set; }

    [DataMember(Order = 1)]
    public int TakeCount { get; set; }

    [DataMember(Order = 2)]
    public long LastId { get; set; }

    [DataMember(Order = 3)]
    public Guid RequestId { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(StartingId: ");
      stringBuilder.Append(this.StartingId);
      stringBuilder.Append(", TakeCount: ");
      stringBuilder.Append(this.TakeCount);
      stringBuilder.Append(", LastId: ");
      stringBuilder.Append(this.LastId);
      stringBuilder.Append(", RequestId: ");
      stringBuilder.Append((object) this.RequestId);
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }
  }
}
