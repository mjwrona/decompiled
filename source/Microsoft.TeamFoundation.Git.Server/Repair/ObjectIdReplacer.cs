// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.ObjectIdReplacer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal static class ObjectIdReplacer
  {
    public static void Execute(
      IVssRequestContext rc,
      ITFLogger log,
      OdbId odbId,
      Sha1Id oldObjectId,
      Sha1Id newObjectId)
    {
      RepairUtils.OdbQuiesce(rc, odbId, log);
      using (Odb odb = DefaultGitDependencyRoot.Instance.CreateOdb(rc, odbId))
      {
        GitPackIndexTransaction indexTransaction = odb.PackIndexTranFactory();
        try
        {
          indexTransaction.EnsureIndexLease();
          Sha1Id oldIndexId = odb.PackIndexPointerProvider.GetIndex().Value;
          log.Info(string.Format("Updating index with id: {0}", (object) oldIndexId));
          byte[] data;
          using (Stream stream = odb.ContentDB.DataFileProvider.GetStream(StorageUtils.GetOdbFileName(oldIndexId, KnownFileType.Index)))
          {
            data = new byte[stream.Length];
            GitStreamUtil.ReadGreedy(stream, data, 0, data.Length);
          }
          ObjectIdReplacer.ReplaceSha1(data, oldObjectId, newObjectId, "ObjectId");
          Sha1Id sha1Id = GitDataFileUtil.WriteIndexRaw(rc, odb.BlobProvider, odbId, indexTransaction.KnownFilesBuilder, (Action<Sha1Id, Stream>) ((newIndexId, output) =>
          {
            ObjectIdReplacer.ReplaceSha1(data, oldIndexId, newIndexId, "IndexId");
            output.Write(data, 0, data.Length);
          }));
          using (ConcatGitPackIndex oldIndex = odb.PackIndexLoader.LoadIndex(new Sha1Id?(oldIndexId)))
          {
            using (ConcatGitPackIndex newIndex = odb.PackIndexLoader.LoadIndex(new Sha1Id?(sha1Id)))
              indexTransaction.CommitAndDispose(oldIndex, newIndex, removedObjects: (IEnumerable<Sha1Id>) new Sha1Id[1]
              {
                oldObjectId
              });
          }
        }
        finally
        {
          indexTransaction.TryExpirePendingExtantAndDispose();
        }
      }
      RepairUtils.OdbUnquiesce(rc, odbId, log);
    }

    private static void ReplaceSha1(byte[] data, Sha1Id oldId, Sha1Id newId, string replaceType)
    {
      int? nullable = new int?();
      for (int index1 = 0; index1 <= data.Length - 20; ++index1)
      {
        bool flag = true;
        for (int index2 = 0; index2 < 20; ++index2)
        {
          if ((int) data[index1 + index2] != (int) oldId[index2])
          {
            flag = false;
            break;
          }
        }
        if (flag)
          nullable = !nullable.HasValue ? new int?(index1) : throw new InvalidOperationException(string.Format("{0}: Multiple instances of original Sha1Id found. Starting position {1} and {2}", (object) replaceType, (object) nullable, (object) index1));
      }
      if (!nullable.HasValue)
        throw new InvalidOperationException(string.Format("{0}: Source Sha1Id not found in the index. Sha1Id: {1}", (object) replaceType, (object) oldId));
      Array.Copy((Array) newId.ToByteArray(), 0, (Array) data, nullable.Value, 20);
    }
  }
}
