// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupScopeTableValuedParameterExtensions
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
  internal static class GroupScopeTableValuedParameterExtensions
  {
    public static SqlParameter BindGroupScopeTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityScope> rows)
    {
      return GroupScopeTableValuedParameterExtensions.GroupScopeTableVersionHelper.Given(component, parameterName, rows).BindTableTyp_GroupScopeTable();
    }

    public static SqlParameter BindGroupScopeTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityScope> rows)
    {
      return GroupScopeTableValuedParameterExtensions.GroupScopeTableVersionHelper.Given(component, parameterName, rows).BindTableTyp_GroupScopeTable2();
    }

    private class GroupScopeTableVersionHelper : TableValuedParameterVersionHelper<IdentityScope>
    {
      private static readonly SqlMetaData[] typ_GroupScopeTable = new SqlMetaData[6]
      {
        new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LocalScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ParentScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
        new SqlMetaData("SecuringHostId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ScopeType", SqlDbType.TinyInt)
      };
      private static readonly SqlMetaData[] typ_GroupScopeTable2 = new SqlMetaData[7]
      {
        new SqlMetaData("ScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LocalScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ParentScopeId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
        new SqlMetaData("SecuringHostId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ScopeType", SqlDbType.TinyInt),
        new SqlMetaData("Active", SqlDbType.Bit)
      };

      private GroupScopeTableVersionHelper(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<IdentityScope> rows)
        : base(component, parameterName, rows)
      {
      }

      private void BindForTyp_GroupScopeTable(SqlDataRecord record, IdentityScope scope)
      {
        record.SetGuid(0, scope.Id);
        record.SetGuid(1, scope.LocalScopeId);
        record.SetNullableGuid(2, scope.ParentId);
        record.SetString(3, DBPath.UserToDatabasePath(scope.Name));
        record.SetGuid(4, scope.SecuringHostId);
        record.SetByte(5, (byte) scope.ScopeType);
      }

      private void BindForTyp_GroupScopeTable2(SqlDataRecord record, IdentityScope scope)
      {
        this.BindForTyp_GroupScopeTable(record, scope);
        record.SetBoolean(6, scope.IsActive);
      }

      public static GroupScopeTableValuedParameterExtensions.GroupScopeTableVersionHelper Given(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<IdentityScope> rows)
      {
        return new GroupScopeTableValuedParameterExtensions.GroupScopeTableVersionHelper(component, parameterName, rows);
      }

      public SqlParameter BindTableTyp_GroupScopeTable() => this.BindTable(GroupScopeTableValuedParameterExtensions.GroupScopeTableVersionHelper.typ_GroupScopeTable, "typ_GroupScopeTable", new Action<SqlDataRecord, IdentityScope>(this.BindForTyp_GroupScopeTable));

      public SqlParameter BindTableTyp_GroupScopeTable2() => this.BindTable(GroupScopeTableValuedParameterExtensions.GroupScopeTableVersionHelper.typ_GroupScopeTable2, "typ_GroupScopeTable2", new Action<SqlDataRecord, IdentityScope>(this.BindForTyp_GroupScopeTable2));
    }
  }
}
