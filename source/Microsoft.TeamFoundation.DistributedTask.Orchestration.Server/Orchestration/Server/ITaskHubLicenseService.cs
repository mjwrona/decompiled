// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ITaskHubLicenseService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkTaskHubLicenseService))]
  public interface ITaskHubLicenseService : IVssFrameworkService
  {
    TaskHubLicenseDetails GetTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      bool includeEnterpriseUsersCount = false,
      bool includeHostedAgentMinutesCount = false);

    TaskHubLicenseDetails UpdateTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      TaskHubLicenseDetails taskHubLicenseDetails);

    IList<ResourceLimit> GetResourceLimits(IVssRequestContext requestContext, bool allowStaleValues = true);

    void NotifyDataChanged(
      IVssRequestContext requestContext,
      OfferSubscriptionQuantityChangeMessage message);
  }
}
