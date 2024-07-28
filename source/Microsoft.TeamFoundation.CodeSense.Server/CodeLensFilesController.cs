// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WebAPI.CodeLensFilesController
// Assembly: Microsoft.TeamFoundation.CodeSense.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FD810605-AAA9-4CE8-B2C6-6B28A5D994C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Services;
using Microsoft.TeamFoundation.Framework.Server;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.TeamFoundation.CodeSense.Server.WebAPI
{
  public abstract class CodeLensFilesController : TfsApiController
  {
    protected HttpResponseMessage CreateResponseFromResult(VersionedContent result, bool compress = false)
    {
      HttpResponseMessage responseFromResult;
      if (result != null)
      {
        if (result.Modified)
        {
          compress = compress && this.Request.Headers.AcceptEncoding.Contains(new StringWithQualityHeaderValue("gzip"));
          responseFromResult = this.GetResponseMessage(result, compress);
          HttpContentHeaders headers = responseFromResult.Content.Headers;
          headers.ContentType = MediaTypeHeaderValue.Parse(string.Format("application/json; api-version=1.0; res-version={0}", (object) result.Version));
          if (compress)
            headers.ContentEncoding.Add("gzip");
        }
        else
          responseFromResult = this.Request.CreateResponse(HttpStatusCode.NotModified);
      }
      else
        responseFromResult = this.Request.CreateResponse(HttpStatusCode.NotFound);
      this.TfsRequestContext.Trace(1023020, TraceLayer.Service, "HTTP response status code: {0}", (object) (int) responseFromResult.StatusCode);
      return responseFromResult;
    }

    private HttpResponseMessage GetResponseMessage(VersionedContent result, bool compress)
    {
      byte[] content = (byte[]) null;
      if (compress)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          CodeLensFilesController.Compress(result, memoryStream);
          content = memoryStream.ToArray();
        }
      }
      return new HttpResponseMessage()
      {
        Content = compress ? (HttpContent) new ByteArrayContent(content) : (HttpContent) new StringContent(result.Content)
      };
    }

    private static void Compress(VersionedContent result, MemoryStream memoryStream)
    {
      using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Compress))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(result.Content);
        if (bytes == null || bytes.Length == 0)
          return;
        gzipStream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
