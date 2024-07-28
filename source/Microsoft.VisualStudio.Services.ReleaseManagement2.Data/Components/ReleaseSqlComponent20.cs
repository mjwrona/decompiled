// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent20
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent20 : ReleaseSqlComponent19
  {
    public override Release CancelDeploymentOnEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      Guid changedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_CancelDeployment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }

    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      List<ReleaseDeployPhaseRef> source = new List<ReleaseDeployPhaseRef>();
      this.PrepareStoredProcedure("Release.prc_ReleaseRunPlanIdRef_Get", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (skipIsDeletedCheck), skipIsDeletedCheck);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhaseRef>((ObjectBinder<ReleaseDeployPhaseRef>) new ReleaseRunPlanIdRefBinder());
        source = resultCollection.GetCurrent<ReleaseDeployPhaseRef>().Items;
      }
      return new ReleaseLogContainers()
      {
        DeployPhases = (IList<ReleaseDeployPhaseRef>) source.Where<ReleaseDeployPhaseRef>((System.Func<ReleaseDeployPhaseRef, bool>) (e => e.PlanId != Guid.Empty)).ToList<ReleaseDeployPhaseRef>()
      };
    }
  }
}
