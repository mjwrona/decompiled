// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTypeletFieldPropertiesCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal class WorkItemTypeletFieldPropertiesCacheService : 
    VssVersionedMemoryCacheService<Guid, IDictionary<int, WorkItemTypeletFieldProperties>>
  {
    private static Guid s_workItemTypeletFieldPropertiesChanged = new Guid("78A7B60C-AA2F-4F58-8BFB-3833A8B4F09E");

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", WorkItemTypeletFieldPropertiesCacheService.s_workItemTypeletFieldPropertiesChanged, new SqlNotificationCallback(this.OnWorkItemTypeletFieldPropertiesChanged), true);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", WorkItemTypeletFieldPropertiesCacheService.s_workItemTypeletFieldPropertiesChanged, new SqlNotificationCallback(this.OnWorkItemTypeletFieldPropertiesChanged), true);
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryGetWorkItemTypeletFieldProperties(
      IVssRequestContext requestContext,
      Guid typeletId,
      int fieldId,
      out WorkItemTypeletFieldProperties properties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      properties = (WorkItemTypeletFieldProperties) null;
      if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || typeletId == Guid.Empty)
        return false;
      IDictionary<int, WorkItemTypeletFieldProperties> dictionary = (IDictionary<int, WorkItemTypeletFieldProperties>) null;
      if (!this.TryGetValue(requestContext, typeletId, out dictionary))
        dictionary = this.RefreshTypeletFieldProperties(requestContext, typeletId);
      return dictionary != null && dictionary.TryGetValue(fieldId, out properties) && properties != null;
    }

    private IDictionary<int, WorkItemTypeletFieldProperties> RefreshTypeletFieldProperties(
      IVssRequestContext requestContext,
      Guid typeletId)
    {
      return requestContext.TraceBlock<IDictionary<int, WorkItemTypeletFieldProperties>>(909933, 909934, "Field", nameof (WorkItemTypeletFieldPropertiesCacheService), nameof (RefreshTypeletFieldProperties), (Func<IDictionary<int, WorkItemTypeletFieldProperties>>) (() =>
      {
        using (IVssVersionedCacheContext<Guid, IDictionary<int, WorkItemTypeletFieldProperties>> versionedContext = this.CreateVersionedContext(requestContext))
        {
          IEnumerable<WorkItemTypeletFieldProperties> propertiesFromDb = this.GetWorkItemTypeletFieldPropertiesFromDB(requestContext, typeletId);
          IDictionary<int, WorkItemTypeletFieldProperties> dictionary = (IDictionary<int, WorkItemTypeletFieldProperties>) new ConcurrentDictionary<int, WorkItemTypeletFieldProperties>((IEnumerable<KeyValuePair<int, WorkItemTypeletFieldProperties>>) ((propertiesFromDb != null ? propertiesFromDb.ToDictionary<WorkItemTypeletFieldProperties, int, WorkItemTypeletFieldProperties>((Func<WorkItemTypeletFieldProperties, int>) (p => p.FieldId), (Func<WorkItemTypeletFieldProperties, WorkItemTypeletFieldProperties>) (p => p)) : (Dictionary<int, WorkItemTypeletFieldProperties>) null) ?? new Dictionary<int, WorkItemTypeletFieldProperties>()));
          int num = (int) versionedContext.TryUpdate(requestContext, typeletId, dictionary);
          return dictionary;
        }
      }));
    }

    private IEnumerable<WorkItemTypeletFieldProperties> GetWorkItemTypeletFieldPropertiesFromDB(
      IVssRequestContext requestContext,
      Guid typeletId)
    {
      using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
        return component.GetWorkItemTypeletFieldProperties(typeletId);
    }

    private void OnWorkItemTypeletFieldPropertiesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceBlock(909935, 909936, "Field", nameof (WorkItemTypeletFieldPropertiesCacheService), nameof (OnWorkItemTypeletFieldPropertiesChanged), (Action) (() =>
      {
        if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || string.IsNullOrEmpty(eventData))
          return;
        WorkItemTypeletFieldProperties[] typeletFieldPropertiesArray = TeamFoundationSerializationUtility.Deserialize<WorkItemTypeletFieldProperties[]>(eventData, new XmlRootAttribute("updated-field-properties"));
        using (IVssVersionedCacheContext<Guid, IDictionary<int, WorkItemTypeletFieldProperties>> versionedContext = this.CreateVersionedContext(requestContext))
        {
          foreach (WorkItemTypeletFieldProperties typeletFieldProperties in typeletFieldPropertiesArray)
          {
            IDictionary<int, WorkItemTypeletFieldProperties> dictionary;
            // ISSUE: reference to a compiler-generated method
            if (!this.\u003C\u003En__0(requestContext, typeletFieldProperties.TypeletId.Value, out dictionary))
              dictionary = (IDictionary<int, WorkItemTypeletFieldProperties>) new ConcurrentDictionary<int, WorkItemTypeletFieldProperties>();
            dictionary[typeletFieldProperties.FieldId] = typeletFieldProperties;
            int num = (int) versionedContext.TryUpdate(requestContext, typeletFieldProperties.TypeletId.Value, dictionary);
          }
        }
      }));
    }
  }
}
