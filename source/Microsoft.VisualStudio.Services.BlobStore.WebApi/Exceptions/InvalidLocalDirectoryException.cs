// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions.InvalidLocalDirectoryException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions
{
  [Obsolete("This Exception is Obselete, use InvalidPathException")]
  public class InvalidLocalDirectoryException : Exception
  {
    public InvalidLocalDirectoryException(string message)
      : base(message)
    {
    }

    public InvalidLocalDirectoryException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvalidLocalDirectoryException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
