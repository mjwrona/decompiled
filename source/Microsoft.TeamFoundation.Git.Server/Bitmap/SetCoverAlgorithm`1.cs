// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.SetCoverAlgorithm`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class SetCoverAlgorithm<T>
  {
    private static SetCoverAlgorithm<T> s_instance = new SetCoverAlgorithm<T>();

    public static SetCoverAlgorithm<T> Instance => SetCoverAlgorithm<T>.s_instance;

    private SetCoverAlgorithm()
    {
    }

    public IEnumerable<T> GetCover(Dictionary<T, RoaringBitmap<T>> sets)
    {
      if (sets.Count != 0)
      {
        RoaringBitmapCombiner<T> combiner = new RoaringBitmapCombiner<T>(sets.Values.First<RoaringBitmap<T>>().FullObjectList);
        RoaringBitmap<T> itemsToCover = combiner.Combine((IEnumerable<RoaringBitmap<T>>) sets.Values);
        List<KeyValuePair<T, RoaringBitmap<T>>> curSetCollection = sets.ToList<KeyValuePair<T, RoaringBitmap<T>>>();
        RoaringBitmap<T> bitmapToSubtract = (RoaringBitmap<T>) null;
        while (itemsToCover.Count > 0)
        {
          T obj = default (T);
          int maxIndex = -1;
          RoaringBitmap<T> maxBitmap = (RoaringBitmap<T>) null;
          int num = 0;
          List<KeyValuePair<T, RoaringBitmap<T>>> newSetCollection = new List<KeyValuePair<T, RoaringBitmap<T>>>();
          for (int index = 0; index < curSetCollection.Count; ++index)
          {
            KeyValuePair<T, RoaringBitmap<T>> keyValuePair = curSetCollection[index];
            RoaringBitmap<T> roaringBitmap = keyValuePair.Value;
            if (bitmapToSubtract != null)
              roaringBitmap = combiner.Combine((IEnumerable<RoaringBitmap<T>>) new RoaringBitmap<T>[1]
              {
                roaringBitmap
              }, bitmapToSubtract);
            if (roaringBitmap.Count > num)
            {
              num = keyValuePair.Value.Count;
              obj = keyValuePair.Key;
              maxBitmap = roaringBitmap;
              maxIndex = index;
            }
            if (roaringBitmap.Count > 0)
              newSetCollection.Add(new KeyValuePair<T, RoaringBitmap<T>>(keyValuePair.Key, roaringBitmap));
          }
          if (num == 0)
            break;
          yield return obj;
          curSetCollection.RemoveAt(maxIndex);
          itemsToCover = combiner.Combine((IEnumerable<RoaringBitmap<T>>) new RoaringBitmap<T>[1]
          {
            itemsToCover
          }, maxBitmap);
          bitmapToSubtract = bitmapToSubtract != null ? RoaringBitmapCombiner<T>.Union(bitmapToSubtract, maxBitmap) : maxBitmap;
          curSetCollection = newSetCollection;
          maxBitmap = (RoaringBitmap<T>) null;
          newSetCollection = (List<KeyValuePair<T, RoaringBitmap<T>>>) null;
        }
      }
    }
  }
}
