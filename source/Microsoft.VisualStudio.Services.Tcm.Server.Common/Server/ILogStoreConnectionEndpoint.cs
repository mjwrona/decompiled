// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ILogStoreConnectionEndpoint
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ILogStoreConnectionEndpoint
  {
    CloudBlobClient GetCloudBlobClient();

    CloudStorageAccount GetCloudStorageAccount();

    string GetAccountName();

    Task<ILogStoreContainerSegment> GetBlobContainersAsync(
      string prefix,
      int maxResults,
      BlobContinuationToken blobContinuationToken,
      BlobRequestOptions blobRequestOptions,
      ILogStoreOperationContext logStoreOperationContext);

    bool EnableCors(string corsDomainName);

    IList<string> GetCorsAllowedHostList();
  }
}
