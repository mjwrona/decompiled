// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationCounterService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationCounterService))]
  public interface ITeamFoundationCounterService : IVssFrameworkService
  {
    void CreateCounter(
      IVssRequestContext requestContext,
      string counterName,
      long nextCounterValue,
      string dataspaceCategory = null,
      Guid dataspaceIdentifier = default (Guid),
      bool isolated = false);

    bool CounterExists(
      IVssRequestContext requestContext,
      string counterName,
      string dataspaceCategory = null,
      Guid dataspaceIdentifier = default (Guid),
      bool isolated = false);

    bool IsIsolated(IVssRequestContext requestContext);

    long ReserveCounterIds(
      IVssRequestContext requestContext,
      string counterName,
      long countToReserve,
      string category = null,
      Guid dataSpaceIdentifier = default (Guid),
      bool isolated = false);

    void ResetCounter(
      IVssRequestContext requestContext,
      string counterName,
      string dataspaceCategory = null,
      Guid dataspaceIdentifier = default (Guid),
      bool isolated = false);

    void SetIsolated(IVssRequestContext requestContext, bool isolated);

    bool SupportsLazyInitialization(IVssRequestContext requestContext);
  }
}
