// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.HashedBlobWriter
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint
{
  public class HashedBlobWriter : HashedBlob
  {
    public void Write(IFileWriter stream, ArraySegment<byte> blob, bool isCompressed)
    {
      byte[] hash = HashedBlob.ComputeHash(blob);
      stream.Write(3138453691U);
      stream.Write(blob.Count);
      stream.Write(hash, 0, hash.Length);
      stream.Write(isCompressed);
      stream.Write(blob.Array, blob.Offset, blob.Count);
    }
  }
}
