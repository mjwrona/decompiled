// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.HttpBuffer
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

using Microsoft.VisualStudio.Services.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  public class HttpBuffer
  {
    public HttpBuffer(int bufferSize)
    {
      ArgumentUtility.CheckForOutOfRange(bufferSize, nameof (bufferSize), 0);
      this.Bytes = new byte[bufferSize];
    }

    public int Length { get; private set; }

    public int BufferSize => this.Bytes.Length;

    public long StartPosition { get; private set; }

    public byte[] Bytes { get; private set; }

    public bool IsRangeInBuffer(long position, int count) => position >= this.StartPosition && position < checked (this.StartPosition + (long) this.Length) && checked (position + (long) count) <= checked (this.StartPosition + (long) this.Length);

    public bool IsPositionInBuffer(long position) => position >= this.StartPosition && position < checked (this.StartPosition + (long) this.Length);

    public async Task ReplaceContentFromStreamAsync(
      Stream responseStream,
      long newStartPosition,
      int expectedLength,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<Stream>(responseStream, nameof (responseStream));
      ArgumentUtility.CheckForOutOfRange(newStartPosition, nameof (newStartPosition), 0L);
      ArgumentUtility.CheckForOutOfRange(expectedLength, nameof (expectedLength), 0, this.BufferSize);
      this.StartPosition = newStartPosition;
      int num;
      for (this.Length = 0; this.Length < expectedLength; this.Length += num)
      {
        num = await responseStream.ReadAsync(this.Bytes, this.Length, expectedLength - this.Length, cancellationToken).ConfigureAwait(false);
        if (num == 0)
          throw new ServerResponseUnderflowException(expectedLength, this.Length);
      }
    }
  }
}
