// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentBinder4
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentBinder4 : DeploymentBinder3
  {
    private SqlColumnBinder queuedOn = new SqlColumnBinder("QueuedOn");
    private SqlColumnBinder startedOn = new SqlColumnBinder("StartedOn");

    public DeploymentBinder4(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override Deployment Bind()
    {
      Deployment deployment = base.Bind();
      deployment.StartedOn = this.startedOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      return deployment;
    }

    protected override void FillQueuedOn(Deployment deployment)
    {
      if (deployment == null)
        throw new ArgumentNullException(nameof (deployment));
      deployment.QueuedOn = this.queuedOn.GetDateTime((IDataReader) this.Reader);
    }
  }
}
