// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  [DataContract]
  public class BuildViewModel
  {
    private List<string> m_buildQualities;

    public BuildViewModel()
    {
      this.PopulateSummarySections();
      this.PopulateLogNodes();
    }

    public BuildViewModel(BuildDetailViewModel buildDetailViewModel)
    {
      ArgumentUtility.CheckForNull<BuildDetailViewModel>(buildDetailViewModel, nameof (buildDetailViewModel));
      this.Mode = BuildViewMode.BuildDetail;
      this.BuildDetailViewModel = buildDetailViewModel;
    }

    [DataMember(Name = "mode")]
    public BuildViewMode Mode { get; set; }

    public BuildDetailViewModel BuildDetailViewModel { get; set; }

    public List<string> BuildQualities
    {
      get
      {
        if (this.m_buildQualities == null)
          this.m_buildQualities = new List<string>();
        return this.m_buildQualities;
      }
    }

    public string[] SummarySections { get; private set; }

    public string[] LogNodes { get; private set; }

    private void PopulateSummarySections() => this.SummarySections = new string[7]
    {
      "LatestActivity",
      "Summary",
      "AssociatedChangeset",
      "AssociatedCommit",
      "AssociatedWorkItem",
      "TestImpact",
      "LabSummary"
    };

    private void PopulateLogNodes() => this.LogNodes = new string[10]
    {
      "ActivityProperties",
      "ActivityTracking",
      "AgentScopeActivityTracking",
      "BuildProject",
      "BuildStep",
      "BuildMessage",
      "BuildError",
      "BuildWarning",
      "ExternalLink",
      "OpenedWorkItem"
    };
  }
}
