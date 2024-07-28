// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent18
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent18 : ReleaseEnvironmentStepSqlComponent17
  {
    public override IEnumerable<ReleaseEnvironmentStep> GetPendingApprovalsForReleaseDefinitions(
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      this.PrepareStoredProcedure("Release.prc_GetPendingApprovalsForReleaseDefinitions", projectId);
      this.BindInt32Table("@definitionIds", releaseDefinitionIds);
      this.BindDateTime(nameof (minModifiedTime), minModifiedTime);
      this.BindDateTime(nameof (maxModifiedTime), maxModifiedTime);
      return this.GetReleaseEnvironmentStepsObject();
    }

    public override IEnumerable<ReleaseEnvironmentStep> ListApprovalsForAnIdentity(
      Guid projectId,
      Guid approverId,
      ReleaseEnvironmentStepStatus statusFilter,
      int maxApprovals,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      return this.ListReleaseApprovalSteps(projectId, (IEnumerable<int>) new List<int>(), new ReleaseEnvironmentStepStatus?(statusFilter), new Guid?(approverId), EnvironmentStepType.All, new Guid?(), maxApprovals, 0, ReleaseQueryOrder.IdDescending, new DateTime?(minModifiedTime), new DateTime?(maxModifiedTime));
    }
  }
}
