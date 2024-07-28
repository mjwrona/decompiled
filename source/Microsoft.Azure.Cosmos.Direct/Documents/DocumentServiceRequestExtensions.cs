// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentServiceRequestExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents
{
  internal static class DocumentServiceRequestExtensions
  {
    public static bool IsValidStatusCodeForExceptionlessRetry(
      this DocumentServiceRequest request,
      int statusCode,
      SubStatusCodes subStatusCode = SubStatusCodes.Unknown)
    {
      if (request.UseStatusCodeForFailures)
      {
        switch (statusCode)
        {
          case 404:
            if (subStatusCode == SubStatusCodes.PartitionKeyRangeGone)
              break;
            goto case 409;
          case 409:
          case 412:
            return true;
        }
      }
      return request.UseStatusCodeFor429 && statusCode == 429 || request.UseStatusCodeForBadRequest && statusCode == 400 && subStatusCode != SubStatusCodes.PartitionKeyMismatch;
    }
  }
}
