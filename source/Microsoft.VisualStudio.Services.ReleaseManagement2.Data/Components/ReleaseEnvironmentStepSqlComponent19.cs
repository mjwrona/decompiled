// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent19
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent19 : ReleaseEnvironmentStepSqlComponent18
  {
    public override IEnumerable<ReleaseEnvironmentStep> ListApprovalsForAnIdentity(
      Guid projectId,
      Guid approverId,
      ReleaseEnvironmentStepStatus statusFilter,
      int maxApprovals,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      this.PrepareStoredProcedure("Release.prc_GetApprovalsForAnIdentity", projectId);
      this.BindNullableGuid(nameof (approverId), approverId);
      this.BindInt(nameof (statusFilter), (int) statusFilter);
      this.BindInt("maxApprovalCount", maxApprovals);
      this.BindNullableDateTime(nameof (minModifiedTime), new DateTime?(minModifiedTime));
      this.BindNullableDateTime(nameof (maxModifiedTime), new DateTime?(maxModifiedTime));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        return (IEnumerable<ReleaseEnvironmentStep>) resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
    }
  }
}
