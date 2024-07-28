// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DeploymentTable2
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
  public static class DeploymentTable2
  {
    private static readonly SqlMetaData[] SqlMetaData2 = new SqlMetaData[10]
    {
      new SqlMetaData("Attempt", SqlDbType.Int),
      new SqlMetaData("Reason", SqlDbType.TinyInt),
      new SqlMetaData("DeploymentStatus", SqlDbType.TinyInt),
      new SqlMetaData("OperationStatus", SqlDbType.Int),
      new SqlMetaData("RequestedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("RequestedFor", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StartedTime", SqlDbType.DateTime),
      new SqlMetaData("LastModifiedOn", SqlDbType.DateTime),
      new SqlMetaData("LastModifiedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BuildId", SqlDbType.Int)
    };

    public static void BindDeploymentTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Deployment> deployments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DeploymentTableV2", DeploymentTable2.GetSqlDataRecords2(deployments));
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords2(IEnumerable<Deployment> rows)
    {
      rows = rows ?? Enumerable.Empty<Deployment>();
      foreach (Deployment deployment in rows.Where<Deployment>((System.Func<Deployment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DeploymentTable2.SqlMetaData2);
        sqlDataRecord.SetInt32(ordinal, deployment.Attempt);
        int num1;
        sqlDataRecord.SetByte(num1 = ordinal + 1, (byte) deployment.Reason);
        int num2;
        sqlDataRecord.SetByte(num2 = num1 + 1, (byte) deployment.Status);
        int num3;
        sqlDataRecord.SetInt32(num3 = num2 + 1, (int) deployment.OperationStatus);
        int num4;
        sqlDataRecord.SetGuid(num4 = num3 + 1, deployment.RequestedBy);
        int num5;
        sqlDataRecord.SetGuid(num5 = num4 + 1, deployment.RequestedFor);
        int num6;
        sqlDataRecord.SetDateTime(num6 = num5 + 1, deployment.QueuedOn.ToUtcDateTime());
        int num7;
        sqlDataRecord.SetDateTime(num7 = num6 + 1, deployment.LastModifiedOn.ToUtcDateTime());
        int num8;
        sqlDataRecord.SetGuid(num8 = num7 + 1, deployment.LastModifiedBy);
        int num9;
        sqlDataRecord.SetInt32(num9 = num8 + 1, 0);
        yield return sqlDataRecord;
      }
    }
  }
}
