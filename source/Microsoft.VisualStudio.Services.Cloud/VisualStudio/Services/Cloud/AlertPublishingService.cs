// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlertPublishingService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AlertPublishingService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateAlertConfiguration(
      IVssRequestContext requestContext,
      VssEvents[] eventCollection)
    {
      Dictionary<string, List<AlertUpdate>> dictionary = new Dictionary<string, List<AlertUpdate>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (VssEvents vssEvents in eventCollection)
      {
        List<AlertUpdate> alertUpdateList;
        if (!dictionary.TryGetValue(vssEvents.EventSource, out alertUpdateList))
        {
          alertUpdateList = new List<AlertUpdate>();
          dictionary.Add(vssEvents.EventSource, alertUpdateList);
        }
        foreach (VssEvent vssEvent in vssEvents.Events)
        {
          AlertUpdate alertUpdate = new AlertUpdate();
          alertUpdate.EventId = vssEvent.EventId;
          alertUpdate.Area = vssEvent.Area;
          alertUpdate.Description = vssEvent.Description;
          alertUpdate.AreaPath = vssEvent.AreaPath;
          alertUpdate.EventName = vssEvent.Name;
          alertUpdate.EventIndex = vssEvent.EventIndex;
          if (vssEvent.Alert != null)
          {
            alertUpdate.Enabled = vssEvent.Alert.Enabled;
            alertUpdate.ScopeIndex = vssEvent.Alert.ScopeIndex;
            alertUpdate.EventIndex = alertUpdate.ScopeIndex;
          }
          if (vssEvent.MdmEvent != null)
            alertUpdate.MdmEventEnabled = vssEvent.MdmEvent.Enabled;
          alertUpdate.Version = 1;
          alertUpdateList.Add(alertUpdate);
        }
      }
      foreach (KeyValuePair<string, List<AlertUpdate>> keyValuePair in dictionary)
      {
        string key = keyValuePair.Key;
        List<AlertUpdate> updates = keyValuePair.Value;
        using (AlertPublishingComponent component = requestContext.CreateComponent<AlertPublishingComponent>())
          component.UpdateAlerts(false, key, updates);
      }
    }
  }
}
