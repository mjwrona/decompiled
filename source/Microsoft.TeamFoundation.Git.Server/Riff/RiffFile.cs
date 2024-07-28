// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Riff.RiffFile
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Riff
{
  internal sealed class RiffFile : RiffList, IDisposable
  {
    private readonly bool m_leaveOpen;

    private RiffFile(uint id, Stream stream, bool leaveOpen, bool longOffsets = false)
      : base(id, stream, longOffsets)
    {
      this.m_leaveOpen = leaveOpen;
    }

    public static bool TryLoad(Stream stream, out RiffFile riff, bool leaveOpen)
    {
      GitServerUtils.CheckIsLittleEndian();
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      riff = (RiffFile) null;
      byte[] buf = new byte[12];
      if (GitStreamUtil.TryReadGreedy(stream, buf, 0, 8) != 8)
        return false;
      uint uint32 = BitConverter.ToUInt32(buf, 0);
      bool longOffsets;
      Stream stream1;
      switch (uint32)
      {
        case 843467090:
          longOffsets = true;
          int count = 4;
          if (GitStreamUtil.TryReadGreedy(stream, buf, 8, count) != count)
            return false;
          stream1 = (Stream) new GitRestrictedStream(stream, stream.Position, BitConverter.ToInt64(buf, 4), leaveOpen);
          break;
        case 1179011410:
          longOffsets = false;
          stream1 = (Stream) new GitRestrictedStream(stream, stream.Position, (long) BitConverter.ToUInt32(buf, 4), leaveOpen);
          break;
        default:
          return false;
      }
      riff = new RiffFile(uint32, stream1, leaveOpen, longOffsets);
      return true;
    }

    public void Dispose()
    {
      if (this.m_leaveOpen)
        return;
      this.Stream.Dispose();
    }
  }
}
