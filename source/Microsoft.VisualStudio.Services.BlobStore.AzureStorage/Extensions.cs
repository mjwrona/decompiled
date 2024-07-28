// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Extensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal static class Extensions
  {
    public static bool IsCompressed(this ICloudBlockBlob blob)
    {
      switch (blob.Properties.ContentEncoding)
      {
        case "xpress":
          return true;
        case "none":
          return false;
        default:
          throw new ArgumentException("Unknown Content-Encoding:" + blob.Properties.ContentEncoding);
      }
    }
  }
}
