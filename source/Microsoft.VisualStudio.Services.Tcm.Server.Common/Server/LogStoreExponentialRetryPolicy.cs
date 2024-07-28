// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreExponentialRetryPolicy
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreExponentialRetryPolicy : IExtendedRetryPolicy, IRetryPolicy
  {
    private ExponentialRetry m_innerRetryPolicy;
    private IVssRequestContext requestContext;

    public LogStoreExponentialRetryPolicy(
      IVssRequestContext requestContext,
      TimeSpan deltaBackOff,
      int maxAttempts)
    {
      this.requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));
      this.m_innerRetryPolicy = new ExponentialRetry(deltaBackOff, maxAttempts);
    }

    private LogStoreExponentialRetryPolicy(
      IVssRequestContext requestContext,
      ExponentialRetry retryPolicy)
    {
      this.requestContext = requestContext;
      this.m_innerRetryPolicy = retryPolicy;
    }

    public IRetryPolicy CreateInstance() => (IRetryPolicy) new LogStoreExponentialRetryPolicy(this.requestContext, this.m_innerRetryPolicy.CreateInstance() as ExponentialRetry);

    public bool ShouldRetry(
      int currentRetryCount,
      int statusCode,
      Exception lastException,
      out TimeSpan retryInterval,
      OperationContext operationContext)
    {
      return this.IsContainerBeingDeleted(statusCode, lastException) ? this.m_innerRetryPolicy.ShouldRetry(currentRetryCount, 503, lastException, out retryInterval, operationContext) : this.m_innerRetryPolicy.ShouldRetry(currentRetryCount, statusCode, lastException, out retryInterval, operationContext);
    }

    public RetryInfo Evaluate(Microsoft.Azure.Storage.RetryPolicies.RetryContext retryContext, OperationContext operationContext)
    {
      if (this.IsContainerBeingDeleted(retryContext.LastRequestResult.HttpStatusCode, retryContext.LastRequestResult.Exception))
        retryContext.LastRequestResult.HttpStatusCode = 503;
      RetryInfo retryInfo = this.m_innerRetryPolicy.Evaluate(retryContext, operationContext);
      if (retryInfo == null)
        return retryInfo;
      this.requestContext.Trace(1015676, TraceLevel.Info, "TestManagement", "LogStorage", "LogStore command retrying");
      return retryInfo;
    }

    private bool IsContainerBeingDeleted(int statusCode, Exception lastException) => statusCode == 409 && (lastException is StorageException storageException ? storageException.RequestInformation?.ExtendedErrorInformation : (StorageExtendedErrorInformation) null) != null && string.Equals(storageException.RequestInformation.ExtendedErrorInformation.ErrorCode, BlobErrorCodeStrings.ContainerBeingDeleted, StringComparison.Ordinal);
  }
}
