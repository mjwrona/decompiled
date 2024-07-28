// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.IndexingUnitWikisDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class IndexingUnitWikisDataAccess : SqlAzureDataAccess, IIndexingUnitWikisDataAccess
  {
    public void AddIndexingUnitWikisEntry(
      IVssRequestContext requestContext,
      IndexingUnitWikisEntry entry)
    {
      using (IndexingUnitWikisComponent component = requestContext.CreateComponent<IndexingUnitWikisComponent>())
      {
        if (!(component is IndexingUnitWikisComponentV1 wikisComponentV1))
          return;
        wikisComponentV1.InsertIndexingUnitWikisEntry(entry);
      }
    }

    public void UpdateIndexingUnitWikisEntry(
      IVssRequestContext requestContext,
      IndexingUnitWikisEntry entry)
    {
      using (IndexingUnitWikisComponent component = requestContext.CreateComponent<IndexingUnitWikisComponent>())
      {
        if (!(component is IndexingUnitWikisComponentV1 wikisComponentV1))
          return;
        wikisComponentV1.UpdateIndexingUnitWikisEntry(entry);
      }
    }

    public virtual IndexingUnitWikisEntry GetIndexingUnitWikisEntry(
      IVssRequestContext requestContext,
      int indexingUnitId)
    {
      using (IndexingUnitWikisComponent component = requestContext.CreateComponent<IndexingUnitWikisComponent>())
        return component is IndexingUnitWikisComponentV1 wikisComponentV1 ? wikisComponentV1.RetrieveIndexingUnitWikisEntry(indexingUnitId) : (IndexingUnitWikisEntry) null;
    }

    public IDictionary<int, IndexingUnitWikisEntry> GetIndexingUnitWikisEntries(
      IVssRequestContext requestContext)
    {
      using (IndexingUnitWikisComponent component = requestContext.CreateComponent<IndexingUnitWikisComponent>())
        return component is IndexingUnitWikisComponentV1 wikisComponentV1 ? wikisComponentV1.GetIndexingUnitWikiEntries() : (IDictionary<int, IndexingUnitWikisEntry>) null;
    }

    public virtual void DeleteIndexingUnitWikisEntry(
      IVssRequestContext requestContext,
      int indexingUnitId)
    {
      using (IndexingUnitWikisComponent component = requestContext.CreateComponent<IndexingUnitWikisComponent>())
      {
        if (!(component is IndexingUnitWikisComponentV2 wikisComponentV2))
          return;
        wikisComponentV2.DeleteIndexingUnitWikisEntry(indexingUnitId);
      }
    }
  }
}
