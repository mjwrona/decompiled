// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IRequestContextInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authorization;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IRequestContextInternal
  {
    bool IsRootContext { get; }

    ISqlConnectionInfo DeploymentFrameworkConnectionInfo { get; }

    bool HasRequestTimedOut { get; }

    Thread MethodExecutionThread { get; }

    IReadOnlyList<IRequestActor> Actors { get; set; }

    VssRequestContextPriority RequestPriority { set; }

    void ResetCancel();

    void CheckCanceled();

    void RemoveDisposableResource(IDisposable resource);

    void DisposeDisposableResources();

    T[] GetDisposableResources<T>();

    void SetAuthenticatedUserName(string authenticatedUserName);

    void SetResponseCode(int responseCode);

    void SetDomainUserName(string domainUserName);

    void AddLease(ILeaseInfo lease);

    void RemoveLease(string leaseName);

    IdentityValidationStatus IdentityValidationStatus { get; set; }

    void ClearActors();

    void SetDataspaceIdentifier(Guid dataspaceIdentifier);

    void ResetActivityId();

    void SetE2EId(Guid identifier);

    void SetOrchestrationId(string identifier);
  }
}
