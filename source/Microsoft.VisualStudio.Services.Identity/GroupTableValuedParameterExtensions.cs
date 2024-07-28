// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class GroupTableValuedParameterExtensions
  {
    public static SqlParameter BindGroupTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<GroupDescription> rows)
    {
      return GroupTableValuedParameterExtensions.GroupTableVersionHelper.Given(component, parameterName, rows).BindTableTyp_GroupTable();
    }

    public static SqlParameter BindGroupTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<GroupDescription> rows)
    {
      return GroupTableValuedParameterExtensions.GroupTableVersionHelper.Given(component, parameterName, rows).BindTableTyp_GroupTable2();
    }

    public static SqlParameter BindGroupTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<GroupDescription> rows)
    {
      return GroupTableValuedParameterExtensions.GroupTableVersionHelper.Given(component, parameterName, rows).BindTableTyp_GroupTable3();
    }

    private class GroupTableVersionHelper : TableValuedParameterVersionHelper<GroupDescription>
    {
      private static readonly SqlMetaData[] typ_GroupTable = new SqlMetaData[8]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("SpecialType", SqlDbType.Int),
        new SqlMetaData("ScopeLocal", SqlDbType.Bit),
        new SqlMetaData("RestrictedView", SqlDbType.Bit)
      };
      private static readonly SqlMetaData[] typ_GroupTable2 = new SqlMetaData[9]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("SpecialType", SqlDbType.Int),
        new SqlMetaData("ScopeLocal", SqlDbType.Bit),
        new SqlMetaData("RestrictedView", SqlDbType.Bit),
        new SqlMetaData("Active", SqlDbType.Bit)
      };
      private static readonly SqlMetaData[] typ_GroupTable3 = new SqlMetaData[10]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("VirtualPlugin", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("SpecialType", SqlDbType.Int),
        new SqlMetaData("ScopeLocal", SqlDbType.Bit),
        new SqlMetaData("RestrictedView", SqlDbType.Bit),
        new SqlMetaData("Active", SqlDbType.Bit)
      };

      private GroupTableVersionHelper(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<GroupDescription> rows)
        : base(component, parameterName, rows)
      {
      }

      private void BindForTyp_GroupTable(SqlDataRecord record, GroupDescription group)
      {
        record.SetString(0, group.Descriptor.Identifier);
        record.SetNullableGuid(1, group.Id);
        record.SetNullableGuid(2, group.ScopeId);
        record.SetString(3, group.Name);
        record.SetString(4, group.Description, BindStringBehavior.Unchanged);
        record.SetInt32(5, (int) group.SpecialType);
        record.SetBoolean(6, group.ScopeLocal);
        record.SetBoolean(7, group.HasRestrictedVisibility);
      }

      private void BindForTyp_GroupTable2(SqlDataRecord record, GroupDescription group)
      {
        this.BindForTyp_GroupTable(record, group);
        record.SetBoolean(8, group.Active);
      }

      private void BindForTyp_GroupTable3(SqlDataRecord record, GroupDescription group)
      {
        record.SetString(0, group.Descriptor.Identifier);
        record.SetNullableGuid(1, group.Id);
        record.SetNullableGuid(2, group.ScopeId);
        record.SetString(3, group.Name);
        record.SetString(4, group.Description, BindStringBehavior.Unchanged);
        record.SetString(5, group.VirtualPlugin, BindStringBehavior.EmptyStringToNull);
        record.SetInt32(6, (int) group.SpecialType);
        record.SetBoolean(7, group.ScopeLocal);
        record.SetBoolean(8, group.HasRestrictedVisibility);
        record.SetBoolean(9, group.Active);
      }

      public static GroupTableValuedParameterExtensions.GroupTableVersionHelper Given(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<GroupDescription> rows)
      {
        return new GroupTableValuedParameterExtensions.GroupTableVersionHelper(component, parameterName, rows);
      }

      public SqlParameter BindTableTyp_GroupTable() => this.BindTable(GroupTableValuedParameterExtensions.GroupTableVersionHelper.typ_GroupTable, "typ_GroupTable", new Action<SqlDataRecord, GroupDescription>(this.BindForTyp_GroupTable));

      public SqlParameter BindTableTyp_GroupTable2() => this.BindTable(GroupTableValuedParameterExtensions.GroupTableVersionHelper.typ_GroupTable2, "typ_GroupTable2", new Action<SqlDataRecord, GroupDescription>(this.BindForTyp_GroupTable2));

      public SqlParameter BindTableTyp_GroupTable3() => this.BindTable(GroupTableValuedParameterExtensions.GroupTableVersionHelper.typ_GroupTable3, "typ_GroupTable3", new Action<SqlDataRecord, GroupDescription>(this.BindForTyp_GroupTable3));
    }
  }
}
