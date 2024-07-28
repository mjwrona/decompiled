// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.AutoTriggerIssuesTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class AutoTriggerIssuesTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[5]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("buildId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("releaseDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("issueSource", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("issueType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("issueMessage", SqlDbType.NVarChar, 4000L)
    };

    public static void BindAutoTriggerIssuesTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<AutoTriggerIssue> autoTriggerIssues)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_AutoTriggerIssue", AutoTriggerIssuesTable.GetSqlDataRecords(autoTriggerIssues));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<AutoTriggerIssue> rows)
    {
      rows = rows ?? Enumerable.Empty<AutoTriggerIssue>();
      foreach (AutoTriggerIssue autoTriggerIssue in rows.Where<AutoTriggerIssue>((System.Func<AutoTriggerIssue, bool>) (r => r != null)))
      {
        int ordinal = 0;
        if ((autoTriggerIssue.ReleaseTriggerType == ReleaseTriggerType.ArtifactSource || autoTriggerIssue.ReleaseTriggerType == ReleaseTriggerType.SourceRepo) && autoTriggerIssue is ContinuousDeploymentTriggerIssue deploymentTriggerIssue)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(AutoTriggerIssuesTable.SqlMetaData);
          sqlDataRecord.SetInt32(ordinal, int.Parse(deploymentTriggerIssue.ArtifactVersionId, (IFormatProvider) CultureInfo.InvariantCulture));
          int num1;
          sqlDataRecord.SetInt32(num1 = ordinal + 1, deploymentTriggerIssue.ReleaseDefinitionId);
          int num2;
          sqlDataRecord.SetByte(num2 = num1 + 1, (byte) deploymentTriggerIssue.IssueSource);
          int num3;
          sqlDataRecord.SetByte(num3 = num2 + 1, (byte) deploymentTriggerIssue.Issue.IssueType);
          int num4;
          sqlDataRecord.SetString(num4 = num3 + 1, string.IsNullOrEmpty(deploymentTriggerIssue.Issue.Message) ? string.Empty : deploymentTriggerIssue.Issue.Message);
          yield return sqlDataRecord;
        }
      }
    }
  }
}
