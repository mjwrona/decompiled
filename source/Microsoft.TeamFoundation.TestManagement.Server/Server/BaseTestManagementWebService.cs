// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BaseTestManagementWebService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public abstract class BaseTestManagementWebService : TeamFoundationWebService
  {
    private static readonly List<string> MicrosoftUserAgents = new List<string>()
    {
      "QTController.exe",
      "TFSBuildServiceHost.exe",
      "mtm.exe",
      "TestDispatcher.exe",
      "QTAgent32",
      "vstest.executionengine.x86.exe",
      "tcm.exe",
      "leviewer.exe",
      "MTMExecutionEndListener.exe",
      "devenv.exe",
      "mfbclient.exe"
    };

    protected override void EnterMethod(MethodInformation methodInformation)
    {
      this.CheckAndBlockSoapAccess();
      base.EnterMethod(methodInformation);
      if (this.RequestContext == null)
        return;
      this.RecordTelemetry(methodInformation.Name);
    }

    private void CheckAndBlockSoapAccess()
    {
      if (this.RequestContext.IsFeatureEnabled("TestManagement.Server.BlockAccessToTestSoapCalls"))
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("MethodName", this.RequestContext.Method?.Name);
        this.RequestContext.GetService<CustomerIntelligenceService>().Publish(this.RequestContext, "TestManagement", "CheckAndBlockTestSoapAccess", properties);
        throw new FeatureDisabledException(ServerResources.TestManagementSoapAccessBlocked);
      }
    }

    private void RecordTelemetry(string command)
    {
      IVssRequestContext requestContext = this.RequestContext;
      string userAgent = requestContext.UserAgent;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      if (requestContext.IsServicingContext)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
        int num1 = this.IsMicrosoftAgent(userAgent) ? 1 : 0;
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        command = num1 == 0 ? Path.Combine(TCMTelemetryConstants.NonMicrosoftUserAgents, command) : Path.Combine(TCMTelemetryConstants.MicrosoftUserAgents, command);
        string str = Path.Combine(TCMTelemetryConstants.TCMTelemetryRegistryRoot, command);
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        int num2 = service.GetValue<int>(vssRequestContext, (RegistryQuery) str, 0) + 1;
        registryEntryList.Add(new RegistryEntry(str, num2.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    private bool IsMicrosoftAgent(string userAgent)
    {
      foreach (string microsoftUserAgent in BaseTestManagementWebService.MicrosoftUserAgents)
      {
        if (userAgent.IndexOf(microsoftUserAgent, StringComparison.OrdinalIgnoreCase) >= 0)
          return true;
      }
      return false;
    }
  }
}
