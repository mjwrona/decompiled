// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CSR.IAzureStorage
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CSR
{
  [DefaultServiceImplementation(typeof (AzureStorage))]
  public interface IAzureStorage : IVssFrameworkService
  {
    Task<CloudBlockBlob> UploadContentToBlobStorageAsync(
      Stream blobContent,
      string containerName,
      string blobName);

    Task<List<IListBlobItem>> ListAllBlobsWithPrefix(string containerName, string prefix);
  }
}
