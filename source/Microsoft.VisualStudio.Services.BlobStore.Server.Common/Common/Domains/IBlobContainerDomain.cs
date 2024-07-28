// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.IBlobContainerDomain
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public interface IBlobContainerDomain : IDomain
  {
    ICloudBlobContainer Find(BlobIdentifier blobId);

    ICloudBlobContainer Find(DedupIdentifier blobId);

    IEnumerable<(IEnumerable<BlobIdentifier> BlobIds, ICloudBlobContainer BlobContainer)> FindBlobContainers(
      IEnumerable<BlobIdentifier> keys);

    IEnumerable<(IEnumerable<DedupIdentifier> DedupIds, ICloudBlobContainer BlobContainer)> FindBlobContainers(
      IEnumerable<DedupIdentifier> keys);

    IEnumerable<ICloudBlobContainer> GetAllContainers();
  }
}
