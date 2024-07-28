// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ManualInterventionSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ManualInterventionSqlComponent2 : ManualInterventionSqlComponent
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ManualInterventionBinder GetManualInterventionBinder() => (ManualInterventionBinder) new ManualInterventionBinder2();

    public override ManualIntervention CreateManualIntervention(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      TaskActivityData taskActivityData,
      string instructions,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_CreateManualIntervention", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (releaseDeployPhaseId), releaseDeployPhaseId);
      this.BindString("TaskActivityData", ServerModelUtility.ToString((object) taskActivityData), 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString(nameof (instructions), instructions, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid(nameof (changedBy), changedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionBinder());
        return resultCollection.GetCurrent<ManualIntervention>().Items.FirstOrDefault<ManualIntervention>();
      }
    }
  }
}
