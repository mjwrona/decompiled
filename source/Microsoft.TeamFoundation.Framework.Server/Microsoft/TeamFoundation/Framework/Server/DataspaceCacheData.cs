// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceCacheData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceCacheData
  {
    public DataspaceCacheData()
    {
      this.DataspacesByCategoryIdentifier = new Dictionary<string, Dictionary<Guid, Dataspace>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.DataspacesById = new Dictionary<int, Dataspace>();
    }

    public DataspaceCacheData(
      Dictionary<string, Dictionary<Guid, Dataspace>> dataspacesByCategoryIdentifier,
      Dictionary<int, Dataspace> dataspacesById)
    {
      this.DataspacesByCategoryIdentifier = dataspacesByCategoryIdentifier;
      this.DataspacesById = dataspacesById;
    }

    public void AddDataspace(IVssRequestContext requestContext, Dataspace dataspace)
    {
      Dictionary<Guid, Dataspace> dictionary;
      if (!this.DataspacesByCategoryIdentifier.TryGetValue(dataspace.DataspaceCategory, out dictionary))
      {
        dictionary = new Dictionary<Guid, Dataspace>();
        this.DataspacesByCategoryIdentifier[dataspace.DataspaceCategory] = dictionary;
      }
      dictionary[dataspace.DataspaceIdentifier] = dataspace;
      this.DataspacesById[dataspace.DataspaceId] = dataspace;
    }

    internal void RemoveDataspace(IVssRequestContext requestContext, Dataspace dataspace)
    {
      foreach (Dictionary<Guid, Dataspace> dictionary in this.DataspacesByCategoryIdentifier.Values)
        dictionary.Remove(dataspace.DataspaceIdentifier);
      this.DataspacesById.Remove(dataspace.DataspaceId);
    }

    internal void UpdateDataspace(IVssRequestContext requestContext, Dataspace dataspace)
    {
      this.RemoveDataspace(requestContext, dataspace);
      this.AddDataspace(requestContext, dataspace);
    }

    public Dictionary<string, Dictionary<Guid, Dataspace>> DataspacesByCategoryIdentifier { get; private set; }

    public Dictionary<int, Dataspace> DataspacesById { get; private set; }
  }
}
