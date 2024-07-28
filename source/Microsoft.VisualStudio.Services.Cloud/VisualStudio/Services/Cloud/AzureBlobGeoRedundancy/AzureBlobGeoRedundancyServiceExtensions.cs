// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public static class AzureBlobGeoRedundancyServiceExtensions
  {
    public static void CreateBlob(
      this IAzureBlobGeoRedundancyService service,
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      ICloudBlob blob)
    {
      service.CreateBlob(requestContext, endpoint, blob.Container.Name, blob.Name);
    }

    public static void DeleteBlob(
      this IAzureBlobGeoRedundancyService service,
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      ICloudBlob blob)
    {
      service.DeleteBlob(requestContext, endpoint, blob.Container.Name, blob.Name);
    }

    public static void DeleteBlobs(
      this IAzureBlobGeoRedundancyService service,
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName,
      IEnumerable<string> blobNames)
    {
      AzureBlobGeoRedundancyServiceSettings settings = service.Settings;
      Parallel.ForEach<string>(blobNames, (Action<string>) (blobName =>
      {
        BlobActionMessage blobActionMessage = new BlobActionMessage()
        {
          Action = BlobAction.DeleteBlob,
          ContainerName = containerName,
          BlobName = blobName
        };
        AzureBlobGeoRedundancyUtils.CheckValidBlobActionMessageValues(blobActionMessage);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        AzureBlobGeoRedundancyService.EnqueueMessage(endpoint.QueueClient, blobActionMessage, settings, AzureBlobGeoRedundancyServiceExtensions.\u003C\u003EO.\u003C0\u003E__TraceRaw ?? (AzureBlobGeoRedundancyServiceExtensions.\u003C\u003EO.\u003C0\u003E__TraceRaw = new TraceMethod(TeamFoundationTracingService.TraceRaw)));
      }));
    }
  }
}
