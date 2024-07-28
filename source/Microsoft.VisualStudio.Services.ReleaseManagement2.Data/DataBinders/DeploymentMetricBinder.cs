// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentMetricBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentMetricBinder : ObjectBinder<DeploymentMetric>
  {
    private SqlColumnBinder deploymentStatus = new SqlColumnBinder("DeploymentStatus");
    private SqlColumnBinder count = new SqlColumnBinder("Count");

    protected override DeploymentMetric Bind() => new DeploymentMetric()
    {
      Count = this.count.GetInt32((IDataReader) this.Reader),
      Status = (DeploymentStatus) this.deploymentStatus.GetInt32((IDataReader) this.Reader)
    };
  }
}
