// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MultipartBinaryExtensionControllerHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class MultipartBinaryExtensionControllerHelper
  {
    private const string Layer = "MultipartBinaryExtensionControllerHelper";
    private readonly string _tempRootPath;

    public MultipartBinaryExtensionControllerHelper(string tempRootPath) => this._tempRootPath = tempRootPath;

    public async Task ReadFilePartsAsync(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      Stream inputStream,
      long maxPackageSize)
    {
      StreamContent bufferlessContent;
      using (MultipartBinaryExtensionControllerHelper.MaxBytesReadStream requestStream = new MultipartBinaryExtensionControllerHelper.MaxBytesReadStream(inputStream, maxPackageSize))
      {
        if (request.Content.Headers.ContentLength.HasValue && request.Content.Headers.ContentLength.Value > maxPackageSize)
        {
          string message = GalleryWebApiResources.ExtensionSizeExceeded((object) request.Content.Headers.ContentLength.Value, (object) maxPackageSize);
          requestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionControllerHelper), message);
          throw new ExtensionSizeExceededException(message);
        }
        bufferlessContent = new StreamContent((Stream) requestStream);
        try
        {
          foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
            bufferlessContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
          Collection<MultipartFileData> fileData = (await bufferlessContent.ReadAsMultipartAsync<MultipartFileStreamProvider>(new MultipartFileStreamProvider(this._tempRootPath))).FileData;
          if (fileData.Count != 2)
          {
            string message = GalleryResources.MultipartInvalidNumberOfPartsException((object) fileData.Count, (object) 2);
            requestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionControllerHelper), message);
            throw new HttpException(400, message);
          }
          foreach (MultipartFileData part in fileData)
            this.HandleFilePart(requestContext, request, part);
          this.ValidateFileParts(requestContext, request);
          requestContext.Items["IsPublisherSigned"] = (object) true;
        }
        finally
        {
          bufferlessContent?.Dispose();
        }
      }
      bufferlessContent = (StreamContent) null;
    }

    private void HandleFilePart(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      MultipartFileData part)
    {
      if (part.Headers.ContentDisposition == null || part.Headers.ContentDisposition.DispositionType != "attachment" || part.Headers.ContentDisposition.Name == null || part.Headers.ContentType?.MediaType != "application/octet-stream")
      {
        string message = GalleryResources.MultipartInvalidPartHeadersException();
        requestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionControllerHelper), message);
        throw new HttpException(400, message);
      }
      string str;
      switch (part.Headers.ContentDisposition.Name.ToLowerInvariant())
      {
        case "vsix":
          str = "Gallery_PackageFileName";
          break;
        case "sigzip":
          str = "Gallery_SignatureArchiveFileName";
          break;
        default:
          throw new HttpException(400, GalleryResources.MultipartUnknownFileTypeException());
      }
      string key = str;
      request.Properties[key] = (object) part.LocalFileName;
    }

    private void ValidateFileParts(IVssRequestContext requestContext, HttpRequestMessage request)
    {
      if (!request.Properties.ContainsKey("Gallery_SignatureArchiveFileName"))
      {
        string message = GalleryResources.MultipartMissingSignatureArchiveException();
        requestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionControllerHelper), message);
        throw new HttpException(400, message);
      }
      if (!request.Properties.ContainsKey("Gallery_PackageFileName"))
      {
        string message = GalleryResources.MultipartMissingPackageException();
        requestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionControllerHelper), message);
        throw new HttpException(400, message);
      }
    }

    private class MaxBytesReadStream : Stream
    {
      private readonly long _maxBytesToRead;
      private readonly Stream _stream;
      private long _bytesRead;

      public MaxBytesReadStream(Stream wrappedStream, long maxBytesToRead)
      {
        this._stream = wrappedStream;
        this._maxBytesToRead = maxBytesToRead;
      }

      public override bool CanRead => this._stream.CanRead;

      public override bool CanSeek => false;

      public override bool CanWrite => false;

      public override long Length => this._stream.Length;

      public override long Position
      {
        get => this._stream.Position;
        set => throw new NotSupportedException();
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        int num = this._stream.Read(buffer, offset, count);
        this._bytesRead += (long) num;
        if (this._bytesRead > this._maxBytesToRead)
          throw new ExtensionSizeExceededException(GalleryResources.MultipartMaxSizeExceededException((object) this._maxBytesToRead));
        return num;
      }

      protected override void Dispose(bool disposing)
      {
        this._stream?.Dispose();
        base.Dispose(disposing);
      }

      public override void Flush() => throw new NotSupportedException();

      public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

      public override void SetLength(long value) => throw new NotSupportedException();

      public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
  }
}
