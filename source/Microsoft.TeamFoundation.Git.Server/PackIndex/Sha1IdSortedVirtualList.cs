// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.Sha1IdSortedVirtualList
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal sealed class Sha1IdSortedVirtualList : 
    VirtualReadOnlyListBase<Sha1Id>,
    ISha1IdTwoWayReadOnlyList,
    ITwoWayReadOnlyList<Sha1Id>,
    IReadOnlyList<Sha1Id>,
    IReadOnlyCollection<Sha1Id>,
    IEnumerable<Sha1Id>,
    IEnumerable
  {
    private readonly byte[] m_buf;
    private readonly Stream m_stream;
    private readonly FanoutTable m_fanout;
    private const int c_linearThreshold = 16;

    public Sha1IdSortedVirtualList(Stream stream, FanoutTable fanout)
    {
      this.m_buf = new byte[320];
      this.m_stream = stream;
      this.Count = checked ((int) unchecked (this.m_stream.Length / 20L));
      this.m_fanout = fanout;
    }

    public override int Count { get; }

    protected override Sha1Id DoGet(int index)
    {
      this.m_stream.Seek((long) (index * 20), SeekOrigin.Begin);
      GitStreamUtil.ReadGreedy(this.m_stream, this.m_buf, 0, 20);
      return new Sha1Id(this.m_buf);
    }

    public bool TryGetIndex(Sha1Id objectId, out int index)
    {
      long loAll = (long) this.m_fanout.GetLowerBound(objectId);
      long num1 = this.m_fanout.GetUpperBound(objectId);
      Sha1Id sha1Id1 = Sha1Id.Empty;
      Sha1Id sha1Id2 = Sha1Id.Maximum;
      int index1 = 2;
      int num2 = (int) objectId[index1 - 2] << 16;
      int num3 = num2;
      int num4 = num2 + (int) ushort.MaxValue;
      int num5 = num2 + (((int) objectId[index1 - 1] << 8) + (int) objectId[index1]);
      while (loAll <= num1)
      {
        long width = num1 - loAll;
        if (width < 16L)
          return this.TryGetIndexLinear(objectId, (int) loAll, (int) width, out index);
        for (; index1 < 20 && num4 - num3 < (int) byte.MaxValue; num4 = (num4 << 8 & 16777215) + (int) sha1Id2[index1])
        {
          ++index1;
          num3 = (num3 << 8 & 16777215) + (int) sha1Id1[index1];
          num5 = (num5 << 8 & 16777215) + (int) objectId[index1];
        }
        long num6 = num3 != num4 ? loAll + (num1 - loAll) * (long) (num5 - num3) / (long) (num4 - num3) : loAll + (num1 - loAll >> 1);
        try
        {
          this.m_stream.Seek(num6 * 20L, SeekOrigin.Begin);
          GitStreamUtil.ReadGreedy(this.m_stream, this.m_buf, 0, 20);
        }
        catch (Exception ex)
        {
          throw new InvalidGitIndexException(ex);
        }
        Sha1Id sha1Id3 = new Sha1Id(this.m_buf);
        int num7 = sha1Id3.CompareTo(objectId);
        if (num7 == 0)
        {
          index = (int) num6;
          return true;
        }
        if (num7 < 0)
        {
          loAll = num6 + 1L;
          if (index1 < 20)
          {
            sha1Id1 = sha1Id3;
            num3 = ((int) sha1Id1[index1 - 2] << 16) + ((int) sha1Id1[index1 - 1] << 8) + (int) sha1Id1[index1];
          }
        }
        else
        {
          num1 = num6 - 1L;
          if (index1 < 20)
          {
            sha1Id2 = sha1Id3;
            num4 = ((int) sha1Id2[index1 - 2] << 16) + ((int) sha1Id2[index1 - 1] << 8) + (int) sha1Id2[index1];
          }
        }
      }
      index = (int) loAll;
      return false;
    }

    private bool TryGetIndexLinear(Sha1Id objectId, int loAll, int width, out int index)
    {
      try
      {
        this.m_stream.Seek((long) (loAll * 20), SeekOrigin.Begin);
        GitStreamUtil.ReadGreedy(this.m_stream, this.m_buf, 0, (width + 1) * 20);
      }
      catch (Exception ex)
      {
        throw new InvalidGitIndexException(ex);
      }
      int num1 = 0;
      int num2 = width;
      while (num1 <= num2)
      {
        int num3 = num1 + (num2 - num1 >> 1);
        int num4 = new Sha1Id(this.m_buf, num3 * 20).CompareTo(objectId);
        if (num4 == 0)
        {
          index = loAll + num3;
          return true;
        }
        if (num4 < 0)
          num1 = num3 + 1;
        else
          num2 = num3 - 1;
      }
      index = loAll + num1;
      return false;
    }

    public IEnumerable<Sha1Id> FindBetween(Sha1Id min, Sha1Id max)
    {
      Sha1IdSortedVirtualList sortedVirtualList = this;
      int index;
      sortedVirtualList.TryGetIndex(min, out index);
      int maxIndex;
      if (sortedVirtualList.TryGetIndex(max, out maxIndex))
        ++maxIndex;
      for (int i = index; i < maxIndex; ++i)
        yield return sortedVirtualList.DoGet(i);
    }
  }
}
