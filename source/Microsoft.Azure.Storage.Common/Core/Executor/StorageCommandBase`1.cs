// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Executor.StorageCommandBase`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Executor
{
  internal abstract class StorageCommandBase<T>
  {
    public int? ServerTimeoutInSeconds;
    internal DateTime? OperationExpiryTime;
    internal TimeSpan? NetworkTimeout;
    internal object OperationState;
    private volatile StreamDescriptor streamCopyState;
    private volatile RequestResult currentResult;
    private IList<RequestResult> requestResults = (IList<RequestResult>) new List<RequestResult>();
    public Action<StorageCommandBase<T>, Exception, OperationContext> RecoveryAction;
    public Func<Stream, HttpResponseMessage, string, StorageExtendedErrorInformation> ParseError;
    public Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>> ParseErrorAsync;
    public Func<Stream, IDictionary<string, string>, string, StorageExtendedErrorInformation> ParseDataServiceError;

    internal StreamDescriptor StreamCopyState
    {
      get => this.streamCopyState;
      set => this.streamCopyState = value;
    }

    internal RequestResult CurrentResult
    {
      get => this.currentResult;
      set => this.currentResult = value;
    }

    internal IList<RequestResult> RequestResults => this.requestResults;
  }
}
