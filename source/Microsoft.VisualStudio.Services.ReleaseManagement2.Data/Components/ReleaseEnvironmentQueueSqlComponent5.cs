// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentQueueSqlComponent5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentQueueSqlComponent5 : ReleaseEnvironmentQueueSqlComponent4
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override IEnumerable<ReleaseEnvironmentQueueData> GetUnhealthyReleaseEnvironments(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int daysToCheck,
      int skipHours = 6)
    {
      this.PrepareStoredProcedure("Release.prc_GetUnhealthyReleaseEnvironments", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      this.BindInt(nameof (daysToCheck), daysToCheck);
      this.BindInt(nameof (skipHours), skipHours);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentQueueData>((ObjectBinder<ReleaseEnvironmentQueueData>) this.GetReleaseEnvironmentQueueBinder());
        return (IEnumerable<ReleaseEnvironmentQueueData>) resultCollection.GetCurrent<ReleaseEnvironmentQueueData>().Items;
      }
    }
  }
}
