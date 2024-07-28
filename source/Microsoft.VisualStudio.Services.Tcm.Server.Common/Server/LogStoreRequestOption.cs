// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreRequestOption
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreRequestOption : ILogStoreRequestOption
  {
    private BlobRequestOptions _blobRequestOptions;

    public LogStoreRequestOption(
      int serverTimeoutInSeconds,
      int maximumExecutionTimeInSeconds,
      LogStoreExponentialRetryPolicy logStoreExponentialRetryPolicy)
    {
      this._blobRequestOptions = new BlobRequestOptions()
      {
        ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds((double) serverTimeoutInSeconds)),
        MaximumExecutionTime = new TimeSpan?(TimeSpan.FromSeconds((double) maximumExecutionTimeInSeconds)),
        RetryPolicy = (IRetryPolicy) logStoreExponentialRetryPolicy,
        StoreBlobContentMD5 = new bool?(true)
      };
    }

    public BlobRequestOptions GetBlobRequestOptions() => this._blobRequestOptions;
  }
}
