// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DeploymentsEnvironmentsFilterTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class DeploymentsEnvironmentsFilterTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[2]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Id1", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Id2", SqlDbType.Int)
    };

    public static void BindDeploymentsEnvironmentsFilterTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironmentReference> environments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_Int32Int32Table", DeploymentsEnvironmentsFilterTable.GetSqlRecords(environments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlRecords(
      IEnumerable<DefinitionEnvironmentReference> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironmentReference>();
      foreach (DefinitionEnvironmentReference environmentReference in rows.Where<DefinitionEnvironmentReference>((System.Func<DefinitionEnvironmentReference, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlRecord = new SqlDataRecord(DeploymentsEnvironmentsFilterTable.SqlMetaData);
        sqlRecord.SetInt32(ordinal, environmentReference.ReleaseDefinitionId);
        int num;
        sqlRecord.SetInt32(num = ordinal + 1, environmentReference.DefinitionEnvironmentId);
        yield return sqlRecord;
      }
    }
  }
}
