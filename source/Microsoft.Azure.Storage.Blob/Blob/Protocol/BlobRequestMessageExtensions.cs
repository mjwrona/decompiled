// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobRequestMessageExtensions
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Shared.Protocol;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  internal static class BlobRequestMessageExtensions
  {
    internal static void ApplyBlobContentChecksumHeaders(
      this StorageRequestMessage request,
      Checksum blobContentChecksum)
    {
      request.AddOptionalHeader("x-ms-blob-content-md5", blobContentChecksum?.MD5);
      request.AddOptionalHeader("x-ms-blob-content-crc64", blobContentChecksum?.CRC64);
    }
  }
}
