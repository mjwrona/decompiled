// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Hub.IBuildTestHubDispatcher
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server.Hub
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TestHubDispatcher))]
  public interface IBuildTestHubDispatcher : IVssFrameworkService
  {
    Task Watch(IVssRequestContext requestContext, int buildId, string clientId);

    Task UnWatch(IVssRequestContext requestContext, int buildId, string clientId);

    void SendTestRunStatsChanged(IVssRequestContext requestContext, int buildId);

    void HandleTestRunStatsChange(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int buildId);

    bool AnyClient(IVssRequestContext requestContext, int buildId);

    void DeleteNotification(IVssRequestContext requestContext, Guid projectId, int buildId);
  }
}
