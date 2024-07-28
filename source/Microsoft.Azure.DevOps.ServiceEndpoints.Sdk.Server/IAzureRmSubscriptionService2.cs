// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.IAzureRmSubscriptionService2
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  [DefaultServiceImplementation(typeof (FrameworkAzureRmSubscriptionService))]
  public interface IAzureRmSubscriptionService2 : IVssFrameworkService
  {
    AzureSubscriptionQueryResult GetAzureSubscriptions(IVssRequestContext tfsRequestContext);

    AzureManagementGroupQueryResult GetAzureManagementGroups(IVssRequestContext vssRequestContext);
  }
}
