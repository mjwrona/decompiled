// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultRequirementAssociation
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestResultRequirementAssociation : IEquatable<TestResultRequirementAssociation>
  {
    private int _testRunId;
    private int _testResultId;
    private int _workItemId;

    public TestResultRequirementAssociation()
    {
    }

    public TestResultRequirementAssociation(int testRunId, int testResultId)
    {
      this._testRunId = testRunId;
      this._testResultId = testResultId;
    }

    public int TestRunId
    {
      get => this._testRunId;
      set => this._testRunId = value;
    }

    public int TestResultId
    {
      get => this._testResultId;
      set => this._testResultId = value;
    }

    public int WorkItemId
    {
      get => this._workItemId;
      set => this._workItemId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0}, {1}, {2})", (object) this.TestRunId, (object) this.TestResultId, (object) this.WorkItemId);

    public bool Equals(TestResultRequirementAssociation other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.TestRunId == other.TestRunId && this.TestResultId == other.TestResultId;
    }

    public override int GetHashCode() => (391 + this.TestRunId) * 23 + this.TestResultId;
  }
}
