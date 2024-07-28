// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.IAzureBlobGeoRedundancyService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  [DefaultServiceImplementation(typeof (AzureBlobGeoRedundancyService))]
  public interface IAzureBlobGeoRedundancyService : IVssFrameworkService
  {
    AzureBlobGeoRedundancyServiceSettings Settings { get; }

    bool IsEnabled(IVssRequestContext requestContext);

    int GetNumberOfQueues(IVssRequestContext requestContext);

    bool AreSynchronousWritesEnabled(IVssRequestContext requestContext);

    BlobGeoRedundancyEndpoint SetupEndpoint(
      IVssRequestContext requestContext,
      CloudStorageAccount storageAccount);

    void CreateBlob(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName,
      string blobName);

    void DeleteBlob(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName,
      string blobName);

    void CreateContainer(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName);

    void DeleteContainer(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName);
  }
}
