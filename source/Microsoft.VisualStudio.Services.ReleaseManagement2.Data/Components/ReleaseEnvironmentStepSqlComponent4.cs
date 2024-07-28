// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent4
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseEnvironmentStepSqlComponent4 : ReleaseEnvironmentStepSqlComponent3
  {
    private static readonly SqlMetaData[] TypInt32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };

    public override IEnumerable<ReleaseEnvironmentStep> ListReleaseApprovalSteps(
      Guid projectId,
      IEnumerable<int> releaseIds,
      ReleaseEnvironmentStepStatus? status,
      Guid? approverId,
      EnvironmentStepType typeFilter,
      Guid? actualApproverId,
      int top,
      int continuationToken,
      ReleaseQueryOrder queryOrder,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime)
    {
      releaseIds = releaseIds ?? Enumerable.Empty<int>();
      this.PrepareStoredProcedure("Release.prc_GetApprovals", projectId);
      this.BindNullableGuid("approverIdFilter", approverId ?? Guid.Empty);
      this.BindInt("statusFilter", (int) status.GetValueOrDefault());
      this.BindTable(nameof (releaseIds), "dbo.typ_Int32Table", releaseIds.Select<int, SqlDataRecord>(new System.Func<int, SqlDataRecord>(this.ConvertToSqlDataRecord)));
      List<ReleaseEnvironmentStep> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        items = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
      return typeFilter != EnvironmentStepType.All ? items.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (approval => approval.StepType == typeFilter)) : (IEnumerable<ReleaseEnvironmentStep>) items;
    }

    public override void BindToReleaseEnvironmentStepTable(
      string parameterName,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.BindReleaseEnvironmentStepTable3(parameterName, (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>()
      {
        releaseEnvironmentStep
      });
    }

    public virtual SqlDataRecord ConvertToSqlDataRecord(int releaseId)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseEnvironmentStepSqlComponent4.TypInt32Table);
      sqlDataRecord.SetInt32(0, releaseId);
      return sqlDataRecord;
    }
  }
}
