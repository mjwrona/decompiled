// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestActionResultUpdateComparator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestActionResultUpdateComparator : IEqualityComparer<TestActionResult>
  {
    public bool Equals(TestActionResult x, TestActionResult y) => x != null && y != null && x.TestResultId == y.TestResultId && x.TestRunId == y.TestRunId && x.IterationId == y.IterationId && string.Equals(x.ActionPath, y.ActionPath, StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(TestActionResult obj) => (obj.TestResultId ^ obj.TestRunId ^ obj.IterationId).GetHashCode();
  }
}
