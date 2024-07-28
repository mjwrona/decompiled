// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestBranchFlakinesStateMap
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestBranchFlakinesStateMap : IEquatable<TestBranchFlakinesStateMap>
  {
    public string BranchName;
    public bool IsFlaky;

    public TestBranchFlakinesStateMap(string branchName, bool isFlaky)
    {
      this.BranchName = branchName;
      this.IsFlaky = isFlaky;
    }

    public override bool Equals(object obj) => this.Equals(obj as TestBranchFlakinesStateMap);

    public bool Equals(TestBranchFlakinesStateMap other) => other != null && this.BranchName == other.BranchName && this.IsFlaky == other.IsFlaky;

    public override int GetHashCode() => this.BranchName.GetHashCode() + this.IsFlaky.GetHashCode();
  }
}
