// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackDeserializer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Server.Core.Security;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackDeserializer
  {
    private GitPackDeserializer.ObjectContentHandler m_objectContent;
    private readonly ITraceRequest m_tracer;
    private readonly Stream m_stream;
    private readonly long m_startPosition;
    private readonly IBufferStreamFactory m_bufferStreamFactory;
    private readonly Action m_checkIfCanceled;
    private readonly bool m_allowOfsDeltas;
    private readonly ClientTraceData m_ctData;
    private readonly bool m_throwIfTrailingJunk;
    private GitPackDeserializerProgress m_progress;
    private bool m_streamHasSubview;
    private readonly List<GitPackDeserializer.IndexEntry> m_indexList = new List<GitPackDeserializer.IndexEntry>();
    private readonly Dictionary<Sha1Id, GitPackDeserializer.IndexEntry> m_indexMap = new Dictionary<Sha1Id, GitPackDeserializer.IndexEntry>();
    private readonly List<OffsetLength> m_deltas = new List<OffsetLength>();
    private readonly List<GitPackDeserializer.OfsDelta> m_ofsDeltas = new List<GitPackDeserializer.OfsDelta>();
    private readonly List<GitPackDeserializer.RefDelta> m_refDeltas = new List<GitPackDeserializer.RefDelta>();
    private const string c_layer = "GitPackDeserializer";
    private const int s_largePushThreshold = 100000;
    private const int s_mediumPushThreshold = 10000;
    private const int s_smallPushThreshold = 1000;
    private const int c_defaultBufferSize = 4096;
    private const int c_maxDeltaChainLength = 200;
    private static readonly VssPerformanceCounter s_objectsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitObjectsPerSec");
    private static readonly VssPerformanceCounter s_xLargeRepos = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitXLargeRepos");
    private static readonly VssPerformanceCounter s_largeRepos = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitLargeRepos");
    private static readonly VssPerformanceCounter s_mediumRepos = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitMediumRepos");
    private static readonly VssPerformanceCounter s_smallRepos = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitSmallRepos");

    public GitPackDeserializer(
      ITraceRequest tracer,
      Stream stream,
      IGitObjectSet baseObjects,
      IBufferStreamFactory bufferStreamFactory,
      Action checkIfCanceled,
      ClientTraceData ctData,
      bool allowOfsDeltas,
      bool throwIfTrailingJunk)
    {
      this.m_tracer = tracer;
      this.m_stream = stream;
      this.m_startPosition = this.m_stream.Position;
      this.BaseObjects = baseObjects;
      this.m_bufferStreamFactory = bufferStreamFactory;
      this.m_checkIfCanceled = checkIfCanceled;
      this.m_ctData = ctData;
      this.m_allowOfsDeltas = allowOfsDeltas;
      this.m_throwIfTrailingJunk = throwIfTrailingJunk;
    }

    public static GitPackDeserializer CreateForOdbFsck(
      IVssRequestContext rc,
      Odb odb,
      Stream stream,
      bool isFullPack)
    {
      return new GitPackDeserializer(rc.RequestTracer, stream, isFullPack ? (IGitObjectSet) EmptyGitObjectSet.Instance : (IGitObjectSet) odb.ObjectSet, (IBufferStreamFactory) odb.ContentDB.DataFileProvider, (Action) (() => rc.RequestContextInternal().CheckCanceled()), (ClientTraceData) null, false, true);
    }

    public void AddTrait(IGitPackDeserializerTrait trait) => trait.AddToDeserializer(this);

    public event GitPackDeserializer.ObjectContentHandler ObjectContent
    {
      add
      {
        if (this.m_objectContent != null)
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} != null", (object) "ObjectContent")));
        this.m_objectContent += value;
      }
      remove => this.m_objectContent -= value;
    }

    public event GitPackDeserializer.ObjectInfoHandler ObjectInfo;

    public event GitPackDeserializer.DeserializationCompleteHandler DeserializationComplete;

    public void Deserialize()
    {
      using (ObjectPool<HashingStream<SHA1Cng2>> hashPool = new ObjectPool<HashingStream<SHA1Cng2>>())
      {
        HashingStream<SHA1Cng2> hashingStream = hashPool.Next();
        hashingStream.Setup(this.m_stream, FileAccess.Read, 4096, true);
        int entryCount = GitServerUtils.ReadPackHeader((Stream) hashingStream);
        this.m_progress.TotalObjects = entryCount;
        VssPerformanceCounter performanceCounter = GitPackDeserializer.PrepareDeserializePerfCounter(entryCount);
        try
        {
          if (entryCount > 100000)
            this.m_tracer.Trace(1013113, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitPackDeserializer), "Large push starting: {0} objects.", (object) entryCount);
          Sha1Id packHash;
          long num;
          using (ByteArray byteArray = new ByteArray(4096))
          {
            byte[] bytes = byteArray.Bytes;
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.WalkFullObjectsAndConstructDeltas<SHA1Cng2>(bytes, (Stream) hashingStream, hashPool, entryCount);
            stopwatch.Stop();
            packHash = new Sha1Id(hashingStream.Hash);
            this.ValidateTrailingHash(bytes, hashingStream.Hash);
            hashingStream.PrepareForPool();
            hashPool.ReturnToPool(hashingStream);
            num = this.m_stream.Position - this.m_startPosition;
            this.LogDeserializeKPIs(entryCount, num, stopwatch.ElapsedMilliseconds);
            this.m_ofsDeltas.Sort((IComparer<GitPackDeserializer.OfsDelta>) GitPackDeserializer.OfsDeltaComparer.Instance);
            this.m_refDeltas.Sort((IComparer<GitPackDeserializer.RefDelta>) GitPackDeserializer.RefDeltaComparer.Instance);
            this.WalkDeltaChains<SHA1Cng2>(bytes, hashPool);
          }
          if (this.m_progress.ObjectsEnumerated != this.m_progress.TotalObjects)
            throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0} != {1}", (object) "ObjectsEnumerated", (object) "TotalObjects")));
          GitPackDeserializer.DeserializationCompleteHandler deserializationComplete = this.DeserializationComplete;
          if (deserializationComplete == null)
            return;
          deserializationComplete(packHash, this.m_stream, num);
        }
        finally
        {
          if (entryCount > 100000)
            this.m_tracer.Trace(1013115, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitPackDeserializer), "Large push finished: {0} objects.", (object) entryCount);
          performanceCounter.Decrement();
        }
      }
    }

    private static VssPerformanceCounter PrepareDeserializePerfCounter(int entryCount)
    {
      GitPackDeserializer.s_objectsPerSec.IncrementBy((long) entryCount);
      VssPerformanceCounter performanceCounter = entryCount <= 100000 ? (entryCount <= 10000 ? (entryCount <= 1000 ? GitPackDeserializer.s_smallRepos : GitPackDeserializer.s_mediumRepos) : GitPackDeserializer.s_largeRepos) : GitPackDeserializer.s_xLargeRepos;
      performanceCounter.Increment();
      return performanceCounter;
    }

    private void LogDeserializeKPIs(int entryCount, long totalSize, long readingMillis)
    {
      if (this.m_ctData == null)
        return;
      this.m_ctData.Add("NumberOfObjects", (object) entryCount);
      this.m_ctData.Add("NumberOfOffsetDeltas", (object) this.m_ofsDeltas.Count);
      this.m_ctData.Add("NumberOfRefDeltas", (object) this.m_refDeltas.Count);
      this.m_ctData.Add("NumberOfBytes", (object) totalSize);
      this.m_ctData.Add("PackReadMillis", (object) readingMillis);
    }

    private void WalkFullObjectsAndConstructDeltas<T>(
      byte[] buffer,
      Stream hashingStream,
      ObjectPool<HashingStream<T>> hashPool,
      int entryCount)
      where T : HashAlgorithm, new()
    {
      long num = 0;
      for (int index = 0; index < entryCount; ++index)
      {
        long position = hashingStream.Position;
        GitPackObjectType type;
        long uncompressedSize;
        GitServerUtils.ReadPackEntryHeader(hashingStream, out type, out uncompressedSize);
        num += uncompressedSize;
        switch (type)
        {
          case GitPackObjectType.OfsDelta:
            if (!this.m_allowOfsDeltas)
              throw new InvalidGitPackEntryHeaderException("OfsDelta");
            long baseOffset = position - GitServerUtils.ReadOfsDeltaOffset(hashingStream);
            using (Stream inflateStream = GitServerUtils.CreateInflateStream(hashingStream, true, uncompressedSize))
            {
              GitStreamUtil.EnsureDrained(inflateStream, buffer);
              if (inflateStream.Position != uncompressedSize)
                throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0}: {1} != {2}", (object) "OfsDelta", (object) "Position", (object) "entrySize")));
            }
            this.m_deltas.Add(new OffsetLength(position, hashingStream.Position - position));
            this.m_ofsDeltas.Add(new GitPackDeserializer.OfsDelta(baseOffset, this.m_deltas.Count - 1));
            break;
          case GitPackObjectType.RefDelta:
            Sha1Id baseObjectId;
            try
            {
              baseObjectId = Sha1Id.FromStream(hashingStream);
            }
            catch (Sha1IdStreamReadException ex)
            {
              throw new InvalidGitPackEntryHeaderException((Exception) ex);
            }
            using (Stream inflateStream = GitServerUtils.CreateInflateStream(hashingStream, true, uncompressedSize))
            {
              GitStreamUtil.EnsureDrained(inflateStream, buffer);
              if (inflateStream.Position != uncompressedSize)
                throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0}: {1} != {2}", (object) "RefDelta", (object) "Position", (object) "entrySize")));
            }
            this.m_deltas.Add(new OffsetLength(position, hashingStream.Position - position));
            this.m_refDeltas.Add(new GitPackDeserializer.RefDelta(baseObjectId, this.m_deltas.Count - 1));
            break;
          default:
            Sha1Id objectId;
            using (Stream inflateStream = GitServerUtils.CreateInflateStream(hashingStream, true, uncompressedSize))
            {
              HashingStream<T> hashingStream1 = (HashingStream<T>) null;
              try
              {
                hashingStream1 = hashPool.Next();
                hashingStream1.Setup(inflateStream, FileAccess.Read);
                hashingStream1.AddToHash(GitServerUtils.CreateObjectHeader(type, uncompressedSize));
                ++this.m_progress.ObjectsEnumerated;
                this.GetObjectContent<T>(buffer, type, uncompressedSize, (Stream) hashingStream1, hashingStream1);
                GitStreamUtil.EnsureDrained((Stream) hashingStream1, buffer);
                if (hashingStream1.Position != uncompressedSize)
                  throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("default: {0} != {1}", (object) "Position", (object) "entrySize")));
                objectId = new Sha1Id(hashingStream1.Hash);
              }
              finally
              {
                if (hashingStream1 != null)
                {
                  hashingStream1.PrepareForPool();
                  hashPool.ReturnToPool(hashingStream1);
                }
              }
            }
            GitPackDeserializer.ObjectInfoHandler objectInfo = this.ObjectInfo;
            if (objectInfo != null)
              objectInfo(this.m_progress, objectId, type, uncompressedSize, position, hashingStream.Position - position);
            this.AddIndexEntry(objectId, new GitPackDeserializer.IndexEntry(type, new OffsetLength(position, hashingStream.Position - position), uncompressedSize));
            break;
        }
        Action checkIfCanceled = this.m_checkIfCanceled;
        if (checkIfCanceled != null)
          checkIfCanceled();
      }
      if (this.m_ctData != null)
        this.m_ctData.Add("NumberOfBytesDecompressed", (object) num);
      this.m_tracer.Trace(1013100, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackDeserializer), "Done with first pass");
    }

    private void ValidateTrailingHash(byte[] buffer, byte[] actualHash)
    {
      int count = this.m_throwIfTrailingJunk ? 21 : 20;
      int num = GitStreamUtil.TryReadGreedy(this.m_stream, buffer, 0, count);
      if (num < 20)
        throw new InvalidGitPackEntryHeaderException((Exception) new EndOfStreamException());
      if (num > 20 || !GitUtils.CompareByteArrays(buffer, 0, actualHash, 0, 20))
        throw new InvalidGitPackHeaderException((Exception) new InvalidDataException("HashDoesNotMatch"));
    }

    private void WalkDeltaChains<THash>(byte[] buffer, ObjectPool<HashingStream<THash>> hashPool) where THash : HashAlgorithm, new()
    {
      this.m_tracer.Trace(1013032, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackDeserializer), "Walking {0} deltas", (object) this.m_deltas.Count);
      for (int index = 0; index < this.m_deltas.Count; ++index)
      {
        OffsetLength delta = this.m_deltas[index];
        if (delta.Length >= 0L)
        {
          using (Stream restrictedStream = this.GetRestrictedStream(delta.Offset, delta.Length))
          {
            GitPackObjectType type;
            GitServerUtils.ReadPackEntryHeader(restrictedStream, out type, out long _);
            if (GitPackObjectType.OfsDelta == type)
            {
              long num = GitServerUtils.ReadOfsDeltaOffset(restrictedStream);
              restrictedStream.Dispose();
              this.ProcessDeltaChain<THash>(buffer, hashPool, delta.Offset - num);
            }
            else
            {
              if (GitPackObjectType.RefDelta != type)
                throw new InvalidOperationException("We should only be walking deltas during this step.");
              try
              {
                Sha1Id baseObjectId = Sha1Id.FromStream(restrictedStream);
                restrictedStream.Dispose();
                this.ProcessDeltaChain<THash>(buffer, hashPool, baseObjectId);
              }
              catch (Sha1IdStreamReadException ex)
              {
                throw new InvalidGitPackEntryHeaderException((Exception) ex);
              }
            }
          }
          Action checkIfCanceled = this.m_checkIfCanceled;
          if (checkIfCanceled != null)
            checkIfCanceled();
        }
      }
      this.m_tracer.Trace(1013033, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackDeserializer), "Delta chain processing complete!");
    }

    private void GetObjectContent<T>(
      byte[] buffer,
      GitPackObjectType packType,
      long objectLength,
      Stream objectStream,
      HashingStream<T> hashingStream)
      where T : HashAlgorithm, new()
    {
      try
      {
        GitPackDeserializer.ObjectContentHandler objectContent = this.m_objectContent;
        if (objectContent == null)
          return;
        objectContent(this.m_progress, objectStream, objectLength, packType);
      }
      catch (GitParserException ex)
      {
        GitStreamUtil.EnsureDrained(objectStream, buffer);
        throw new GitPackDeserializerException(Resources.Format("ObjectRejected", (object) packType.ToString().ToLower(), (object) new Sha1Id(hashingStream.Hash).ToString(), (object) ex.Message), (Exception) ex);
      }
    }

    private void ProcessDeltaChain<THash>(
      byte[] buffer,
      ObjectPool<HashingStream<THash>> hashPool,
      long baseOffset)
      where THash : HashAlgorithm, new()
    {
      int indexEntryByOffset = this.GetIndexEntryByOffset(baseOffset);
      GitPackDeserializer.IndexEntry indexEntry = indexEntryByOffset >= 0 ? this.m_indexList[indexEntryByOffset] : throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0} < 0", (object) "i")));
      using (Stream baseStreamFromPack = this.GetBaseStreamFromPack(indexEntry))
      {
        int[] ofsDeltaRange = this.GetOfsDeltaRange(baseOffset);
        if (ofsDeltaRange.Length == 0)
          throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("0 == {0}", (object) "Length")));
        this.ApplyDeltas<THash>(buffer, hashPool, ofsDeltaRange, baseStreamFromPack, indexEntry.PackType);
      }
    }

    private void ProcessDeltaChain<THash>(
      byte[] buffer,
      ObjectPool<HashingStream<THash>> hashPool,
      Sha1Id baseObjectId)
      where THash : HashAlgorithm, new()
    {
      int[] refDeltaRange = this.GetRefDeltaRange(baseObjectId);
      if (refDeltaRange.Length == 0)
        throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("0 == {0}", (object) "Length")));
      GitPackDeserializer.IndexEntry indexEntry;
      if (this.m_indexMap.TryGetValue(baseObjectId, out indexEntry))
      {
        using (Stream baseStreamFromPack = this.GetBaseStreamFromPack(indexEntry))
          this.ApplyDeltas<THash>(buffer, hashPool, refDeltaRange, baseStreamFromPack, indexEntry.PackType);
      }
      else
      {
        Stream content = (Stream) null;
        try
        {
          GitObjectType objectType;
          if (!this.BaseObjects.TryGetContent(baseObjectId, out content, out objectType))
            throw new GitPackMissingBaseFileException(baseObjectId);
          GitStreamUtil.MakeStreamSeekable(this.m_bufferStreamFactory, ref content);
          this.ApplyDeltas<THash>(buffer, hashPool, refDeltaRange, content, objectType.GetPackType());
        }
        finally
        {
          content?.Dispose();
        }
      }
    }

    private Stream GetBaseStreamFromPack(GitPackDeserializer.IndexEntry indexEntry)
    {
      Stream destination = this.m_bufferStreamFactory.GetBufferStream(indexEntry.UncompressedLength);
      try
      {
        OffsetLength offsetLength = indexEntry.OffsetLength;
        using (Stream restrictedStream = this.GetRestrictedStream(offsetLength.Offset, offsetLength.Length))
        {
          GitServerUtils.ReadPackEntryHeader(restrictedStream, out GitPackObjectType _, out long _);
          using (Stream inflateStream = GitServerUtils.CreateInflateStream(restrictedStream))
            inflateStream.CopyTo(destination);
        }
        Stream baseStreamFromPack = destination;
        destination = (Stream) null;
        return baseStreamFromPack;
      }
      finally
      {
        destination?.Dispose();
      }
    }

    private void ApplyDeltas<THash>(
      byte[] buffer,
      ObjectPool<HashingStream<THash>> hashPool,
      int[] deltaRange,
      Stream baseStream,
      GitPackObjectType objectType,
      int chainLength = 0)
      where THash : HashAlgorithm, new()
    {
      if (chainLength > 200)
        throw new GitDeltaChainIsTooLongException(200);
      for (int index = 0; index < deltaRange.Length; ++index)
      {
        Stream stream1 = (Stream) null;
        OffsetLength delta = this.m_deltas[deltaRange[index]];
        int[] ofsDeltaRange = this.GetOfsDeltaRange(delta.Offset);
        bool flag = this.m_refDeltas.Count > 0 || ofsDeltaRange.Length != 0;
        try
        {
          long length;
          Sha1Id sha1Id;
          using (Stream restrictedStream = this.GetRestrictedStream(delta.Offset, delta.Length))
          {
            GitPackObjectType type;
            GitServerUtils.ReadPackEntryHeader(restrictedStream, out type, out long _);
            if (GitPackObjectType.OfsDelta == type)
            {
              GitServerUtils.ReadOfsDeltaOffset(restrictedStream);
            }
            else
            {
              if (GitPackObjectType.RefDelta != type)
                throw new InvalidOperationException();
              if (GitStreamUtil.TryReadGreedy(restrictedStream, buffer, 0, 20) < 20)
                throw new InvalidGitPackEntryHeaderException("Incomplete base object ID");
            }
            baseStream.Seek(0L, SeekOrigin.Begin);
            using (DeltaStream deltaStream = new DeltaStream(GitServerUtils.CreateInflateStream(restrictedStream, true), baseStream, leaveBaseOpen: true))
            {
              HashingStream<THash> hashingStream = (HashingStream<THash>) null;
              try
              {
                hashingStream = hashPool.Next();
                hashingStream.Setup((Stream) deltaStream, FileAccess.Read, leaveOpen: true);
                length = deltaStream.Length;
                hashingStream.AddToHash(GitServerUtils.CreateObjectHeader(objectType, length));
                ++this.m_progress.ObjectsEnumerated;
                if (flag)
                {
                  stream1 = this.m_bufferStreamFactory.GetBufferStream(length);
                  using (Stream stream2 = (Stream) new RedirectingStream((Stream) hashingStream, stream1, true, true))
                  {
                    this.GetObjectContent<THash>(buffer, objectType, length, stream2, hashingStream);
                    GitStreamUtil.EnsureDrained(stream2, buffer);
                  }
                }
                else
                {
                  this.GetObjectContent<THash>(buffer, objectType, length, (Stream) hashingStream, hashingStream);
                  GitStreamUtil.EnsureDrained((Stream) hashingStream, buffer);
                }
                sha1Id = new Sha1Id(hashingStream.Hash);
              }
              finally
              {
                if (hashingStream != null)
                {
                  hashingStream.PrepareForPool();
                  hashPool.ReturnToPool(hashingStream);
                }
              }
            }
          }
          GitPackDeserializer.ObjectInfoHandler objectInfo = this.ObjectInfo;
          if (objectInfo != null)
            objectInfo(this.m_progress, sha1Id, objectType, length, delta.Offset, delta.Length);
          this.m_deltas[deltaRange[index]] = new OffsetLength(delta.Offset, -1L);
          int[] refDeltaRange = this.GetRefDeltaRange(sha1Id);
          if (refDeltaRange.Length != 0)
            this.ApplyDeltas<THash>(buffer, hashPool, refDeltaRange, stream1, objectType, chainLength + 1);
          if (ofsDeltaRange.Length != 0)
            this.ApplyDeltas<THash>(buffer, hashPool, ofsDeltaRange, stream1, objectType, chainLength + 1);
        }
        finally
        {
          stream1?.Dispose();
        }
      }
    }

    private void AddIndexEntry(Sha1Id objectId, GitPackDeserializer.IndexEntry entry)
    {
      this.m_indexList.Add(entry);
      this.m_indexMap[objectId] = entry;
    }

    private int GetIndexEntryByOffset(long offset)
    {
      int num1 = 0;
      int num2 = this.m_indexList.Count - 1;
      while (num1 <= num2)
      {
        int index = num1 + (num2 - num1 >> 1);
        int num3 = Math.Sign(this.m_indexList[index].OffsetLength.Offset - offset);
        if (num3 == 0)
          return index;
        if (num3 < 0)
          num1 = index + 1;
        else
          num2 = index - 1;
      }
      return ~num1;
    }

    private int[] GetOfsDeltaRange(long baseOffset)
    {
      int count = this.m_ofsDeltas.Count;
      int index1 = this.m_ofsDeltas.BinarySearch(new GitPackDeserializer.OfsDelta(baseOffset, 0), (IComparer<GitPackDeserializer.OfsDelta>) GitPackDeserializer.OfsDeltaComparer.Instance);
      if (index1 < 0)
      {
        index1 = ~index1;
        if (index1 >= count || this.m_ofsDeltas[index1].BaseOffset != baseOffset)
          return Array.Empty<int>();
      }
      int length = 1;
      for (int index2 = index1 + 1; index2 < count && this.m_ofsDeltas[index2].BaseOffset == baseOffset; ++index2)
        ++length;
      int[] ofsDeltaRange = new int[length];
      for (int index3 = 0; index3 < length; ++index3)
        ofsDeltaRange[index3] = this.m_ofsDeltas[index1 + index3].DependentIndex;
      return ofsDeltaRange;
    }

    private int[] GetRefDeltaRange(Sha1Id baseObjectId)
    {
      int count = this.m_refDeltas.Count;
      int index1 = this.m_refDeltas.BinarySearch(new GitPackDeserializer.RefDelta(baseObjectId, 0), (IComparer<GitPackDeserializer.RefDelta>) GitPackDeserializer.RefDeltaComparer.Instance);
      if (index1 < 0)
      {
        index1 = ~index1;
        if (index1 >= count || this.m_refDeltas[index1].BaseObjectId != baseObjectId)
          return Array.Empty<int>();
      }
      int length = 1;
      for (int index2 = index1 + 1; index2 < count && !(this.m_refDeltas[index2].BaseObjectId != baseObjectId); ++index2)
        ++length;
      int[] refDeltaRange = new int[length];
      for (int index3 = 0; index3 < length; ++index3)
        refDeltaRange[index3] = this.m_refDeltas[index1 + index3].DependentIndex;
      return refDeltaRange;
    }

    private Stream GetRestrictedStream(long offset, long length)
    {
      this.m_streamHasSubview = !this.m_streamHasSubview ? true : throw new InvalidOperationException("m_streamHasSubview");
      GitRestrictedStream restrictedStream = new GitRestrictedStream(this.m_stream, offset, length, true);
      restrictedStream.PushActionOnDispose((Action) (() => this.m_streamHasSubview = false));
      return (Stream) restrictedStream;
    }

    public IGitObjectSet BaseObjects { get; }

    internal int TEST_OfsDeltaCount => this.m_ofsDeltas.Count;

    internal int TEST_RefDeltaCount => this.m_refDeltas.Count;

    public delegate void ObjectContentHandler(
      GitPackDeserializerProgress progress,
      Stream content,
      long objectLength,
      GitPackObjectType type);

    public delegate void ObjectInfoHandler(
      GitPackDeserializerProgress progress,
      Sha1Id objectId,
      GitPackObjectType objectType,
      long objectLength,
      long offsetInPack,
      long lengthInPack);

    public delegate void DeserializationCompleteHandler(
      Sha1Id packHash,
      Stream packStream,
      long packLength);

    private class IndexEntry
    {
      public GitPackObjectType PackType;
      public OffsetLength OffsetLength;
      public long UncompressedLength;

      public IndexEntry(GitPackObjectType packType, OffsetLength ofsLen, long uncompressedLength)
      {
        this.PackType = packType;
        this.OffsetLength = ofsLen;
        this.UncompressedLength = uncompressedLength;
      }
    }

    private struct OfsDelta
    {
      public long BaseOffset;
      public int DependentIndex;

      public OfsDelta(long baseOffset, int dependentIndex)
      {
        this.BaseOffset = baseOffset;
        this.DependentIndex = dependentIndex;
      }
    }

    private struct RefDelta
    {
      public readonly Sha1Id BaseObjectId;
      public readonly int DependentIndex;

      public RefDelta(Sha1Id baseObjectId, int dependentIndex)
      {
        this.BaseObjectId = baseObjectId;
        this.DependentIndex = dependentIndex;
      }
    }

    private class OfsDeltaComparer : IComparer<GitPackDeserializer.OfsDelta>
    {
      public static readonly GitPackDeserializer.OfsDeltaComparer Instance = new GitPackDeserializer.OfsDeltaComparer();

      public int Compare(GitPackDeserializer.OfsDelta x, GitPackDeserializer.OfsDelta y)
      {
        int num = Math.Sign(x.BaseOffset - y.BaseOffset);
        if (num == 0)
          num = x.DependentIndex - y.DependentIndex;
        return num;
      }
    }

    private class RefDeltaComparer : IComparer<GitPackDeserializer.RefDelta>
    {
      public static readonly GitPackDeserializer.RefDeltaComparer Instance = new GitPackDeserializer.RefDeltaComparer();

      public int Compare(GitPackDeserializer.RefDelta x, GitPackDeserializer.RefDelta y)
      {
        int num = x.BaseObjectId.CompareTo(y.BaseObjectId);
        if (num == 0)
          num = x.DependentIndex - y.DependentIndex;
        return num;
      }
    }
  }
}
