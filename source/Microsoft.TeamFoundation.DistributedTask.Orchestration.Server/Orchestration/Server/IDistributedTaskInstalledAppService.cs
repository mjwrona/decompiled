// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskInstalledAppService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (DistributedTaskInstalledAppService))]
  public interface IDistributedTaskInstalledAppService : IVssFrameworkService
  {
    void AddInstallation(
      IVssRequestContext requestContext,
      string appId,
      string installationId,
      DistributedTaskInstalledAppData appData);

    IDictionary<string, DistributedTaskInstalledAppData> GetInstallations(
      IVssRequestContext requestContext,
      string appId);

    bool IsInstalled(IVssRequestContext requestContext, string appId);

    void RemoveInstallation(IVssRequestContext requestContext, string appId, string installationId);

    bool TryGetInstallationData(
      IVssRequestContext requestContext,
      string appId,
      string installationId,
      out DistributedTaskInstalledAppData appData);
  }
}
