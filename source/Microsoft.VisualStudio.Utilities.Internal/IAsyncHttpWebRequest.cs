// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.IAsyncHttpWebRequest
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System.Collections.Generic;
using System.Net.Cache;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public interface IAsyncHttpWebRequest
  {
    string Url { get; }

    string Method { get; set; }

    int Timeout { get; set; }

    RequestCachePolicy CachePolicy { get; set; }

    string ContentType { get; set; }

    void AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);

    Task<IStreamedHttpWebResponse> GetResponseAsync();
  }
}
