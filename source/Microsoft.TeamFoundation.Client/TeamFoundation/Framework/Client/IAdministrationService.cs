// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IAdministrationService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface IAdministrationService
  {
    void CancelRequest(Guid hostId, long requestId, string reason);

    TeamFoundationServiceHostActivity QueryActiveRequests(
      TfsConnection teamFoundationServer,
      bool includeDetails);

    TeamFoundationServiceHostActivity QueryActiveRequests(Guid hostId, bool includeDetails);

    ReadOnlyCollection<TeamFoundationServiceHostActivity> QueryActiveRequests(
      IEnumerable<Guid> hostIds,
      bool includeDetails);
  }
}
