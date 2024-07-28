// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SpsToUserMigrationUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SpsToUserMigrationUtil
  {
    private const string Area = "SpsToUserMigrationUtil";

    internal static TResult PerformOperationOnLegacySpsIdentityStore<TResult>(
      IVssRequestContext deploymentContext,
      Func<IdentityHttpClient, TResult> onFrameworkOrSecondarySps,
      Func<ILegacySpsIdentityServiceInternal, TResult> onPrimarySps,
      [CallerMemberName] string caller = null)
    {
      string layer = "PerformOperationOnLegacySpsIdentityStore@" + caller;
      IdentityHttpClient clientTargetingPrimarySps = SpsToUserMigrationUtil.GetClientTargetingPrimarySps<IdentityHttpClient>(deploymentContext, caller);
      if (clientTargetingPrimarySps != null)
      {
        deploymentContext.TraceDataConditionally(88545651, TraceLevel.Verbose, nameof (SpsToUserMigrationUtil), layer, "Making call to legacy SPS identity store using client targeting primary SPS", (Func<object>) (() => (object) new
        {
          name = clientTargetingPrimarySps.GetType(),
          address = clientTargetingPrimarySps.BaseAddress
        }), nameof (PerformOperationOnLegacySpsIdentityStore));
        return onFrameworkOrSecondarySps(clientTargetingPrimarySps);
      }
      ILegacySpsIdentityServiceInternal serviceOnPrimarySps = deploymentContext.GetService<ILegacySpsIdentityServiceInternal>();
      deploymentContext.TraceDataConditionally(88545652, TraceLevel.Verbose, nameof (SpsToUserMigrationUtil), layer, "Making call to legacy SPS identity store using service on primary SPS", (Func<object>) (() => (object) new
      {
        name = serviceOnPrimarySps.GetType()
      }), nameof (PerformOperationOnLegacySpsIdentityStore));
      return onPrimarySps(serviceOnPrimarySps);
    }

    public static TClient GetClientTargetingPrimarySps<TClient>(
      IVssRequestContext deploymentContext,
      [CallerMemberName] string caller = null)
      where TClient : class, IVssHttpClient
    {
      deploymentContext.CheckHostedDeployment();
      deploymentContext.CheckDeploymentRequestContext();
      string str = "GetClientTargetingPrimarySps@" + caller;
      if (deploymentContext.ServiceHost.InstanceId == SpsToUserMigrationUtil.Constants.SpsSu1InstanceId)
      {
        deploymentContext.TraceDataConditionally(88545661, TraceLevel.Verbose, nameof (SpsToUserMigrationUtil), str, "Skipping client creation since we are already on primary SPS in production (SPS SU1)", methodName: nameof (GetClientTargetingPrimarySps));
        return default (TClient);
      }
      IList<PartitionContainer> spsContainers = deploymentContext.GetService<IPartitioningService>().QueryPartitionContainers(deploymentContext, ServiceInstanceTypes.SPS);
      if (spsContainers.Count == 0)
        throw new CannotFindLegacySpsIdentityStoreException("Partitioning service did not return any SPS scale units");
      if (spsContainers.Count == 1)
      {
        deploymentContext.TraceDataConditionally(88545663, TraceLevel.Verbose, nameof (SpsToUserMigrationUtil), str, "Skipping client creation since we are already on primary SPS on a single-instance-SPS deployment", (Func<object>) (() => (object) spsContainers), nameof (GetClientTargetingPrimarySps));
        return default (TClient);
      }
      deploymentContext.TraceDataConditionally(88545664, TraceLevel.Verbose, nameof (SpsToUserMigrationUtil), str, "Found the following SPS scale units", (Func<object>) (() => (object) spsContainers), nameof (GetClientTargetingPrimarySps));
      PartitionContainer primarySpsContainer = spsContainers.FirstOrDefault<PartitionContainer>((Func<PartitionContainer, bool>) (c => c.ContainerId == SpsToUserMigrationUtil.Constants.SpsSu1InstanceId)) ?? spsContainers.First<PartitionContainer>((Func<PartitionContainer, bool>) (c => c.ContainerId != deploymentContext.ServiceHost.InstanceId));
      deploymentContext.TraceDataConditionally(88545665, TraceLevel.Verbose, nameof (SpsToUserMigrationUtil), str, "Selected primary SPS scale unit", (Func<object>) (() => (object) primarySpsContainer), nameof (GetClientTargetingPrimarySps));
      string address = primarySpsContainer.Address;
      return (deploymentContext.ClientProvider as ICreateClient).CreateClient<TClient>(deploymentContext, new Uri(address), str, (ApiResourceLocationCollection) null, false);
    }

    private static class Constants
    {
      public static readonly Guid SpsSu1InstanceId = new Guid("A5CA35EB-148E-4CCD-BBB3-D31576D75958");
    }
  }
}
