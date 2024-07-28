// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.DeltaListProvider`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal abstract class DeltaListProvider<TRawKey>
  {
    public abstract Stream GetRawContent(TRawKey location);

    public abstract TRawKey GetLocation(Sha1Id objectId);

    public IReadOnlyDictionary<Sha1Id, IList<Sha1Id>> BuildBasesAndDeltas(
      IEnumerable<Sha1Id> toEnumerate)
    {
      Dictionary<Sha1Id, IList<Sha1Id>> dictionary1 = new Dictionary<Sha1Id, IList<Sha1Id>>();
      Dictionary<Sha1Id, bool> dictionary2 = new Dictionary<Sha1Id, bool>();
      int num = 0;
      foreach (Sha1Id sha1Id1 in toEnumerate)
      {
        ++num;
        if (!dictionary2.ContainsKey(sha1Id1))
        {
          List<Sha1Id> collection = new List<Sha1Id>();
          Queue<KeyValuePair<Sha1Id, TRawKey>> keyValuePairQueue = new Queue<KeyValuePair<Sha1Id, TRawKey>>();
          keyValuePairQueue.Enqueue(new KeyValuePair<Sha1Id, TRawKey>(sha1Id1, this.GetLocation(sha1Id1)));
          while (keyValuePairQueue.Count > 0)
          {
            KeyValuePair<Sha1Id, TRawKey> keyValuePair = keyValuePairQueue.Dequeue();
            if (!dictionary2.ContainsKey(keyValuePair.Key))
              dictionary2.Add(keyValuePair.Key, false);
            using (Stream rawContent = this.GetRawContent(keyValuePair.Value))
            {
              GitPackObjectType type;
              GitServerUtils.ReadPackEntryHeader(rawContent, out type, out long _);
              if (type == GitPackObjectType.RefDelta)
              {
                bool flag;
                if (dictionary2.TryGetValue(keyValuePair.Key, out flag) && !flag)
                  collection.Add(keyValuePair.Key);
                dictionary2[keyValuePair.Key] = true;
                Sha1Id sha1Id2 = Sha1Id.FromStream(rawContent);
                keyValuePairQueue.Enqueue(new KeyValuePair<Sha1Id, TRawKey>(sha1Id2, this.GetLocation(sha1Id2)));
              }
              else if (dictionary1.ContainsKey(keyValuePair.Key))
              {
                List<Sha1Id> sha1IdList = new List<Sha1Id>(dictionary1[keyValuePair.Key].Count + collection.Count);
                sha1IdList.AddRange((IEnumerable<Sha1Id>) dictionary1[keyValuePair.Key]);
                sha1IdList.AddRange((IEnumerable<Sha1Id>) collection);
                dictionary1[keyValuePair.Key] = (IList<Sha1Id>) sha1IdList;
              }
              else
                dictionary1.Add(keyValuePair.Key, (IList<Sha1Id>) collection);
            }
          }
        }
      }
      if (dictionary2.Count != num)
        throw new InvalidOperationException("Did not visit every enumerated object - buggy traversal.");
      return (IReadOnlyDictionary<Sha1Id, IList<Sha1Id>>) dictionary1;
    }
  }
}
