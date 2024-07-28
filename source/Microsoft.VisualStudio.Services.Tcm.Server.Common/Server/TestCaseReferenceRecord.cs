// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseReferenceRecord
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestCaseReferenceRecord
  {
    public Guid ProjectGuid { get; internal set; }

    public int TestCaseReferenceId { get; internal set; }

    public int TestPointId { get; internal set; }

    public string AutomatedTestName { get; internal set; }

    public string AutomatedTestStorage { get; internal set; }

    public string TestCaseTitle { get; internal set; }

    public int Priority { get; internal set; }

    public string Owner { get; internal set; }

    public TestArtifactSource DataSourceId { get; internal set; }

    public List<string> TestFlakyBranchName { get; internal set; }
  }
}
