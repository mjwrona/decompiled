// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileRequestMessageExtensions
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Shared.Protocol;

namespace Microsoft.Azure.Storage.File.Protocol
{
  internal static class FileRequestMessageExtensions
  {
    internal static void ApplyFileContentChecksumHeaders(
      this StorageRequestMessage request,
      Checksum fileContentChecksum)
    {
      request.AddOptionalHeader("x-ms-content-md5", fileContentChecksum?.MD5);
      request.AddOptionalHeader("x-ms-content-crc64", fileContentChecksum?.CRC64);
    }
  }
}
