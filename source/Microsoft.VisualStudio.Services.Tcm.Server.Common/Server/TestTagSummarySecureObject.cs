// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestTagSummarySecureObject
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestTagSummarySecureObject
  {
    public TestTagSummarySecureObject(Guid projectId, TestTagSummary TestTagSummary)
    {
      this.TestTagSummary = TestTagSummary;
      this.secureTestTagSummary(projectId);
    }

    public TestTagSummary TestTagSummary { get; private set; }

    private void secureTestTagSummary(Guid projectId)
    {
      if (this.TestTagSummary == null)
        return;
      this.TestTagSummary.InitializeSecureObject((ISecuredObject) new TestLogStoreSecuredObject(projectId));
    }
  }
}
