// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SpsIdentifier
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SpsIdentifier
  {
    private const string TraceArea = "Server";
    private const string TraceLayer = "SpsIdentifier";

    public static Guid GetSpsIdentifierForHostContext(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new NotSupportedException();
      IVssRequestContext rootContext = requestContext.RootContext;
      Guid guid = rootContext.ServiceInstanceType();
      requestContext.Trace(15190910, TraceLevel.Verbose, "Server", nameof (SpsIdentifier), "Service instance type: {0}", (object) rootContext.ServiceInstanceType());
      Guid sps = ServiceInstanceTypes.SPS;
      if (guid == sps)
      {
        Guid instanceId = rootContext.ServiceHost.DeploymentServiceHost.InstanceId;
        requestContext.Trace(15190911, TraceLevel.Verbose, "Server", nameof (SpsIdentifier), "Platform SPS service info: Root context: {0} | Deployment ID: {1}", (object) rootContext.ServiceHost.InstanceId, (object) instanceId);
        return instanceId;
      }
      ILocationService service = rootContext.GetService<ILocationService>();
      ServiceDefinition serviceDefinition = service.FindServiceDefinition(rootContext, "LocationService2", ServiceInstanceTypes.SPS);
      if (serviceDefinition != null && serviceDefinition.ParentIdentifier != Guid.Empty)
      {
        requestContext.TraceSerializedConditionally(15190912, TraceLevel.Verbose, "Server", nameof (SpsIdentifier), "spsDefinition: {0}", (object) serviceDefinition);
        return serviceDefinition.ParentIdentifier;
      }
      if (!rootContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidOperationException("Cannot find SPS instance associated with the hostContext");
      ILocationDataProvider locationData = service.GetLocationData(rootContext, ServiceInstanceTypes.SPS);
      requestContext.TraceSerializedConditionally(15190913, TraceLevel.Verbose, "Server", nameof (SpsIdentifier), "spsLocationData: {0}", (object) locationData);
      return locationData.HostId;
    }
  }
}
