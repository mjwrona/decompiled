// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.CreateProjectData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public sealed class CreateProjectData : IValidatable
  {
    public string VersionControlOption { get; set; }

    public string ProjectVisibilityOption { get; set; }

    public bool? CreateReadMe { get; set; }

    public void Validate() => ArgumentUtility.CheckStringForNullOrEmpty(this.VersionControlOption, "VersionControlOption");

    internal void Populate(IDictionary<string, string> servicingTokens)
    {
      servicingTokens[ProjectServicingTokenConstants.VersionControlOption] = this.VersionControlOption;
      servicingTokens[ProjectServicingTokenConstants.ProjectVisibilityOption] = this.ProjectVisibilityOption;
      servicingTokens[ProjectServicingTokenConstants.CreateReadMe] = this.CreateReadMe.GetValueOrDefault().ToString();
    }
  }
}
