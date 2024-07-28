// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class KpiComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<KpiComponent>(1),
      (IComponentCreator) new ComponentCreator<KpiComponent.KpiComponent2>(2)
    }, "Kpi");
    private static readonly SqlMetaData[] typ_KpiStateTable = new SqlMetaData[3]
    {
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("Limit", SqlDbType.Float),
      new SqlMetaData("EventId", SqlDbType.Int)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static KpiComponent()
    {
      KpiComponent.s_sqlExceptionFactories.Add(800093, new SqlExceptionFactory(typeof (KpiExistsException)));
      KpiComponent.s_sqlExceptionFactories.Add(800094, new SqlExceptionFactory(typeof (KpiNotFoundException)));
    }

    private SqlParameter BindKpiStateTable(
      string parameterName,
      IEnumerable<KpiStateDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<KpiStateDefinition>();
      System.Func<KpiStateDefinition, SqlDataRecord> selector = (System.Func<KpiStateDefinition, SqlDataRecord>) (state =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(KpiComponent.typ_KpiStateTable);
        sqlDataRecord.SetByte(0, (byte) state.KpiState);
        sqlDataRecord.SetDouble(1, state.Limit);
        sqlDataRecord.SetInt32(2, state.EventId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_KpiStateTable", rows.Select<KpiStateDefinition, SqlDataRecord>(selector));
    }

    public virtual void DeleteKpiStates(KpiDefinition definition) => throw new NotImplementedException();

    public void SaveKpi(KpiDefinition definition)
    {
      this.PrepareStoredProcedure("prc_SaveKpi");
      this.BindString("@area", definition.Area, 128, false, SqlDbType.NVarChar);
      this.BindString("@name", definition.Name, 128, false, SqlDbType.NVarChar);
      this.BindString("@scope", definition.Scope, 128, true, SqlDbType.NVarChar);
      this.BindString("@displayName", definition.DisplayName, 128, false, SqlDbType.NVarChar);
      this.BindString("@description", definition.Description, -1, false, SqlDbType.NVarChar);
      this.BindBoolean("@higherIsBetter", definition.HigherIsBetter);
      this.ExecuteNonQuery();
    }

    public void SetStates(string area, string name, string scope, List<KpiStateDefinition> states)
    {
      this.PrepareStoredProcedure("prc_SetKpiStates");
      this.BindString("@area", area, 128, false, SqlDbType.NVarChar);
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.BindString("@scope", scope, 128, true, SqlDbType.NVarChar);
      this.BindKpiStateTable("@states", (IEnumerable<KpiStateDefinition>) states);
      this.ExecuteNonQuery();
    }

    public ResultCollection GetKpi(string area, string name, string scope)
    {
      this.PrepareStoredProcedure("prc_GetKpi");
      this.BindString("@area", area, 128, false, SqlDbType.NVarChar);
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.BindString("@scope", scope, 128, true, SqlDbType.NVarChar);
      ResultCollection kpi = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetKpi", this.RequestContext);
      kpi.AddBinder<KpiDefinition>((ObjectBinder<KpiDefinition>) new KpiComponent.KpiDefinitionBinder());
      kpi.AddBinder<KpiStateDefinition>((ObjectBinder<KpiStateDefinition>) new KpiComponent.KpiStateDefinitionBinder());
      return kpi;
    }

    internal class KpiDefinitionBinder : ObjectBinder<KpiDefinition>
    {
      protected SqlColumnBinder KpiId = new SqlColumnBinder(nameof (KpiId));
      protected SqlColumnBinder Area = new SqlColumnBinder(nameof (Area));
      protected SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      protected SqlColumnBinder Scope = new SqlColumnBinder(nameof (Scope));
      protected SqlColumnBinder DisplayName = new SqlColumnBinder(nameof (DisplayName));
      protected SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      protected SqlColumnBinder HigherIsBetter = new SqlColumnBinder(nameof (HigherIsBetter));

      internal KpiDefinitionBinder()
      {
      }

      internal void Bind(out KpiDefinition result) => result = this.Bind();

      protected override KpiDefinition Bind() => new KpiDefinition(this.KpiId.GetInt32((IDataReader) this.Reader))
      {
        Area = this.Area.GetString((IDataReader) this.Reader, false),
        Name = this.Name.GetString((IDataReader) this.Reader, false),
        Scope = this.Scope.GetString((IDataReader) this.Reader, true),
        DisplayName = this.DisplayName.GetString((IDataReader) this.Reader, false),
        Description = this.Description.GetString((IDataReader) this.Reader, true),
        HigherIsBetter = this.HigherIsBetter.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class KpiComponent2 : KpiComponent
    {
      public override void DeleteKpiStates(KpiDefinition definition)
      {
        this.PrepareStoredProcedure("prc_DeleteKpiStates");
        this.BindString("@area", definition.Area, 128, false, SqlDbType.NVarChar);
        this.BindString("@name", definition.Name, 128, false, SqlDbType.NVarChar);
        this.BindString("@scope", definition.Scope, 128, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    internal class KpiStateDefinitionBinder : ObjectBinder<KpiStateDefinition>
    {
      protected SqlColumnBinder KpiId = new SqlColumnBinder(nameof (KpiId));
      protected SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      protected SqlColumnBinder Limit = new SqlColumnBinder(nameof (Limit));
      protected SqlColumnBinder EventId = new SqlColumnBinder(nameof (EventId));

      internal KpiStateDefinitionBinder()
      {
      }

      internal KpiStateDefinitionBinder(SqlDataReader dataReader, string storedProcedure)
        : base(dataReader, storedProcedure)
      {
      }

      internal void Bind(out KpiStateDefinition result) => result = this.Bind();

      protected override KpiStateDefinition Bind() => new KpiStateDefinition(this.KpiId.GetInt32((IDataReader) this.Reader))
      {
        KpiState = (KpiState) this.State.GetByte((IDataReader) this.Reader),
        Limit = this.Limit.GetDouble((IDataReader) this.Reader),
        EventId = this.EventId.GetInt32((IDataReader) this.Reader, 0)
      };
    }
  }
}
