// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.IHttpWebRequest
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal interface IHttpWebRequest
  {
    string Url { get; }

    string Method { get; set; }

    RequestCachePolicy CachePolicy { get; set; }

    string ContentType { get; set; }

    long ContentLength { get; set; }

    bool AllowAutoRedirect { get; set; }

    void AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);

    Task<IHttpWebResponse> GetResponseAsync(CancellationToken token);

    Task<Stream> GetRequestStreamAsync(CancellationToken token);
  }
}
