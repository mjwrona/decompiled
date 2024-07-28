// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseRunPlanIdRefBinder3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseRunPlanIdRefBinder3 : ReleaseRunPlanIdRefBinder2
  {
    private SqlColumnBinder deployPhaseSnapshots = new SqlColumnBinder("DeployPhaseSnapshots");

    protected override ReleaseDeployPhaseRef Bind()
    {
      string str = this.deployPhaseSnapshots.GetString((IDataReader) this.Reader, true);
      ReleaseDeployPhaseRef releaseDeployPhaseRef = base.Bind();
      try
      {
        if ((str != null ? JsonConvert.DeserializeObject<IDeploymentSnapshot>(str) : (IDeploymentSnapshot) new DesignerDeploymentSnapshot()) is DesignerDeploymentSnapshot deploymentSnapshot)
          releaseDeployPhaseRef.PhaseName = deploymentSnapshot.GetPhaseName(releaseDeployPhaseRef.PhaseRank, releaseDeployPhaseRef.PhaseType);
      }
      catch (JsonReaderException ex)
      {
        releaseDeployPhaseRef.PhaseName = ServerModelUtility.GetDefaultPhaseName(DeployPhaseTypes.AgentBasedDeployment);
      }
      return releaseDeployPhaseRef;
    }
  }
}
