// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.EnvironmentRetentionPolicyTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class EnvironmentRetentionPolicyTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[4]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int, true, false, System.Data.SqlClient.SortOrder.Unspecified, -1),
      new Microsoft.SqlServer.Server.SqlMetaData("EnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleasesToKeep", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("RetentionCutOffDate", SqlDbType.DateTime)
    };

    public static void BindEnvironmentRetentionPolicyTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_EnvironmentRetentionPolicyTable", EnvironmentRetentionPolicyTable.GetSqlDataRecords(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(EnvironmentRetentionPolicyTable.SqlMetaData);
        int num2;
        sqlDataRecord.SetInt32(num2 = num1 + 1, definitionEnvironment.Id);
        int num3;
        sqlDataRecord.SetInt32(num3 = num2 + 1, definitionEnvironment.RetentionPolicy.ReleasesToKeep);
        DateTime dateTime = DateTimeExtensions.GetMaxYesterdayDateTimeInUtc().AddDays((double) -definitionEnvironment.RetentionPolicy.DaysToKeep);
        int num4;
        sqlDataRecord.SetDateTime(num4 = num3 + 1, dateTime);
        yield return sqlDataRecord;
      }
    }
  }
}
