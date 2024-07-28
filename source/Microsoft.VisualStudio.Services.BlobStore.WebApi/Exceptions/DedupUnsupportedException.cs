// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions.DedupUnsupportedException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions
{
  public class DedupUnsupportedException : Exception
  {
    public DedupUnsupportedException(string message)
      : base(message)
    {
    }

    public DedupUnsupportedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected DedupUnsupportedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
