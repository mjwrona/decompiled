// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogSecureObject
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestLogSecureObject
  {
    public TestLogSecureObject(Guid projectId, TestLog testLog)
    {
      this.TestLog = testLog;
      this.secureTestLog(projectId);
    }

    public TestLog TestLog { get; private set; }

    private void secureTestLog(Guid projectId)
    {
      if (this.TestLog == null)
        return;
      this.TestLog.InitializeSecureObject((ISecuredObject) new TestLogStoreSecuredObject(projectId));
    }
  }
}
