// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent11
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
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "This is our versioning mechanism")]
  public class ReleaseSqlComponent11 : ReleaseSqlComponent10
  {
    public override IEnumerable<int> SoftDeleteReleases(
      Guid projectId,
      int definitionId,
      DateTime maxReleaseModifiedTimeForSoftDelete,
      IEnumerable<DefinitionEnvironment> definitionEnvironments,
      Guid modifiedBy,
      int defaultNumberOfDaysToRetainRelease)
    {
      this.PrepareStoredProcedure("Release.prc_ProcessRetentionPolicy", projectId);
      this.BindInt("releaseDefinitionId", definitionId);
      this.BindThresholdDate("thresholdDate", maxReleaseModifiedTimeForSoftDelete);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      this.BindDefinitionEnvironmentRetentionPolicy("environmentRetentionPolicies", definitionEnvironments);
      this.BindDefaultMaxTimeToRetainRelease(defaultNumberOfDaysToRetainRelease);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) this.GetReleaseIdListBinder());
        return (IEnumerable<int>) resultCollection.GetCurrent<int>().Items;
      }
    }

    public override void HardDeleteReleases(Guid projectId, IEnumerable<int> releaseIds)
    {
      this.PrepareStoredProcedure("Release.prc_HardDeleteReleases", projectId);
      this.BindInt32Table(nameof (releaseIds), releaseIds);
      this.ExecuteNonQuery();
    }

    public override void DeleteRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_SoftDeleteRelease", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release PatchRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      IList<ReleaseEnvironment> releaseEnvironments,
      string comment,
      ReleaseStatus? status = null,
      bool? keepForever = null)
    {
      this.PrepareStoredProcedure("Release.prc_PatchRelease", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      if (status.HasValue)
        this.BindByte("releaseStatus", (byte) status.Value);
      if (keepForever.HasValue)
        this.BindBoolean(nameof (keepForever), keepForever.Value);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) (releaseEnvironments ?? (IList<ReleaseEnvironment>) new List<ReleaseEnvironment>()));
      return this.GetReleaseObject(projectId);
    }

    public override Release RejectReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      string comment)
    {
      return this.UpdateEnvironmentAndStepsStatus(projectId, releaseId, releaseEnvironmentId, ReleaseEnvironmentStatus.Rejected, ReleaseEnvironmentStepStatus.Pending, ReleaseEnvironmentStepStatus.Abandoned);
    }

    protected override void BindMaxModifiedTime(string parameterName, DateTime? maxModifiedTime) => this.BindNullableDateTime(nameof (maxModifiedTime), maxModifiedTime);

    protected override void BindIsDeleted(string parameterName, bool isDeleted) => this.BindBoolean(parameterName, isDeleted);

    protected virtual void BindDefinitionEnvironmentRetentionPolicy(
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
    }

    protected virtual void BindDefaultMaxTimeToRetainRelease(int defaultNumberOfDaysToRetainRelease)
    {
    }

    protected virtual void BindThresholdDate(string parameterName, DateTime thresholdDate) => this.BindDateTime(parameterName, thresholdDate);
  }
}
