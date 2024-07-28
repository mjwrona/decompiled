// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.ObjectStoreHelpers
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint
{
  public static class ObjectStoreHelpers
  {
    public static void CopyBlob(
      byte[] blob,
      IFileWriter sink,
      CompressorAlgorithm inputCompressorAlgorithm,
      CompressorAlgorithm outputCompressorAlgorithm)
    {
      ArraySegment<byte> arraySegment;
      if (inputCompressorAlgorithm != outputCompressorAlgorithm)
      {
        if (inputCompressorAlgorithm != CompressorAlgorithm.None)
          blob = Compressor.Decompress(inputCompressorAlgorithm, blob);
        arraySegment = outputCompressorAlgorithm != CompressorAlgorithm.None ? Compressor.Compress(outputCompressorAlgorithm, blob) : new ArraySegment<byte>(blob);
      }
      else
        arraySegment = new ArraySegment<byte>(blob);
      sink.Write(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
    }

    public static void GetStatistics(
      IStorageEndpoint storageEndpoint,
      out long count,
      out long size)
    {
      count = 0L;
      size = 0L;
      foreach (IObjectStoreItem objectStoreItem in (IEnumerable<IObjectStoreItem>) storageEndpoint)
      {
        ++count;
        size += objectStoreItem.Size;
      }
    }
  }
}
