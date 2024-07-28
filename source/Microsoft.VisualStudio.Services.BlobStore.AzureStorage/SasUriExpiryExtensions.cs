// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.SasUriExpiryExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public static class SasUriExpiryExtensions
  {
    public static SharedAccessBlobPolicy GetSASPolicy(
      this SASUriExpiry expiry,
      DateTimeOffset expiryTime,
      TimeSpan boost)
    {
      DateTimeOffset now = expiry.Clock.Now;
      TimeSpan timeSpan = expiryTime - now;
      if (timeSpan < TimeSpan.Zero)
        throw new ArgumentException(string.Format("{0} {1} is negative ({2} {3} - {4} {5})", (object) "expiryPeriod", (object) timeSpan, (object) nameof (expiryTime), (object) expiryTime, (object) "now", (object) now));
      if (timeSpan > expiry.Bounds.MaxExpiry)
        throw new ArgumentException(string.Format("{0} {1} exceeds the maximum allowed of {2}", (object) "expiryPeriod", (object) timeSpan, (object) expiry.Bounds.MaxExpiry));
      if (timeSpan < SASUriExpiry.ClockSkewTime)
        throw new ArgumentException(string.Format("{0} {1} is below the minimum allowed of {2}", (object) "expiryPeriod", (object) timeSpan, (object) SASUriExpiry.ClockSkewTime));
      expiryTime += boost;
      return new SharedAccessBlobPolicy()
      {
        SharedAccessExpiryTime = new DateTimeOffset?(expiryTime)
      };
    }
  }
}
