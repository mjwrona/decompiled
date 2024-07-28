// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IInternalTeamFoundationHostManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationHostManagementService))]
  internal interface IInternalTeamFoundationHostManagementService : 
    ITeamFoundationHostManagementService,
    IVssFrameworkService
  {
    IVssRequestContext BeginRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      RequestContextType contextType,
      bool loadIfNecessary,
      bool throwIfShutdown,
      IReadOnlyList<IRequestActor> actors,
      HostRequestType type,
      params object[] additionalParameters);

    void SetupUserContext(IVssRequestContext userRequestContext, IdentityDescriptor userContext);

    void SetupUserContext(
      IVssRequestContext userRequestContext,
      IReadOnlyList<IRequestActor> actors,
      string authenticatedUserName = null,
      string domainUserName = null);

    void StartHostInternal(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostSubStatus subStatus = ServiceHostSubStatus.None,
      bool reenableJobs = true);

    DateTime UpdateServiceHostLastUserAccess(
      IVssRequestContext requestContext,
      IEnumerable<Guid> hostsToUpdate);

    Guid ProcessId { get; }
  }
}
