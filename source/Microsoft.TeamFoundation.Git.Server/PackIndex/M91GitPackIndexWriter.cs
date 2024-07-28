// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.M91GitPackIndexWriter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Riff;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal sealed class M91GitPackIndexWriter : IGitPackIndexWriter
  {
    private readonly IVssRequestContext m_rc;

    public M91GitPackIndexWriter(IVssRequestContext rc) => this.m_rc = rc;

    public void Write(
      GitPackIndexer indexer,
      Sha1Id indexId,
      Stream stream,
      IPackIndexMergeStrategy mergeStrategy)
    {
      ArgumentUtility.CheckForNull<GitPackIndexer>(indexer, nameof (indexer));
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ConcatGitPackIndex index1 = indexer.ToIndex();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IGitPackIndex index2 = (IGitPackIndex) mergeStrategy.Merge(this.m_rc, index1, M91GitPackIndexWriter.\u003C\u003EO.\u003C0\u003E__EstimateSize ?? (M91GitPackIndexWriter.\u003C\u003EO.\u003C0\u003E__EstimateSize = new PackIndexSizeEstimator(M91GitPackIndexWriter.EstimateSize)));
      SortedGitPackIndexObjects packIndexObjects = new SortedGitPackIndexObjects(index2);
      using (ByteArray byteArray1 = new ByteArray(Math.Max(Math.Min(packIndexObjects.SortedToRawObjectIntIds.Length * 4, GitStreamUtil.OptimalBufferSize), 128)))
      {
        using (RiffWriter riffWriter = new RiffWriter(stream, true))
        {
          riffWriter.BeginRiffChunk(863528041U, 0U);
          riffWriter.BeginChunk(538995042U, checked ((uint) (index2.BaseIndexIds.Count * 20)));
          foreach (Sha1Id baseIndexId in (IEnumerable<Sha1Id>) index2.BaseIndexIds)
            stream.Write(baseIndexId.ToByteArray(), 0, 20);
          riffWriter.EndChunk();
          riffWriter.BeginChunk(538995568U, checked ((uint) (index2.PackIds.Count * 20)));
          foreach (Sha1Id packId in (IEnumerable<Sha1Id>) index2.PackIds)
            stream.Write(packId.ToByteArray(), 0, 20);
          riffWriter.EndChunk();
          riffWriter.BeginChunk(538997616U, checked ((uint) index2.PackStates.Count));
          foreach (GitPackStates packState in (IEnumerable<GitPackStates>) index2.PackStates)
            stream.WriteByte((byte) packState);
          riffWriter.EndChunk();
          ushort[] values = new ushort[1]
          {
            index2.PackWatermarks[GitPackWatermark.NumRepacked]
          };
          riffWriter.BeginChunk(538998640U, checked ((uint) (values.Length * 2)));
          GitStreamUtil.WriteArray<ushort>(stream, values, 2, 0, values.Length, byteArray1.Bytes);
          riffWriter.EndChunk();
          riffWriter.BeginChunk(538996326U, 1024U);
          FanoutTable.ToStream((IReadOnlyList<Sha1Id>) packIndexObjects.ObjectIds, stream);
          riffWriter.EndChunk();
          riffWriter.BeginChunk(538976367U, checked ((uint) (packIndexObjects.ObjectIds.Count * 20)));
          foreach (Sha1Id objectId in (IEnumerable<Sha1Id>) packIndexObjects.ObjectIds)
            stream.Write(objectId.ToByteArray(), 0, 20);
          riffWriter.EndChunk();
          riffWriter.BeginChunk(538976357U, checked ((uint) (packIndexObjects.Entries.Count * 10)));
          List<long> longList1 = new List<long>();
          List<long> longList2 = new List<long>();
          foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) packIndexObjects.Entries)
          {
            stream.Write(BitConverter.GetBytes(entry.Location.PackIntId), 0, 2);
            uint num1 = (uint) entry.Location.Offset & (uint) int.MaxValue;
            if ((long) num1 != entry.Location.Offset)
            {
              num1 = (uint) (longList1.Count | int.MinValue);
              longList1.Add(entry.Location.Offset);
            }
            stream.Write(BitConverter.GetBytes(num1), 0, 4);
            uint num2 = (uint) entry.Location.Length & 268435455U;
            if ((long) num2 != entry.Location.Length)
            {
              num2 = (uint) (longList2.Count | 268435456);
              longList2.Add(entry.Location.Length);
            }
            uint num3 = num2 | (uint) entry.ObjectType << 29;
            stream.Write(BitConverter.GetBytes(num3), 0, 4);
          }
          riffWriter.EndChunk();
          riffWriter.BeginChunk(544170597U, checked ((uint) (longList1.Count * 8)));
          foreach (long num in longList1)
            stream.Write(BitConverter.GetBytes(num), 0, 8);
          riffWriter.EndChunk();
          riffWriter.BeginChunk(543973989U, checked ((uint) (longList2.Count * 8)));
          foreach (long num in longList2)
            stream.Write(BitConverter.GetBytes(num), 0, 8);
          riffWriter.EndChunk();
          Sha1Id? objectOrderEpoch = index2.StableObjectOrderEpoch;
          if (objectOrderEpoch.HasValue)
          {
            riffWriter.BeginChunk(544173939U, checked ((uint) (20 + packIndexObjects.SortedToRawObjectIntIds.Length * 4 + packIndexObjects.RawToSortedObjectIntIds.Length * 4)));
            Stream stream1 = stream;
            objectOrderEpoch = index2.StableObjectOrderEpoch;
            byte[] byteArray2 = objectOrderEpoch.Value.ToByteArray();
            stream1.Write(byteArray2, 0, 20);
            GitStreamUtil.WriteArray<int>(stream, packIndexObjects.SortedToRawObjectIntIds, 4, 0, packIndexObjects.SortedToRawObjectIntIds.Length, byteArray1.Bytes);
            GitStreamUtil.WriteArray<int>(stream, packIndexObjects.RawToSortedObjectIntIds, 4, 0, packIndexObjects.RawToSortedObjectIntIds.Length, byteArray1.Bytes);
            riffWriter.EndChunk();
          }
          riffWriter.BeginChunk(538993769U, 20U);
          stream.Write(indexId.ToByteArray(), 0, 20);
          riffWriter.EndChunk();
          riffWriter.EndChunk();
        }
      }
    }

    public ConcatGitPackIndex Merge(ConcatGitPackIndex index, IPackIndexMergeStrategy mergeStrategy) => mergeStrategy.Merge(this.m_rc, index, M91GitPackIndexWriter.\u003C\u003EO.\u003C0\u003E__EstimateSize ?? (M91GitPackIndexWriter.\u003C\u003EO.\u003C0\u003E__EstimateSize = new PackIndexSizeEstimator(M91GitPackIndexWriter.EstimateSize)));

    internal static long TEST_EstimateSize(IGitPackIndex index) => M91GitPackIndexWriter.EstimateSize(index);

    private static long EstimateSize(IGitPackIndex index) => checked ((long) (index.PackIds.Count * 20 + index.PackStates.Count) + (long) index.ObjectIds.Count * 20L + (long) index.Entries.Count * 10L + index.StableObjectOrderEpoch.HasValue ? (long) index.ObjectIds.Count * 8L : 0L);
  }
}
