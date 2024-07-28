// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentServiceRequestExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
      return request.UseStatusCodeFor429 && statusCode == 429;
    }
  }
}
