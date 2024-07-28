// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.CreateContainerExponentialRetryPolicy
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.RetryPolicies;
using System;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  internal class CreateContainerExponentialRetryPolicy : IExtendedRetryPolicy, IRetryPolicy
  {
    private ExponentialRetry m_innerRetryPolicy;

    public CreateContainerExponentialRetryPolicy() => this.m_innerRetryPolicy = new ExponentialRetry();

    public IRetryPolicy CreateInstance() => (IRetryPolicy) new CreateContainerExponentialRetryPolicy();

    public bool ShouldRetry(
      int currentRetryCount,
      int statusCode,
      Exception lastException,
      out TimeSpan retryInterval,
      OperationContext operationContext)
    {
      return this.IsContainerBeingDeleted(statusCode, lastException) ? this.m_innerRetryPolicy.ShouldRetry(currentRetryCount, 503, lastException, out retryInterval, operationContext) : this.m_innerRetryPolicy.ShouldRetry(currentRetryCount, statusCode, lastException, out retryInterval, operationContext);
    }

    public RetryInfo Evaluate(RetryContext retryContext, OperationContext operationContext)
    {
      if (this.IsContainerBeingDeleted(retryContext.LastRequestResult.HttpStatusCode, retryContext.LastRequestResult.Exception))
        retryContext.LastRequestResult.HttpStatusCode = 503;
      return this.m_innerRetryPolicy.Evaluate(retryContext, operationContext);
    }

    private bool IsContainerBeingDeleted(int statusCode, Exception lastException) => statusCode == 409 && (lastException is StorageException storageException ? storageException.RequestInformation?.ExtendedErrorInformation : (StorageExtendedErrorInformation) null) != null && string.Equals(storageException.RequestInformation.ExtendedErrorInformation.ErrorCode, BlobErrorCodeStrings.ContainerBeingDeleted, StringComparison.Ordinal);
  }
}
