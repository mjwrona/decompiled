// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.IStorageAccountAdapter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public interface IStorageAccountAdapter
  {
    IEnumerable<IListBlobItem> GetCloudBlobs(string connectionString, string prefix);

    string GetStorageAccountName(string connectionString);

    IEnumerable<string> PopulateStorageAccountList(IVssRequestContext requestContext);

    IEnumerable<StorageLogRecord> GetLogRecords(ICloudBlob cloudBlob, string filterString);

    Task WriteLogBlobToDiskAsync(
      ICloudBlob cloudBlob,
      string filterString,
      string defaultDownloadDirectory);
  }
}
