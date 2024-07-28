// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.FileUploadHandler
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class FileUploadHandler : DelegatingHandler
  {
    private const string Layer = "FileUploadHandler";
    private readonly bool _allowJson;
    private readonly long _suggestedMaxPackageSize;
    private readonly int _suggestedTimeoutMinutes;

    public FileUploadHandler(
      bool allowJson,
      long suggestedMaxPackageSize,
      int suggestedTimeoutMinutes)
    {
      this._allowJson = allowJson;
      this._suggestedMaxPackageSize = suggestedMaxPackageSize;
      this._suggestedTimeoutMinutes = suggestedTimeoutMinutes;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (!this.IsValidRequestMessage(request))
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
      object obj1 = (object) null;
      object obj2 = (object) null;
      if (!request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContext, out obj1) || !request.Properties.TryGetValue(TfsApiPropertyKeys.HttpContext, out obj2))
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
      IVssRequestContext requestContext = obj1 as IVssRequestContext;
      HttpContextBase httpContextBase = obj2 as HttpContextBase;
      requestContext.TraceEnter(12062010, "Gallery", nameof (FileUploadHandler), nameof (SendAsync));
      using (Stream inputStream = httpContextBase.Request.GetBufferlessInputStream(true))
      {
        long packageLength = inputStream.Length;
        string tempFile = FileSpec.GetTempFileName();
        requestContext.Trace(12062010, TraceLevel.Info, "Gallery", nameof (FileUploadHandler), string.Format("Creating package file {0} of size {1}", (object) tempFile, (object) packageLength));
        HttpContent requestContent = request.Content;
        try
        {
          CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GalleryServerUtil.GetFileUploadTimeout(requestContext, this._suggestedTimeoutMinutes));
          using (Stream outputFileStream = (Stream) File.Create(tempFile, 1048576, FileOptions.RandomAccess))
            await inputStream.CopyToAsync(outputFileStream, 32768, cancellationTokenSource.Token);
          if (this.HasContentType(request, "application/json") && packageLength <= 104857600L)
          {
            tempFile = this.SaveJsonFileAsBinaryFile(tempFile);
            requestContext.Trace(12062010, TraceLevel.Error, "Gallery", nameof (FileUploadHandler), "Compat: converting JSON payload to binary file");
          }
          long length = new FileInfo(tempFile).Length;
          if (length > packageLength)
          {
            requestContext.Trace(12062010, TraceLevel.Error, "Gallery", nameof (FileUploadHandler), "Setting Content-Length header to {0} bytes", (object) length);
            requestContent.Headers.ContentLength = new long?(length);
          }
          request.Properties["Gallery_PackageFileName"] = (object) tempFile;
          requestContext.TraceLeave(12062010, "Gallery", nameof (FileUploadHandler), nameof (SendAsync));
          return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case IOException _:
            case TaskCanceledException _:
            case AggregateException _:
              num = 1;
              break;
            default:
              num = ex is JsonReaderException ? 1 : 0;
              break;
          }
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          if (File.Exists(tempFile))
            File.Delete(tempFile);
          if (ex is AggregateException)
          {
            foreach (Exception innerException in (ex as AggregateException).InnerExceptions)
              requestContext.TraceException(12062010, "Gallery", nameof (FileUploadHandler), innerException);
          }
          else
            requestContext.TraceException(12062010, "Gallery", nameof (FileUploadHandler), ex);
          throw;
        }
      }
    }

    private bool IsValidRequestMessage(HttpRequestMessage request)
    {
      bool flag1 = this.HasContentType(request, "application/octet-stream");
      int num = request.Method.Equals(HttpMethod.Post) ? 1 : (request.Method.Equals(HttpMethod.Put) ? 1 : 0);
      bool flag2 = this.HasContentType(request, "application/json");
      ApiResourceVersion apiResourceVersion = request.GetApiResourceVersion();
      if (num == 0)
        return false;
      if (flag1)
        return true;
      return this._allowJson & flag2 && apiResourceVersion.ResourceVersion != 1;
    }

    private bool HasContentType(HttpRequestMessage request, string matchContentType)
    {
      string contentType = this.GetContentType(request);
      return !contentType.IsNullOrEmpty<char>() && contentType.Equals(matchContentType);
    }

    private string GetContentType(HttpRequestMessage request)
    {
      string contentType = this.GetHeader((HttpHeaders) request.Content.Headers, "Content-Type");
      if (contentType != null)
      {
        if (contentType.Contains(";"))
          contentType = contentType.Split(';')[0];
        contentType = contentType.Trim();
      }
      return contentType;
    }

    private string GetHeader(HttpHeaders headers, string key)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return !headers.TryGetValues(key, out values) ? (string) null : values.First<string>();
    }

    private string SaveJsonFileAsBinaryFile(string tempFile)
    {
      string tempFileName = FileSpec.GetTempFileName();
      File.WriteAllBytes(tempFileName, Convert.FromBase64String(JsonUtilities.Deserialize<ExtensionPackage>(File.ReadAllText(tempFile)).ExtensionManifest));
      File.Delete(tempFile);
      return tempFileName;
    }
  }
}
