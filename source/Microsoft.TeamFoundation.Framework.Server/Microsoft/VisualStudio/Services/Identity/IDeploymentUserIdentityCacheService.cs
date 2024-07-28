// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IDeploymentUserIdentityCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DefaultServiceImplementation(typeof (DeploymentUserIdentityCacheService))]
  public interface IDeploymentUserIdentityCacheService : IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Identity.Identity Get(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor);

    Microsoft.VisualStudio.Services.Identity.Identity Get(
      IVssRequestContext deploymentContext,
      IdentityDescriptor identityDescriptor);

    Microsoft.VisualStudio.Services.Identity.Identity Get(
      IVssRequestContext deploymentContext,
      Guid identityId);

    void Set(IVssRequestContext deploymentContext, Microsoft.VisualStudio.Services.Identity.Identity identity);

    void Remove(IVssRequestContext deploymentContext, Guid identityId, bool invalidateRemoteCache = true);

    void SendIdentityChangedNotification(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity);

    void SendIdentityChangedNotification(
      IVssRequestContext requestContext,
      DeploymentUserIdentityChange changeData);
  }
}
