// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.M44GitPackIndexWriter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal class M44GitPackIndexWriter : IGitPackIndexWriter
  {
    private readonly GitPackIndexVersion m_version;
    private const string c_layer = "M44GitPackIndexWriter";

    public M44GitPackIndexWriter(GitPackIndexVersion version) => this.m_version = version == GitPackIndexVersion.M44 || version == GitPackIndexVersion.M88 ? version : throw new ArgumentOutOfRangeException(nameof (version), (object) version, CommonResources.OutOfRange((object) version));

    public void Write(
      GitPackIndexer indexer,
      Sha1Id indexId,
      Stream stream,
      IPackIndexMergeStrategy mergeStrategy)
    {
      ArgumentUtility.CheckForNull<GitPackIndexer>(indexer, nameof (indexer));
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      Dictionary<string, M43GitPackIndex.TypeAndExpiration> dictionary = new Dictionary<string, M43GitPackIndex.TypeAndExpiration>();
      ConcatGitPackIndex index1 = indexer.ToIndex();
      M43GitPackIndex subindex = index1.Subindexes.Count > 1 ? index1.Subindexes[index1.Subindexes.Count - 2] as M43GitPackIndex : (M43GitPackIndex) null;
      SortedGitPackIndexObjects packIndexObjects = new SortedGitPackIndexObjects((IGitPackIndex) index1);
      bool flag = packIndexObjects.Entries.Any<GitPackIndexEntry>((Func<GitPackIndexEntry, bool>) (x => x.Location.Offset > (long) uint.MaxValue || x.Location.Length > (long) uint.MaxValue));
      foreach (Sha1Id packId in (IEnumerable<Sha1Id>) index1.PackIds)
      {
        string packFileName = StorageUtils.GetPackFileName(packId);
        dictionary[packFileName] = new M43GitPackIndex.TypeAndExpiration(KnownFileType.RawPackfile, DateTime.MaxValue);
      }
      if (this.m_version == GitPackIndexVersion.M44 && subindex != null)
      {
        foreach (KeyValuePair<string, M43GitPackIndex.TypeAndExpiration> knownFile in (IEnumerable<KeyValuePair<string, M43GitPackIndex.TypeAndExpiration>>) subindex.KnownFiles)
        {
          if (!dictionary.ContainsKey(knownFile.Key))
          {
            DateTime dateTime = DateTime.UtcNow.AddHours(48.0);
            dictionary[knownFile.Key] = new M43GitPackIndex.TypeAndExpiration(knownFile.Value.Type, dateTime < knownFile.Value.ExpirationDate ? dateTime : knownFile.Value.ExpirationDate);
          }
        }
        string odbFileName = StorageUtils.GetOdbFileName(subindex.Id.Value, KnownFileType.Index);
        dictionary[odbFileName] = new M43GitPackIndex.TypeAndExpiration(KnownFileType.Index, DateTime.UtcNow.AddHours(48.0));
      }
      stream.Write(M43GitPackIndex.CurrentIndexHeader, 0, M43GitPackIndex.CurrentIndexHeader.Length);
      byte version = (byte) this.m_version;
      if (flag)
        version |= M43GitPackIndex.LargeFileFlag;
      stream.WriteByte(version);
      stream.Write(BitConverter.GetBytes((ushort) index1.PackIds.Count), 0, 2);
      for (int index2 = 0; index2 < index1.PackIds.Count; ++index2)
        stream.Write(index1.PackIds[index2].ToByteArray(), 0, 20);
      if (this.m_version == GitPackIndexVersion.M44)
      {
        stream.Write(BitConverter.GetBytes(dictionary.Count), 0, 4);
        StringBuilder stringBuilder = new StringBuilder();
        List<M43GitPackIndex.TypeAndExpiration> typeAndExpirationList = new List<M43GitPackIndex.TypeAndExpiration>(dictionary.Count);
        foreach (KeyValuePair<string, M43GitPackIndex.TypeAndExpiration> keyValuePair in dictionary)
        {
          stringBuilder.AppendLine(keyValuePair.Key);
          typeAndExpirationList.Add(keyValuePair.Value);
        }
        byte[] bytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(stringBuilder.ToString());
        stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
        stream.Write(bytes, 0, bytes.Length);
        foreach (M43GitPackIndex.TypeAndExpiration typeAndExpiration in typeAndExpirationList)
        {
          stream.Write(BitConverter.GetBytes(typeAndExpiration.ExpirationDate.ToBinary()), 0, 8);
          stream.WriteByte((byte) typeAndExpiration.Type);
        }
      }
      FanoutTable.ToStream((IReadOnlyList<Sha1Id>) packIndexObjects.ObjectIds, stream);
      foreach (Sha1Id objectId in (IEnumerable<Sha1Id>) packIndexObjects.ObjectIds)
        stream.Write(objectId.ToByteArray(), 0, 20);
      foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) packIndexObjects.Entries)
      {
        ushort packIntId = entry.Location.PackIntId;
        stream.Write(BitConverter.GetBytes(packIntId), 0, 2);
        stream.WriteByte((byte) entry.ObjectType);
        if (flag)
        {
          stream.Write(BitConverter.GetBytes(entry.Location.Offset), 0, 8);
          stream.Write(BitConverter.GetBytes(entry.Location.Length), 0, 8);
        }
        else
        {
          stream.Write(BitConverter.GetBytes(checked ((uint) entry.Location.Offset)), 0, 4);
          stream.Write(BitConverter.GetBytes(checked ((uint) entry.Location.Length)), 0, 4);
        }
      }
      stream.Write(indexId.ToByteArray(), 0, 20);
    }

    public ConcatGitPackIndex Merge(ConcatGitPackIndex index, IPackIndexMergeStrategy mergeStrategy) => index;
  }
}
