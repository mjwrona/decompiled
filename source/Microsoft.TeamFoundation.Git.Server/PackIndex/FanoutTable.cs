// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.FanoutTable
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal struct FanoutTable
  {
    private readonly uint[] m_fanout;
    public const int ByteLength = 1024;

    private FanoutTable(uint[] fanout) => this.m_fanout = fanout;

    public static FanoutTable FromStream(Stream stream)
    {
      byte[] buf = new byte[4];
      uint[] fanout = new uint[256];
      for (int index = 0; index < 256; ++index)
      {
        fanout[index] = GitStreamUtil.TryReadGreedy(stream, buf, 0, 4) >= 4 ? BitConverter.ToUInt32(buf, 0) : throw new InvalidGitIndexException();
        if (index > 0 && fanout[index] < fanout[index - 1])
          throw new InvalidGitIndexException();
      }
      return new FanoutTable(fanout);
    }

    public static void ToStream(IReadOnlyList<Sha1Id> objectIds, Stream stream)
    {
      byte[] objectId = new byte[20];
      int lo = 0;
      for (int index = 1; index < 256; ++index)
      {
        objectId[0] = (byte) index;
        lo = FanoutTable.FindFanoutValue(objectIds, objectId, lo);
        stream.Write(BitConverter.GetBytes(lo), 0, 4);
      }
      stream.Write(BitConverter.GetBytes(objectIds.Count), 0, 4);
    }

    private static int FindFanoutValue(IReadOnlyList<Sha1Id> objectIds, byte[] objectId, int lo)
    {
      int num1 = objectIds.Count - 1;
      while (lo <= num1)
      {
        int index = lo + (num1 - lo >> 1);
        int num2 = objectIds[index].CompareTo(objectId);
        if (num2 == 0)
          return index;
        if (num2 < 0)
          lo = index + 1;
        else
          num1 = index - 1;
      }
      return lo;
    }

    public uint GetLowerBound(Sha1Id objectId) => objectId[0] != (byte) 0 ? this.m_fanout[(int) objectId[0] - 1] : 0U;

    public long GetUpperBound(Sha1Id objectId) => (long) this.m_fanout[(int) objectId[0]] - 1L;

    public uint EntryCount => this.m_fanout[(int) byte.MaxValue];
  }
}
