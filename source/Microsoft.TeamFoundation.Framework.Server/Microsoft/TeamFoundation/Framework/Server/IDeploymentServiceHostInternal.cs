// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IDeploymentServiceHostInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IDeploymentServiceHostInternal
  {
    IVssRequestContext CreateContext(HostRequestType type, bool throwIfShutdown);

    TeamFoundationHostManagementService HostManagement { get; }

    TeamFoundationMetabase SharedMetabase { get; }

    int MaxSqlComponents { get; }

    Guid SystemServicePrincipal { get; }

    Guid S2STenantId { get; }

    IdentityDescriptor SystemDescriptor { get; }

    IRequestActor SystemActor { get; }

    DeploymentServiceHostOptions CreationOptions { get; }

    TeamFoundationTracingService TracingService { get; }
  }
}
