// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.IndexingUnitComponentV8
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class IndexingUnitComponentV8 : IndexingUnitComponentV7
  {
    public IndexingUnitComponentV8()
    {
    }

    internal IndexingUnitComponentV8(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override List<IndexingUnit> GetOrphanIndexingUnitsOfTypeGitOrTfvcRepository()
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_GetOrphanIndexingUnitsOfTypeGitOrTfvcRepository");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null)
          {
            if (current.Items.Count > 0)
              return current.Items;
          }
        }
        return new List<IndexingUnit>();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve Orphan Repo Indexing Units")));
      }
    }

    public override void SoftDeleteIndexingUnitBatch(IEnumerable<int> indexingUnitIdList)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_SoftDeleteIndexingUnitBatch");
        this.BindIndexingUnitIdTable("@idList", indexingUnitIdList);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to execute SoftDeleteIndexingUnitBatch operation with SQL Azure Platform");
      }
    }
  }
}
