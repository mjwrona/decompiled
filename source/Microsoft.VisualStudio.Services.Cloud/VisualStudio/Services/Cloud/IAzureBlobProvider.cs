// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IAzureBlobProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public interface IAzureBlobProvider : IBlobProvider
  {
    string StorageAccountName { get; }

    Uri Uri { get; }

    CloudBlobContainer GetCloudBlobContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null);

    void PutStreamRaw(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout);

    void PutStreamRawUsingBlockBlobClient(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      int? blockSize,
      CancellationToken cancellationToken);

    void DisableBufferManager();

    Page<TaggedBlobItem> FindBlobsByTags(
      IDictionary<string, string> tags,
      string containerName,
      string continuationToken);
  }
}
