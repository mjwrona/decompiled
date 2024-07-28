// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.RedeployTriggerEnvironmentDGPhaseDataBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "As per Vssf")]
  public class RedeployTriggerEnvironmentDGPhaseDataBinder : 
    ObjectBinder<RedeployTriggerEnvironmentDGPhaseData>
  {
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder deploymentGroupId = new SqlColumnBinder("DeploymentGroupId");
    private SqlColumnBinder tags = new SqlColumnBinder("Tags");

    protected override RedeployTriggerEnvironmentDGPhaseData Bind() => new RedeployTriggerEnvironmentDGPhaseData()
    {
      ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
      EnvironmentId = this.environmentId.GetInt32((IDataReader) this.Reader),
      DeploymentGroupId = this.deploymentGroupId.GetInt32((IDataReader) this.Reader),
      Tags = RedeployTriggerEnvironmentDGPhaseDataBinder.ConvertStringToList(this.tags.GetString((IDataReader) this.Reader, true))
    };

    private static IList<string> ConvertStringToList(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (IList<string>) new List<string>();
      return (IList<string>) new List<string>((IEnumerable<string>) value.Split(','));
    }
  }
}
