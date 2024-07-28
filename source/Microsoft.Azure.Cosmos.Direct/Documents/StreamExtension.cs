// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StreamExtension
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
label_10:
      buffer = (byte[]) null;
    }

    public static MemoryStream CreateExportableMemoryStream(byte[] body) => new MemoryStream(body, 0, body.Length, false, true);

    public static Task<CloneableStream> AsClonableStreamAsync(Stream mediaStream) => mediaStream is MemoryStream internalStream && internalStream.TryGetBuffer(out ArraySegment<byte> _) ? Task.FromResult<CloneableStream>(new CloneableStream(internalStream)) : StreamExtension.CopyStreamAndReturnAsync(mediaStream);

    private static async Task<CloneableStream> CopyStreamAndReturnAsync(Stream mediaStream)
    {
      MemoryStream memoryStreamClone = new MemoryStream();
      if (mediaStream.CanSeek)
        mediaStream.Position = 0L;
      await mediaStream.CopyToAsync((Stream) memoryStreamClone);
      memoryStreamClone.Position = 0L;
      CloneableStream cloneableStream = new CloneableStream(memoryStreamClone);
      memoryStreamClone = (MemoryStream) null;
      return cloneableStream;
    }
  }
}
