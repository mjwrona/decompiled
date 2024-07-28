// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WhitelistingComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class WhitelistingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<WhitelistingComponent>(1),
      (IComponentCreator) new ComponentCreator<WhitelistingComponent>(2),
      (IComponentCreator) new ComponentCreator<WhitelistingComponent>(3)
    }, "Whitelisting");
    private static readonly SqlMetaData[] typ_SlowCommandTable = new SqlMetaData[4]
    {
      new SqlMetaData("Application", SqlDbType.VarChar, 128L),
      new SqlMetaData("Command", SqlDbType.VarChar, 1024L),
      new SqlMetaData("ExecutionTimeThreshold", SqlDbType.BigInt),
      new SqlMetaData("Note", SqlDbType.NVarChar, 2048L)
    };
    private static readonly SqlMetaData[] typ_ExpectedExceptionTable = new SqlMetaData[2]
    {
      new SqlMetaData("Exception", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Note", SqlDbType.NVarChar, 2048L)
    };
    private static readonly SqlMetaData[] typ_ExpectedExceptionTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("Exception", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Note", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Area", SqlDbType.NVarChar, 128L),
      new SqlMetaData("DynamicOnly", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_InteractiveUserAgentTable = new SqlMetaData[2]
    {
      new SqlMetaData("UserAgent", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Note", SqlDbType.NVarChar, 2048L)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public virtual ResultCollection GetSlowCommands()
    {
      this.PrepareStoredProcedure("prc_QuerySlowCommand");
      ResultCollection slowCommands = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySlowCommand", this.RequestContext);
      slowCommands.AddBinder<SlowCommandDefinition>((ObjectBinder<SlowCommandDefinition>) new WhitelistingComponent.SlowCommandDefinitionBinder());
      return slowCommands;
    }

    public virtual void UpdateSlowCommands(string commandSource, List<SlowCommandUpdate> updates)
    {
      this.PrepareStoredProcedure("prc_UpdateSlowCommands");
      this.BindString("@commandSource", commandSource, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindSlowCommandTable("@updates", (IEnumerable<SlowCommandUpdate>) updates);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection GetExpectedExceptions()
    {
      this.PrepareStoredProcedure("prc_QueryExpectedException");
      ResultCollection expectedExceptions = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryExpectedException", this.RequestContext);
      expectedExceptions.AddBinder<ExpectedExceptionType>((ObjectBinder<ExpectedExceptionType>) new WhitelistingComponent.ExpectedExceptionDefinitionBinder());
      return expectedExceptions;
    }

    public virtual void UpdateExpectedException(
      string exceptionSource,
      List<ExpectedExceptionUpdate> updates)
    {
      this.PrepareStoredProcedure("prc_UpdateExpectedExceptions");
      this.BindString("@exceptionSource", exceptionSource, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (this.Version == 1)
        this.BindExpectedExceptionTable("@updates", (IEnumerable<ExpectedExceptionUpdate>) updates);
      else
        this.BindExpectedExceptionTable2("@updates", (IEnumerable<ExpectedExceptionUpdate>) updates);
      this.ExecuteNonQuery();
    }

    public virtual List<string> GetInteractiveUserAgents()
    {
      if (this.Version < 3)
        return new List<string>();
      this.PrepareStoredProcedure("prc_QueryInteractiveUserAgents");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new ProjectionBinder<string>((System.Func<SqlDataReader, string>) (reader => reader.GetString(0))));
        return resultCollection.GetCurrent<string>().Items;
      }
    }

    public virtual void UpdateInteractiveUserAgents(
      string interactiveUserAgentsSource,
      List<InteractiveUserAgentUpdate> interactiveUserAgents)
    {
      if (this.Version < 3)
        return;
      this.PrepareStoredProcedure("prc_UpdateInteractiveUserAgents");
      this.BindString("@userAgentSource", interactiveUserAgentsSource, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInteractiveUserAgentsTable("@updates", (IEnumerable<InteractiveUserAgentUpdate>) interactiveUserAgents);
      this.ExecuteNonQuery();
    }

    private SqlParameter BindSlowCommandTable(
      string parameterName,
      IEnumerable<SlowCommandUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<SlowCommandUpdate>();
      System.Func<SlowCommandUpdate, SqlDataRecord> selector = (System.Func<SlowCommandUpdate, SqlDataRecord>) (commandUpdate =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(WhitelistingComponent.typ_SlowCommandTable);
        sqlDataRecord.SetString(0, commandUpdate.Application);
        sqlDataRecord.SetString(1, commandUpdate.Command);
        sqlDataRecord.SetSqlInt64(2, (SqlInt64) commandUpdate.ExecutionTimeThreshold);
        sqlDataRecord.SetString(3, commandUpdate.Note);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_SlowCommandTable", rows.Select<SlowCommandUpdate, SqlDataRecord>(selector));
    }

    private SqlParameter BindExpectedExceptionTable(
      string parameterName,
      IEnumerable<ExpectedExceptionUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<ExpectedExceptionUpdate>();
      HashSet<string> distinctExceptionTypes = new HashSet<string>();
      rows = (IEnumerable<ExpectedExceptionUpdate>) rows.Where<ExpectedExceptionUpdate>((System.Func<ExpectedExceptionUpdate, bool>) (x => distinctExceptionTypes.Add(x.ExceptionType))).ToList<ExpectedExceptionUpdate>();
      System.Func<ExpectedExceptionUpdate, SqlDataRecord> selector = (System.Func<ExpectedExceptionUpdate, SqlDataRecord>) (exceptionUpdate =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(WhitelistingComponent.typ_ExpectedExceptionTable);
        sqlDataRecord.SetString(0, exceptionUpdate.ExceptionType);
        sqlDataRecord.SetString(1, exceptionUpdate.Note);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ExpectedExceptionTable", (IEnumerable<SqlDataRecord>) rows.Select<ExpectedExceptionUpdate, SqlDataRecord>(selector).ToArray<SqlDataRecord>());
    }

    private SqlParameter BindExpectedExceptionTable2(
      string parameterName,
      IEnumerable<ExpectedExceptionUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<ExpectedExceptionUpdate>();
      System.Func<ExpectedExceptionUpdate, SqlDataRecord> selector = (System.Func<ExpectedExceptionUpdate, SqlDataRecord>) (exceptionUpdate =>
      {
        SqlDataRecord record = new SqlDataRecord(WhitelistingComponent.typ_ExpectedExceptionTable2);
        record.SetString(0, exceptionUpdate.ExceptionType);
        record.SetString(1, exceptionUpdate.Note);
        record.SetNullableString(2, exceptionUpdate.Area);
        record.SetBoolean(3, exceptionUpdate.DynamicOnly);
        return record;
      });
      return this.BindTable(parameterName, "typ_ExpectedExceptionTable2", rows.Select<ExpectedExceptionUpdate, SqlDataRecord>(selector));
    }

    private SqlParameter BindInteractiveUserAgentsTable(
      string parameterName,
      IEnumerable<InteractiveUserAgentUpdate> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<InteractiveUserAgentUpdate>();
      HashSet<string> distinctUserAgents = new HashSet<string>();
      rows = rows.Where<InteractiveUserAgentUpdate>((System.Func<InteractiveUserAgentUpdate, bool>) (x => distinctUserAgents.Add(x.UserAgent)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.BindTable(parameterName, "typ_InteractiveUserAgentTable", (IEnumerable<SqlDataRecord>) rows.Select<InteractiveUserAgentUpdate, SqlDataRecord>(WhitelistingComponent.\u003C\u003EO.\u003C0\u003E__rowBinder ?? (WhitelistingComponent.\u003C\u003EO.\u003C0\u003E__rowBinder = new System.Func<InteractiveUserAgentUpdate, SqlDataRecord>(rowBinder))).ToArray<SqlDataRecord>());

      static SqlDataRecord rowBinder(InteractiveUserAgentUpdate update)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(WhitelistingComponent.typ_InteractiveUserAgentTable);
        sqlDataRecord.SetString(0, update.UserAgent);
        sqlDataRecord.SetString(1, update.Note);
        return sqlDataRecord;
      }
    }

    internal class SlowCommandDefinitionBinder : ObjectBinder<SlowCommandDefinition>
    {
      protected SqlColumnBinder Application = new SqlColumnBinder(nameof (Application));
      protected SqlColumnBinder Command = new SqlColumnBinder(nameof (Command));
      protected SqlColumnBinder ExecutionTimeThreshold = new SqlColumnBinder(nameof (ExecutionTimeThreshold));

      internal SlowCommandDefinitionBinder()
      {
      }

      internal void Bind(out SlowCommandDefinition result) => result = this.Bind();

      protected override SlowCommandDefinition Bind() => new SlowCommandDefinition()
      {
        Application = this.Application.GetString((IDataReader) this.Reader, false),
        Command = this.Command.GetString((IDataReader) this.Reader, false),
        ExecutionTimeThreshold = this.ExecutionTimeThreshold.GetInt64((IDataReader) this.Reader, long.MaxValue)
      };
    }

    internal class ExpectedExceptionDefinitionBinder : ObjectBinder<ExpectedExceptionType>
    {
      protected SqlColumnBinder ExceptionType = new SqlColumnBinder(nameof (ExceptionType));
      protected SqlColumnBinder Area = new SqlColumnBinder(nameof (Area));
      protected SqlColumnBinder DynamicOnly = new SqlColumnBinder(nameof (DynamicOnly));

      internal ExpectedExceptionDefinitionBinder()
      {
      }

      internal void Bind(out ExpectedExceptionType result) => result = this.Bind();

      protected override ExpectedExceptionType Bind()
      {
        ExpectedExceptionType expectedExceptionType = new ExpectedExceptionType();
        expectedExceptionType.Type = this.ExceptionType.GetString((IDataReader) this.Reader, false);
        if (this.DynamicOnly.ColumnExists((IDataReader) this.Reader))
          expectedExceptionType.DynamicOnly = this.DynamicOnly.GetBoolean((IDataReader) this.Reader, false);
        if (this.Area.ColumnExists((IDataReader) this.Reader))
          expectedExceptionType.Area = this.Area.GetString((IDataReader) this.Reader, (string) null);
        return expectedExceptionType;
      }
    }
  }
}
