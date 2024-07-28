// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient.GitEndPointResponseData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.IO;
using System.Net;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient
{
  internal class GitEndPointResponseData
  {
    private const string LooseObjectContentType = "application/x-git-loose-object";
    private const string PackFileContentType = "application/x-git-packfile";
    private const string ApplicationJson = "application/json";
    private HttpStatusCode m_statusCode;

    public GitEndPointResponseData(HttpStatusCode statusCode, string error, bool shouldRetry)
    {
      this.m_statusCode = statusCode;
      this.ErrorMessage = error;
      this.ShouldRetry = shouldRetry;
    }

    public GitEndPointResponseData(
      HttpStatusCode statusCode,
      string contentType,
      Stream responseStream)
      : this(statusCode, string.Empty, false)
    {
      this.ContentType = contentType;
      this.Stream = responseStream;
    }

    public string ContentType { get; private set; }

    public Stream Stream { get; }

    public string ErrorMessage { get; }

    public bool ShouldRetry { get; }

    public bool HasErrors => this.m_statusCode != HttpStatusCode.OK;

    public bool ContainsLooseObject => this.ContentType.Equals("application/x-git-loose-object", StringComparison.OrdinalIgnoreCase);

    public bool ContainsPackFile => this.ContentType.Equals("application/x-git-packfile", StringComparison.OrdinalIgnoreCase);

    public bool ContainsApplicationJson => this.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase);
  }
}
