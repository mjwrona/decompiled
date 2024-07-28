// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GitCommitGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Riff;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class GitCommitGraph : 
    DirectedGraphWrapper<int, Sha1Id>,
    IGitCommitGraph,
    IDirectedGraph<int, Sha1Id>,
    IVertexSet<int, Sha1Id>,
    IPackedBloomFilters
  {
    private readonly IntSha1IdGraph m_shaGraph;
    private byte[] m_packedCommitTimes;
    private Sha1Id[] m_rootTreeIds;
    private byte[] m_generations;
    private readonly int c_bytesPerCommitTime = 5;

    public GitCommitGraph(
      int capacity = 128,
      int numObjectsAtSerialization = 0,
      PackedBloomFilters changedPathFilter = null)
      : this(new IntSha1IdGraph(capacity), capacity, numObjectsAtSerialization, changedPathFilter)
    {
    }

    public GitCommitGraph(
      IntSha1IdGraph graph,
      int capacity = 128,
      int numObjectsAtSerialization = 0,
      PackedBloomFilters changedPathFilter = null)
      : base((IDirectedGraph<int, Sha1Id>) graph)
    {
      int length = Math.Max(graph.NumVertices, capacity);
      this.m_shaGraph = graph;
      this.m_packedCommitTimes = new byte[length * this.c_bytesPerCommitTime];
      this.m_rootTreeIds = new Sha1Id[length];
      this.NumObjectsAtSerialization = numObjectsAtSerialization;
      if (changedPathFilter != null)
        this.ChangedPathFilters = changedPathFilter;
      else
        this.ChangedPathFilters = new PackedBloomFilters();
    }

    internal GitCommitGraph(
      Sha1Id[] vertices,
      int numVertices,
      int[] edges,
      int[] depths,
      Sha1Id[] trees,
      byte[] commitTimes,
      int numObjectsAtSerialization,
      PackedBloomFilters changedPathFilters = null,
      byte[] generations = null)
      : base((IDirectedGraph<int, Sha1Id>) new IntSha1IdGraph(vertices, numVertices, edges, depths))
    {
      this.m_shaGraph = (IntSha1IdGraph) this.m_graph;
      this.m_packedCommitTimes = commitTimes;
      this.m_rootTreeIds = trees;
      this.NumObjectsAtSerialization = numObjectsAtSerialization;
      this.ChangedPathFilters = changedPathFilters ?? new PackedBloomFilters();
      this.m_generations = generations;
    }

    public bool AddVertex(Sha1Id vertex, IEnumerable<Sha1Id> neighbors = null) => this.m_shaGraph.AddVertex(vertex, neighbors);

    public void SetAncestryDepth(int label, int depth) => this.m_shaGraph.SetAncestryDepth(label, depth);

    public void SetCommitTime(int label, DateTime commitTime) => this.SetCommitTime(label, (long) commitTime.Subtract(GitServerConstants.UtcEpoch).TotalSeconds);

    public void SetCommitTime(int label, long secondsSinceEpoch)
    {
      if (label < 0)
        throw new GraphLabelNotFoundException<int>(label);
      if ((label + 1) * this.c_bytesPerCommitTime > this.m_packedCommitTimes.Length)
        Array.Resize<byte>(ref this.m_packedCommitTimes, Math.Max(this.m_packedCommitTimes.Length * 2, (label + 1) * this.c_bytesPerCommitTime));
      this.SetCommitTimeToArray(label, secondsSinceEpoch);
    }

    private void SetCommitTimeToArray(int label, long value)
    {
      for (int index = 0; index < this.c_bytesPerCommitTime; ++index)
        this.m_packedCommitTimes[this.c_bytesPerCommitTime * label + index] = (byte) ((value & (long) byte.MaxValue << 8 * index) >> 8 * index);
    }

    private long GetCommitTimeFromArray(int label)
    {
      long commitTimeFromArray = 0;
      for (int index = 0; index < this.c_bytesPerCommitTime; ++index)
        commitTimeFromArray |= (long) this.m_packedCommitTimes[this.c_bytesPerCommitTime * label + index] << 8 * index;
      return commitTimeFromArray;
    }

    public long GetCommitTime(int label) => this.m_shaGraph.HasLabel(label) ? this.GetCommitTimeFromArray(label) : throw new GraphLabelNotFoundException<int>(label);

    public void SetRootTreeId(int label, Sha1Id id)
    {
      if (label < 0)
        throw new GraphLabelNotFoundException<int>(label);
      if (label >= this.m_rootTreeIds.Length)
        Array.Resize<Sha1Id>(ref this.m_rootTreeIds, Math.Max(this.m_rootTreeIds.Length * 2, label + 1));
      this.m_rootTreeIds[label] = id;
    }

    public Sha1Id GetRootTreeId(int label) => this.m_shaGraph.HasLabel(label) ? this.m_rootTreeIds[label] : throw new GraphLabelNotFoundException<int>(label);

    internal int NumObjectsAtSerialization { get; set; }

    public long GetSize() => (long) (CacheUtil.ObjectOverhead + 4 + IntPtr.Size) + this.m_shaGraph.GetSize() + (long) IntPtr.Size + (long) CacheUtil.ObjectOverhead + (long) this.m_packedCommitTimes.Length + (long) IntPtr.Size + (long) CacheUtil.ObjectOverhead + (long) (this.m_rootTreeIds.Length * 20) + (long) IntPtr.Size + this.ChangedPathFilters.GetSize();

    public PackedBloomFilters ChangedPathFilters { get; }

    public byte GetGeneration(int label)
    {
      if (label >= 0)
      {
        int num = label;
        int? length = this.m_generations?.Length;
        int valueOrDefault = length.GetValueOrDefault();
        if (num < valueOrDefault & length.HasValue)
          return this.m_generations[label];
      }
      return 0;
    }

    public void IncrementGenerations()
    {
      this.ExtendGenerations();
      for (int index = 0; index < this.m_generations.Length; ++index)
        ++this.m_generations[index];
    }

    public void SetGeneration(int label, byte value)
    {
      this.ExtendGenerations();
      this.m_generations[label] = value;
    }

    private void ExtendGenerations()
    {
      if (this.m_generations == null)
      {
        this.m_generations = new byte[this.NumVertices];
      }
      else
      {
        if (this.m_generations.Length >= this.NumVertices)
          return;
        Array.Resize<byte>(ref this.m_generations, Math.Max(this.NumVertices, this.m_generations.Length * 2));
      }
    }

    internal byte[] Generations => this.m_generations;

    public BloomKey EncodeKey(string path) => this.ChangedPathFilters.EncodeKey(path);

    public BloomFilterStatus GetFilterStatus(int label) => this.ChangedPathFilters.GetFilterStatus(label);

    public IReadOnlyBloomFilter GetReadOnlyFilter(int label) => this.ChangedPathFilters.GetReadOnlyFilter(label);

    internal sealed class M101Format : IGitCommitGraphWriter, IGitCommitGraphReader
    {
      public const uint FormType = 828859239;
      public const int BytesPerCommitTime = 5;
      public const int BytesPerBloomFilterStatus = 5;
      private const int c_numExtraVertices = 128;

      public void Write(GitCommitGraph graph, Stream stream)
      {
        ArgumentUtility.CheckForNull<GitCommitGraph>(graph, nameof (graph));
        ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
        int numVertices = graph.NumVertices;
        using (RiffWriter riffWriter = new RiffWriter(stream, true))
        {
          riffWriter.BeginRiffChunk(828859239U, 0U);
          int sizeRequested = Math.Min(numVertices * 20, GitStreamUtil.OptimalBufferSize);
          using (ByteArray byteArray = new ByteArray(sizeRequested))
          {
            riffWriter.BeginChunk(1668248681U, checked ((uint) numVertices * 20U));
            Sha1Id[] vertices = graph.m_shaGraph.Vertices;
            RiffUtil.WriteSha1Ids(stream, vertices, numVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1701147252U, checked ((uint) numVertices * 20U));
            Sha1Id[] rootTreeIds = graph.m_rootTreeIds;
            RiffUtil.WriteSha1Ids(stream, rootTreeIds, numVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1701015137U, checked ((uint) numVertices * 4U));
            int[] depths = graph.m_shaGraph.Depths;
            GitStreamUtil.WriteArray<int>(stream, depths, 4, 0, numVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1836346723U, checked ((uint) numVertices * 5U));
            stream.Write(graph.m_packedCommitTimes, 0, checked (numVertices * 5));
            riffWriter.EndChunk();
            int numEdges = graph.m_shaGraph.NumEdges;
            riffWriter.BeginChunk(1701274725U, checked ((uint) numEdges * 4U));
            int num1 = sizeRequested / 4;
            int intOffset = 0;
            int label = 0;
            while (label < numVertices)
            {
              int i = int.MinValue;
              bool flag = false;
              foreach (int num2 in graph.OutNeighborsOfLabel(label))
              {
                if (flag)
                {
                  RiffUtil.FillByteArray(byteArray.Bytes, intOffset, i);
                  checked { ++intOffset; }
                  if (intOffset >= num1)
                  {
                    stream.Write(byteArray.Bytes, 0, checked (intOffset * 4));
                    intOffset = 0;
                  }
                }
                flag = true;
                i = num2;
              }
              if (flag)
                i = this.GetInvolution(i);
              RiffUtil.FillByteArray(byteArray.Bytes, intOffset, i);
              checked { ++intOffset; }
              if (intOffset >= num1)
              {
                stream.Write(byteArray.Bytes, 0, checked (intOffset * 4));
                intOffset = 0;
              }
              checked { ++label; }
            }
            if (intOffset > 0)
              stream.Write(byteArray.Bytes, 0, checked (intOffset * 4));
            riffWriter.EndChunk();
            if (graph.ChangedPathFilters != null)
            {
              ushort[] values = new ushort[3]
              {
                graph.ChangedPathFilters.Settings.NumBitsOnInMask,
                graph.ChangedPathFilters.Settings.NumBitsPerEntry,
                (ushort) graph.ChangedPathFilters.Settings.HashFunctionVersion
              };
              riffWriter.BeginChunk(1702063202U, (uint) (2 * values.Length));
              GitStreamUtil.WriteArray<ushort>(stream, values, 2, 0, values.Length, new byte[6]);
              riffWriter.EndChunk();
              byte[] statuses = graph.ChangedPathFilters.Statuses;
              int num3 = Math.Min(checked (graph.ChangedPathFilters.CurNumLabels * 5), statuses.Length);
              riffWriter.BeginChunk(1953721442U, checked ((uint) num3));
              stream.Write(statuses, 0, num3);
              riffWriter.EndChunk();
              int curFilterSize = checked ((int) graph.ChangedPathFilters.CurFilterSize);
              long[] filterData = graph.ChangedPathFilters.FilterData;
              riffWriter.BeginChunk(1818651746U, checked ((uint) (8 * curFilterSize)));
              GitStreamUtil.WriteArray<long>(stream, filterData, 8, 0, curFilterSize, byteArray.Bytes);
              riffWriter.EndChunk();
            }
            if (graph.Generations != null)
            {
              riffWriter.BeginChunk(1701733735U, (uint) graph.NumVertices);
              GitStreamUtil.WriteArray<byte>(stream, graph.Generations, 1, 0, graph.NumVertices, byteArray.Bytes);
              riffWriter.EndChunk();
            }
          }
          riffWriter.BeginChunk(1869444462U, 4U);
          stream.Write(BitConverter.GetBytes(graph.NumObjectsAtSerialization), 0, 4);
          riffWriter.EndChunk();
          riffWriter.EndChunk();
        }
      }

      public GitCommitGraph Read(RiffFile riff, bool loadGenerations)
      {
        if (riff.ListType != 828859239U)
          throw new InvalidDataException(string.Format("Expected RIFF form type 0x{0:x8}, actual was 0x{1:x8}", (object) 828859239U, (object) riff.ListType));
        byte[] generations = (byte[]) null;
        using (ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize))
        {
          ILookup<uint, RiffChunk> lookup = riff.ToLookup<RiffChunk, uint>((Func<RiffChunk, uint>) (x => x.Id));
          RiffChunk result1;
          int numObjectsAtSerialization;
          if (lookup.TryGetChunk(1869444462U, out result1))
          {
            result1.Stream.Position = 0L;
            GitStreamUtil.ReadGreedy(result1.Stream, byteArray.Bytes, 0, 4);
            numObjectsAtSerialization = BitConverter.ToInt32(byteArray.Bytes, 0);
          }
          else
            numObjectsAtSerialization = 0;
          RiffChunk chunk = RiffUtil.GetChunk(lookup, 1668248681U);
          int num = checked ((int) unchecked (chunk.Stream.Length / 20L));
          Sha1Id[] vertices = RiffUtil.ReadSha1Ids(chunk.Stream, byteArray.Bytes);
          int[] edges = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1701274725U).Stream, 4, byteArray.Bytes);
          byte[] commitTimes = RiffUtil.ReadBytes(RiffUtil.GetChunk(lookup, 1836346723U).Stream, num * 5);
          Sha1Id[] trees = RiffUtil.ReadSha1Ids(RiffUtil.GetChunk(lookup, 1701147252U).Stream, byteArray.Bytes);
          int[] depths = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1701015137U).Stream, 4, byteArray.Bytes);
          RiffChunk result2;
          if (loadGenerations && lookup.TryGetChunk(1701733735U, out result2))
            generations = RiffUtil.ReadArray<byte>(result2.Stream, 1, byteArray.Bytes);
          RiffChunk result3;
          PackedBloomFilters changedPathFilters;
          if (lookup.TryGetChunk(1702063202U, out result3))
          {
            ushort[] numArray = RiffUtil.ReadArray<ushort>(result3.Stream, 2, byteArray.Bytes);
            BloomFilterSettings settings = numArray.Length >= 3 ? BloomFilterSettings.CreateCustomBloomFilterSettings(numArray[0], numArray[1], (BloomFilterHashVersion) numArray[2]) : throw new InvalidDataException(string.Format("Expected 3 UInt16 values in BLOOM_SETTINGS chunk, but instead found {0}.", (object) numArray.Length));
            changedPathFilters = new PackedBloomFilters(RiffUtil.ReadBytes(RiffUtil.GetChunk(lookup, 1953721442U).Stream, num * 5), RiffUtil.ReadArray<long>(RiffUtil.GetChunk(lookup, 1818651746U).Stream, 8, byteArray.Bytes), settings);
          }
          else
            changedPathFilters = (PackedBloomFilters) null;
          return new GitCommitGraph(vertices, vertices.Length, edges, depths, trees, commitTimes, numObjectsAtSerialization, changedPathFilters, generations);
        }
      }

      private long BytesToLong(byte[] array, int startIndex, int numBytes)
      {
        long num = 0;
        for (int index = 0; index < numBytes; ++index)
          num |= (long) array[startIndex + index] << 8 * index;
        return num;
      }

      private int GetInvolution(int i) => -(i + 1);

      public static class ChunkIds
      {
        public const uint CommitIds = 1668248681;
        public const uint TreeIds = 1701147252;
        public const uint AncestryDepths = 1701015137;
        public const uint CommitTimes = 1836346723;
        public const uint Edges = 1701274725;
        public const uint NumObjects = 1869444462;
        public const uint BloomSettings = 1702063202;
        public const uint BloomFilter = 1818651746;
        public const uint BloomStatus = 1953721442;
        public const uint Generations = 1701733735;
      }
    }
  }
}
