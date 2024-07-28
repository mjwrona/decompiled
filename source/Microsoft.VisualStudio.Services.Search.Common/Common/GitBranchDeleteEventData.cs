// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.GitBranchDeleteEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Common;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class GitBranchDeleteEventData : ChangeEventData
  {
    private GitBranchDeleteEventData()
    {
    }

    public GitBranchDeleteEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember(Order = 0)]
    public string BranchName { get; set; }

    [DataMember(Order = 1)]
    public string ProjectName { get; set; }

    [DataMember(Order = 2)]
    public bool CleanUpIndexingUnitData { get; set; }

    [DataMember(Order = 3)]
    public HashSet<string> Branches { get; set; }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext ctx)
    {
      if (string.IsNullOrWhiteSpace(this.BranchName))
        return;
      if (this.Branches.IsNullOrEmpty<string>())
        this.Branches = new HashSet<string>();
      this.Branches.Add(this.BranchName);
    }
  }
}
