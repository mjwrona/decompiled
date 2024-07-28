// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.BlobNotFoundException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [Serializable]
  public class BlobNotFoundException : BlobServiceException
  {
    public BlobNotFoundException(string message)
      : base(message)
    {
    }

    public BlobNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public static BlobNotFoundException Create(string blobId) => new BlobNotFoundException(BlobNotFoundException.MakeMessage(blobId));

    private static string MakeMessage(string identifier) => string.Format(Resources.BlobNotFoundException((object) identifier));
  }
}
