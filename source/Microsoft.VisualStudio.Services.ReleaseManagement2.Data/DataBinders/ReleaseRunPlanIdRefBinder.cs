// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseRunPlanIdRefBinder
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
  public class ReleaseRunPlanIdRefBinder : ObjectBinder<ReleaseDeployPhaseRef>
  {
    private SqlColumnBinder runPlanId = new SqlColumnBinder("RunPlanId");
    private SqlColumnBinder environmentName = new SqlColumnBinder("EnvironmentName");
    private SqlColumnBinder environmentRank = new SqlColumnBinder("EnvironmentRank");
    private SqlColumnBinder attempt = new SqlColumnBinder("TrialNumber");

    protected override ReleaseDeployPhaseRef Bind() => new ReleaseDeployPhaseRef()
    {
      PlanId = this.runPlanId.GetGuid((IDataReader) this.Reader, true),
      EnvironmentName = this.environmentName.GetString((IDataReader) this.Reader, false),
      EnvironmentRank = this.environmentRank.GetInt32((IDataReader) this.Reader),
      Attempt = this.attempt.GetInt32((IDataReader) this.Reader),
      PhaseType = DeployPhaseTypes.AgentBasedDeployment,
      PhaseRank = 1,
      PhaseName = ServerModelUtility.GetDefaultPhaseName(DeployPhaseTypes.AgentBasedDeployment)
    };
  }
}
