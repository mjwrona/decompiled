// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssDeploymentServiceHost
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssDeploymentServiceHost : 
    IVssServiceHost,
    IVssServiceHostControl,
    IDisposable,
    IVssServiceHostProperties
  {
    TimeSpan UserRequestTimeout { get; }

    IVssRequestContext CreateSystemContext(bool throwIfShutdown = true);

    ITeamFoundationDatabaseProperties DatabaseProperties { get; }

    bool IsHosted { get; }

    Guid ServiceInstanceType { get; }

    HostProcessType ProcessType { get; }
  }
}
