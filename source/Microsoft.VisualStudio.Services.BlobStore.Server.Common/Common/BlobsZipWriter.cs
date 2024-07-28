// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobsZipWriter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Artifact.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class BlobsZipWriter : IBlobsZipWriter
  {
    public Task WriteZippedContentsToStreamAsync(
      IDictionary<ManifestItem, IEnumerable<Uri>> nodeZipMappings,
      string zipRootFolderName,
      Stream outputStream,
      HttpClient httpClient,
      IAppTraceSource tracer,
      IClock clock,
      CancellationToken cancellationToken)
    {
      return BlobsZipWriter.WriteZippedContentsToStreamAsync((IEnumerable<ManifestItem>) nodeZipMappings.Keys, zipRootFolderName, outputStream, clock, (Func<IDomainId, ManifestItem, Task<Stream>>) ((domainId, item) => Task.FromResult<Stream>((Stream) Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.BlobStitcher.GetTransitiveContent((long) item.Blob.Size, nodeZipMappings[item], httpClient, tracer, cancellationToken))), cancellationToken);
    }

    public Task WriteZippedContentsToStreamAsync(
      IEnumerable<ManifestItem> itemsInManifest,
      string zipRootFolderName,
      Stream outputStream,
      IDedupBlobMultipartHttpClient dedupBlobHttpClient,
      IClock clock,
      CancellationToken cancellationToken)
    {
      return BlobsZipWriter.WriteZippedContentsToStreamAsync(itemsInManifest, zipRootFolderName, outputStream, clock, (Func<IDomainId, ManifestItem, Task<Stream>>) ((domainId, item) => dedupBlobHttpClient.GetFileStreamAsync(domainId, DedupIdentifier.Create(item.Blob.Id), false, cancellationToken)), cancellationToken);
    }

    private static Task WriteZippedContentsToStreamAsync(
      IEnumerable<ManifestItem> itemsInManifest,
      string zipRootFolderName,
      Stream outputStream,
      IClock clock,
      Func<IDomainId, ManifestItem, Task<Stream>> getDedupStreamAsync,
      CancellationToken cancellationToken)
    {
      return BlobsZipWriter.SafeWriteZipToStreamAsync(outputStream, (Func<ZipArchive, Task>) (async archive =>
      {
        DateTimeOffset startWriteDateTime = (DateTimeOffset) TimeZoneInfo.ConvertTimeFromUtc(clock.Now.DateTime, TimeZoneInfo.Local);
        foreach (ManifestItem manifestItem in itemsInManifest)
        {
          if (manifestItem.Type == ManifestItemType.File)
          {
            string str = zipRootFolderName == null ? manifestItem.Path : zipRootFolderName + manifestItem.Path;
            char[] chArray = new char[1]{ '/' };
            ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(str.TrimStart(chArray));
            zipArchiveEntry.LastWriteTime = startWriteDateTime;
            Stream nodeStream = await getDedupStreamAsync(WellKnownDomainIds.DefaultDomainId, manifestItem).ConfigureAwait(false);
            Stream stream;
            try
            {
              stream = zipArchiveEntry.Open();
              try
              {
                await nodeStream.CopyToAsync(stream, 1048576, cancellationToken).ConfigureAwait(false);
              }
              finally
              {
                stream?.Dispose();
              }
            }
            finally
            {
              nodeStream?.Dispose();
            }
            zipArchiveEntry = (ZipArchiveEntry) null;
            nodeStream = (Stream) null;
            stream = (Stream) null;
          }
          else
          {
            if (manifestItem.Type != ManifestItemType.EmptyDirectory)
              throw new UnsupportedManifestItemException("The manifest item type is unsupported");
            string entryName = (zipRootFolderName == null ? manifestItem.Path : zipRootFolderName + manifestItem.Path).TrimStart('/');
            if (!entryName.EndsWith("/", StringComparison.Ordinal) && !entryName.EndsWith("\\", StringComparison.Ordinal))
              entryName += "/";
            archive.CreateEntry(entryName).LastWriteTime = startWriteDateTime;
          }
        }
      }));
    }

    private static async Task SafeWriteZipToStreamAsync(
      Stream outputStream,
      Func<ZipArchive, Task> zipArchiveTask)
    {
      SmartPushStreamContentStream smartStream = new SmartPushStreamContentStream(outputStream);
      try
      {
        ZipArchive archive = new ZipArchive((Stream) smartStream, ZipArchiveMode.Create);
        try
        {
          await zipArchiveTask(archive).ConfigureAwait(false);
          smartStream = (SmartPushStreamContentStream) null;
          archive = (ZipArchive) null;
        }
        catch
        {
          outputStream.Dispose();
          smartStream = (SmartPushStreamContentStream) null;
          archive = (ZipArchive) null;
        }
        finally
        {
          archive?.Dispose();
        }
      }
      finally
      {
        smartStream?.Dispose();
      }
    }
  }
}
