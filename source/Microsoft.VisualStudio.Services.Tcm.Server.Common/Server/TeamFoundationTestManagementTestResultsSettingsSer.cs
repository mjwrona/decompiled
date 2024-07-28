// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementTestResultsSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementTestResultsSettingsService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementTestResultsSettingsService,
    IVssFrameworkService
  {
    private readonly Guid testResultsSettingsArtifact = new Guid("98CE0696-08C6-4f5D-AD99-23843056FFDE");

    public TeamFoundationTestManagementTestResultsSettingsService()
    {
    }

    public TeamFoundationTestManagementTestResultsSettingsService(
      TestManagementRequestContext requestContext)
    {
    }

    public TestResultsSettings GetTestResultsSettings(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestResultsSettingsType testResultsSettingsType)
    {
      string actionName = "TeamFoundationTestManagementTestResultsSettingsService.GetTestResultsSettings";
      TestResultsSettings testResultsSettings = new TestResultsSettings();
      List<string> stringList = new List<string>();
      switch (testResultsSettingsType)
      {
        case TestResultsSettingsType.Flaky:
          stringList.Add("FlakySettings");
          break;
        case TestResultsSettingsType.NewTestLogging:
          stringList.Add("NewTestResultSettings");
          break;
        default:
          stringList.Add("FlakySettings");
          stringList.Add("NewTestResultSettings");
          break;
      }
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", actionName, isTopLevelScenario: true))
      {
        try
        {
          ITeamFoundationPropertyService service = context.RequestContext.GetService<ITeamFoundationPropertyService>();
          ArtifactSpec artifactSpec1 = new ArtifactSpec(this.testResultsSettingsArtifact, "TestResults.TestSettings", 0, projectInfo.Id);
          IVssRequestContext requestContext = context.RequestContext;
          ArtifactSpec artifactSpec2 = artifactSpec1;
          List<string> propertyNameFilters = stringList;
          using (TeamFoundationDataReader properties = service.GetProperties(requestContext, artifactSpec2, (IEnumerable<string>) propertyNameFilters))
          {
            if (properties != null)
            {
              foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
              {
                foreach (Microsoft.TeamFoundation.Framework.Server.PropertyValue propertyValue in current.PropertyValues)
                {
                  switch (propertyValue.PropertyName)
                  {
                    case "FlakySettings":
                      testResultsSettings.FlakySettings = JsonConvert.DeserializeObject<FlakySettings>((string) propertyValue.Value);
                      continue;
                    case "NewTestResultSettings":
                      if (this.GetNewTestLoggingSettingsFF(context))
                      {
                        testResultsSettings.NewTestResultLoggingSettings = JsonConvert.DeserializeObject<NewTestResultLoggingSettings>((string) propertyValue.Value);
                        continue;
                      }
                      continue;
                    default:
                      continue;
                  }
                }
              }
            }
          }
        }
        catch (DataspaceNotFoundException ex)
        {
          context.Logger.Info(1015099, string.Format("TeamFoundationTestManagementTestResultsSettingsService.GetTestResultsSettings:getting settings lead to DataSpaceNotFoundException: {0}", (object) ex));
        }
        if (testResultsSettings.FlakySettings == null)
          testResultsSettings.FlakySettings = this.GetDefaultFlakySettings(context);
        if (this.GetNewTestLoggingSettingsFF(context) && testResultsSettings.NewTestResultLoggingSettings == null)
          testResultsSettings.NewTestResultLoggingSettings = this.GetDefaultNewTestResultLoggingSettings();
        return testResultsSettings;
      }
    }

    public TestResultsSettings UpdateTestResultsSettings(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestResultsUpdateSettings testResultsUpdateSettings)
    {
      TestResultsSettings testResultsSettings1 = new TestResultsSettings();
      string str = "TeamFoundationTestManagementTestResultsSettingsService.UpdateTestResultsSettings";
      this.CheckTestResultsSettingPermission(context, projectInfo);
      this.ValidateSettings(testResultsUpdateSettings);
      this.ValidateAllowedPipelines(context, projectInfo, testResultsUpdateSettings);
      context.RequestContext.TraceEnter(1015097, "TestManagement", "BusinessLayer", str);
      if (testResultsUpdateSettings.FlakySettings != null || testResultsUpdateSettings.NewTestResultLoggingSettings != null)
      {
        TestResultsSettings testResultsSettings2 = this.GetTestResultsSettings(context, projectInfo, TestResultsSettingsType.All);
        testResultsSettings1 = this.Update(context, projectInfo, testResultsSettings2, testResultsUpdateSettings, str);
      }
      return testResultsSettings1;
    }

    private void ValidateAllowedPipelines(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestResultsUpdateSettings testResultsUpdateSettings)
    {
      if (testResultsUpdateSettings.FlakySettings?.FlakyDetection?.FlakyDetectionPipelines?.AllowedPipelines == null)
        return;
      List<BuildDefinitionReference> definitionReferenceList = this.BuildServiceHelper.QueryBuildDefinitionsByIds(context.RequestContext, projectInfo.Id, testResultsUpdateSettings.FlakySettings.FlakyDetection.FlakyDetectionPipelines.AllowedPipelines);
      if ((definitionReferenceList == null ? 0 : (definitionReferenceList.Count == testResultsUpdateSettings.FlakySettings.FlakyDetection.FlakyDetectionPipelines.AllowedPipelines.Length ? 1 : 0)) == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "AllowedPipelines")).Expected(context.RequestContext.ServiceName);
    }

    private void CheckTestResultsSettingPermission(
      TestManagementRequestContext context,
      ProjectInfo projectInfo)
    {
      context.RequestContext.TraceEnter(1015097, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementTestResultsSettingsService.CheckTestResultsSettingPermission");
      if (!context.SecurityManager.CheckProjectSettingsPermission(context, projectInfo.Id.ToString()))
        throw new AccessDeniedException(ServerResources.TestResultsSettingsSecurityErrorMessage);
    }

    private void ValidateSettings(TestResultsUpdateSettings settings)
    {
      ArgumentUtility.CheckForNull<TestResultsUpdateSettings>(settings, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "testUpdateResultsSettings"));
      if (settings.FlakySettings == null)
        return;
      this.ValidateFlakySettings(settings.FlakySettings);
    }

    private void ValidateFlakySettings(FlakySettings flakySetting)
    {
      if (flakySetting.FlakyDetection == null)
        return;
      if (flakySetting.FlakyDetection.FlakyDetectionType == FlakyDetectionType.System)
      {
        ArgumentUtility.CheckForNull<FlakyDetectionPipelines>(flakySetting.FlakyDetection.FlakyDetectionPipelines, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "FlakyDetectionPipelines"));
        if (!flakySetting.FlakyDetection.FlakyDetectionPipelines.IsAllPipelinesAllowed)
          ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) flakySetting.FlakyDetection.FlakyDetectionPipelines.AllowedPipelines, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PropertyCannotBeNullOrEmpty, (object) "AllowedPipelines"));
        else
          ArgumentUtility.EnsureIsNull((object) flakySetting.FlakyDetection.FlakyDetectionPipelines.AllowedPipelines, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PropertyCanNotBeNonNull, (object) "AllowedPipelines"));
      }
      else
      {
        if (flakySetting.FlakyDetection.FlakyDetectionType != FlakyDetectionType.Custom)
          return;
        ArgumentUtility.EnsureIsNull((object) flakySetting.FlakyDetection.FlakyDetectionPipelines, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "FlakyDetectionPipelines"));
      }
    }

    private TestResultsSettings Update(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestResultsSettings existingSettings,
      TestResultsUpdateSettings testResultsUpdateSettings,
      string actionName)
    {
      this.PopulateUpdateSettingsInputWithExistingData(context, existingSettings, testResultsUpdateSettings);
      string str1 = JsonConvert.SerializeObject((object) testResultsUpdateSettings.FlakySettings);
      string str2 = JsonConvert.SerializeObject((object) testResultsUpdateSettings.NewTestResultLoggingSettings);
      bool loggingSettingsFf = this.GetNewTestLoggingSettingsFF(context);
      if (testResultsUpdateSettings.FlakySettings.FlakyDetection != existingSettings.FlakySettings.FlakyDetection)
        new FlakyTestDataDeletionManager().QueueFlakyTestDataDeletion(context, projectInfo.Id);
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", actionName, isTopLevelScenario: true))
      {
        try
        {
          IList<Microsoft.TeamFoundation.Framework.Server.PropertyValue> propertyValueList1 = (IList<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) new List<Microsoft.TeamFoundation.Framework.Server.PropertyValue>();
          propertyValueList1.Add(new Microsoft.TeamFoundation.Framework.Server.PropertyValue("FlakySettings", (object) str1));
          if (loggingSettingsFf)
            propertyValueList1.Add(new Microsoft.TeamFoundation.Framework.Server.PropertyValue("NewTestResultSettings", (object) str2));
          ITeamFoundationPropertyService service = context.RequestContext.GetService<ITeamFoundationPropertyService>();
          ArtifactSpec artifactSpec1 = new ArtifactSpec(this.testResultsSettingsArtifact, "TestResults.TestSettings", 0, projectInfo.Id);
          IVssRequestContext requestContext = context.RequestContext;
          ArtifactSpec artifactSpec2 = artifactSpec1;
          IList<Microsoft.TeamFoundation.Framework.Server.PropertyValue> propertyValueList2 = propertyValueList1;
          service.SetProperties(requestContext, artifactSpec2, (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) propertyValueList2);
        }
        catch (DataspaceNotFoundException ex)
        {
          context.Logger.Info(1015099, string.Format("TeamFoundationTestManagementTestResultsSettingsService.Update:updating settings lead to DataSpaceNotFoundException: {0}", (object) ex));
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.NoTestResultFoundForUpdatingFlakySettings, (object) projectInfo.Name));
        }
        TestResultsSettings testResultsSettings = new TestResultsSettings();
        testResultsSettings.FlakySettings = testResultsUpdateSettings.FlakySettings;
        if (loggingSettingsFf)
          testResultsSettings.NewTestResultLoggingSettings = testResultsUpdateSettings.NewTestResultLoggingSettings;
        return testResultsSettings;
      }
    }

    private void PopulateUpdateSettingsInputWithExistingData(
      TestManagementRequestContext context,
      TestResultsSettings existingSettings,
      TestResultsUpdateSettings testResultsUpdateSettings)
    {
      if ((testResultsUpdateSettings.FlakySettings == null || testResultsUpdateSettings.FlakySettings.FlakyDetection != null || testResultsUpdateSettings.FlakySettings.FlakyInSummaryReport.HasValue || testResultsUpdateSettings.FlakySettings.ManualMarkUnmarkFlaky.HasValue || testResultsUpdateSettings.FlakySettings.IsFlakyBugCreated.HasValue) && existingSettings.FlakySettings != null)
      {
        if (testResultsUpdateSettings.FlakySettings == null)
          testResultsUpdateSettings.FlakySettings = this.GetDefaultFlakySettings(context);
        if (testResultsUpdateSettings.FlakySettings.FlakyDetection == null)
          testResultsUpdateSettings.FlakySettings.FlakyDetection = existingSettings.FlakySettings.FlakyDetection;
        if (!testResultsUpdateSettings.FlakySettings.FlakyInSummaryReport.HasValue)
          testResultsUpdateSettings.FlakySettings.FlakyInSummaryReport = existingSettings.FlakySettings.FlakyInSummaryReport;
        if (!testResultsUpdateSettings.FlakySettings.ManualMarkUnmarkFlaky.HasValue)
          testResultsUpdateSettings.FlakySettings.ManualMarkUnmarkFlaky = existingSettings.FlakySettings.ManualMarkUnmarkFlaky;
        if (!testResultsUpdateSettings.FlakySettings.IsFlakyBugCreated.HasValue)
          testResultsUpdateSettings.FlakySettings.IsFlakyBugCreated = existingSettings.FlakySettings.IsFlakyBugCreated;
      }
      if (this.GetNewTestLoggingSettingsFF(context) && existingSettings.NewTestResultLoggingSettings != null)
      {
        if (testResultsUpdateSettings.NewTestResultLoggingSettings == null)
          testResultsUpdateSettings.NewTestResultLoggingSettings = NewTestResultLoggingSettings.DefaultSettings;
        if (!testResultsUpdateSettings.NewTestResultLoggingSettings.LogNewTests.HasValue)
          testResultsUpdateSettings.NewTestResultLoggingSettings.LogNewTests = existingSettings.NewTestResultLoggingSettings.LogNewTests;
      }
      TestManagementServiceUtility.PublishTelemetry(context.RequestContext, "FlakySettingsTelemetry", new Dictionary<string, object>()
      {
        {
          "FlakyUpdateSettings",
          (object) this.getFlakyUpdateSettingsTelemtryObject(context, testResultsUpdateSettings)
        },
        {
          "ExistingFlakySettings",
          (object) this.getExistingFlakySettingsTelemtryObject(context, existingSettings)
        }
      });
    }

    private Dictionary<string, object> getExistingFlakySettingsTelemtryObject(
      TestManagementRequestContext context,
      TestResultsSettings existingSettings)
    {
      Dictionary<string, object> settingsTelemtryObject = new Dictionary<string, object>();
      try
      {
        settingsTelemtryObject.Add("FlakyDetection", (object) existingSettings?.FlakySettings?.FlakyDetection);
        settingsTelemtryObject.Add("FlakyInSummaryReport", (object) (bool?) existingSettings?.FlakySettings?.FlakyInSummaryReport);
        settingsTelemtryObject.Add("ManualMarkUnmarkFlaky", (object) (bool?) existingSettings?.FlakySettings?.ManualMarkUnmarkFlaky);
        settingsTelemtryObject.Add("IsFlakyBugCreated", (object) (bool?) existingSettings?.FlakySettings?.IsFlakyBugCreated);
        settingsTelemtryObject.Add("LogNewTests", (object) (bool?) existingSettings?.NewTestResultLoggingSettings?.LogNewTests);
      }
      catch (Exception ex)
      {
        context.Logger.Info(1015100, string.Format("TestResultServiceAPISettingServiceTelemetryPublisherFailureTracePoint: {0}", (object) ex));
      }
      return settingsTelemtryObject;
    }

    private Dictionary<string, object> getFlakyUpdateSettingsTelemtryObject(
      TestManagementRequestContext context,
      TestResultsUpdateSettings testResultsUpdateSettings)
    {
      Dictionary<string, object> settingsTelemtryObject = new Dictionary<string, object>();
      try
      {
        settingsTelemtryObject.Add("FlakyDetection", (object) testResultsUpdateSettings?.FlakySettings?.FlakyDetection);
        settingsTelemtryObject.Add("FlakyInSummaryReport", (object) (bool?) testResultsUpdateSettings?.FlakySettings?.FlakyInSummaryReport);
        settingsTelemtryObject.Add("ManualMarkUnmarkFlaky", (object) (bool?) testResultsUpdateSettings?.FlakySettings?.ManualMarkUnmarkFlaky);
        settingsTelemtryObject.Add("IsFlakyBugCreated", (object) (bool?) testResultsUpdateSettings?.FlakySettings?.IsFlakyBugCreated);
        settingsTelemtryObject.Add("LogNewTests", (object) (bool?) testResultsUpdateSettings?.NewTestResultLoggingSettings?.LogNewTests);
      }
      catch (Exception ex)
      {
        context.Logger.Info(1015100, string.Format("TestResultServiceAPISettingServiceTelemetryPublisherFailureTracePoint: {0}", (object) ex));
      }
      return settingsTelemtryObject;
    }

    private FlakySettings GetDefaultFlakySettings(TestManagementRequestContext context)
    {
      FlakySettings defaultFlakySettings;
      switch (context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/OnBoardingFlakyProjectSettingsStatus", 0))
      {
        case 1:
          defaultFlakySettings = FlakySettings.DefaultForExistingAccounts;
          break;
        case 2:
          defaultFlakySettings = FlakySettings.DefaultForExistingAccountsWithManualMarkEnabled;
          break;
        default:
          defaultFlakySettings = FlakySettings.DefaultForNewAccounts;
          break;
      }
      string methodName = "TeamFoundationTestManagementTestResultsSettingsService.PopulateUpdateSettingsInputWithExistingData";
      context.RequestContext.TraceEnter(1015101, "TestManagement", "BusinessLayer", methodName);
      return defaultFlakySettings;
    }

    private NewTestResultLoggingSettings GetDefaultNewTestResultLoggingSettings() => NewTestResultLoggingSettings.DefaultSettings;

    private bool GetNewTestLoggingSettingsFF(TestManagementRequestContext context) => context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableNewTestResultLogging");
  }
}
