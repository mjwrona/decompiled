// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ITaskHubLicensePlugin
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [InheritedExport]
  internal interface ITaskHubLicensePlugin
  {
    IList<ResourceLimit> GetResourceLimits(IVssRequestContext requestContext);

    int? GetUsedHostedMinutesForPrivateProjects(IVssRequestContext requestContext);

    TaskHubLicenseDetails GetTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      bool includeEnterpriseUsersCount = false,
      bool includeHostedAgentMinutesCount = false);

    TaskHubLicenseDetails UpdateTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      TaskHubLicenseDetails taskHubLicenseDetails);

    int GetEnterpriseUsersCount(IVssRequestContext collectionRequestContext);
  }
}
