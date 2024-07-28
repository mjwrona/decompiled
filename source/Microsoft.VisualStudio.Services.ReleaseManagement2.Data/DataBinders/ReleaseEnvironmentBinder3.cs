// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseEnvironmentBinder3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseEnvironmentBinder3 : ReleaseEnvironmentBinder2
  {
    private SqlColumnBinder deploymentGates = new SqlColumnBinder("Gates");
    private SqlColumnBinder deploymentLastModifiedOn = new SqlColumnBinder("DeploymentLastModifiedOn");

    public ReleaseEnvironmentBinder3(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseEnvironment Bind()
    {
      string str = this.deploymentGates.GetString((IDataReader) this.Reader, (string) null);
      DateTime? nullableDateTime = this.deploymentLastModifiedOn.GetNullableDateTime((IDataReader) this.Reader, new DateTime?());
      ReleaseEnvironment releaseEnvironment = base.Bind();
      releaseEnvironment.DeploymentLastModifiedOn = nullableDateTime;
      if (str != null)
      {
        IDictionary<string, ReleaseDefinitionGatesStep> deploymentGates = ServerModelUtility.FromString<IDictionary<string, ReleaseDefinitionGatesStep>>(str);
        releaseEnvironment.PopulateDefinitionGates(deploymentGates);
      }
      return releaseEnvironment;
    }
  }
}
