// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.Utilities
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public static class Utilities
  {
    private static ITestManagementRunHelper _testManagementRunHelper;
    private static ITestExecutionServiceIdentityHelper _identityHelper;

    public static Exception MapTestExecutionServiceException(TestExecutionServiceSqlException ex)
    {
      switch (ex)
      {
        case TestExecutionObjectAlreadyExistsSqlException _:
          return (Exception) new TestExecutionObjectAlreadyExistsException(ex.Message, (Exception) ex);
        case TestExecutionServiceInvalidOperationSqlException _:
          return (Exception) new TestExecutionServiceInvalidOperationException(ex.Message, (Exception) ex);
        case TestExecutionObjectNotFoundSqlException _:
          return (Exception) new TestExecutionObjectNotFoundException(ex.Message, (Exception) ex);
        case TestEnvironmentAlreadyExistsSqlException _:
          return (Exception) new TestEnvironmentAlreadyExistsException(ex.Message, (Exception) ex);
        default:
          return (Exception) ex;
      }
    }

    public static string GetProjectNameFromEnvironmentUrl(
      TestExecutionRequestContext context,
      string environmentUrl)
    {
      string serviceName = context?.RequestContext.ServiceName;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(environmentUrl, nameof (environmentUrl), serviceName);
      string uriString = environmentUrl;
      if (!Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
      {
        uriString = Uri.EscapeUriString(environmentUrl);
        if (!Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
          return string.Empty;
      }
      string[] segments = new Uri(uriString).Segments;
      int index = Array.IndexOf<string>(segments, "_apis/") - 1;
      return index >= 0 && index < ((IEnumerable<string>) segments).Count<string>() ? HttpUtility.UrlDecode(segments[index].TrimEnd('/', ' ')) : throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidDtlEnvironmentUrl, (object) environmentUrl));
    }

    public static TeamProjectReference HydrateProjectReference(
      TestExecutionRequestContext context,
      TeamProjectReference project)
    {
      ProjectInfo projectInfo = string.IsNullOrEmpty(project.Name) ? context.ProjectServiceHelper.GetProjectFromGuid(project.Id) : context.ProjectServiceHelper.GetProjectFromName(project.Name);
      if (project == null)
        return (TeamProjectReference) null;
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Name = projectInfo.Name
      };
    }

    public static T GetValueFromTfsRegistry<T>(
      TestExecutionRequestContext requestContext,
      string registryPath,
      T DefaultTime,
      ParseDelegate<T> parse)
      where T : struct
    {
      string s = requestContext.RequestContext.GetService<IVssRegistryService>().GetValue(requestContext.RequestContext, (RegistryQuery) registryPath, false, (string) null);
      requestContext.RequestContext.Trace(6200822, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.JobsLayer, "RegistryPath: {0}\n RegistryValue : {1}\nTimestamp: {2}\n", (object) registryPath, (object) s, (object) DateTimeOffset.Now);
      T result;
      if (!parse(s, out result))
        result = DefaultTime;
      return result;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByAccountId(
      TestExecutionRequestContext context,
      Guid accountId)
    {
      if (accountId != Guid.Empty)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = context.IdentityService.ReadIdentities(context.RequestContext, (IList<Guid>) new Guid[1]
        {
          accountId
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (source != null)
          return source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public static string CodeCoverageEnabled(XmlDocument testSettingsXml)
    {
      string nodePath1 = "/*[local-name()='TestSettings']/*[local-name()='Execution']/*[local-name()='AgentRule']/*[local-name()='DataCollectors']/*[local-name()='DataCollector']";
      string nodePath2 = "/*[local-name()= 'RunSettings']/*[local-name()= 'DataCollectionRunSettings']/*[local-name()='DataCollectors']/*[local-name()= 'DataCollector']";
      string codeCoverageUri1 = "datacollector://microsoft/CodeCoverage/2.0";
      string codeCoverageUri2 = "datacollector://microsoft/CodeCoverage/1.0";
      if (Utilities.ContainsCodeCoverage(testSettingsXml, nodePath1, codeCoverageUri1) || Utilities.ContainsCodeCoverage(testSettingsXml, nodePath2, codeCoverageUri1))
        return "DynamicCodeCoverage";
      return Utilities.ContainsCodeCoverage(testSettingsXml, nodePath1, codeCoverageUri2) || Utilities.ContainsCodeCoverage(testSettingsXml, nodePath2, codeCoverageUri2) ? "StaticCodeCoverage" : "None";
    }

    public static bool CheckCustomSlicingFlagInSettingsAndRemove(
      XmlDocument testSettingsXml,
      TestRunProperties testRunProperties)
    {
      string nodePath1 = "/*[local-name()= 'RunSettings']/*[local-name()= 'RunConfiguration']/*[local-name()='CustomSlicing']";
      string nodePath2 = "/*[local-name()='TestSettings']/*[local-name()='Execution']/*[local-name()='CustomSlicing']";
      return Utilities.TryGetCustomSliceFromXmlDocument(testSettingsXml, nodePath1, testRunProperties) || Utilities.TryGetCustomSliceFromXmlDocument(testSettingsXml, nodePath2, testRunProperties);
    }

    public static bool CheckRerunInformationInSettingsAndRemove(
      XmlDocument testSettingsXml,
      TestRunProperties testRunProperties)
    {
      string nodePath1 = "/*[local-name()= 'RunSettings']/*[local-name()= 'RunConfiguration']/*[local-name()='Rerun']";
      string nodePath2 = "/*[local-name()='TestSettings']/*[local-name()='Execution']/*[local-name()='Rerun']";
      return Utilities.TryGetRerunInfoFromXmlDocument(testSettingsXml, nodePath1, testRunProperties) || Utilities.TryGetRerunInfoFromXmlDocument(testSettingsXml, nodePath2, testRunProperties);
    }

    public static bool CheckIsTestImpactOnInSettingsAndRemove(XmlDocument testSettingsXml)
    {
      string nodePath1 = "/*[local-name()= 'RunSettings']/*[local-name()= 'RunConfiguration']/*[local-name()='TestImpact']";
      string nodePath2 = "/*[local-name()='TestSettings']/*[local-name()='Execution']/*[local-name()='TestImpact']";
      return Utilities.TryGetBoolValueFromXmlDocument(testSettingsXml, nodePath2) || Utilities.TryGetBoolValueFromXmlDocument(testSettingsXml, nodePath1);
    }

    public static int GetBaseLineBuildAndRemove(XmlDocument testSettingsXml)
    {
      string nodePath1 = "/*[local-name()= 'RunSettings']/*[local-name()= 'RunConfiguration']/*[local-name()='BaseLineRunId']";
      string nodePath2 = "/*[local-name()='TestSettings']/*[local-name()='Execution']/*[local-name()='BaseLineRunId']";
      int valueFromXmlDocument = Utilities.TryGetIntValueFromXmlDocument(testSettingsXml, nodePath2);
      if (valueFromXmlDocument == 0)
        valueFromXmlDocument = Utilities.TryGetIntValueFromXmlDocument(testSettingsXml, nodePath1);
      return valueFromXmlDocument;
    }

    public static string GetProjectUri(TestExecutionRequestContext context, string projectName) => context.ProjectServiceHelper.GetProjectUri(projectName);

    public static string GetQueueNameFromTestRunId(int testRunId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}-{2}", (object) DtaConstants.MessageQueuePrefix, (object) testRunId, (object) DtaConstants.MessageQueueId);

    public static TestAutomationRunSlice GetSliceToStopAgent(int testRunId = -1) => new TestAutomationRunSlice()
    {
      Id = -1,
      TestRunInformation = {
        TcmRun = new ShallowReference() { Id = testRunId },
        IsTestRunComplete = true
      }
    };

    public static bool SlicingAlreadyDone(TestExecutionRequestContext requestContext, int testRunId) => requestContext.RequestContext.GetService<ITestExecutionService>().QuerySlicesByTestRunId(requestContext, testRunId).FirstOrDefault<TestAutomationRunSlice>((Func<TestAutomationRunSlice, bool>) (s => s.Type == AutomatedTestRunSliceType.Execution)) != null;

    private static bool ContainsCodeCoverage(
      XmlDocument testSettingsXml,
      string nodePath,
      string codeCoverageUri)
    {
      foreach (XPathNavigator xpathNavigator in testSettingsXml.CreateNavigator().Select(nodePath))
      {
        string attribute = xpathNavigator.GetAttribute("uri", string.Empty);
        if (string.Equals(codeCoverageUri, attribute, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static bool TryGetCustomSliceFromXmlDocument(
      XmlDocument testSettingsXml,
      string nodePath,
      TestRunProperties testRunProperties)
    {
      XPathNavigator node = testSettingsXml.CreateNavigator().SelectSingleNode(nodePath);
      bool sliceFromXmlDocument = false;
      if (node != null)
      {
        if (string.Equals(node.GetAttribute("enabled", string.Empty), "true", StringComparison.OrdinalIgnoreCase))
        {
          sliceFromXmlDocument = true;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          testRunProperties.MaxAgents = node.GetAttributeValue<int>("maxagents", Utilities.\u003C\u003EO.\u003C0\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          testRunProperties.NumberOfTestCasesPerSlice = node.GetAttributeValue<int>("numberOfTestCasesPerSlice", Utilities.\u003C\u003EO.\u003C0\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          testRunProperties.IsTimeBasedSlicing = node.GetAttributeValue<bool>("isTimeBasedSlicing", Utilities.\u003C\u003EO.\u003C1\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C1\u003E__TryParse = new ParseDelegate<bool>(bool.TryParse)));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          testRunProperties.SliceTime = node.GetAttributeValue<int>("sliceTime", Utilities.\u003C\u003EO.\u003C0\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
        }
        node.DeleteSelf();
      }
      return sliceFromXmlDocument;
    }

    private static bool TryGetRerunInfoFromXmlDocument(
      XmlDocument testSettingsXml,
      string nodePath,
      TestRunProperties testRunProperties)
    {
      XPathNavigator node = testSettingsXml.CreateNavigator().SelectSingleNode(nodePath);
      if (node == null)
        return false;
      bool infoFromXmlDocument = false;
      if (string.Equals(node.GetAttribute("enabled", string.Empty), "true", StringComparison.OrdinalIgnoreCase))
      {
        infoFromXmlDocument = true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        testRunProperties.RerunFailedThreshold = node.GetAttributeValue<int>("rerunFailedThreshold", Utilities.\u003C\u003EO.\u003C0\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        testRunProperties.RerunMaxAttempts = node.GetAttributeValue<int>("rerunMaxAttempts", Utilities.\u003C\u003EO.\u003C0\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        testRunProperties.RerunFailedTestCasesMaxLimit = node.GetAttributeValue<int>("rerunFailedTestCasesMaxLimit", Utilities.\u003C\u003EO.\u003C0\u003E__TryParse ?? (Utilities.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
      }
      node.DeleteSelf();
      return infoFromXmlDocument;
    }

    private static bool TryGetBoolValueFromXmlDocument(XmlDocument testSettingsXml, string nodePath)
    {
      XPathNavigator xpathNavigator = testSettingsXml.CreateNavigator().SelectSingleNode(nodePath);
      if (xpathNavigator != null)
      {
        string attribute = xpathNavigator.GetAttribute("enabled", string.Empty);
        xpathNavigator.DeleteSelf();
        if (string.Equals(attribute, "true", StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static int TryGetIntValueFromXmlDocument(XmlDocument testSettingsXml, string nodePath)
    {
      XPathNavigator xpathNavigator = testSettingsXml.CreateNavigator().SelectSingleNode(nodePath);
      if (xpathNavigator == null)
        return 0;
      string attribute = xpathNavigator.GetAttribute("value", string.Empty);
      xpathNavigator.DeleteSelf();
      int result;
      int.TryParse(attribute, out result);
      return result;
    }

    public static ITestExecutionServiceIdentityHelper IdentityHelper
    {
      get => Utilities._identityHelper ?? (Utilities._identityHelper = (ITestExecutionServiceIdentityHelper) new TestExecutionServiceIdentityHelper());
      set => Utilities._identityHelper = value;
    }

    public static ITestManagementRunHelper TestManagementRunHelper
    {
      get => Utilities._testManagementRunHelper ?? (Utilities._testManagementRunHelper = (ITestManagementRunHelper) new Microsoft.TeamFoundation.TestExecution.Server.TestManagementRunHelper());
      set => Utilities._testManagementRunHelper = value;
    }
  }
}
