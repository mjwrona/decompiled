// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ReparentCollectionExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class ReparentCollectionExtensions
  {
    private const string c_area = "ReparentCollection";
    private const string c_layer = "ReparentCollectionExtensions";

    public static void MarkAsFatalReparentCollectionException(this Exception ex) => ex.MarkAsFatalServicingOrchestrationException();

    public static T AsFatalReparentCollectionException<T>(this T ex) where T : Exception => ex.AsFatalServicingOrchestrationException<T>();

    public static bool IsFatalReparentCollectionException(this Exception ex) => ex.IsFatalServicingOrchestrationException();

    public static ReparentCollectionHostAction GetReparentCollectionHostAction(
      this IVssRequestContext requestContext,
      ServiceInstance serviceInstance,
      Guid targetOrganizationId)
    {
      ArgumentUtility.CheckForNull<ServiceInstance>(serviceInstance, nameof (serviceInstance));
      ArgumentUtility.CheckForEmptyGuid(targetOrganizationId, nameof (targetOrganizationId));
      Guid guid = !(serviceInstance.InstanceType == ServiceInstanceTypes.SPS) ? ReparentCollectionExtensions.GetTargetOrganizationServiceInstanceId(requestContext, serviceInstance, targetOrganizationId) : PartitionedClientHelper.GetSpsInstanceForHostId(requestContext, targetOrganizationId);
      if (!serviceInstance.HasPhysicalOrganizationHosts())
        return ReparentCollectionHostAction.SwitchParent;
      if (serviceInstance.HasLightweightCollectionHosts())
        return ReparentCollectionHostAction.DeleteHost;
      return guid == serviceInstance.InstanceId ? ReparentCollectionHostAction.SwitchParent : ReparentCollectionHostAction.MoveHost;
    }

    public static ReparentCollectionHostAction GetReparentCollectionHostAction(
      this IVssRequestContext requestContext,
      Guid targetOrganizationId)
    {
      ServiceInstance serviceInstance;
      if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
        serviceInstance = new ServiceInstance()
        {
          InstanceId = requestContext.ServiceInstanceId(),
          InstanceType = requestContext.ServiceInstanceType()
        };
      else
        serviceInstance = requestContext.GetService<IInstanceManagementService>().GetServiceInstance(requestContext, requestContext.ServiceInstanceId());
      return requestContext.GetReparentCollectionHostAction(serviceInstance, targetOrganizationId);
    }

    private static Guid GetTargetOrganizationServiceInstanceId(
      IVssRequestContext requestContext,
      ServiceInstance serviceInstance,
      Guid targetOrganizationId)
    {
      try
      {
        Guid instanceType = serviceInstance.InstanceType;
        requestContext.TraceAlways(15080217, TraceLevel.Info, "ReparentCollection", nameof (ReparentCollectionExtensions), string.Format("Entering {0} {1}:{2}, {3}:{4}", (object) nameof (GetTargetOrganizationServiceInstanceId), (object) "instanceType", (object) instanceType, (object) nameof (targetOrganizationId), (object) targetOrganizationId));
        IVssDeploymentServiceHost deploymentHost = requestContext.ServiceHost.DeploymentServiceHost;
        (Exception innerException, Guid serviceInstanceId) = Task.Factory.StartNew<(Exception, Guid)>((Func<(Exception, Guid)>) (() => ReparentCollectionExtensions.GetTargetOrganizationServiceInstanceId(deploymentHost, instanceType, targetOrganizationId))).SyncResult<(Exception, Guid)>();
        if (innerException != null)
          throw new ApplicationException("An Exception was caught while invoking Task to perform GetTargetOrganizationServiceInstanceId", innerException);
        return serviceInstanceId;
      }
      finally
      {
        requestContext.TraceAlways(15080218, TraceLevel.Info, "ReparentCollection", nameof (ReparentCollectionExtensions), "Leaving GetTargetOrganizationServiceInstanceId");
      }
    }

    private static (Exception ex, Guid targetOrganizationServiceInstanceId) GetTargetOrganizationServiceInstanceId(
      IVssDeploymentServiceHost deploymentHost,
      Guid instanceType,
      Guid targetOrganizationId)
    {
      try
      {
        using (IVssRequestContext servicingContext = deploymentHost.CreateServicingContext())
          return ((Exception) null, servicingContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(servicingContext, targetOrganizationId, instanceType)?.ServiceInstance?.InstanceId ?? Guid.Empty);
      }
      catch (Exception ex)
      {
        Guid empty = Guid.Empty;
        return (ex, empty);
      }
    }
  }
}
