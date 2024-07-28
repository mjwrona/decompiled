// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemUpdateHandler : IUpdateHandler
  {
    private bool m_isBulk;

    public WorkItemUpdateHandler(bool isBulk) => this.m_isBulk = isBulk;

    public XElement ProcessUpdate(
      IVssRequestContext requestContext,
      XElement updatePackage,
      out IEnumerable<XElement> result)
    {
      int clientVersion = requestContext.GetClientVersion();
      WorkItemUpdateSerializer updateSerializer = new WorkItemUpdateSerializer(clientVersion, this.m_isBulk);
      WorkItemUpdateDeserializeResult deserializedPackage = updateSerializer.Deserialize(requestContext, updatePackage);
      if (deserializedPackage.HasUpdates)
      {
        requestContext.TraceEnter(905053, "WebServices", "ClientService", "NewAPI.CallServerUpdateWorkItems");
        try
        {
          requestContext.Trace(905055, TraceLevel.Info, "WebServices", "ClientService", "ClientVersion: {0}. Deserialized package has updates for NewAPI", (object) clientVersion);
          TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
          bool bypassRules = deserializedPackage.BypassRules;
          WorkItemUpdateRuleExecutionMode ruleExecutionMode = bypassRules ? WorkItemUpdateRuleExecutionMode.Bypass : WorkItemUpdateRuleExecutionMode.ValidationOnly;
          IVssRequestContext requestContext1 = requestContext;
          ReadOnlyCollection<WorkItemUpdateWrapper> updates = deserializedPackage.Updates;
          int num1 = (int) ruleExecutionMode;
          int num2 = WorkItemTrackingFeatureFlags.IsEnforceBypassRulesPermissionInClientOM(requestContext) & bypassRules ? 1 : 0;
          IEnumerable<WorkItemUpdateResult> results = service.UpdateWorkItems(requestContext1, (IEnumerable<WorkItemUpdate>) updates, (WorkItemUpdateRuleExecutionMode) num1, false, false, true, (IReadOnlyCollection<int>) null, false, num2 != 0, false);
          int areaId = 1;
          foreach (WorkItemUpdateResult itemUpdateResult in results)
          {
            if (itemUpdateResult.Fields.TryGetValue<string, int>(-2.ToString(), out areaId) && !requestContext.WitContext().WorkItemProjectPermissionChecker.HasWorkItemPermission(areaId, AuthorizationProjectPermissions.BypassRules))
            {
              requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, CustomerIntelligenceFeature.WorkItemUpdate, CustomerIntelligenceProperty.Action, "AttemptToBypassRulesWithoutPermission");
              break;
            }
          }
          result = updateSerializer.Serialize(requestContext, deserializedPackage, results).Elements();
          return deserializedPackage.HasLegacyElements ? deserializedPackage.ModifiedPackageElement : (XElement) null;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(905054, TraceLevel.Error, "WebServices", "ClientService", ex);
          if (ex.Message.Contains("TF401322"))
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, CustomerIntelligenceFeature.WorkItemUpdate, CustomerIntelligenceProperty.Action, "AttemptToBypassRulesWithoutPermission");
          throw;
        }
        finally
        {
          requestContext.TraceLeave(905056, "WebServices", "ClientService", "NewAPI.CallServerUpdateWorkItems");
        }
      }
      else
      {
        result = Enumerable.Empty<XElement>();
        return updatePackage;
      }
    }
  }
}
