// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IInternalHostDeletionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (HostDeletionService))]
  internal interface IInternalHostDeletionService : IHostDeletionService, IVssFrameworkService
  {
    void StopHost(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      bool skipPublishNotification = false);

    void RemoveHostInstanceMapping(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      ITFLogger logger);

    void DeleteServiceHost(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      HostDeletionReason hostDeletionReason,
      ITFLogger logger);
  }
}
