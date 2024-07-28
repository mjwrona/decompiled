// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.HttpHeadersExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class HttpHeadersExtensions
  {
    public static bool TryGetSingleValue(
      this HttpHeaders headers,
      string name,
      out string value,
      bool splitOnComma = true)
    {
      IEnumerable<string> values;
      if (!headers.TryGetValues(name, out values) || !values.Any<string>())
      {
        value = (string) null;
        return false;
      }
      if (values.Count<string>() == 1 & splitOnComma)
        values = (IEnumerable<string>) values.Single<string>().Split(',');
      if (values.Count<string>() > 1)
      {
        value = (string) null;
        return false;
      }
      value = values.Single<string>();
      return true;
    }
  }
}
