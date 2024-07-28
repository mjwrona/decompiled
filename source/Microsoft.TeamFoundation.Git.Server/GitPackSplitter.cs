// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackSplitter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitPackSplitter
  {
    private readonly List<PendingObject> m_pendingObjects;
    private readonly Dictionary<Sha1Id, int> m_objectIdToFirstIndex;
    private readonly int m_packSoftCapSize;
    private SplitPackResult? m_result;

    public GitPackSplitter(int packSoftCapSize)
    {
      this.m_packSoftCapSize = packSoftCapSize;
      this.m_pendingObjects = new List<PendingObject>();
      this.m_objectIdToFirstIndex = new Dictionary<Sha1Id, int>();
    }

    public int PendingObjectsCount => this.m_objectIdToFirstIndex.Count;

    public IEnumerable<OffsetLength> PendingObjectLocations => this.m_pendingObjects.Select<PendingObject, OffsetLength>((Func<PendingObject, OffsetLength>) (p => new OffsetLength(p.Offset, p.Length)));

    public void AddPendingObject(
      Sha1Id objectId,
      GitPackObjectType objectType,
      long offset,
      long length)
    {
      if (this.m_result.HasValue)
        throw new InvalidOperationException("AddPendingObject cannot be called after ProcessPendingObjectInfo.");
      this.m_pendingObjects.Add(new PendingObject(objectId, objectType, offset, length));
    }

    public bool TryGetPendingObject(Sha1Id objectId, out PendingObject pendingObject)
    {
      int index;
      if (this.m_objectIdToFirstIndex.TryGetValue(objectId, out index))
      {
        pendingObject = this.m_pendingObjects[index];
        return true;
      }
      pendingObject = (PendingObject) null;
      return false;
    }

    public SplitPackResult ProcessPendingObjectInfo()
    {
      if (!this.m_result.HasValue)
      {
        if (this.m_pendingObjects.Count == 0)
          throw new InvalidOperationException("No pending objects to process.");
        this.m_pendingObjects.Sort((Comparison<PendingObject>) ((x, y) => x.Offset.CompareTo(y.Offset)));
        GitPackIndexer indexer = new GitPackIndexer();
        List<PackRange> splitPacks = new List<PackRange>();
        (long, long, int) valueTuple = ();
        for (int index = 0; index < this.m_pendingObjects.Count; ++index)
        {
          PendingObject pendingObject1 = this.m_pendingObjects[index];
          if (index > 0)
          {
            PendingObject pendingObject2 = this.m_pendingObjects[index - 1];
            if (pendingObject1.Offset != pendingObject2.Offset + pendingObject2.Length)
              throw new InvalidOperationException(string.Format("Noncontiguous split packs are not supported: {0} != {1} + {2}", (object) pendingObject1.Offset, (object) pendingObject2.Offset, (object) pendingObject2.Length));
          }
          if (!this.m_objectIdToFirstIndex.ContainsKey(pendingObject1.ObjectId))
            this.m_objectIdToFirstIndex.Add(pendingObject1.ObjectId, index);
          if (valueTuple.Item3 == 0)
          {
            indexer.BeginPackFile(new Sha1Id?(), GitPackStates.None);
            valueTuple = (pendingObject1.Offset, pendingObject1.Length, 1);
          }
          else
          {
            valueTuple.Item2 += pendingObject1.Length;
            ++valueTuple.Item3;
          }
          indexer.AddObject(pendingObject1.ObjectId, pendingObject1.ObjectType, (long) GitPackSerializer.PackHeaderLength + pendingObject1.Offset - valueTuple.Item1, pendingObject1.Length);
          if ((long) GitPackSerializer.PackHeaderLength + valueTuple.Item2 > (long) this.m_packSoftCapSize || index == this.m_pendingObjects.Count - 1)
          {
            PackRange packRange = new PackRange(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3);
            splitPacks.Add(packRange);
            indexer.EndPackFile(packRange.PackId);
            valueTuple = ();
          }
        }
        this.m_result = new SplitPackResult?(new SplitPackResult(indexer, splitPacks));
      }
      return this.m_result.Value;
    }
  }
}
