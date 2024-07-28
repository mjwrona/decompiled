// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ContentSizeException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [Serializable]
  public class ContentSizeException : Exception
  {
    public ContentSizeException()
    {
    }

    public ContentSizeException(string message)
      : base(message)
    {
    }

    public ContentSizeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ContentSizeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
