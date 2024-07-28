// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationCounterService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamFoundationCounterService : ITeamFoundationCounterService, IVssFrameworkService
  {
    private bool? m_IsolatedCache;

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.CounterDataChanged, new SqlNotificationCallback(this.OnCounterDataChanged), true);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public long ReserveCounterIds(
      IVssRequestContext requestContext,
      string counterName,
      long countToReserve,
      string category = null,
      Guid dataSpaceIdentifier = default (Guid),
      bool isolated = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(counterName, nameof (counterName));
      ArgumentUtility.CheckForOutOfRange(countToReserve, nameof (countToReserve), 0L);
      category = string.IsNullOrEmpty(category) ? "Default" : category;
      using (CounterComponent component = requestContext.CreateComponent<CounterComponent>(category))
        return component.ReserveCounterIds(counterName, countToReserve, dataSpaceIdentifier, isolated).Value;
    }

    public void CreateCounter(
      IVssRequestContext requestContext,
      string counterName,
      long nextCounterValue,
      string dataspaceCategory = null,
      Guid dataspaceIdentifier = default (Guid),
      bool isolated = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(counterName, nameof (counterName));
      ArgumentUtility.CheckForOutOfRange(nextCounterValue, nameof (nextCounterValue), 1L);
      dataspaceCategory = string.IsNullOrEmpty(dataspaceCategory) ? "Default" : dataspaceCategory;
      using (CounterComponent component = requestContext.CreateComponent<CounterComponent>(dataspaceCategory, dataspaceIdentifier))
      {
        if (component.ReserveCounterIds(counterName, 0L, dataspaceIdentifier, isolated).HasValue)
          return;
        component.PopulateCounter(counterName, nextCounterValue, dataspaceIdentifier, isolated);
      }
    }

    public bool CounterExists(
      IVssRequestContext requestContext,
      string counterName,
      string dataspaceCategory = null,
      Guid dataspaceIdentifier = default (Guid),
      bool isolated = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(counterName, nameof (counterName));
      dataspaceCategory = string.IsNullOrEmpty(dataspaceCategory) ? "Default" : dataspaceCategory;
      using (CounterComponent component = requestContext.CreateComponent<CounterComponent>(dataspaceCategory, dataspaceIdentifier))
        return component.ReserveCounterIds(counterName, 0L, dataspaceIdentifier, isolated).HasValue;
    }

    public void SetIsolated(IVssRequestContext requestContext, bool isolated)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (CounterComponent component = requestContext.CreateComponent<CounterComponent>())
        component.SetIsolated(isolated);
    }

    public bool IsIsolated(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_IsolatedCache.HasValue)
      {
        using (CounterComponent component = requestContext.CreateComponent<CounterComponent>())
          this.m_IsolatedCache = new bool?(component.IsIsolated());
      }
      return this.m_IsolatedCache.Value;
    }

    public void ResetCounter(
      IVssRequestContext requestContext,
      string counterName,
      string dataspaceCategory = null,
      Guid dataspaceIdentifier = default (Guid),
      bool isolated = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(counterName, nameof (counterName));
      dataspaceCategory = string.IsNullOrEmpty(dataspaceCategory) ? "Default" : dataspaceCategory;
      using (CounterComponent component = requestContext.CreateComponent<CounterComponent>(dataspaceCategory))
        component.PopulateCounter(counterName, 0L, dataspaceIdentifier, isolated);
    }

    private void OnCounterDataChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.m_IsolatedCache = new bool?();
    }

    public bool SupportsLazyInitialization(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "Counter", "Default").Version >= 6;
  }
}
