// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitComponentV5
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Data;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class IndexingUnitComponentV5 : IndexingUnitComponentV4
  {
    public IndexingUnitComponentV5()
    {
    }

    internal IndexingUnitComponentV5(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public virtual IndexingUnit AssociateJobId(IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      this.ValidateNotNull<Guid?>("indexingUnit.AssociatedJobId", indexingUnit.AssociatedJobId);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AssociateJobId");
        this.BindInt("@IndexingUnitId", indexingUnit.IndexingUnitId);
        this.BindGuid("@associatedJobId", indexingUnit.AssociatedJobId.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null)
          {
            if (current.Items.Count == 1)
              return resultCollection.GetCurrent<IndexingUnit>().Items[0];
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to update Indexing Unit with ID {0} and AssociatedJobId {1} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.AssociatedJobId.Value));
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format("Failed to update Indexing Unit with ID {0} and AssociatedJobId {1} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.AssociatedJobId.Value));
    }

    public override IndexingUnit Update(IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateIndexingUnit");
        this.BindInt("@IndexingUnitId", indexingUnit.IndexingUnitId);
        this.BindString("@tfsEntityAttributes", SQLTable<IndexingUnit>.ToString((object) indexingUnit.TFSEntityAttributes, typeof (TFSEntityAttributes), this.m_tfsEntityAttributesKnownTypes), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@properties", SQLTable<IndexingUnit>.ToString((object) indexingUnit.Properties, typeof (IndexingProperties), this.m_indexingPropertiesKnownTypes), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null)
          {
            if (current.Items.Count == 1)
              return resultCollection.GetCurrent<IndexingUnit>().Items[0];
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update Indexing Unit with ID {0}, TFSEntityId {1}  and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update Indexing Unit with ID {0}, TFSEntityId {1}  and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
    }
  }
}
