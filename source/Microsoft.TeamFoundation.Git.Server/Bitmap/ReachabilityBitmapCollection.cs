// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.ReachabilityBitmapCollection
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Riff;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class ReachabilityBitmapCollection : IDisposable
  {
    private ByteArray m_buffer;
    private Stream m_bitmapDataStream;
    private readonly Sha1IdDeltaForest m_deltaForest;
    private readonly long[] m_bitmapPositions;
    private readonly IRoaringBitmapReader<Sha1Id> m_reader;
    private IDisposable m_toDispose;
    private bool m_disposed;
    private readonly Dictionary<int, RoaringBitmap<Sha1Id>> m_bitmapCache;
    private const string c_layer = "ReachabilityBitmapCollection";

    public ReachabilityBitmapCollection(Sha1Id epoch, ITwoWayReadOnlyList<Sha1Id> objectList)
    {
      this.StableOrderEpoch = epoch;
      this.ObjectList = objectList;
      this.NumObjectsAtSerialization = objectList.Count;
      this.m_deltaForest = new Sha1IdDeltaForest();
      this.m_bitmapCache = new Dictionary<int, RoaringBitmap<Sha1Id>>();
      this.m_bitmapPositions = Array.Empty<long>();
      this.m_bitmapDataStream = (Stream) null;
    }

    internal ReachabilityBitmapCollection(
      IRoaringBitmapReader<Sha1Id> reader,
      Sha1Id epoch,
      ITwoWayReadOnlyList<Sha1Id> objectList,
      int numObjectsAtSerialization,
      Sha1Id[] forestCommitIds,
      int[] forestParents,
      int[] forestChainLengths,
      long[] bitmapPositions,
      Stream bitmapDataStream,
      IDisposable toDispose = null)
    {
      this.m_reader = reader;
      this.StableOrderEpoch = epoch;
      this.ObjectList = objectList;
      this.NumObjectsAtSerialization = numObjectsAtSerialization;
      this.m_deltaForest = new Sha1IdDeltaForest(new Sha1IdLookup(forestCommitIds, forestCommitIds.Length), forestParents, forestChainLengths);
      this.m_bitmapPositions = bitmapPositions;
      this.m_bitmapDataStream = bitmapDataStream;
      this.m_toDispose = toDispose;
      this.m_buffer = new ByteArray(Math.Max((int) Math.Min(bitmapDataStream.Length, (long) GitStreamUtil.OptimalBufferSize), 128));
      this.m_bitmapCache = new Dictionary<int, RoaringBitmap<Sha1Id>>();
    }

    public void Dispose()
    {
      if (!this.m_disposed)
      {
        this.m_bitmapDataStream?.Dispose();
        this.m_bitmapDataStream = (Stream) null;
        this.m_toDispose?.Dispose();
        this.m_toDispose = (IDisposable) null;
        this.m_buffer?.Dispose();
        this.m_buffer = (ByteArray) null;
      }
      this.m_disposed = true;
    }

    public Sha1Id StableOrderEpoch { get; }

    public ITwoWayReadOnlyList<Sha1Id> ObjectList { get; }

    public int NumObjectsAtSerialization { get; }

    public IDeltaForest<int, Sha1Id> DeltaForest
    {
      get
      {
        this.EnsureNotDisposed();
        return (IDeltaForest<int, Sha1Id>) this.m_deltaForest;
      }
    }

    public RoaringBitmap<Sha1Id> GetBitmap(int label)
    {
      this.EnsureNotDisposed();
      RoaringBitmap<Sha1Id> bitmap1;
      if (this.m_bitmapCache.TryGetValue(label, out bitmap1))
        return bitmap1;
      this.m_bitmapDataStream.Seek(this.m_bitmapPositions[label], SeekOrigin.Begin);
      RoaringBitmap<Sha1Id> bitmap2 = this.m_reader.Read(this.m_bitmapDataStream, this.m_buffer.Bytes);
      this.m_bitmapCache[label] = bitmap2;
      return bitmap2;
    }

    public bool AddBitmap(Sha1Id commitId, RoaringBitmap<Sha1Id> bitmap, Sha1Id? parentId = null)
    {
      this.EnsureNotDisposed();
      if (this.DeltaForest.HasVertex(commitId))
        return false;
      if (parentId.HasValue)
        this.m_deltaForest.AddDeltaVertex(commitId, parentId.Value);
      else
        this.m_deltaForest.AddBaseVertex(commitId);
      this.m_bitmapCache.Add(this.DeltaForest.GetLabel(commitId), bitmap);
      return true;
    }

    public RoaringBitmap<Sha1Id> Combine(
      IEnumerable<int> unionLabels,
      RoaringBitmap<Sha1Id> notInSet)
    {
      this.EnsureNotDisposed();
      return new RoaringBitmapCombiner<Sha1Id>(this.ObjectList).Combine(unionLabels.Select<int, RoaringBitmap<Sha1Id>>((Func<int, RoaringBitmap<Sha1Id>>) (label => this.GetBitmap(label))), notInSet);
    }

    private void EnsureNotDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException("m_bitmapDataStream");
    }

    internal class M119Format : IReachabilityBitmapFormat
    {
      public const uint FormType = 828596850;

      public void Write(ReachabilityBitmapCollection collection, Stream stream)
      {
        ArgumentUtility.CheckForNull<ReachabilityBitmapCollection>(collection, nameof (collection));
        ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
        Sha1IdDeltaForest deltaForest = (Sha1IdDeltaForest) collection.DeltaForest;
        using (RiffWriter riffWriter = new RiffWriter(stream, true))
        {
          riffWriter.BeginRiffChunk(828596850U, 0U);
          using (ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize))
          {
            riffWriter.BeginChunk(1668247653U, 20U);
            RiffUtil.WriteSha1Ids(stream, new Sha1Id[1]
            {
              collection.StableOrderEpoch
            }, 1, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1869444462U, 4U);
            GitStreamUtil.WriteArray<int>(stream, new int[1]
            {
              collection.NumObjectsAtSerialization
            }, 4, 0, 1, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1668248681U, checked ((uint) deltaForest.NumVertices * 20U));
            Sha1Id[] vertices = deltaForest.Vertices;
            RiffUtil.WriteSha1Ids(stream, vertices, deltaForest.NumVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1634492784U, checked ((uint) deltaForest.NumVertices * 4U));
            int[] parents = deltaForest.Parents;
            GitStreamUtil.WriteArray<int>(stream, parents, 4, 0, deltaForest.NumVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1852140643U, checked ((uint) deltaForest.NumVertices * 4U));
            int[] chainLengths = deltaForest.ChainLengths;
            GitStreamUtil.WriteArray<int>(stream, chainLengths, 4, 0, deltaForest.NumVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            uint[] values = new uint[deltaForest.NumVertices];
            RoaringBitmap<Sha1Id>.M118Format m118Format = new RoaringBitmap<Sha1Id>.M118Format(collection.ObjectList);
            uint dataLength = 0;
            int label1 = 0;
            while (label1 < deltaForest.NumVertices)
            {
              values[label1] = dataLength;
              checked { dataLength += (uint) m118Format.GetSizeToWrite(collection.GetBitmap(label1)); }
              checked { ++label1; }
            }
            riffWriter.BeginChunk(1936683106U, checked ((uint) deltaForest.NumVertices * 4U));
            GitStreamUtil.WriteArray<uint>(stream, values, 4, 0, deltaForest.NumVertices, byteArray.Bytes);
            riffWriter.EndChunk();
            riffWriter.BeginChunk(1685350754U, dataLength);
            int label2 = 0;
            while (label2 < deltaForest.NumVertices)
            {
              m118Format.Write(collection.GetBitmap(label2), stream, byteArray.Bytes);
              checked { ++label2; }
            }
            riffWriter.EndChunk();
            riffWriter.EndChunk();
          }
        }
      }

      public ReachabilityBitmapCollection Read(
        Sha1Id stableOrderEpoch,
        ITwoWayReadOnlyList<Sha1Id> stableObjectOrder,
        Stream stream)
      {
        RiffFile riff = (RiffFile) null;
        try
        {
          stream = RiffFile.TryLoad(stream, out riff, false) ? (Stream) null : throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Expected RIFF form type 0x{0:x8}, but failed to read RIFF format.", (object) 828596850U)));
          if (riff.ListType != 828596850U)
            throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Expected RIFF form type 0x{0:x8}, actual was 0x{1:x8}", (object) 828596850U, (object) riff.ListType)));
          ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize);
          ILookup<uint, RiffChunk> lookup = riff.ToLookup<RiffChunk, uint>((Func<RiffChunk, uint>) (x => x.Id));
          Sha1Id readSha1Id = RiffUtil.ReadSha1Ids(RiffUtil.GetChunk(lookup, 1668247653U).Stream, byteArray.Bytes)[0];
          int read = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1869444462U).Stream, 4, byteArray.Bytes)[0];
          if (!readSha1Id.Equals(stableOrderEpoch))
            throw new InvalidOperationException("stable orders do not match!");
          RiffChunk chunk = RiffUtil.GetChunk(lookup, 1668248681U);
          int num = checked ((int) unchecked (chunk.Stream.Length / 20L));
          Sha1Id[] forestCommitIds = RiffUtil.ReadSha1Ids(chunk.Stream, byteArray.Bytes);
          int[] forestParents = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1634492784U).Stream, 4, byteArray.Bytes);
          int[] forestChainLengths = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1852140643U).Stream, 4, byteArray.Bytes);
          uint[] sourceArray = RiffUtil.ReadArray<uint>(RiffUtil.GetChunk(lookup, 1936683106U).Stream, 4, byteArray.Bytes);
          long[] numArray = new long[sourceArray.Length];
          Array.Copy((Array) sourceArray, 0, (Array) numArray, 0, sourceArray.Length);
          Stream stream1 = RiffUtil.GetChunk(lookup, 1685350754U).Stream;
          ReachabilityBitmapCollection bitmapCollection = new ReachabilityBitmapCollection((IRoaringBitmapReader<Sha1Id>) new RoaringBitmap<Sha1Id>.M118Format(stableObjectOrder), readSha1Id, stableObjectOrder, read, forestCommitIds, forestParents, forestChainLengths, numArray, stream1, (IDisposable) riff);
          riff = (RiffFile) null;
          return bitmapCollection;
        }
        finally
        {
          riff?.Dispose();
          stream?.Dispose();
        }
      }

      public static class ChunkIds
      {
        public const uint Epoch = 1668247653;
        public const uint NumObjects = 1869444462;
        public const uint CommitIds = 1668248681;
        public const uint ParentLabels = 1634492784;
        public const uint ChainLengths = 1852140643;
        public const uint BitmapPositions = 1936683106;
        public const uint BitmapData = 1685350754;
      }
    }

    internal class M161Format : IReachabilityBitmapFormat
    {
      public const uint FormType = 845374066;

      public void Write(ReachabilityBitmapCollection collection, Stream stream)
      {
        ArgumentUtility.CheckForNull<ReachabilityBitmapCollection>(collection, nameof (collection));
        ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
        Sha1IdDeltaForest deltaForest = (Sha1IdDeltaForest) collection.DeltaForest;
        using (Riff2Writer riff2Writer = new Riff2Writer(stream, true))
        {
          riff2Writer.BeginRiffChunk(845374066U, 0L);
          using (ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize))
          {
            riff2Writer.BeginChunk(1668247653U, 20L);
            RiffUtil.WriteSha1Ids(stream, new Sha1Id[1]
            {
              collection.StableOrderEpoch
            }, 1, byteArray.Bytes);
            riff2Writer.EndChunk();
            riff2Writer.BeginChunk(1869444462U, 4L);
            GitStreamUtil.WriteArray<int>(stream, new int[1]
            {
              collection.NumObjectsAtSerialization
            }, 4, 0, 1, byteArray.Bytes);
            riff2Writer.EndChunk();
            riff2Writer.BeginChunk(1668248681U, (long) checked ((uint) deltaForest.NumVertices * 20U));
            Sha1Id[] vertices = deltaForest.Vertices;
            RiffUtil.WriteSha1Ids(stream, vertices, deltaForest.NumVertices, byteArray.Bytes);
            riff2Writer.EndChunk();
            riff2Writer.BeginChunk(1634492784U, (long) checked ((uint) deltaForest.NumVertices * 4U));
            int[] parents = deltaForest.Parents;
            GitStreamUtil.WriteArray<int>(stream, parents, 4, 0, deltaForest.NumVertices, byteArray.Bytes);
            riff2Writer.EndChunk();
            riff2Writer.BeginChunk(1852140643U, (long) checked ((uint) deltaForest.NumVertices * 4U));
            int[] chainLengths = deltaForest.ChainLengths;
            GitStreamUtil.WriteArray<int>(stream, chainLengths, 4, 0, deltaForest.NumVertices, byteArray.Bytes);
            riff2Writer.EndChunk();
            long[] values = new long[deltaForest.NumVertices];
            RoaringBitmap<Sha1Id>.M118Format m118Format = new RoaringBitmap<Sha1Id>.M118Format(collection.ObjectList);
            long dataLength = 0;
            int label1 = 0;
            while (label1 < deltaForest.NumVertices)
            {
              values[label1] = dataLength;
              checked { dataLength += (long) m118Format.GetSizeToWrite(collection.GetBitmap(label1)); }
              checked { ++label1; }
            }
            riff2Writer.BeginChunk(1936683106U, checked ((long) deltaForest.NumVertices * 8L));
            GitStreamUtil.WriteArray<long>(stream, values, 8, 0, deltaForest.NumVertices, byteArray.Bytes);
            riff2Writer.EndChunk();
            riff2Writer.BeginChunk(1685350754U, dataLength);
            int label2 = 0;
            while (label2 < deltaForest.NumVertices)
            {
              m118Format.Write(collection.GetBitmap(label2), stream, byteArray.Bytes);
              checked { ++label2; }
            }
            riff2Writer.EndChunk();
            riff2Writer.EndChunk();
          }
        }
      }

      public ReachabilityBitmapCollection Read(
        Sha1Id stableOrderEpoch,
        ITwoWayReadOnlyList<Sha1Id> stableObjectOrder,
        Stream stream)
      {
        RiffFile riff = (RiffFile) null;
        try
        {
          stream = RiffFile.TryLoad(stream, out riff, false) ? (Stream) null : throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Expected RIFF form type 0x{0:x8}, but failed to read RIFF format.", (object) 845374066U)));
          if (riff.ListType != 845374066U)
            throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Expected RIFF form type 0x{0:x8}, actual was 0x{1:x8}", (object) 845374066U, (object) riff.ListType)));
          ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize);
          ILookup<uint, RiffChunk> lookup = riff.ToLookup<RiffChunk, uint>((Func<RiffChunk, uint>) (x => x.Id));
          Sha1Id readSha1Id = RiffUtil.ReadSha1Ids(RiffUtil.GetChunk(lookup, 1668247653U).Stream, byteArray.Bytes)[0];
          int read = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1869444462U).Stream, 4, byteArray.Bytes)[0];
          if (!readSha1Id.Equals(stableOrderEpoch))
            throw new InvalidOperationException("stable orders do not match!");
          RiffChunk chunk = RiffUtil.GetChunk(lookup, 1668248681U);
          int num = checked ((int) unchecked (chunk.Stream.Length / 20L));
          Sha1Id[] forestCommitIds = RiffUtil.ReadSha1Ids(chunk.Stream, byteArray.Bytes);
          int[] forestParents = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1634492784U).Stream, 4, byteArray.Bytes);
          int[] forestChainLengths = RiffUtil.ReadArray<int>(RiffUtil.GetChunk(lookup, 1852140643U).Stream, 4, byteArray.Bytes);
          long[] bitmapPositions = RiffUtil.ReadArray<long>(RiffUtil.GetChunk(lookup, 1936683106U).Stream, 8, byteArray.Bytes);
          Stream stream1 = RiffUtil.GetChunk(lookup, 1685350754U).Stream;
          ReachabilityBitmapCollection bitmapCollection = new ReachabilityBitmapCollection((IRoaringBitmapReader<Sha1Id>) new RoaringBitmap<Sha1Id>.M118Format(stableObjectOrder), readSha1Id, stableObjectOrder, read, forestCommitIds, forestParents, forestChainLengths, bitmapPositions, stream1, (IDisposable) riff);
          riff = (RiffFile) null;
          return bitmapCollection;
        }
        finally
        {
          riff?.Dispose();
          stream?.Dispose();
        }
      }

      public static class ChunkIds
      {
        public const uint Epoch = 1668247653;
        public const uint NumObjects = 1869444462;
        public const uint CommitIds = 1668248681;
        public const uint ParentLabels = 1634492784;
        public const uint ChainLengths = 1852140643;
        public const uint BitmapPositions = 1936683106;
        public const uint BitmapData = 1685350754;
      }
    }

    internal class MetaFormat : IReachabilityBitmapFormat
    {
      public ReachabilityBitmapCollection Read(
        Sha1Id stableOrderEpoch,
        ITwoWayReadOnlyList<Sha1Id> stableObjectOrder,
        Stream stream)
      {
        byte[] buffer = new byte[4];
        stream.Read(buffer, 0, buffer.Length);
        stream.Position = 0L;
        return (BitConverter.ToUInt32(buffer, 0) != 1179011410U ? (IReachabilityBitmapFormat) new ReachabilityBitmapCollection.M161Format() : (IReachabilityBitmapFormat) new ReachabilityBitmapCollection.M119Format()).Read(stableOrderEpoch, stableObjectOrder, stream);
      }

      public void Write(ReachabilityBitmapCollection collection, Stream stream) => throw new NotImplementedException();
    }
  }
}
