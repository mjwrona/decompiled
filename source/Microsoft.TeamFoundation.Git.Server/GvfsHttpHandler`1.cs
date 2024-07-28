// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsHttpHandler`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal abstract class GvfsHttpHandler<TRequest> : GvfsHttpHandler
  {
    private const string c_plainTextContentType = "text/plain";

    public GvfsHttpHandler()
    {
    }

    public GvfsHttpHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected virtual int MaxRequestBytes => 5242880;

    internal abstract void ProcessPost(RepoNameKey nameKey, TRequest toProcess);

    internal override sealed void ProcessPost(RepoNameKey nameKey)
    {
      HttpRequestBase request = this.HandlerHttpContext.Request;
      Stream bufferlessInputStream = request.GetBufferlessInputStream();
      GvfsHttpHandler<TRequest>.DecompressInputIfNecessary(request, ref bufferlessInputStream);
      if (bufferlessInputStream.Length > (long) this.MaxRequestBytes)
      {
        HttpResponseBase response = this.HandlerHttpContext.Response;
        response.StatusCode = 413;
        response.ContentType = "text/plain";
        response.Write("The request body is too large.");
      }
      else
      {
        TRequest toProcess;
        using (StreamReader streamReader = new StreamReader(bufferlessInputStream))
        {
          try
          {
            toProcess = JsonConvert.DeserializeObject<TRequest>(streamReader.ReadToEnd());
          }
          catch (Exception ex) when (
          {
            // ISSUE: unable to correctly present filter
            int num;
            switch (ex)
            {
              case JsonSerializationException _:
              case ArgumentNullException _:
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
            this.WriteBadRequestResponse();
            return;
          }
        }
        this.ProcessPost(nameKey, toProcess);
      }
    }

    protected void WriteBadRequestResponse()
    {
      HttpResponseBase response = this.HandlerHttpContext.Response;
      response.StatusCode = 400;
      response.ContentType = "text/plain";
      response.Write("The request body could not be deserialized.");
    }

    private static void DecompressInputIfNecessary(HttpRequestBase request, ref Stream inputStream)
    {
      string header = request.Headers["Content-Encoding"];
      if (string.IsNullOrEmpty(header) || !string.Equals(header, "gzip", StringComparison.OrdinalIgnoreCase))
        return;
      inputStream = (Stream) new GZipStream(inputStream, CompressionMode.Decompress, true);
    }
  }
}
