// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.AutoTriggerIssuesTable2
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class AutoTriggerIssuesTable2
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[9]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactType", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("SourceId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("ArtifactVersionId", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("DataspaceId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("IssueSource", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("IssueType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("IssueMessage", SqlDbType.NVarChar, 4000L)
    };

    public static void BindAutoTriggerIssuesTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<AutoTriggerIssue> autoTriggerIssues)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_AutoTriggerIssue2", AutoTriggerIssuesTable2.GetSqlDataRecords(component, autoTriggerIssues));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      TeamFoundationSqlResourceComponent component,
      IEnumerable<AutoTriggerIssue> rows)
    {
      rows = rows ?? Enumerable.Empty<AutoTriggerIssue>();
      foreach (AutoTriggerIssue autoTriggerIssue in rows.Where<AutoTriggerIssue>((System.Func<AutoTriggerIssue, bool>) (r => r != null)))
      {
        int dataspaceId = component.GetDataspaceId(autoTriggerIssue.ProjectId);
        int num1 = 0;
        if (autoTriggerIssue.ReleaseTriggerType == ReleaseTriggerType.ArtifactSource || autoTriggerIssue.ReleaseTriggerType == ReleaseTriggerType.SourceRepo || autoTriggerIssue.ReleaseTriggerType == ReleaseTriggerType.ContainerImage)
        {
          if (autoTriggerIssue is ContinuousDeploymentTriggerIssue deploymentTriggerIssue)
          {
            SqlDataRecord sqlDataRecord1 = new SqlDataRecord(AutoTriggerIssuesTable2.SqlMetaData);
            SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
            int ordinal1 = num1;
            int num2 = ordinal1 + 1;
            string artifactType = deploymentTriggerIssue.ArtifactType;
            sqlDataRecord2.SetString(ordinal1, artifactType);
            SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
            int ordinal2 = num2;
            int num3 = ordinal2 + 1;
            string str1 = string.IsNullOrEmpty(deploymentTriggerIssue.SourceId) ? string.Empty : deploymentTriggerIssue.SourceId;
            sqlDataRecord3.SetString(ordinal2, str1);
            SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
            int ordinal3 = num3;
            int num4 = ordinal3 + 1;
            string artifactVersionId = deploymentTriggerIssue.ArtifactVersionId;
            sqlDataRecord4.SetString(ordinal3, artifactVersionId);
            SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
            int ordinal4 = num4;
            int num5 = ordinal4 + 1;
            int releaseTriggerType = (int) (byte) deploymentTriggerIssue.ReleaseTriggerType;
            sqlDataRecord5.SetByte(ordinal4, (byte) releaseTriggerType);
            SqlDataRecord sqlDataRecord6 = sqlDataRecord1;
            int ordinal5 = num5;
            int num6 = ordinal5 + 1;
            int num7 = dataspaceId;
            sqlDataRecord6.SetInt32(ordinal5, num7);
            SqlDataRecord sqlDataRecord7 = sqlDataRecord1;
            int ordinal6 = num6;
            int num8 = ordinal6 + 1;
            int releaseDefinitionId = deploymentTriggerIssue.ReleaseDefinitionId;
            sqlDataRecord7.SetInt32(ordinal6, releaseDefinitionId);
            SqlDataRecord sqlDataRecord8 = sqlDataRecord1;
            int ordinal7 = num8;
            int num9 = ordinal7 + 1;
            int issueSource = (int) (byte) deploymentTriggerIssue.IssueSource;
            sqlDataRecord8.SetByte(ordinal7, (byte) issueSource);
            SqlDataRecord sqlDataRecord9 = sqlDataRecord1;
            int ordinal8 = num9;
            int num10 = ordinal8 + 1;
            int issueType = (int) (byte) deploymentTriggerIssue.Issue.IssueType;
            sqlDataRecord9.SetByte(ordinal8, (byte) issueType);
            SqlDataRecord sqlDataRecord10 = sqlDataRecord1;
            int ordinal9 = num10;
            int num11 = ordinal9 + 1;
            string str2 = string.IsNullOrEmpty(deploymentTriggerIssue.Issue.Message) ? string.Empty : deploymentTriggerIssue.Issue.Message;
            sqlDataRecord10.SetString(ordinal9, str2);
            yield return sqlDataRecord1;
          }
        }
        else
        {
          int releaseTriggerType1 = (int) autoTriggerIssue.ReleaseTriggerType;
        }
      }
    }
  }
}
