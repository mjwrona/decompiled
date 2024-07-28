// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskContributionPreInstallController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "preinstall")]
  [ClientIgnore]
  public sealed class TaskContributionPreInstallController : DistributedTaskApiController
  {
    private static readonly string s_layer = nameof (TaskContributionPreInstallController);

    [HttpPost]
    public async Task<ExtensionEventCallbackResult> PreInstall(ExtensionEventCallbackData data)
    {
      TaskContributionPreInstallController installController = this;
      ArgumentUtility.CheckForNull<ExtensionEventCallbackData>(data, nameof (data), "DistributedTask");
      installController.PublishTelemetry(data);
      try
      {
        IDistributedTaskService taskService = installController.TfsRequestContext.GetService<IDistributedTaskService>();
        if (installController.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          installController.TfsRequestContext.RunSynchronously((Func<Task>) (() => taskService.ValidateInstallAsync(this.TfsRequestContext, data.PublisherName, data.ExtensionName, data.Version)));
        }
        else
        {
          await taskService.ValidateInstallAsync(installController.TfsRequestContext, data.PublisherName, data.ExtensionName, data.Version);
          int maxDelaySeconds = 120;
          installController.TfsRequestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(installController.TfsRequestContext, (IEnumerable<Guid>) new Guid[1]
          {
            TaskConstants.TaskContributionInstallJob
          }, maxDelaySeconds, JobPriorityLevel.Highest);
        }
      }
      catch (Exception ex)
      {
        installController.TfsRequestContext.TraceError(10015035, TaskContributionPreInstallController.s_layer, "Exception occurred during pre-install check of the extension {0} with version {1} published by {2}. Exception is {3}", (object) data.ExtensionName, (object) data.Version, (object) data.PublisherName, (object) ex.ToString());
        return new ExtensionEventCallbackResult()
        {
          Allow = false,
          Message = ex.Message
        };
      }
      return new ExtensionEventCallbackResult()
      {
        Allow = true
      };
    }

    private void PublishTelemetry(ExtensionEventCallbackData data)
    {
      this.TfsRequestContext.TraceInfo(10015036, TaskContributionPreInstallController.s_layer, "Preinstall check for extension {0} with version {1} published by {2}", (object) data.ExtensionName, (object) data.Version, (object) data.PublisherName);
      if (data.Version == null)
        this.TfsRequestContext.TraceWarning(10015037, "ContributionBuildTask", "Extension version is null for extension '{0}' published by '{1}'", (object) data.ExtensionName, (object) data.PublisherName);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Event Type", TaskConstants.PreInstalledEventName);
      properties.Add("Extension Name", data.ExtensionName);
      properties.Add("Publisher Name", data.PublisherName);
      properties.Add("Host Id", (object) this.TfsRequestContext.ServiceHost.InstanceId);
      properties.Add("Version", data.Version);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "DistributedTask", "ContributionBuildTask", properties);
    }
  }
}
