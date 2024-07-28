// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestTagSecureObject
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestTagSecureObject
  {
    public TestTagSecureObject(Guid projectId, TestTag TestTag)
    {
      this.TestTag = TestTag;
      this.secureTestTag(projectId);
    }

    public TestTag TestTag { get; private set; }

    private void secureTestTag(Guid projectId)
    {
      if (this.TestTag == null)
        return;
      this.TestTag.InitializeSecureObject((ISecuredObject) new TestLogStoreSecuredObject(projectId));
    }
  }
}
