// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestRunProperties
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public class TestRunProperties
  {
    public TestRunProperties()
    {
    }

    public TestRunProperties(TestRun testRun, Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings)
    {
      this.PopulateRunProperties(testRun);
      this.UpdateTestRunProperties(testSettings);
    }

    public TestRunProperties(TestRun testRun, RunProperties runProperties)
    {
      this.PopulateRunProperties(testRun);
      this.UpdateTestRunProperties(runProperties);
    }

    public void PopulateRunProperties(TestRun testRun)
    {
      this.CodeCoverageEnabled = "None";
      this.TestRunId = testRun.Id;
      if (testRun.BuildConfiguration != null)
      {
        this.BuildId = testRun.BuildConfiguration.Id;
        this.BuildPlatform = testRun.BuildConfiguration.Platform;
        this.BuildFlavor = testRun.BuildConfiguration.Flavor;
      }
      int result;
      if (testRun.Plan != null && int.TryParse(testRun.Plan.Id, out result))
        this.TestPlanId = result;
      this.DropLocation = testRun.DropLocation;
    }

    public void UpdateTestRunProperties(Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings)
    {
      if (testSettings == null)
        return;
      try
      {
        XmlDocument testSettingsXml = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(testSettings.TestSettingsContent))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
            testSettingsXml.Load(reader);
        }
        if (string.Equals("testsettings", testSettingsXml.LastChild.Name, StringComparison.InvariantCultureIgnoreCase))
          this.SettingsType = "TestSettings";
        if (string.Equals("runsettings", testSettingsXml.LastChild.Name, StringComparison.InvariantCultureIgnoreCase))
          this.SettingsType = "RunSettings";
        this.TestSettings = testSettings.TestSettingsContent;
        this.CodeCoverageEnabled = Utilities.CodeCoverageEnabled(testSettingsXml);
        this.IsCustomSlicingEnabled = Utilities.CheckCustomSlicingFlagInSettingsAndRemove(testSettingsXml, this);
        this.IsRerunEnabled = Utilities.CheckRerunInformationInSettingsAndRemove(testSettingsXml, this);
        this.IsTestImpactOn = Utilities.CheckIsTestImpactOnInSettingsAndRemove(testSettingsXml);
        this.BaseLineBuildId = Utilities.GetBaseLineBuildAndRemove(testSettingsXml);
      }
      catch
      {
      }
    }

    public void UpdateTestRunProperties(RunProperties runProperties)
    {
      this.IsCustomSlicingEnabled = this.GetCustomSlicingProperties(runProperties);
      this.IsRerunEnabled = this.GetRerunProperties(runProperties);
      this.IsTestImpactOn = runProperties.IsTestImpactOn;
      this.BaseLineBuildId = runProperties.BaseLineBuildId;
    }

    private bool GetCustomSlicingProperties(RunProperties runProperties)
    {
      if (runProperties.IsCustomSlicingEnabled)
      {
        this.MaxAgents = runProperties.MaxAgents;
        this.NumberOfTestCasesPerSlice = runProperties.NumberOfTestCasesPerSlice;
        this.IsTimeBasedSlicing = runProperties.IsTimeBasedSlicing;
        this.SliceTime = runProperties.SliceTime;
      }
      return runProperties.IsCustomSlicingEnabled;
    }

    private bool GetRerunProperties(RunProperties runProperties)
    {
      if (runProperties.IsRerunEnabled)
      {
        this.RerunFailedThreshold = runProperties.RerunFailedThreshold;
        this.RerunMaxAttempts = runProperties.RerunMaxAttempts;
        this.RerunFailedTestCasesMaxLimit = runProperties.RerunFailedTestCasesMaxLimit;
      }
      return runProperties.IsRerunEnabled;
    }

    public string CodeCoverageEnabled { get; set; }

    public int MaxAgents { get; set; }

    public bool IsCustomSlicingEnabled { get; set; }

    public int NumberOfTestCasesPerSlice { get; set; }

    public bool IsTimeBasedSlicing { get; set; }

    public int SliceTime { get; set; }

    public bool IsTestImpactOn { get; set; }

    public int BaseLineBuildId { get; set; }

    public bool IsRerunEnabled { get; set; }

    public int RerunFailedThreshold { get; set; }

    public int RerunMaxAttempts { get; set; }

    public int RerunFailedTestCasesMaxLimit { get; set; }

    public int TestRunId { get; set; }

    public string DropLocation { get; set; }

    public int TestPlanId { get; set; }

    public string SettingsType { get; set; }

    public string TestSettings { get; set; }

    public string BuildFlavor { get; set; }

    public string BuildPlatform { get; set; }

    public int BuildId { get; set; }
  }
}
