// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RequestAndCommitToOpHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RequestAndCommitToOpHandler<TReq, TOp> : 
    IAsyncHandler<(TReq, ICommitLogEntry), (TReq, TOp)>,
    IHaveInputType<(TReq, ICommitLogEntry)>,
    IHaveOutputType<(TReq, TOp)>
    where TOp : class
  {
    public Task<(TReq, TOp)> Handle((TReq, ICommitLogEntry) request)
    {
      TReq req = request.Item1;
      if (!(request.Item2.CommitOperationData is TOp commitOperationData))
        throw new ArgumentException(string.Format("Bad operation type: {0}", (object) request.Item2.CommitOperationData));
      return Task.FromResult<(TReq, TOp)>((req, commitOperationData));
    }
  }
}
