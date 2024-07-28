// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV10
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV10 : IndexingUnitChangeEventComponentV9
  {
    public IndexingUnitChangeEventComponentV10()
    {
    }

    internal IndexingUnitChangeEventComponentV10(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public virtual int GetCountOfIndexingUnitChangeEvents(
      IndexingUnitChangeEventState state,
      int numberOfDays)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryCountOfEventsCreatedXDaysAgoByState");
        this.BindString("@state", state.ToString(), 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindInt("@numberOfDays", numberOfDays);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve count of long pending IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }
  }
}
