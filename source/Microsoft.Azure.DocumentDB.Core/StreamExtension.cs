// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StreamExtension
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class StreamExtension
  {
    public static async Task CopyToAsync(
      this Stream srcStream,
      Stream destinationStream,
      long maxSizeToCopy = 9223372036854775807)
    {
      if (srcStream == null)
        throw new ArgumentNullException(nameof (srcStream));
      if (destinationStream == null)
        throw new ArgumentNullException(nameof (destinationStream));
      byte[] buffer = new byte[1024];
      long numberOfBytesRead = 0;
      while (true)
      {
        int count = await srcStream.ReadAsync(buffer, 0, 1024);
        if (count > 0)
        {
          numberOfBytesRead += (long) count;
          if (numberOfBytesRead <= maxSizeToCopy)
            await destinationStream.WriteAsync(buffer, 0, count);
          else
            break;
        }
        else
          goto label_10;
      }
      throw new RequestEntityTooLargeException(RMResources.RequestTooLarge);
label_10:;
    }

    public static Task<CloneableStream> AsClonableStreamAsync(Stream mediaStream) => StreamExtension.CopyStreamAndReturnAsync(mediaStream);

    private static async Task<CloneableStream> CopyStreamAndReturnAsync(Stream mediaStream)
    {
      MemoryStream memoryStreamClone = new MemoryStream();
      if (mediaStream.CanSeek)
        mediaStream.Position = 0L;
      await mediaStream.CopyToAsync((Stream) memoryStreamClone);
      memoryStreamClone.Position = 0L;
      return new CloneableStream(memoryStreamClone);
    }
  }
}
