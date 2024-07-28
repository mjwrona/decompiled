// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildDetailViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildDetailViewModel
  {
    public BuildDetailViewModel(BuildQueryResult queryResult, string currentUser)
    {
      this.BuildDetail = new BuildDetailModel(queryResult, currentUser);
      this.PopulateSummarySections();
      this.PopulateLogNodes();
    }

    public BuildDetailViewModel(Exception error) => this.ErrorMessage = error.Message;

    public BuildDetailModel BuildDetail { get; private set; }

    public string[] SummarySections { get; private set; }

    public string[] LogNodes { get; private set; }

    public string ErrorMessage { get; private set; }

    public bool ErrorExists => !string.IsNullOrWhiteSpace(this.ErrorMessage);

    private void PopulateSummarySections() => this.SummarySections = new string[6]
    {
      "LatestActivity",
      "Summary",
      "AssociatedChangeset",
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

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("buildDetail", (object) this.BuildDetail.ToJson());
      json.Add("summarySections", (object) this.SummarySections);
      json.Add("logNodes", (object) this.LogNodes);
      return json;
    }
  }
}
