// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentQueueSqlComponent1
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

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseEnvironmentQueueSqlComponent1 : ReleaseEnvironmentQueueSqlComponent
  {
    public override QueuingPolicyResult GetReleaseEnvironmentsTorunAfterEnforcingQueuingPolicy(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_HandleReleaseEnvironmentStartOrStop", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (maxConcurrent), maxConcurrent);
      this.BindInt(nameof (maxQueueDepth), maxQueueDepth);
      QueuingPolicyResult enforcingQueuingPolicy = new QueuingPolicyResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentQueueData>((ObjectBinder<ReleaseEnvironmentQueueData>) this.GetReleaseEnvironmentQueueBinder());
        enforcingQueuingPolicy.EnvironmentsToQueue = (IEnumerable<ReleaseEnvironmentQueueData>) resultCollection.GetCurrent<ReleaseEnvironmentQueueData>().Items;
        return enforcingQueuingPolicy;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseEnvironmentQueueBinder GetReleaseEnvironmentQueueBinder() => new ReleaseEnvironmentQueueBinder();
  }
}
