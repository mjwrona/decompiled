// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelinesResources
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal static class PipelinesResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (PipelinesResources), typeof (PipelinesResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelinesResources.s_resMgr;

    private static string Get(string resourceName) => PipelinesResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelinesResources.Get(resourceName) : PipelinesResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelinesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelinesResources.GetInt(resourceName) : (int) PipelinesResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelinesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelinesResources.GetBool(resourceName) : (bool) PipelinesResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelinesResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelinesResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string BuildFrameworkDetectionFailedNoTreeAnalysis() => PipelinesResources.Get(nameof (BuildFrameworkDetectionFailedNoTreeAnalysis));

    public static string BuildFrameworkDetectionFailedNoTreeAnalysis(CultureInfo culture) => PipelinesResources.Get(nameof (BuildFrameworkDetectionFailedNoTreeAnalysis), culture);

    public static string BuildNotFound(object arg0) => PipelinesResources.Format(nameof (BuildNotFound), arg0);

    public static string BuildNotFound(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (BuildNotFound), culture, arg0);

    public static string ExceptionAgentQueueNotFound(object arg0) => PipelinesResources.Format(nameof (ExceptionAgentQueueNotFound), arg0);

    public static string ExceptionAgentQueueNotFound(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (ExceptionAgentQueueNotFound), culture, arg0);

    public static string ExceptionAgentNoAvailableQueueFound() => PipelinesResources.Get(nameof (ExceptionAgentNoAvailableQueueFound));

    public static string ExceptionAgentNoAvailableQueueFound(CultureInfo culture) => PipelinesResources.Get(nameof (ExceptionAgentNoAvailableQueueFound), culture);

    public static string ExceptionIdentityNotFound(object arg0, object arg1) => PipelinesResources.Format(nameof (ExceptionIdentityNotFound), arg0, arg1);

    public static string ExceptionIdentityNotFound(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (ExceptionIdentityNotFound), culture, arg0, arg1);

    public static string ExceptionInvalidToken() => PipelinesResources.Get(nameof (ExceptionInvalidToken));

    public static string ExceptionInvalidToken(CultureInfo culture) => PipelinesResources.Get(nameof (ExceptionInvalidToken), culture);

    public static string ExceptionPayloadSignatureMismatch() => PipelinesResources.Get(nameof (ExceptionPayloadSignatureMismatch));

    public static string ExceptionPayloadSignatureMismatch(CultureInfo culture) => PipelinesResources.Get(nameof (ExceptionPayloadSignatureMismatch), culture);

    public static string ExceptionProjectCreationFailed(object arg0) => PipelinesResources.Format(nameof (ExceptionProjectCreationFailed), arg0);

    public static string ExceptionProjectCreationFailed(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (ExceptionProjectCreationFailed), culture, arg0);

    public static string ExceptionServiceEndpointNotFound(object arg0) => PipelinesResources.Format(nameof (ExceptionServiceEndpointNotFound), arg0);

    public static string ExceptionServiceEndpointNotFound(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (ExceptionServiceEndpointNotFound), culture, arg0);

    public static string ExceptionServiceEndpointNotFoundForRepository(object arg0) => PipelinesResources.Format(nameof (ExceptionServiceEndpointNotFoundForRepository), arg0);

    public static string ExceptionServiceEndpointNotFoundForRepository(
      object arg0,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (ExceptionServiceEndpointNotFoundForRepository), culture, arg0);
    }

    public static string ExceptionStatusUpdateFailed(object arg0, object arg1) => PipelinesResources.Format(nameof (ExceptionStatusUpdateFailed), arg0, arg1);

    public static string ExceptionStatusUpdateFailed(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (ExceptionStatusUpdateFailed), culture, arg0, arg1);

    public static string ExceptionTestServiceNotFound() => PipelinesResources.Get(nameof (ExceptionTestServiceNotFound));

    public static string ExceptionTestServiceNotFound(CultureInfo culture) => PipelinesResources.Get(nameof (ExceptionTestServiceNotFound), culture);

    public static string ExceptionTokenGenerationFailed(object arg0) => PipelinesResources.Format(nameof (ExceptionTokenGenerationFailed), arg0);

    public static string ExceptionTokenGenerationFailed(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (ExceptionTokenGenerationFailed), culture, arg0);

    public static string FileContentProviderFailed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return PipelinesResources.Format(nameof (FileContentProviderFailed), arg0, arg1, arg2, arg3, arg4);
    }

    public static string FileContentProviderFailed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (FileContentProviderFailed), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string GitHubAppConnectedToAnotherOrg() => PipelinesResources.Get(nameof (GitHubAppConnectedToAnotherOrg));

    public static string GitHubAppConnectedToAnotherOrg(CultureInfo culture) => PipelinesResources.Get(nameof (GitHubAppConnectedToAnotherOrg), culture);

    public static string GitHubCheckRunOutputBuildCanceledTitle(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildCanceledTitle), arg0);

    public static string GitHubCheckRunOutputBuildCanceledTitle(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildCanceledTitle), culture, arg0);

    public static string GitHubCheckRunOutputBuildFailedTitle(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildFailedTitle), arg0);

    public static string GitHubCheckRunOutputBuildFailedTitle(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildFailedTitle), culture, arg0);

    public static string GitHubCheckRunOutputBuildPartiallySucceededTitle(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildPartiallySucceededTitle), arg0);

    public static string GitHubCheckRunOutputBuildPartiallySucceededTitle(
      object arg0,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildPartiallySucceededTitle), culture, arg0);
    }

    public static string GitHubCheckRunOutputBuildSucceededTitle(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildSucceededTitle), arg0);

    public static string GitHubCheckRunOutputBuildSucceededTitle(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildSucceededTitle), culture, arg0);

    public static string GitHubCheckRunOutputBuildWithFailedTestsTitle(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildWithFailedTestsTitle), arg0);

    public static string GitHubCheckRunOutputBuildWithFailedTestsTitle(
      object arg0,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputBuildWithFailedTestsTitle), culture, arg0);
    }

    public static string GitHubCheckRunOutputCodeCoverageDetailsCoverageText(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputCodeCoverageDetailsCoverageText), arg0, arg1, arg2, arg3);
    }

    public static string GitHubCheckRunOutputCodeCoverageDetailsCoverageText(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputCodeCoverageDetailsCoverageText), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubCheckRunOutputDefaultTitle(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputDefaultTitle), arg0);

    public static string GitHubCheckRunOutputDefaultTitle(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (GitHubCheckRunOutputDefaultTitle), culture, arg0);

    public static string GitHubCheckRunOutputDetailsCodeCoverageHeader(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputDetailsCodeCoverageHeader), arg0);

    public static string GitHubCheckRunOutputDetailsCodeCoverageHeader(
      object arg0,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputDetailsCodeCoverageHeader), culture, arg0);
    }

    public static string GitHubCheckRunOutputDetailsResultsHeader(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputDetailsResultsHeader), arg0);

    public static string GitHubCheckRunOutputDetailsResultsHeader(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (GitHubCheckRunOutputDetailsResultsHeader), culture, arg0);

    public static string GitHubCheckRunOutputDetailsTestSectionHeader(object arg0) => PipelinesResources.Format(nameof (GitHubCheckRunOutputDetailsTestSectionHeader), arg0);

    public static string GitHubCheckRunOutputDetailsTestSectionHeader(
      object arg0,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputDetailsTestSectionHeader), culture, arg0);
    }

    public static string GitHubCheckRunOutputSummary(object arg0, object arg1, object arg2) => PipelinesResources.Format(nameof (GitHubCheckRunOutputSummary), arg0, arg1, arg2);

    public static string GitHubCheckRunOutputSummary(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubCheckRunOutputSummary), culture, arg0, arg1, arg2);
    }

    public static string GitHubCheckRunTestFailedStatus() => PipelinesResources.Get(nameof (GitHubCheckRunTestFailedStatus));

    public static string GitHubCheckRunTestFailedStatus(CultureInfo culture) => PipelinesResources.Get(nameof (GitHubCheckRunTestFailedStatus), culture);

    public static string GitHubConnectionExceptionCannotCreateBuildRepository(object arg0) => PipelinesResources.Format(nameof (GitHubConnectionExceptionCannotCreateBuildRepository), arg0);

    public static string GitHubConnectionExceptionCannotCreateBuildRepository(
      object arg0,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (GitHubConnectionExceptionCannotCreateBuildRepository), culture, arg0);
    }

    public static string GitHubProviderUserName() => PipelinesResources.Get(nameof (GitHubProviderUserName));

    public static string GitHubProviderUserName(CultureInfo culture) => PipelinesResources.Get(nameof (GitHubProviderUserName), culture);

    public static string MissingYamlDetails(object arg0, object arg1) => PipelinesResources.Format(nameof (MissingYamlDetails), arg0, arg1);

    public static string MissingYamlDetails(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (MissingYamlDetails), culture, arg0, arg1);

    public static string MissingYamlWithPullRequestDetails(object arg0, object arg1, object arg2) => PipelinesResources.Format(nameof (MissingYamlWithPullRequestDetails), arg0, arg1, arg2);

    public static string MissingYamlWithPullRequestDetails(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (MissingYamlWithPullRequestDetails), culture, arg0, arg1, arg2);
    }

    public static string MissingYamlSummary() => PipelinesResources.Get(nameof (MissingYamlSummary));

    public static string MissingYamlSummary(CultureInfo culture) => PipelinesResources.Get(nameof (MissingYamlSummary), culture);

    public static string MissingYamlTitle(object arg0) => PipelinesResources.Format(nameof (MissingYamlTitle), arg0);

    public static string MissingYamlTitle(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (MissingYamlTitle), culture, arg0);

    public static string NewDefinitionComment(object arg0) => PipelinesResources.Format(nameof (NewDefinitionComment), arg0);

    public static string NewDefinitionComment(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (NewDefinitionComment), culture, arg0);

    public static string NewProjectDescription(object arg0) => PipelinesResources.Format(nameof (NewProjectDescription), arg0);

    public static string NewProjectDescription(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (NewProjectDescription), culture, arg0);

    public static string TemplateNotFound(object arg0) => PipelinesResources.Format(nameof (TemplateNotFound), arg0);

    public static string TemplateNotFound(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (TemplateNotFound), culture, arg0);

    public static string TestDetailsFailingTestsPRText(object arg0, object arg1) => PipelinesResources.Format(nameof (TestDetailsFailingTestsPRText), arg0, arg1);

    public static string TestDetailsFailingTestsPRText(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (TestDetailsFailingTestsPRText), culture, arg0, arg1);
    }

    public static string TestDetailsFailingTestsText(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelinesResources.Format(nameof (TestDetailsFailingTestsText), arg0, arg1, arg2, arg3);
    }

    public static string TestDetailsFailingTestsText(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelinesResources.Format(nameof (TestDetailsFailingTestsText), culture, arg0, arg1, arg2, arg3);
    }

    public static string TestDetailsOtherTestsText(object arg0, object arg1) => PipelinesResources.Format(nameof (TestDetailsOtherTestsText), arg0, arg1);

    public static string TestDetailsOtherTestsText(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (TestDetailsOtherTestsText), culture, arg0, arg1);

    public static string TestDetailsPassingTestsText(object arg0, object arg1) => PipelinesResources.Format(nameof (TestDetailsPassingTestsText), arg0, arg1);

    public static string TestDetailsPassingTestsText(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (TestDetailsPassingTestsText), culture, arg0, arg1);

    public static string TestDetailsTotalTestText(object arg0) => PipelinesResources.Format(nameof (TestDetailsTotalTestText), arg0);

    public static string TestDetailsTotalTestText(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (TestDetailsTotalTestText), culture, arg0);

    public static string UnkownUserName() => PipelinesResources.Get(nameof (UnkownUserName));

    public static string UnkownUserName(CultureInfo culture) => PipelinesResources.Get(nameof (UnkownUserName), culture);

    public static string UnkownItem() => PipelinesResources.Get(nameof (UnkownItem));

    public static string UnkownItem(CultureInfo culture) => PipelinesResources.Get(nameof (UnkownItem), culture);

    public static string ArtifactNameWithSuffix(object arg0, object arg1) => PipelinesResources.Format(nameof (ArtifactNameWithSuffix), arg0, arg1);

    public static string ArtifactNameWithSuffix(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (ArtifactNameWithSuffix), culture, arg0, arg1);

    public static string AzureSubscriptionCannotBeEmpty() => PipelinesResources.Get(nameof (AzureSubscriptionCannotBeEmpty));

    public static string AzureSubscriptionCannotBeEmpty(CultureInfo culture) => PipelinesResources.Get(nameof (AzureSubscriptionCannotBeEmpty), culture);

    public static string TenantIdCannotBeEmpty() => PipelinesResources.Get(nameof (TenantIdCannotBeEmpty));

    public static string TenantIdCannotBeEmpty(CultureInfo culture) => PipelinesResources.Get(nameof (TenantIdCannotBeEmpty), culture);

    public static string TenantIdsCannotBeDifferent(object arg0, object arg1) => PipelinesResources.Format(nameof (TenantIdsCannotBeDifferent), arg0, arg1);

    public static string TenantIdsCannotBeDifferent(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (TenantIdsCannotBeDifferent), culture, arg0, arg1);

    public static string AzureRmServiceEndpointCreationFailed(object arg0) => PipelinesResources.Format(nameof (AzureRmServiceEndpointCreationFailed), arg0);

    public static string AzureRmServiceEndpointCreationFailed(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (AzureRmServiceEndpointCreationFailed), culture, arg0);

    public static string JiraAppInstallationDataNotFound() => PipelinesResources.Get(nameof (JiraAppInstallationDataNotFound));

    public static string JiraAppInstallationDataNotFound(CultureInfo culture) => PipelinesResources.Get(nameof (JiraAppInstallationDataNotFound), culture);

    public static string JiraConnectAppConnectAlreadyExist(object arg0) => PipelinesResources.Format(nameof (JiraConnectAppConnectAlreadyExist), arg0);

    public static string JiraConnectAppConnectAlreadyExist(object arg0, CultureInfo culture) => PipelinesResources.Format(nameof (JiraConnectAppConnectAlreadyExist), culture, arg0);

    public static string JwtTokenNotPresentInHeaders() => PipelinesResources.Get(nameof (JwtTokenNotPresentInHeaders));

    public static string JwtTokenNotPresentInHeaders(CultureInfo culture) => PipelinesResources.Get(nameof (JwtTokenNotPresentInHeaders), culture);

    public static string ResourceNameFormat(object arg0, object arg1) => PipelinesResources.Format(nameof (ResourceNameFormat), arg0, arg1);

    public static string ResourceNameFormat(object arg0, object arg1, CultureInfo culture) => PipelinesResources.Format(nameof (ResourceNameFormat), culture, arg0, arg1);

    public static string InvalidEnvironmentCreateRequest() => PipelinesResources.Get(nameof (InvalidEnvironmentCreateRequest));

    public static string InvalidEnvironmentCreateRequest(CultureInfo culture) => PipelinesResources.Get(nameof (InvalidEnvironmentCreateRequest), culture);

    public static string JiraConnectAppConnectionCreateAccessDenied() => PipelinesResources.Get(nameof (JiraConnectAppConnectionCreateAccessDenied));

    public static string JiraConnectAppConnectionCreateAccessDenied(CultureInfo culture) => PipelinesResources.Get(nameof (JiraConnectAppConnectionCreateAccessDenied), culture);

    public static string JiraAppDescriptorNotFound() => PipelinesResources.Get(nameof (JiraAppDescriptorNotFound));

    public static string JiraAppDescriptorNotFound(CultureInfo culture) => PipelinesResources.Get(nameof (JiraAppDescriptorNotFound), culture);

    public static string JiraAppDescriptorShouldNotBeEmpty() => PipelinesResources.Get(nameof (JiraAppDescriptorShouldNotBeEmpty));

    public static string JiraAppDescriptorShouldNotBeEmpty(CultureInfo culture) => PipelinesResources.Get(nameof (JiraAppDescriptorShouldNotBeEmpty), culture);
  }
}
