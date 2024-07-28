// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.HttpContentFactory
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class HttpContentFactory
  {
    public static HttpContent BuildContentFromStream<T>(
      Stream stream,
      long offset,
      long? length,
      Checksum checksum,
      RESTCommand<T> cmd,
      OperationContext operationContext)
    {
      stream.Seek(offset, SeekOrigin.Begin);
      stream = stream.WrapWithByteCountingStream(cmd.CurrentResult, true);
      HttpContent httpContent = (HttpContent) new StreamContent((Stream) new NonCloseableStream(stream));
      httpContent.Headers.ContentLength = length;
      if (checksum != null && checksum.MD5 != null)
        httpContent.Headers.ContentMD5 = Convert.FromBase64String(checksum.MD5);
      if (checksum != null && checksum.CRC64 != null)
        httpContent.Headers.Add("x-ms-content-crc64", checksum.CRC64);
      return httpContent;
    }
  }
}
