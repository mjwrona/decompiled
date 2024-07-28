// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogSingleTransferContext
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.DataMovement;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestLogSingleTransferContext : ITestLogSingleTransferContext
  {
    private SingleTransferContext _singleTransferContext;

    public TestLogSingleTransferContext(string activityId)
    {
      SingleTransferContext singleTransferContext = new SingleTransferContext();
      ((TransferContext) singleTransferContext).ClientRequestId = activityId;
      this._singleTransferContext = singleTransferContext;
    }

    public TestLogSingleTransferContext(string activityId, bool overwrite)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      TestLogSingleTransferContext.\u003C\u003Ec__DisplayClass1_0 cDisplayClass10 = new TestLogSingleTransferContext.\u003C\u003Ec__DisplayClass1_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass10.overwrite = overwrite;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      SingleTransferContext singleTransferContext = new SingleTransferContext();
      ((TransferContext) singleTransferContext).ClientRequestId = activityId;
      // ISSUE: method pointer
      ((TransferContext) singleTransferContext).ShouldOverwriteCallbackAsync = new ShouldOverwriteCallbackAsync((object) cDisplayClass10, __methodptr(\u003C\u002Ector\u003Eb__0));
      this._singleTransferContext = singleTransferContext;
    }

    public SingleTransferContext GetSingleTransferContext() => this._singleTransferContext;
  }
}
