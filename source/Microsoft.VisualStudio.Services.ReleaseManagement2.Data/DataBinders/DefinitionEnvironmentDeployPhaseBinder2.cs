// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentDeployPhaseBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  public class DefinitionEnvironmentDeployPhaseBinder2 : DefinitionEnvironmentDeployPhaseBinder
  {
    private SqlColumnBinder refName = new SqlColumnBinder("RefName");

    protected override DeployPhase Bind()
    {
      DeployPhase deployPhase = base.Bind();
      deployPhase.RefName = this.refName.GetString((IDataReader) this.Reader, true);
      return deployPhase;
    }
  }
}
