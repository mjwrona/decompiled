// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.WebExceptionUtility
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
