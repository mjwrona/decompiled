// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Riff.RiffList
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Riff
{
  internal class RiffList : RiffChunk, IEnumerable<RiffChunk>, IEnumerable
  {
    private readonly byte[] m_buf;
    private readonly Lazy<uint> m_listType;
    private readonly bool m_longOffsets;

    public RiffList(uint id, Stream stream, bool longOffsets)
      : base(id, stream)
    {
      this.m_buf = new byte[12];
      this.m_listType = new Lazy<uint>(new Func<uint>(this.GetListType), false);
      this.m_longOffsets = longOffsets;
    }

    public uint ListType => this.m_listType.Value;

    private uint GetListType()
    {
      this.Stream.Position = 0L;
      GitStreamUtil.ReadGreedy(this.Stream, this.m_buf, 0, 4);
      return BitConverter.ToUInt32(this.m_buf, 0);
    }

    public IEnumerator<RiffChunk> GetEnumerator()
    {
      RiffList riffList = this;
      long position = 4;
      int width = riffList.m_longOffsets ? 12 : 8;
      while (true)
      {
        riffList.Stream.Position = position;
        int num1 = GitStreamUtil.TryReadGreedy(riffList.Stream, riffList.m_buf, 0, width);
        if (num1 != 0)
        {
          if (num1 == width)
          {
            position += (long) width;
            uint uint32 = BitConverter.ToUInt32(riffList.m_buf, 0);
            long offset = position;
            long length = riffList.m_longOffsets ? BitConverter.ToInt64(riffList.m_buf, 4) : (long) BitConverter.ToUInt32(riffList.m_buf, 4);
            int num2 = (int) (length % 2L);
            position += length + (long) num2;
            Stream stream = (Stream) new GitRestrictedStream(riffList.Stream, offset, length, true);
            yield return uint32 == 1414744396U ? (RiffChunk) new RiffList(uint32, stream, riffList.m_longOffsets) : new RiffChunk(uint32, stream);
          }
          else
            goto label_3;
        }
        else
          break;
      }
      yield break;
label_3:
      throw new EndOfStreamException();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
