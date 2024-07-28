// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupNotFoundException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [Serializable]
  public class DedupNotFoundException : BlobServiceException
  {
    public DedupNotFoundException(string message)
      : base(message)
    {
    }

    public DedupNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected DedupNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public static DedupNotFoundException Create(string dedupId) => new DedupNotFoundException(DedupNotFoundException.MakeMessage(dedupId));

    public static DedupNotFoundException Create(DedupIdentifier dedupId) => DedupNotFoundException.Create(dedupId.ValueString);

    private static string MakeMessage(string identifier) => string.Format(Resources.DedupNotFoundException((object) identifier));
  }
}
