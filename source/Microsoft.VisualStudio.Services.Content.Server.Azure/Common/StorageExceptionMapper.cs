// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Common.StorageExceptionMapper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Common
{
  public static class StorageExceptionMapper
  {
    public static HttpStatusCode MapStatusCode(int? code)
    {
      if (code.HasValue)
      {
        switch (code.GetValueOrDefault())
        {
          case 0:
            return HttpStatusCode.ServiceUnavailable;
          case 306:
            return HttpStatusCode.ServiceUnavailable;
          case 408:
            return HttpStatusCode.ServiceUnavailable;
          case 409:
            return HttpStatusCode.Conflict;
          case 429:
            return HttpStatusCode.ServiceUnavailable;
          case 500:
            return HttpStatusCode.ServiceUnavailable;
          case 502:
            return HttpStatusCode.ServiceUnavailable;
          case 503:
            return HttpStatusCode.ServiceUnavailable;
          case 504:
            return HttpStatusCode.ServiceUnavailable;
        }
      }
      return HttpStatusCode.InternalServerError;
    }

    public static bool IsExpectedResponseCode(HttpStatusCode code) => code < HttpStatusCode.InternalServerError || code == HttpStatusCode.ServiceUnavailable;
  }
}
