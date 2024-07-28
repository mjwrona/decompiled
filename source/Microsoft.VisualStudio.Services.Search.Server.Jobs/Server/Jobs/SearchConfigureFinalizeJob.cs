// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.SearchConfigureFinalizeJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class SearchConfigureFinalizeJob : ITeamFoundationJobExtension
  {
    private readonly StringBuilder m_resultMessage = new StringBuilder();
    private readonly HashSet<string> m_installedExtensions = new HashSet<string>();
    private const string ResetCollectionIndexingStateFlag = "/Service/ALMSearch/Settings/CRESETSEARCHINDEXINGSTATE";
    private const string InstallSearchExtensionOnExistingCollectionEntries = "/Service/ALMSearch/Settings/CINSTALLSEARCHEXTENSIONONCOLLECTION";
    private const string TriggerSearchIndexingOnExistingCollectionEntries = "/Service/ALMSearch/Settings/CTRIGGERSEARCHINDEXINGONEXISTINGCOLLECTION";
    private const string ExtensionPublisher = "ms";
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080329, "Indexing Pipeline", "Job", nameof (Run));
      TeamFoundationJobExecutionResult jobExecutionResult1 = TeamFoundationJobExecutionResult.Failed;
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        TeamFoundationJobExecutionResult jobExecutionResult2 = TeamFoundationJobExecutionResult.Succeeded;
        resultMessage = "SearchConfigureFinalizeJob applicable only for On-Premises.";
        return jobExecutionResult2;
      }
      bool flag = true;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (string.Equals(vssRequestContext.GetService<ISqlRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsSearchConfigured", false, (string) null), bool.TrueString, StringComparison.OrdinalIgnoreCase))
        {
          ISqlRegistryService service = requestContext.GetService<ISqlRegistryService>();
          string a = service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/CRESETSEARCHINDEXINGSTATE", false, (string) null);
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("[CRESETSEARCHINDEXINGSTATE: '{0}'] ", (object) a)));
          if (string.Equals(a, bool.TrueString, StringComparison.OrdinalIgnoreCase))
          {
            this.ResetIndexingState(requestContext);
            service.SetValue<string>(requestContext, "/Service/ALMSearch/Settings/CRESETSEARCHINDEXINGSTATE", bool.FalseString);
          }
          this.m_installedExtensions.Clear();
          string extensionsToInstall = service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/CINSTALLSEARCHEXTENSIONONCOLLECTION", false, (string) null);
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("[CINSTALLSEARCHEXTENSIONONCOLLECTION: '{0}'] ", (object) extensionsToInstall)));
          if (!string.IsNullOrWhiteSpace(extensionsToInstall))
          {
            if (!this.InstallExtensions(requestContext, extensionsToInstall))
              flag = false;
            service.SetValue<string>(requestContext, "/Service/ALMSearch/Settings/CINSTALLSEARCHEXTENSIONONCOLLECTION", string.Empty);
          }
          string extensionAndJobGuidsStr = service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/CTRIGGERSEARCHINDEXINGONEXISTINGCOLLECTION", false, (string) null);
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("[CTRIGGERSEARCHINDEXINGONEXISTINGCOLLECTION: '{0}'] ", (object) extensionAndJobGuidsStr)));
          if (!string.IsNullOrWhiteSpace(extensionAndJobGuidsStr))
          {
            if (!this.TriggerIndexingOnInstalledExtensionHost(requestContext, extensionAndJobGuidsStr))
              flag = false;
            service.SetValue<string>(requestContext, "/Service/ALMSearch/Settings/CTRIGGERSEARCHINDEXINGONEXISTINGCOLLECTION", string.Empty);
          }
          if (flag)
            this.m_resultMessage.Append("Successfully completed SearchConfigureFinalizeJob. ");
          else
            this.m_resultMessage.Append("SearchConfigureFinalizeJob did not fully succeed. ");
        }
        else
          this.m_resultMessage.Append("Search is not configured. Nothing to process. ");
        jobExecutionResult1 = TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        jobExecutionResult1 = TeamFoundationJobExecutionResult.Failed;
        this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("SearchConfigureFinalizeJob failed with exception : {0}. ", (object) ex)));
      }
      finally
      {
        resultMessage = this.m_resultMessage.ToString();
        if (!flag)
          jobExecutionResult1 = TeamFoundationJobExecutionResult.Failed;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080329, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return jobExecutionResult1;
    }

    private void ResetIndexingState(IVssRequestContext requestContext)
    {
      using (SearchCleanupComponent component = requestContext.CreateComponent<SearchCleanupComponent>())
      {
        component.DeleteSearchIndexingUnits();
        this.m_resultMessage.Append("Truncated Search tables. ");
      }
    }

    private bool InstallExtensions(IVssRequestContext requestContext, string extensionsToInstall)
    {
      bool flag = true;
      ExtensionManagementHttpClient client = requestContext.GetClient<ExtensionManagementHttpClient>();
      string str = extensionsToInstall;
      char[] chArray = new char[1]{ ',' };
      foreach (string extensionName in str.Split(chArray))
      {
        try
        {
          if (client.InstallExtensionByNameAsync("ms", extensionName).SyncResult<InstalledExtension>() != null)
          {
            this.m_installedExtensions.Add(extensionName);
            this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully installed extension '{0}'. ", (object) extensionName)));
          }
          else
            this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Failed to install extension '{0}'. ", (object) extensionName)));
        }
        catch (ExtensionAlreadyInstalledException ex)
        {
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Extension '{0}' already installed. ", (object) extensionName)));
        }
        catch (Exception ex)
        {
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Error while attempting to install extension '{0}': {1}. ", (object) extensionName, (object) ex)));
          flag = false;
        }
      }
      return flag;
    }

    private bool TriggerIndexingOnInstalledExtensionHost(
      IVssRequestContext requestContext,
      string extensionAndJobGuidsStr)
    {
      bool flag = true;
      ExtensionManagementHttpClient client = requestContext.GetClient<ExtensionManagementHttpClient>();
      string str1 = extensionAndJobGuidsStr;
      char[] chArray = new char[1]{ ',' };
      foreach (string str2 in ((IEnumerable<string>) str1.Split(chArray)).ToList<string>())
      {
        string empty = string.Empty;
        Guid jobId = Guid.Empty;
        try
        {
          string[] strArray = str2.Split(':');
          empty = strArray[0];
          jobId = new Guid(strArray[1]);
          if (this.m_installedExtensions.Contains(empty))
          {
            this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Extension '{0}' already installed in prior step. Ignore re-trigger indexing. ", (object) empty)));
          }
          else
          {
            InstalledExtension installedExtension = (InstalledExtension) null;
            try
            {
              installedExtension = client.GetInstalledExtensionByNameAsync("ms", empty).Result;
            }
            catch (Exception ex)
            {
            }
            if (installedExtension != null)
            {
              requestContext.QueueDelayedNamedJob(jobId, 0, JobPriorityLevel.Normal);
              this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued AccountFaultInJob '{0}' for extension '{1}'. ", (object) jobId, (object) empty)));
            }
            else
              this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Extension '{0}' not installed for host. ", (object) empty)));
          }
        }
        catch (Exception ex)
        {
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Error while attempting to queue AccountFaultInJob '{0}' for extension '{1}': {2}. ", (object) jobId, (object) empty, (object) ex)));
          flag = false;
        }
      }
      return flag;
    }
  }
}
