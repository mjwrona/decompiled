// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CommandComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CommandComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<CommandComponent>(1),
      (IComponentCreator) new ComponentCreator<CommandComponent2>(2),
      (IComponentCreator) new ComponentCreator<CommandComponent3>(3),
      (IComponentCreator) new ComponentCreator<CommandComponent4>(4),
      (IComponentCreator) new ComponentCreator<CommandComponent5>(5),
      (IComponentCreator) new ComponentCreator<CommandComponent6>(6),
      (IComponentCreator) new ComponentCreator<CommandComponent7>(7)
    }, "Command");
    private static readonly string s_area = nameof (CommandComponent);

    protected override string TraceArea => CommandComponent.s_area;

    public virtual ResultCollection GetActivityLogEntry(int CommandId)
    {
      try
      {
        this.TraceEnter(70210, nameof (GetActivityLogEntry));
        this.PrepareStoredProcedure("prc_GetActivityLogCommand");
        this.BindPartitionId();
        this.BindInt("@commandId", CommandId);
        ResultCollection activityLogEntry = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        activityLogEntry.AddBinder<ActivityLogEntry>((ObjectBinder<ActivityLogEntry>) this.GetActivityLogEntryColumns());
        activityLogEntry.AddBinder<ActivityLogParameter>((ObjectBinder<ActivityLogParameter>) new ActivityLogParameterColumns(this.RequestContext));
        return activityLogEntry;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (GetActivityLogEntry));
      }
    }

    protected virtual ActivityLogEntryColumns GetActivityLogEntryColumns() => new ActivityLogEntryColumns(this.RequestContext);

    protected virtual void BindPartitionId()
    {
    }

    protected virtual void BindPartitionIdText(StringBuilder sqlStmt) => sqlStmt.Append(" WHERE 1 = 1 ");

    protected virtual void BindSortText(StringBuilder sqlStmt, ActivityLogColumns column)
    {
      switch (column)
      {
        case ActivityLogColumns.CommandId:
          sqlStmt.Append(" CommandId ");
          break;
        case ActivityLogColumns.Application:
          sqlStmt.Append(" Application ");
          break;
        case ActivityLogColumns.Command:
          sqlStmt.Append(" Command ");
          break;
        case ActivityLogColumns.Status:
          sqlStmt.Append(" Status ");
          break;
        case ActivityLogColumns.StartTime:
          sqlStmt.Append(" StartTime ");
          break;
        case ActivityLogColumns.ExecutionTime:
          sqlStmt.Append(" ExecutionTime ");
          break;
        case ActivityLogColumns.IdentityName:
          sqlStmt.Append(" IdentityName ");
          break;
        case ActivityLogColumns.IPAddress:
          sqlStmt.Append(" IPAddress ");
          break;
        case ActivityLogColumns.UniqueIdentifier:
          sqlStmt.Append(" UniqueIdentifier ");
          break;
        case ActivityLogColumns.UserAgent:
          sqlStmt.Append(" UserAgent ");
          break;
        case ActivityLogColumns.CommandIdentifier:
          sqlStmt.Append(" CommandIdentifier ");
          break;
        case ActivityLogColumns.ExecutionCount:
          sqlStmt.Append(" ExecutionCount ");
          break;
      }
    }

    public virtual ResultCollection QueryActivityLogIds(
      string identitiy,
      int limit,
      IEnumerable<KeyValuePair<ActivityLogColumns, SortOrder>> sortColumns)
    {
      try
      {
        this.TraceEnter(70225, "QueryActivityLog");
        StringBuilder sqlStmt = new StringBuilder();
        sqlStmt.Append("SELECT ");
        if (limit != -1)
          sqlStmt.AppendFormat("TOP ({0}) ", (object) limit);
        sqlStmt.Append("CommandId FROM tbl_Command ");
        this.BindPartitionIdText(sqlStmt);
        if (!string.IsNullOrEmpty(identitiy))
          sqlStmt.Append(" AND IdentityName = @identity ");
        int num = 0;
        foreach (KeyValuePair<ActivityLogColumns, SortOrder> sortColumn in sortColumns)
        {
          if (num++ == 0)
            sqlStmt.Append(" ORDER BY ");
          else
            sqlStmt.Append(", ");
          this.BindSortText(sqlStmt, sortColumn.Key);
          if (sortColumn.Value == SortOrder.Ascending)
            sqlStmt.Append(" ASC ");
          else
            sqlStmt.Append(" DESC ");
        }
        this.PrepareSqlBatch(sqlStmt.Length);
        this.AddStatement(sqlStmt.ToString(), 1);
        this.BindPartitionId();
        this.BindString("@identity", identitiy, 75, true, SqlDbType.NVarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<int>((ObjectBinder<int>) new CommandIdColumn());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70300, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70305, "QueryActivityLog");
      }
    }

    public virtual ResultCollection QueryActivitylogEntries(int[] ids)
    {
      try
      {
        this.TraceEnter(70310, nameof (QueryActivitylogEntries));
        this.PrepareStoredProcedure("prc_QueryCommandsById");
        this.BindPartitionId();
        this.BindInt32Table("@commandIds", (IEnumerable<int>) ids);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ActivityLogEntry>((ObjectBinder<ActivityLogEntry>) this.GetActivityLogEntryColumns());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70315, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70320, nameof (QueryActivitylogEntries));
      }
    }

    public virtual List<CommandCount> QueryApplicationCommandCount(
      DateTime startTime,
      DateTime endTime)
    {
      try
      {
        this.TraceEnter(70330, nameof (QueryApplicationCommandCount));
        string str = "stmt_QueryCommandCountPerApplication.sql";
        string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(str);
        this.PrepareSqlBatch(resourceAsString.Length);
        this.AddStatement(resourceAsString);
        this.BindPartitionId();
        this.BindDateTime2("@startTime", startTime);
        this.BindDateTime2("@endTime", endTime);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext);
        resultCollection.AddBinder<CommandCount>((ObjectBinder<CommandCount>) new CommandCountBinder());
        return resultCollection.GetCurrent<CommandCount>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(70335, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70340, nameof (QueryApplicationCommandCount));
      }
    }

    public virtual List<CommandCount> QueryIdentityCommandCount(
      DateTime startTime,
      DateTime endTime)
    {
      try
      {
        this.TraceEnter(70350, nameof (QueryIdentityCommandCount));
        string str = "stmt_QueryCommandCountPerIdentity.sql";
        string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(str);
        this.PrepareSqlBatch(resourceAsString.Length);
        this.AddStatement(resourceAsString);
        this.BindPartitionId();
        this.BindDateTime2("@startTime", startTime);
        this.BindDateTime2("@endTime", endTime);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext);
        resultCollection.AddBinder<CommandCount>((ObjectBinder<CommandCount>) new CommandCountBinder());
        return resultCollection.GetCurrent<CommandCount>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(70355, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70360, nameof (QueryIdentityCommandCount));
      }
    }
  }
}
