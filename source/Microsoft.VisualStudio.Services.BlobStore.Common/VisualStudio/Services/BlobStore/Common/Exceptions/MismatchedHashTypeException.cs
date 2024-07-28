// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions.MismatchedHashTypeException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions
{
  [Serializable]
  public class MismatchedHashTypeException : InvalidOperationException
  {
    public MismatchedHashTypeException(bool isChunked, HashType hashType)
      : this(Resources.MismatchedChunkedStatusToHashTypeException((object) isChunked, (object) hashType.ToString()))
    {
    }

    public MismatchedHashTypeException(string message)
      : base(message)
    {
    }
  }
}
