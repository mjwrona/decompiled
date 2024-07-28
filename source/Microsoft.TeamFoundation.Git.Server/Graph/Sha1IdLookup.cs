// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.Sha1IdLookup
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public class Sha1IdLookup : 
    ITwoWayReadOnlyList<Sha1Id>,
    IReadOnlyList<Sha1Id>,
    IReadOnlyCollection<Sha1Id>,
    IEnumerable<Sha1Id>,
    IEnumerable
  {
    private int[] m_trieData;
    private int m_nextTrieIndex;
    private Sha1Id[] m_intToSha1IdData;
    private int m_numLabels;
    private readonly object m_lock;
    private const int c_depthBits = 2;
    private const int c_depthPerByte = 4;
    private const int c_trieWidth = 4;
    private const int c_firstBranchWidth = 256;

    public Sha1IdLookup(int capacity = 128)
    {
      this.m_trieData = new int[256 + capacity * 4];
      this.m_nextTrieIndex = 256;
      this.m_intToSha1IdData = new Sha1Id[capacity];
      this.m_lock = new object();
    }

    internal Sha1IdLookup(Sha1Id[] ids, int numIds)
    {
      this.m_intToSha1IdData = ids;
      this.m_nextTrieIndex = 256;
      this.m_trieData = new int[256 + numIds * 4];
      this.m_lock = new object();
      for (int index = 0; index < numIds; ++index)
        this.Add(ref ids[index], false);
    }

    public bool Add(Sha1Id id)
    {
      lock (this.m_lock)
        return this.Add(ref id, true);
    }

    private bool Add(ref Sha1Id id, bool checkAndCopy)
    {
      int depth;
      int i;
      int index1 = this.NavigateIntoTrieData(ref id, out depth, out i);
      int involution = this.GetInvolution(i);
      if (i < 0 && involution < this.m_numLabels)
      {
        if (checkAndCopy && id == this.m_intToSha1IdData[involution])
          return false;
        int num1 = depth;
        int num2 = involution;
        int numLabels = this.m_numLabels;
        if (checkAndCopy)
        {
          this.EnsureSha1IdDataArrayCapacity(this.m_numLabels + 1);
          this.m_intToSha1IdData[numLabels] = id;
        }
        List<int> intList = new List<int>();
        intList.Add(index1);
        byte depthKey1;
        byte depthKey2;
        for (; (int) (depthKey1 = this.GetDepthKey(ref id, depth)) == (int) (depthKey2 = this.GetDepthKey(num2, depth)); ++depth)
        {
          index1 = this.m_nextTrieIndex;
          intList.Add(index1);
          this.m_nextTrieIndex += 4;
          this.EnsureTrieArrayCapacity(this.m_nextTrieIndex + 4 + 1);
        }
        this.m_trieData[index1 + (int) depthKey2] = this.GetInvolution(num2);
        this.m_trieData[index1 + (int) depthKey1] = this.GetInvolution(numLabels);
        for (int index2 = intList.Count - 2; index2 > 0; --index2)
          this.m_trieData[intList[index2] + (int) this.GetDepthKey(ref id, num1 + index2)] = intList[index2 + 1];
        this.m_trieData[intList[0]] = intList[1];
        ++this.m_numLabels;
        return true;
      }
      int numLabels1 = this.m_numLabels;
      if (checkAndCopy)
      {
        this.EnsureSha1IdDataArrayCapacity(this.m_numLabels + 1);
        this.m_intToSha1IdData[numLabels1] = id;
      }
      this.m_trieData[index1] = this.GetInvolution(numLabels1);
      ++this.m_numLabels;
      return true;
    }

    public Sha1Id GetValue(int label)
    {
      Sha1Id id;
      if (this.TryGetValue(label, out id))
        return id;
      throw new GraphLabelNotFoundException<int>(label);
    }

    public bool TryGetValue(int label, out Sha1Id id)
    {
      if (label < 0 || label >= this.m_numLabels)
      {
        id = Sha1Id.Empty;
        return false;
      }
      id = this.m_intToSha1IdData[label];
      return true;
    }

    public int GetIndex(Sha1Id id)
    {
      int label;
      if (this.TryGetIndex(id, out label))
        return label;
      throw new VertexNotFoundException<Sha1Id>(id);
    }

    public bool TryGetIndex(Sha1Id id, out int label)
    {
      int i;
      this.NavigateIntoTrieData(ref id, out int _, out i);
      label = this.GetInvolution(i);
      return label >= 0 && label < this.m_numLabels && id == this.m_intToSha1IdData[label];
    }

    public long GetSize() => (long) (CacheUtil.ObjectOverhead + IntPtr.Size + CacheUtil.ObjectOverhead + 4 * this.m_trieData.Length + 4 + IntPtr.Size + CacheUtil.ObjectOverhead + 20 * this.m_intToSha1IdData.Length + 4 + IntPtr.Size + CacheUtil.ObjectOverhead);

    public int Count => this.m_numLabels;

    public Sha1Id this[int index] => this.GetValue(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDepthByteIndex(int depth) => depth == 0 ? 0 : (depth - 1 >> 2) + 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetDepthKey(ref Sha1Id id, int depth)
    {
      int depthByteIndex = this.GetDepthByteIndex(depth);
      return this.GetBytePortion(id[depthByteIndex], depth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetDepthKey(int label, int depth)
    {
      int depthByteIndex = this.GetDepthByteIndex(depth);
      return this.GetBytePortion(this.m_intToSha1IdData[label][depthByteIndex], depth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetBytePortion(byte b, int depth)
    {
      if (depth == 0)
        return b;
      switch ((depth - 1) % 4)
      {
        case 0:
          return (byte) (((int) b & 192) >> 6);
        case 1:
          return (byte) (((int) b & 48) >> 4);
        case 2:
          return (byte) (((int) b & 12) >> 2);
        case 3:
          return (byte) ((uint) b & 3U);
        default:
          throw new InvalidOperationException();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NavigateIntoTrieData(ref Sha1Id id, out int depth, out int value)
    {
      int depth1 = 0;
      int index;
      for (index = (int) this.GetDepthKey(ref id, depth1); (value = this.m_trieData[index]) > 0; index = this.m_trieData[index] + (int) this.GetDepthKey(ref id, depth1))
        ++depth1;
      depth = depth1;
      return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureTrieArrayCapacity(int minTrieLength)
    {
      if (this.m_trieData.Length >= minTrieLength)
        return;
      int[] destinationArray = new int[(int) ((double) minTrieLength * 1.5)];
      Array.Copy((Array) this.m_trieData, (Array) destinationArray, this.m_trieData.Length);
      this.m_trieData = destinationArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureSha1IdDataArrayCapacity(int minIdLength)
    {
      if (this.m_intToSha1IdData.Length >= minIdLength)
        return;
      Sha1Id[] destinationArray = new Sha1Id[(int) ((double) minIdLength * 1.5)];
      Array.Copy((Array) this.m_intToSha1IdData, (Array) destinationArray, this.m_intToSha1IdData.Length);
      this.m_intToSha1IdData = destinationArray;
    }

    public IEnumerator<Sha1Id> GetEnumerator()
    {
      int numLabels = Volatile.Read(ref this.m_numLabels);
      for (int i = 0; i < numLabels; ++i)
        yield return this.m_intToSha1IdData[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    internal Sha1Id[] ObjectIds => this.m_intToSha1IdData;

    private int GetInvolution(int i) => -(i + 1);
  }
}
