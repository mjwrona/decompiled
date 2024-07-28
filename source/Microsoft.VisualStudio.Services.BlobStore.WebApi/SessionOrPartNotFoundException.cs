// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.SessionOrPartNotFoundException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [Serializable]
  public class SessionOrPartNotFoundException : VssServiceException
  {
    public SessionOrPartNotFoundException(string message)
      : base(message)
    {
    }

    public SessionOrPartNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected SessionOrPartNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
