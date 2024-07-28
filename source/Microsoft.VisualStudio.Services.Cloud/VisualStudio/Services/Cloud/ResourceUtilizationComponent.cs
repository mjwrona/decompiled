// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceUtilizationComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ResourceUtilizationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ResourceUtilizationComponent>(1)
    }, "ResourceUtilization");

    public ResourceUtilizationComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public ResourceUtilizationComponent DontBindPartitionId()
    {
      this.SelectedFeatures &= (SqlResourceComponentFeatures) 16777214;
      return this;
    }

    public int SetRUMacro(string macroName, string macroDefinition)
    {
      this.PrepareStoredProcedure("ResourceUtilization.prc_SetMacro");
      this.BindString("@macroName", macroName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@macroDefinition", macroDefinition, -1, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      return (int) this.ExecuteScalar();
    }

    public int DeleteRUMacro(string macroName)
    {
      this.PrepareStoredProcedure("ResourceUtilization.prc_DeleteMacro");
      this.BindString("@macroName", macroName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      return (int) this.ExecuteScalar();
    }

    public virtual int SetRURule(string ruleName, string ruleDefinition)
    {
      this.PrepareStoredProcedure("ResourceUtilization.prc_SetRule");
      this.BindString("@ruleName", ruleName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@ruleDefinition", ruleDefinition, -1, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      return (int) this.ExecuteScalar();
    }

    public int DeleteRURule(string ruleName)
    {
      this.PrepareStoredProcedure("ResourceUtilization.prc_DeleteRule");
      this.BindString("@ruleName", ruleName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      return (int) this.ExecuteScalar();
    }

    public virtual int SetRUThreshold(
      string ruleName,
      string entity,
      long? flag,
      long? tarpit,
      long? block,
      double? dpMagnitude,
      string note,
      DateTime? expiration)
    {
      this.PrepareStoredProcedure("ResourceUtilization.prc_SetThreshold");
      this.BindString("@ruleName", ruleName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@entity", entity ?? string.Empty, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindNullableLong("@flag", flag);
      this.BindNullableLong("@tarpit", tarpit);
      this.BindNullableLong("@block", block);
      this.BindNullableDouble("@dpMagnitude", dpMagnitude);
      this.BindString("@note", note, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindNullableDateTime("@expiration", expiration);
      return (int) this.ExecuteScalar();
    }

    public virtual int DeleteRUThreshold(string ruleName, string entity)
    {
      this.PrepareStoredProcedure("ResourceUtilization.prc_DeleteThreshold");
      this.BindString("@ruleName", ruleName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@entity", entity ?? string.Empty, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      return (int) this.ExecuteScalar();
    }

    public virtual List<RURule> ReadRURulesAndThresholds(
      out List<RUMacro> macros,
      out List<RUThreshold> thresholds,
      bool allHosts = false)
    {
      this.PrepareStoredProcedure(allHosts ? "ResourceUtilization.prc_ReadAllRulesAndThresholds" : "ResourceUtilization.prc_ReadRulesAndThresholds");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReaderWithoutBreaker(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<RUMacro>((ObjectBinder<RUMacro>) new ResourceUtilizationComponent.RUMacroBinder(allHosts));
      resultCollection.AddBinder<RURule>((ObjectBinder<RURule>) new ResourceUtilizationComponent.RURuleBinder(allHosts));
      resultCollection.AddBinder<RUThreshold>((ObjectBinder<RUThreshold>) new ResourceUtilizationComponent.RUThresholdBinder(allHosts));
      macros = resultCollection.GetCurrent<RUMacro>().Items;
      resultCollection.NextResult();
      List<RURule> items = resultCollection.GetCurrent<RURule>().Items;
      resultCollection.NextResult();
      thresholds = resultCollection.GetCurrent<RUThreshold>().Items;
      return items;
    }

    protected SqlDataReader ExecuteReaderWithoutBreaker()
    {
      this.ExecuteCommand(ExecuteType.ExecuteReader, CommandBehavior.Default, "SQL");
      return this.DataReader;
    }

    private class RUMacroBinder : ObjectBinder<RUMacro>
    {
      private bool m_includeHostInfo;
      private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
      private SqlColumnBinder ServiceHostIdColumn = new SqlColumnBinder("ServiceHostId");
      private SqlColumnBinder MacroNameColumn = new SqlColumnBinder("MacroName");
      private SqlColumnBinder MacroDefinitionColumn = new SqlColumnBinder("MacroDefinition");

      public RUMacroBinder(bool includeHostInfo = false) => this.m_includeHostInfo = includeHostInfo;

      protected override RUMacro Bind()
      {
        RUMacro ruMacro = new RUMacro();
        ruMacro.PartitionId = this.m_includeHostInfo ? this.PartitionIdColumn.GetInt32((IDataReader) this.Reader) : 0;
        ruMacro.HostId = this.m_includeHostInfo ? this.ServiceHostIdColumn.GetGuid((IDataReader) this.Reader) : Guid.Empty;
        ruMacro.MacroName = this.MacroNameColumn.GetString((IDataReader) this.Reader, true);
        ruMacro.MacroDefinition = this.MacroDefinitionColumn.GetString((IDataReader) this.Reader, true);
        return ruMacro;
      }
    }

    private class RURuleBinder : ObjectBinder<RURule>
    {
      private bool m_includeHostInfo;
      private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
      private SqlColumnBinder ServiceHostIdColumn = new SqlColumnBinder("ServiceHostId");
      private SqlColumnBinder RuleNameColumn = new SqlColumnBinder("RuleName");
      private SqlColumnBinder RuleDefinitionColumn = new SqlColumnBinder("RuleDefinition");

      public RURuleBinder(bool includeHostInfo = false) => this.m_includeHostInfo = includeHostInfo;

      protected override RURule Bind()
      {
        RURule ruRule = new RURule();
        ruRule.PartitionId = this.m_includeHostInfo ? this.PartitionIdColumn.GetInt32((IDataReader) this.Reader) : 0;
        ruRule.HostId = this.m_includeHostInfo ? this.ServiceHostIdColumn.GetGuid((IDataReader) this.Reader) : Guid.Empty;
        ruRule.RuleName = this.RuleNameColumn.GetString((IDataReader) this.Reader, true);
        ruRule.RuleDefinition = this.RuleDefinitionColumn.GetString((IDataReader) this.Reader, true);
        return ruRule;
      }
    }

    private class RUThresholdBinder : ObjectBinder<RUThreshold>
    {
      private bool m_includeHostInfo;
      private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
      private SqlColumnBinder ServiceHostIdColumn = new SqlColumnBinder("ServiceHostId");
      private SqlColumnBinder RuleNameColumn = new SqlColumnBinder("RuleName");
      private SqlColumnBinder EntityColumn = new SqlColumnBinder("Entity");
      private SqlColumnBinder FlagColumn = new SqlColumnBinder("Flag");
      private SqlColumnBinder TarpitColumn = new SqlColumnBinder("Tarpit");
      private SqlColumnBinder BlockColumn = new SqlColumnBinder("Block");
      private SqlColumnBinder DPMagnitudeColumn = new SqlColumnBinder("DPMagnitude");
      private SqlColumnBinder NoteColumn = new SqlColumnBinder("Note");
      private SqlColumnBinder LastModificationColumn = new SqlColumnBinder("LastModification");
      private SqlColumnBinder ExpirationColumn = new SqlColumnBinder("Expiration");

      public RUThresholdBinder(bool includeHostInfo = false) => this.m_includeHostInfo = includeHostInfo;

      protected override RUThreshold Bind()
      {
        RUThreshold ruThreshold = new RUThreshold();
        ruThreshold.PartitionId = this.m_includeHostInfo ? this.PartitionIdColumn.GetInt32((IDataReader) this.Reader) : 0;
        ruThreshold.HostId = this.m_includeHostInfo ? this.ServiceHostIdColumn.GetGuid((IDataReader) this.Reader) : Guid.Empty;
        ruThreshold.RuleName = this.RuleNameColumn.GetString((IDataReader) this.Reader, false);
        ruThreshold.Entity = this.EntityColumn.GetString((IDataReader) this.Reader, true);
        ruThreshold.Flag = this.FlagColumn.GetNullableInt64((IDataReader) this.Reader);
        ruThreshold.Tarpit = this.TarpitColumn.GetNullableInt64((IDataReader) this.Reader);
        ruThreshold.Block = this.BlockColumn.GetNullableInt64((IDataReader) this.Reader);
        ruThreshold.DPMagnitude = this.DPMagnitudeColumn.GetNullableDouble((IDataReader) this.Reader);
        ruThreshold.Note = this.NoteColumn.GetString((IDataReader) this.Reader, true);
        ruThreshold.LastModification = this.LastModificationColumn.GetDateTime((IDataReader) this.Reader);
        ruThreshold.Expiration = new DateTime?(this.ExpirationColumn.GetDateTime((IDataReader) this.Reader));
        return ruThreshold;
      }
    }
  }
}
