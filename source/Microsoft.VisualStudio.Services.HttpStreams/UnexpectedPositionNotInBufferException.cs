// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.UnexpectedPositionNotInBufferException
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

using System.IO;

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  public class UnexpectedPositionNotInBufferException : IOException
  {
    public UnexpectedPositionNotInBufferException(string message)
      : base(message)
    {
    }

    public UnexpectedPositionNotInBufferException(long position, HttpBuffer streamBuffer)
      : this(UnexpectedPositionNotInBufferException.MakeMessage(position, streamBuffer))
    {
      this.Position = position;
      this.StreamBuffer = streamBuffer;
    }

    public long Position { get; private set; }

    public HttpBuffer StreamBuffer { get; private set; }

    private static string MakeMessage(long position, HttpBuffer streamBuffer) => streamBuffer == null ? Resources.Error_PositionNotInBuffer_NullBuffer((object) position) : Resources.Error_PositionNotInBuffer((object) position, (object) streamBuffer.StartPosition, (object) streamBuffer.Length);
  }
}
