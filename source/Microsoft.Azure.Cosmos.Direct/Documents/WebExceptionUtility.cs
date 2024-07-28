// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.WebExceptionUtility
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Net;

namespace Microsoft.Azure.Documents
{
  internal static class WebExceptionUtility
  {
    public static bool IsWebExceptionRetriable(Exception ex)
    {
      for (Exception ex1 = ex; ex1 != null; ex1 = ex1.InnerException)
      {
        if (WebExceptionUtility.IsWebExceptionRetriableInternal(ex1))
          return true;
      }
      return false;
    }

    private static bool IsWebExceptionRetriableInternal(Exception ex) => ex is WebException webException && (webException.Status == WebExceptionStatus.ConnectFailure || webException.Status == WebExceptionStatus.NameResolutionFailure || webException.Status == WebExceptionStatus.ProxyNameResolutionFailure || webException.Status == WebExceptionStatus.SecureChannelFailure || webException.Status == WebExceptionStatus.TrustFailure);
  }
}
