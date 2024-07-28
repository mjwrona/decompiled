// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobProviderExtentions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class BlobProviderExtentions
  {
    public static Task<EtagValue<bool>> PutBlobByteArrayAsync(
      this IBlobProvider provider,
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      byte[] data,
      bool useHttpClient)
    {
      return provider.PutBlobByteArrayAsync(processor, blobId, etagToMatch, new ArraySegment<byte>(data), useHttpClient);
    }

    public static Task PutBlobBlockByteArrayAsync(
      this IBlobProvider provider,
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      byte[] blobBlock,
      string blockName,
      bool useHttpClient)
    {
      return provider.PutBlobBlockByteArrayAsync(processor, blobId, new ArraySegment<byte>(blobBlock), blockName, useHttpClient);
    }
  }
}
