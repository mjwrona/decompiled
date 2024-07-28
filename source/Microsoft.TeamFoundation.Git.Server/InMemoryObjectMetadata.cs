// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.InMemoryObjectMetadata
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class InMemoryObjectMetadata : IObjectMetadata
  {
    private readonly Dictionary<Sha1Id, ObjectIdAndSize> m_objectSizeMap;

    public InMemoryObjectMetadata(List<ObjectIdAndSize> objectSizeMap) => this.m_objectSizeMap = objectSizeMap.ToDictionary<ObjectIdAndSize, Sha1Id, ObjectIdAndSize>((Func<ObjectIdAndSize, Sha1Id>) (item => item.Id), (Func<ObjectIdAndSize, ObjectIdAndSize>) (item => item));

    public List<ObjectIdAndSize> GetObjectSizes(
      IEnumerable<Sha1Id> objectIds,
      string eventFeatureSpace,
      bool continueOnError)
    {
      List<ObjectIdAndSize> objectSizes = new List<ObjectIdAndSize>();
      foreach (Sha1Id objectId in objectIds)
      {
        if (this.m_objectSizeMap.ContainsKey(objectId))
          objectSizes.Add(this.m_objectSizeMap[objectId]);
        else if (!continueOnError)
          throw new GitObjectDoesNotExistException(objectId);
      }
      return objectSizes;
    }
  }
}
