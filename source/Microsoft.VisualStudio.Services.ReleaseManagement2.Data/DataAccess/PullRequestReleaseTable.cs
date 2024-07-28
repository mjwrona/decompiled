// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.PullRequestReleaseTable
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
  public static class PullRequestReleaseTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[6]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("PullRequestId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("MergeCommitId", SqlDbType.Char, 40L),
      new Microsoft.SqlServer.Server.SqlMetaData("IterationId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("MergedAt", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("IsActive", SqlDbType.Bit)
    };

    public static void BindPullRequestReleaseTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<PullRequestRelease> pullRequestReleases)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_PullRequestRelease", PullRequestReleaseTable.GetSqlDataRecords(pullRequestReleases));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<PullRequestRelease> rows)
    {
      List<SqlDataRecord> sqlDataRecords = new List<SqlDataRecord>();
      if (rows == null)
        return (IEnumerable<SqlDataRecord>) sqlDataRecords;
      foreach (PullRequestRelease pullRequestRelease in rows.Where<PullRequestRelease>((System.Func<PullRequestRelease, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PullRequestReleaseTable.SqlMetaData);
        sqlDataRecord.SetInt32(ordinal, pullRequestRelease.PullRequestId);
        int num1;
        sqlDataRecord.SetInt32(num1 = ordinal + 1, pullRequestRelease.ReleaseId);
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, string.IsNullOrEmpty(pullRequestRelease.MergeCommitId) ? string.Empty : pullRequestRelease.MergeCommitId);
        int num3;
        sqlDataRecord.SetInt32(num3 = num2 + 1, pullRequestRelease.IterationId);
        int num4;
        sqlDataRecord.SetDateTime(num4 = num3 + 1, pullRequestRelease.MergedAt);
        int num5;
        sqlDataRecord.SetBoolean(num5 = num4 + 1, pullRequestRelease.IsActive);
        sqlDataRecords.Add(sqlDataRecord);
      }
      return (IEnumerable<SqlDataRecord>) sqlDataRecords;
    }
  }
}
