// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobResponse
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public static class BlobResponse
  {
    internal static void ValidateCPKHeaders(
      HttpResponseMessage response,
      BlobRequestOptions options,
      bool upload)
    {
      if (options?.CustomerProvidedKey == null)
        return;
      if (!string.Equals(options.CustomerProvidedKey.KeySHA256, HttpResponseParsers.GetHeader(response, "x-ms-encryption-key-sha256"), StringComparison.OrdinalIgnoreCase))
        throw new StorageException("Hash returned from Client-Provided Key request did not match sent key's hash.");
      if (upload)
      {
        if (!string.Equals("true", HttpResponseParsers.GetHeader(response, "x-ms-request-server-encrypted"), StringComparison.OrdinalIgnoreCase))
          throw new StorageException("Error processing request with client provided key.");
      }
      else if (!string.Equals("true", HttpResponseParsers.GetHeader(response, "x-ms-server-encrypted"), StringComparison.OrdinalIgnoreCase))
        throw new StorageException("Error processing request with client provided key.");
    }
  }
}
