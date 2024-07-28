// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public abstract class BuildAreaController : TfsAreaController
  {
    public override string AreaName => "Build";

    public TeamFoundationBuildService XamlBuildService => this.TfsRequestContext.GetService<TeamFoundationBuildService>();

    internal ITeamFoundationBuildService2 BuildService => this.TfsRequestContext.GetService<ITeamFoundationBuildService2>();

    protected IProjectService ProjectService => this.TfsRequestContext.GetService<IProjectService>();

    protected TeamFoundationBuildResourceService XamlBuildResourceService => this.TfsRequestContext.GetService<TeamFoundationBuildResourceService>();

    protected int? GetInt32Parameter(string parameterName)
    {
      string s = this.Request.Params[parameterName];
      int result;
      return !string.IsNullOrEmpty(s) && int.TryParse(s, out result) ? new int?(result) : new int?();
    }
  }
}
