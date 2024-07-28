// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.AcsRetryHelper
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AcsRetryHelper : HttpRetryHelper
  {
    public AcsRetryHelper(int maxRetries)
      : base(maxRetries, AcsRetryHelper.\u003C\u003EO.\u003C0\u003E__CanRetryOnException ?? (AcsRetryHelper.\u003C\u003EO.\u003C0\u003E__CanRetryOnException = new Func<Exception, bool>(AcsRetryHelper.CanRetryOnException)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public static bool CanRetryOnException(Exception ex)
    {
      WebException webException = ex as WebException;
      HttpStatusCode httpStatusCode = (HttpStatusCode) 0;
      HttpWebResponse response = webException != null ? webException.Response as HttpWebResponse : (HttpWebResponse) null;
      if (response != null)
        httpStatusCode = response.StatusCode;
      return httpStatusCode == (HttpStatusCode) 429 || httpStatusCode == HttpStatusCode.InternalServerError || httpStatusCode == HttpStatusCode.BadGateway || httpStatusCode == HttpStatusCode.ServiceUnavailable || httpStatusCode == HttpStatusCode.GatewayTimeout;
    }
  }
}
