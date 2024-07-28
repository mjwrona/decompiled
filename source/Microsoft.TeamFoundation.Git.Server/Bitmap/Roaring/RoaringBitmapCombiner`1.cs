// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.RoaringBitmapCombiner`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal class RoaringBitmapCombiner<T>
  {
    private const int c_valuesPerChunk = 65536;

    public RoaringBitmapCombiner(ITwoWayReadOnlyList<T> objectList) => this.ObjectList = objectList;

    public ITwoWayReadOnlyList<T> ObjectList { get; }

    public static RoaringBitmap<T> Union(RoaringBitmap<T> bitmap, RoaringBitmap<T> withBitmap)
    {
      RoaringBitmap<T> roaringBitmap = new RoaringBitmap<T>(bitmap.FullObjectList);
      HashSet<ushort> ushortSet1 = new HashSet<ushort>((IEnumerable<ushort>) bitmap.ChunkIds);
      HashSet<ushort> ushortSet2 = new HashSet<ushort>((IEnumerable<ushort>) withBitmap.ChunkIds);
      foreach (ushort chunkId in ushortSet1)
      {
        IChunk chunk1 = bitmap.GetChunk(chunkId);
        if (!ushortSet2.Contains(chunkId))
        {
          roaringBitmap.SetChunk(chunkId, chunk1.Duplicate());
        }
        else
        {
          IChunk chunk2 = withBitmap.GetChunk(chunkId);
          RawChunk rawChunk = new RawChunk();
          rawChunk.Absorb(chunk1);
          rawChunk.Absorb(chunk2);
          roaringBitmap.SetChunk(chunkId, (IChunk) rawChunk);
        }
      }
      foreach (ushort chunkId in ushortSet2)
      {
        if (!ushortSet1.Contains(chunkId))
          roaringBitmap.SetChunk(chunkId, withBitmap.GetChunk(chunkId).Duplicate());
      }
      return roaringBitmap;
    }

    public RoaringBitmap<T> Combine(
      IEnumerable<RoaringBitmap<T>> bitmapsToUnion,
      RoaringBitmap<T> bitmapToSubtract = null)
    {
      RoaringBitmap<T> roaringBitmap1 = new RoaringBitmap<T>(this.ObjectList);
      Dictionary<ushort, List<IChunk>> dictionary = new Dictionary<ushort, List<IChunk>>();
      foreach (RoaringBitmap<T> roaringBitmap2 in bitmapsToUnion)
      {
        foreach (ushort chunkId in (IEnumerable<ushort>) roaringBitmap2.ChunkIds)
        {
          List<IChunk> chunkList;
          if (!dictionary.TryGetValue(chunkId, out chunkList))
          {
            chunkList = new List<IChunk>();
            dictionary[chunkId] = chunkList;
          }
          chunkList.Add(roaringBitmap2.GetChunk(chunkId));
        }
      }
      foreach (KeyValuePair<ushort, List<IChunk>> keyValuePair in dictionary)
      {
        if (keyValuePair.Value.Count > 0)
        {
          RawChunk rawChunk = new RawChunk();
          for (int index = 0; index < keyValuePair.Value.Count; ++index)
            rawChunk.Absorb(keyValuePair.Value[index]);
          IChunk chunk = bitmapToSubtract?.GetChunk(keyValuePair.Key);
          if (chunk != null)
            rawChunk.Remove(chunk);
          if (rawChunk.Count > 0)
            roaringBitmap1.SetChunk(keyValuePair.Key, (IChunk) rawChunk);
        }
      }
      return roaringBitmap1;
    }
  }
}
