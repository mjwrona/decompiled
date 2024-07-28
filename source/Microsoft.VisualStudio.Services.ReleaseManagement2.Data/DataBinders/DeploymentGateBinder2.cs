// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentGateBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentGateBinder2 : DeploymentGateBinder
  {
    private SqlColumnBinder ignoredGates = new SqlColumnBinder("IgnoredGates");
    private SqlColumnBinder succeedingSince = new SqlColumnBinder("SucceedingSince");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder deploymentLastModifiedOn = new SqlColumnBinder("DeploymentLastModifiedOn");

    public DeploymentGateBinder2(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DeploymentGate Bind()
    {
      DeploymentGate deploymentGate = base.Bind();
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      deploymentGate.ProjectId = guid;
      string ignoredGatesJson = this.ignoredGates.GetString((IDataReader) this.Reader, (string) null);
      deploymentGate.SucceedingSince = this.succeedingSince.GetNullableDateTime((IDataReader) this.Reader, new DateTime?());
      DateTime? nullableDateTime = this.deploymentLastModifiedOn.GetNullableDateTime((IDataReader) this.Reader, new DateTime?());
      deploymentGate.DeploymentLastModifiedOn = nullableDateTime;
      if (ignoredGatesJson != null)
        deploymentGate.IgnoredGates = IgnoredGatesExtension.GetIgnoredGates(ignoredGatesJson);
      return deploymentGate;
    }
  }
}
