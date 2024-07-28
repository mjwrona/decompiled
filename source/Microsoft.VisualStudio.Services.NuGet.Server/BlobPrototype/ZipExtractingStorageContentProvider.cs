// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.ZipExtractingStorageContentProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class ZipExtractingStorageContentProvider : ISpecificExtractingStorageContentProvider
  {
    private readonly IAsyncHandler<ContentResult, Stream> contentResultToSeekableStreamHandler;

    public static ISpecificExtractingStorageContentProvider Instance { get; } = (ISpecificExtractingStorageContentProvider) new ZipExtractingStorageContentProvider(GetSeekableStreamFromContentResultHandler.Instance);

    public ZipExtractingStorageContentProvider(
      IAsyncHandler<ContentResult, Stream> contentResultToSeekableStreamHandler)
    {
      this.contentResultToSeekableStreamHandler = contentResultToSeekableStreamHandler;
    }

    public async Task<ContentResult?> GetContentOrDefault(
      IPackageFileRequest request,
      IExtractingStorageId storageId,
      ContentResult containerContent)
    {
      if (!(storageId is ExtractFileFromZipStorageId zipStorageId))
        return (ContentResult) null;
      string targetPath = ZipExtractingStorageContentProvider.NormalizeEntryName(zipStorageId.Path);
      Stream stream = await this.contentResultToSeekableStreamHandler.Handle(containerContent);
      try
      {
        ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        try
        {
          return new ContentResult((Stream) new DisposingStreamWrapper((zipArchive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (entry => ZipExtractingStorageContentProvider.NormalizeEntryName(entry.FullName).Equals(targetPath, StringComparison.OrdinalIgnoreCase))) ?? throw ExceptionHelper.PackageInnerFileNotFound(request.PackageId, request.FilePath, zipStorageId.Path)).Open(), new IDisposable[2]
          {
            (IDisposable) zipArchive,
            (IDisposable) stream
          }), targetPath);
        }
        catch
        {
          zipArchive.Dispose();
          throw;
        }
      }
      catch
      {
        stream.Dispose();
        throw;
      }
    }

    private static string NormalizeEntryName(string path) => path.Replace('/', '\\').Trim('\\');
  }
}
