// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultArtifacts
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestResultArtifacts
  {
    public TestResultArtifacts()
    {
    }

    public TestResultArtifacts(
      TestCaseResult testCaseResult,
      List<TestActionResult> actionResults,
      List<TestResultParameter> parameterResults,
      List<TestResultAttachment> attachmentResults)
    {
      this.TestCaseResult = testCaseResult;
      this.ActionResults = actionResults;
      this.ParameterResults = parameterResults;
      this.AttachmentResults = attachmentResults;
    }

    public TestCaseResult TestCaseResult { get; set; }

    public List<TestActionResult> ActionResults { get; set; }

    public List<TestResultParameter> ParameterResults { get; set; }

    public List<TestResultAttachment> AttachmentResults { get; set; }
  }
}
