// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent15
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent15 : DeploymentSqlComponent14
  {
    public override DeploymentGate UpdateGreenlightingSucceedingSince(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId,
      DateTime? succeedingSince)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateGreenlightingSucceedingSince", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (stepId), stepId);
      this.BindNullableDateTime(nameof (succeedingSince), succeedingSince);
      return this.GetDeploymentGateObject();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public override IEnumerable<DeploymentAttemptData> GetDeploymentsByReasonForMultipleEnvironments(
      Guid projectId,
      HashSet<KeyValuePair<int, int>> definitionEnvironmentReleaseDefinitionIdKeyValuePairs,
      DeploymentReason deploymentReasonFilter,
      int deploymentsPerEnvironment,
      bool isDeleted)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeploymentsByReasonForMultipleEnvironments", projectId);
      this.BindKeyValuePairInt32Int32Table("releaseDefinitionAndEnvironmentIds", (IEnumerable<KeyValuePair<int, int>>) definitionEnvironmentReleaseDefinitionIdKeyValuePairs);
      this.BindByte("deploymentReason", (byte) deploymentReasonFilter);
      this.BindInt(nameof (deploymentsPerEnvironment), deploymentsPerEnvironment);
      this.BindNullableBoolean(nameof (isDeleted), new bool?(isDeleted));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentAttemptData>((ObjectBinder<DeploymentAttemptData>) new DeploymentAttemptDataBinder((ReleaseManagementSqlResourceComponentBase) this));
        return (IEnumerable<DeploymentAttemptData>) resultCollection.GetCurrent<DeploymentAttemptData>().Items;
      }
    }
  }
}
