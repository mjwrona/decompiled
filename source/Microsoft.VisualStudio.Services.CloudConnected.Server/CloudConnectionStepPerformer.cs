// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectionStepPerformer
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  [StepPerformer("CloudConnection")]
  internal class CloudConnectionStepPerformer : TeamFoundationStepPerformerBase
  {
    [ServicingStep]
    public void CleanupConnectedAccountInfo(
      IVssRequestContext targetRequestContext,
      IServicingContext servicingContext)
    {
      targetRequestContext.CheckProjectCollectionRequestContext();
      Guid guid = targetRequestContext.GetService<CachedRegistryService>().GetValue<Guid>(targetRequestContext, (RegistryQuery) FrameworkServerConstants.SnapshotOriginalApplicationInstanceId, false, new Guid());
      if (guid == Guid.Empty)
      {
        servicingContext.Error("Original Server Id not found. Skip this step.");
      }
      else
      {
        servicingContext.LogInfo(string.Format("Original Server Id: {0}, Current Server Id : {1}", (object) guid, (object) servicingContext.DeploymentRequestContext.ServiceHost.InstanceId));
        if (!(guid != servicingContext.DeploymentRequestContext.ServiceHost.InstanceId))
          return;
        servicingContext.LogInfo("Clean up Connected Account Info from registry...");
        CachedRegistryService service = targetRequestContext.GetService<CachedRegistryService>();
        if (string.IsNullOrEmpty(service.GetValue(targetRequestContext, (RegistryQuery) CloudConnectedConstants.ConnectedServerAccountId, false, (string) null)))
          return;
        string registryPathPattern = CloudConnectedConstants.ConnectedServerRoot + "/...";
        service.DeleteEntries(targetRequestContext, registryPathPattern);
      }
    }
  }
}
