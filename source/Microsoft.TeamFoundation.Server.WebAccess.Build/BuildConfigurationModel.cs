// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildConfigurationModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildConfigurationModel : InformationNodeModel
  {
    public BuildConfigurationModel(BuildInformationNode node)
      : base(node)
    {
      this.Flavor = this.GetFieldValue(InformationFields.Flavor);
      this.Platform = this.GetFieldValue(InformationFields.Platform);
    }

    public string Flavor { get; private set; }

    public string Platform { get; private set; }

    private List<BuildTestRunModel> TestRuns { get; set; }

    private BuildCoverageModel CoverageData { get; set; }

    internal void AddRun(BuildTestRunModel run)
    {
      if (this.TestRuns == null)
        this.TestRuns = new List<BuildTestRunModel>();
      this.TestRuns.Add(run);
    }

    internal void SetCoverage(BuildCoverageModel coverage) => this.CoverageData = coverage;

    protected override void ContributeToJson(JsObject json)
    {
      json["text"] = (object) this.GetText();
      json["platform"] = (object) this.Platform;
      json["flavor"] = (object) this.Flavor;
      int fieldValueInt1 = this.GetFieldValueInt(InformationFields.TotalCompilationErrors);
      int fieldValueInt2 = this.GetFieldValueInt(InformationFields.TotalStaticAnalysisErrors);
      int fieldValueInt3 = this.GetFieldValueInt(InformationFields.TotalCompilationWarnings);
      int fieldValueInt4 = this.GetFieldValueInt(InformationFields.TotalStaticAnalysisWarnings);
      json["totalErrors"] = (object) (fieldValueInt1 + fieldValueInt2);
      json["totalWarnings"] = (object) (fieldValueInt3 + fieldValueInt4);
      if (this.TestRuns != null)
        json["testRuns"] = (object) this.TestRuns.Select<BuildTestRunModel, JsObject>((Func<BuildTestRunModel, JsObject>) (r => r.ToJson()));
      if (this.CoverageData == null)
        return;
      json["coverageData"] = (object) this.CoverageData.ToJson();
    }

    private string GetText()
    {
      bool flag1 = string.IsNullOrWhiteSpace(this.Flavor);
      bool flag2 = string.IsNullOrWhiteSpace(this.Platform);
      if (flag1 & flag2)
        return BuildServerResources.BuildDetailViewEmptyConfigurationSummary;
      if (flag1)
        return this.Platform;
      return flag2 ? this.Flavor : string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewConfigurationSummaryFormat, (object) this.Flavor, (object) this.Platform);
    }
  }
}
