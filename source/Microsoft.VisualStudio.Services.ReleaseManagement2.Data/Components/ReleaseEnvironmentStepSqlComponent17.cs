// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent17
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent17 : ReleaseEnvironmentStepSqlComponent16
  {
    public override void BindToReleaseEnvironmentStepTable(
      string parameterName,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.BindReleaseEnvironmentStepTable8(parameterName, (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>()
      {
        releaseEnvironmentStep
      });
    }

    public override void BindToReleaseEnvironmentStepsTable(
      string parameterName,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      this.BindReleaseEnvironmentStepTable8(parameterName, releaseEnvironmentSteps);
    }

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
      this.BindNullableGuid("approverIdFilter", approverId);
      this.BindNullableGuid("actualApproverIdFilter", actualApproverId);
      this.BindInt("statusFilter", (int) status.GetValueOrDefault());
      this.BindTable(nameof (releaseIds), "dbo.typ_Int32Table", releaseIds.Select<int, SqlDataRecord>(new System.Func<int, SqlDataRecord>(((ReleaseEnvironmentStepSqlComponent4) this).ConvertToSqlDataRecord)));
      this.BindInt(nameof (typeFilter), (int) typeFilter);
      this.BindInt(nameof (top), top);
      this.BindInt("approvalsContinuationToken", continuationToken);
      this.BindInt(nameof (queryOrder), (int) queryOrder);
      this.BindNullableDateTime(nameof (minModifiedTime), minModifiedTime);
      this.BindNullableDateTime(nameof (maxModifiedTime), maxModifiedTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        return (IEnumerable<ReleaseEnvironmentStep>) resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
    }
  }
}
