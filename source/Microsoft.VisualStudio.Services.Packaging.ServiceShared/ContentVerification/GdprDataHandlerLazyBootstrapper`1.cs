// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.GdprDataHandlerLazyBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class GdprDataHandlerLazyBootstrapper<TReq> : 
    IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry)>>
    where TReq : class, IFeedRequest
  {
    private readonly IExecutionEnvironment executionEnvironmentFacade;
    private readonly IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>> gdprDataHandlerBootstrapper;

    public GdprDataHandlerLazyBootstrapper(
      IExecutionEnvironment executionEnvironmentFacade,
      IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>> gdprDataHandlerBootstrapper)
    {
      this.executionEnvironmentFacade = executionEnvironmentFacade;
      this.gdprDataHandlerBootstrapper = gdprDataHandlerBootstrapper;
    }

    public IAsyncHandler<(TReq, ICommitLogEntry)> Bootstrap()
    {
      ByFuncAsyncHandler<(TReq, ICommitLogEntry), bool> noOpGdprDatahandler = new ByFuncAsyncHandler<(TReq, ICommitLogEntry), bool>((Func<(TReq, ICommitLogEntry), bool>) (r => false));
      return UntilNonNullHandler.Create<(TReq, ICommitLogEntry), bool>(HostedHandler.Create<(TReq, ICommitLogEntry), bool>(this.executionEnvironmentFacade, (Func<IAsyncHandler<(TReq, ICommitLogEntry), bool>>) (() => this.gdprDataHandlerBootstrapper == null ? (IAsyncHandler<(TReq, ICommitLogEntry), bool>) noOpGdprDatahandler : this.gdprDataHandlerBootstrapper.Bootstrap())), (IAsyncHandler<(TReq, ICommitLogEntry), bool>) noOpGdprDatahandler).ThenReturnNullResult<(TReq, ICommitLogEntry), bool>();
    }
  }
}
