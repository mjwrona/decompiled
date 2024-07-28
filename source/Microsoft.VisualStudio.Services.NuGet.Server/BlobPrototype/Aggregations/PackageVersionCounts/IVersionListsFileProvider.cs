// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.IVersionListsFileProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public interface IVersionListsFileProvider
  {
    Task<EtagValue<IMutableVersionListsFile>> GetVersionListDocument(IFeedRequest feedRequest);

    Task<string> PutVersionListDocument(
      IFeedRequest feedRequest,
      ILazyVersionListsFile versionListsFile,
      string etag);

    Task<EtagValue<byte[]>> GetVersionListDocumentBytes(IFeedRequest feedRequest);
  }
}
