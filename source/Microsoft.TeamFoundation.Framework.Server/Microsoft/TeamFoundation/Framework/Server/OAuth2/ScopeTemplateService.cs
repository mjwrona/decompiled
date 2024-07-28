// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.ScopeTemplateService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Components;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class ScopeTemplateService : IScopeTemplateService, IVssFrameworkService
  {
    private IReadOnlyDictionary<string, ScopeTemplateEntry> m_data;
    private INotificationRegistration m_scopeTemplateRegistration;
    private const string c_area = "OAuth";
    private const string c_layer = "ScopeTemplateService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5510700, "OAuth", nameof (ScopeTemplateService), nameof (ServiceStart));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        this.m_scopeTemplateRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.ScopeTemplateEntriesChanged, new SqlNotificationCallback(this.OnScopeTemplateEntriesChanged), false, false);
        Interlocked.CompareExchange<IReadOnlyDictionary<string, ScopeTemplateEntry>>(ref this.m_data, this.LoadScopeTemplateData(requestContext), (IReadOnlyDictionary<string, ScopeTemplateEntry>) null);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5510701, "OAuth", nameof (ScopeTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5510702, "OAuth", nameof (ScopeTemplateService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.m_scopeTemplateRegistration.Unregister(requestContext);

    public ScopeTemplateEntry GetScopeTemplateEntry(
      IVssRequestContext requestContext,
      string identifier)
    {
      requestContext.TraceEnter(5510703, "OAuth", nameof (ScopeTemplateService), nameof (GetScopeTemplateEntry));
      ScopeTemplateEntry scopeTemplateEntry = (ScopeTemplateEntry) null;
      try
      {
        this.m_data.TryGetValue(identifier, out scopeTemplateEntry);
        return scopeTemplateEntry;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5510704, "OAuth", nameof (ScopeTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5510705, "OAuth", nameof (ScopeTemplateService), nameof (GetScopeTemplateEntry));
      }
    }

    private void OnScopeTemplateEntriesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(5510706, "OAuth", nameof (ScopeTemplateService), nameof (OnScopeTemplateEntriesChanged));
      try
      {
        Volatile.Write<IReadOnlyDictionary<string, ScopeTemplateEntry>>(ref this.m_data, this.LoadScopeTemplateData(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5510707, "OAuth", nameof (ScopeTemplateService), ex);
      }
      finally
      {
        requestContext.TraceLeave(5510708, "OAuth", nameof (ScopeTemplateService), nameof (OnScopeTemplateEntriesChanged));
      }
    }

    private IReadOnlyDictionary<string, ScopeTemplateEntry> LoadScopeTemplateData(
      IVssRequestContext requestContext)
    {
      IReadOnlyList<ScopeTemplateEntry> scopeTemplateEntryList;
      using (ScopeTemplateComponent component = requestContext.CreateComponent<ScopeTemplateComponent>())
        scopeTemplateEntryList = component.QueryScopeTemplateEntries();
      Dictionary<string, ScopeTemplateEntry> dictionary = new Dictionary<string, ScopeTemplateEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ScopeTemplateEntry scopeTemplateEntry in (IEnumerable<ScopeTemplateEntry>) scopeTemplateEntryList)
        dictionary[scopeTemplateEntry.Identifier] = scopeTemplateEntry;
      return (IReadOnlyDictionary<string, ScopeTemplateEntry>) dictionary;
    }
  }
}
