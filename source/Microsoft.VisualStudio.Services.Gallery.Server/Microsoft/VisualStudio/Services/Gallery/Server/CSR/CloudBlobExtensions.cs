// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CSR.CloudBlobExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CSR
{
  public static class CloudBlobExtensions
  {
    public static string GetBlobUriWithSasToken(this ICloudBlob cloudBlob, int noOfDaysToExpiry = 30)
    {
      ArgumentUtility.CheckForNull<ICloudBlob>(cloudBlob, nameof (cloudBlob));
      SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
      {
        Permissions = SharedAccessBlobPermissions.Read,
        SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) DateTime.UtcNow.AddDays((double) noOfDaysToExpiry))
      };
      string sharedAccessSignature = cloudBlob.GetSharedAccessSignature(policy);
      return cloudBlob.Uri?.ToString() + sharedAccessSignature;
    }
  }
}
