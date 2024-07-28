// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DeploymentIssuesTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class DeploymentIssuesTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[2]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("IssueType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("IssueText", SqlDbType.NVarChar, -1L)
    };

    public static void BindDeploymentIssuesTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Issue> issues)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DeploymentIssue", DeploymentIssuesTable.GetSqlDataRecords(issues));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<Issue> rows)
    {
      rows = rows ?? Enumerable.Empty<Issue>();
      foreach (Issue issue in rows.Where<Issue>((System.Func<Issue, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DeploymentIssuesTable.SqlMetaData);
        sqlDataRecord.SetByte(ordinal, (byte) issue.IssueType);
        int num;
        sqlDataRecord.SetString(num = ordinal + 1, string.IsNullOrEmpty(issue.Message) ? string.Empty : issue.Message);
        yield return sqlDataRecord;
      }
    }
  }
}
