// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.UnionObjectMetadata
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class UnionObjectMetadata : IObjectMetadata
  {
    private readonly List<IObjectMetadata> m_submetas;

    public UnionObjectMetadata(List<IObjectMetadata> submetas) => this.m_submetas = submetas;

    public List<ObjectIdAndSize> GetObjectSizes(
      IEnumerable<Sha1Id> objectIds,
      string eventFeatureSpace,
      bool continueOnError)
    {
      HashSet<Sha1Id> source = new HashSet<Sha1Id>(objectIds);
      List<ObjectIdAndSize> objectSizes = new List<ObjectIdAndSize>();
      foreach (IObjectMetadata submeta in this.m_submetas)
      {
        foreach (ObjectIdAndSize objectSiz in submeta.GetObjectSizes(objectIds, eventFeatureSpace, true))
        {
          objectSizes.Add(objectSiz);
          source.Remove(objectSiz.Id);
        }
        if (source.Count == 0)
          break;
      }
      if (source.Count > 0 && !continueOnError)
        throw new GitObjectDoesNotExistException(source.First<Sha1Id>());
      return objectSizes;
    }
  }
}
