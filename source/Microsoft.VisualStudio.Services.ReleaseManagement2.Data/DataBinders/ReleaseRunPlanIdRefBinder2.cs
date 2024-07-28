// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseRunPlanIdRefBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseRunPlanIdRefBinder2 : ReleaseRunPlanIdRefBinder
  {
    private SqlColumnBinder environmentName = new SqlColumnBinder("EnvironmentName");
    private SqlColumnBinder environmentRank = new SqlColumnBinder("EnvironmentRank");
    private SqlColumnBinder attempt = new SqlColumnBinder("TrialNumber");
    private SqlColumnBinder phaseType = new SqlColumnBinder("PhaseType");
    private SqlColumnBinder phaseRank = new SqlColumnBinder("PhaseRank");
    private SqlColumnBinder runPlanId = new SqlColumnBinder("RunPlanId");

    protected override ReleaseDeployPhaseRef Bind()
    {
      ReleaseDeployPhaseRef releaseDeployPhaseRef = new ReleaseDeployPhaseRef()
      {
        PlanId = this.runPlanId.GetGuid((IDataReader) this.Reader, true),
        EnvironmentName = this.environmentName.GetString((IDataReader) this.Reader, false),
        EnvironmentRank = this.environmentRank.GetInt32((IDataReader) this.Reader),
        Attempt = this.attempt.GetInt32((IDataReader) this.Reader),
        PhaseType = (DeployPhaseTypes) this.phaseType.GetByte((IDataReader) this.Reader),
        PhaseRank = this.phaseRank.GetInt32((IDataReader) this.Reader)
      };
      releaseDeployPhaseRef.PhaseName = ServerModelUtility.GetDefaultPhaseName(releaseDeployPhaseRef.PhaseType);
      return releaseDeployPhaseRef;
    }
  }
}
