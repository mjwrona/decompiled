// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Data.CodeSenseSqlResourceComponent2
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Data
{
  internal class CodeSenseSqlResourceComponent2 : CodeSenseSqlResourceComponent
  {
    protected readonly CodeSenseSqlResourceComponent2.Columns2 columns2 = new CodeSenseSqlResourceComponent2.Columns2();

    public override AggregateDescriptor GetAggregate(string aggregatePath, Guid projectGuid)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024715, TraceLayer.ExternalSql, "GetAggregate {0}", new object[1]
      {
        (object) aggregatePath
      }))
        return this.GetAggregateCore(aggregatePath, projectGuid);
    }

    public override void AddAggregates(IEnumerable<AggregateDescriptor> aggregates)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024720, TraceLayer.ExternalSql, "AddAggregates {0}", new object[1]
      {
        (object) aggregates.Count<AggregateDescriptor>()
      }))
      {
        try
        {
          this.AddAggregatesCore(aggregates);
        }
        catch (SqlException ex1)
        {
          this.RequestContext.Trace(1024765, TraceLevel.Error, this.TraceArea, TraceLayer.ExternalSql, "AggregatorJob batch failed for slices with exception : {0}", (object) ex1.ToString());
          foreach (AggregateDescriptor aggregate in aggregates)
          {
            try
            {
              this.AddAggregatesCore((IEnumerable<AggregateDescriptor>) new List<AggregateDescriptor>((IEnumerable<AggregateDescriptor>) new AggregateDescriptor[1]
              {
                aggregate
              }));
            }
            catch (Exception ex2)
            {
              this.RequestContext.Trace(1024770, TraceLevel.Error, this.TraceArea, TraceLayer.ExternalSql, "Aggregation failed for the aggregate {0}, with exception : {1}", (object) aggregate.ToString(), (object) ex2.ToString());
              throw;
            }
          }
        }
      }
    }

    public override IEnumerable<IgnoreListedItem> GetIgnoreListedPaths()
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024730, TraceLayer.ExternalSql, "GetIgnoreListedPaths()", Array.Empty<object>()))
        return this.GetIgnoreListedPathsCore();
    }

    private AggregateDescriptor GetAggregateCore(string aggregatePath, Guid projectGuid)
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetAggregate");
      this.BindString("@aggregatePath", aggregatePath, 400, false, SqlDbType.NVarChar);
      this.BindGuid("@projectGuid", projectGuid);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AggregateDescriptor>((ObjectBinder<AggregateDescriptor>) new ProjectionBinder<AggregateDescriptor>((System.Func<SqlDataReader, AggregateDescriptor>) (r => new AggregateDescriptor(this.columns2.AggregatePath.GetString((IDataReader) r, false), this.columns.GetAggregateFileIdAggregateFileId.GetInt32((IDataReader) r), projectGuid))));
        return resultCollection.GetCurrent<AggregateDescriptor>().Items.SingleOrDefault<AggregateDescriptor>();
      }
    }

    private void AddAggregatesCore(IEnumerable<AggregateDescriptor> aggregates)
    {
      this.PrepareStoredProcedure("CodeSense.prc_AddAggregates");
      this.BindAggregateDescriptor("@itemList", aggregates);
      this.ExecuteNonQuery(false);
    }

    private IEnumerable<IgnoreListedItem> GetIgnoreListedPathsCore()
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetIgnoreList");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IgnoreListedItem>((ObjectBinder<IgnoreListedItem>) new ProjectionBinder<IgnoreListedItem>((System.Func<SqlDataReader, IgnoreListedItem>) (reader => new IgnoreListedItem(this.columns.IgnoreListServerPath.GetString((IDataReader) reader, false), this.GetDataspaceIdentifier(this.columns.DataspaceId.GetInt32((IDataReader) reader))))));
        return (IEnumerable<IgnoreListedItem>) resultCollection.GetCurrent<IgnoreListedItem>().Items.ToReadOnlyList<IgnoreListedItem>();
      }
    }

    protected class Columns2 : CodeSenseSqlResourceComponent.Columns
    {
      public SqlColumnBinder AggregatePath = new SqlColumnBinder(nameof (AggregatePath));
    }
  }
}
