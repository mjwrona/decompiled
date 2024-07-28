// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitComponentV6
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class IndexingUnitComponentV6 : IndexingUnitComponentV5
  {
    public IndexingUnitComponentV6()
    {
    }

    internal IndexingUnitComponentV6(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public virtual Dictionary<Guid, Guid> GetAssociatedJobIds()
    {
      try
      {
        Dictionary<Guid, Guid> collection = new Dictionary<Guid, Guid>();
        this.PrepareStoredProcedure("Search.prc_QueryAssociatedJobIds");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new IndexingUnitComponentV6.AssociatedJobIdColumns());
          ObjectBinder<KeyValuePair<Guid, Guid>> current = resultCollection.GetCurrent<KeyValuePair<Guid, Guid>>();
          if (current != null && current.Items != null && current.Items.Count > 0)
            collection.AddRange<KeyValuePair<Guid, Guid>, Dictionary<Guid, Guid>>((IEnumerable<KeyValuePair<Guid, Guid>>) current.Items);
          return collection;
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to get AssociatedJobIds from tbl_IndexingUnits");
      }
    }

    internal class AssociatedJobIdColumns : ObjectBinder<KeyValuePair<Guid, Guid>>
    {
      private SqlColumnBinder m_tfsEntityId = new SqlColumnBinder("TFSEntityId");
      private SqlColumnBinder m_associatedJobId = new SqlColumnBinder("AssociatedJobId");

      protected override KeyValuePair<Guid, Guid> Bind() => new KeyValuePair<Guid, Guid>(this.m_tfsEntityId.GetGuid((IDataReader) this.Reader), this.m_associatedJobId.GetGuid((IDataReader) this.Reader));
    }
  }
}
