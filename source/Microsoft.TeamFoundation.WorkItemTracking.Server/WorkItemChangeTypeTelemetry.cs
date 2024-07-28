// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemChangeTypeTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemChangeTypeTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_workItemChange = "WorkItemChangeType";
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemChangeAboveThreshold";
    private const int c_defaultThresholdTime = 0;

    public WorkItemChangeTypeTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemChangeAboveThreshold", 0)
    {
    }

    public static string Feature => "WorkItemChangeType";

    public override void AddData(params object[] param)
    {
      if (param.Length != 2)
        return;
      WorkItemTrackingRequestContext witRequestContext = param[0] as WorkItemTrackingRequestContext;
      IEnumerable<WorkItemUpdateState> source1 = param[1] as IEnumerable<WorkItemUpdateState>;
      if (witRequestContext == null || source1 == null)
        return;
      List<string> stringList = new List<string>();
      int num1 = 0;
      Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
      Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
      Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
      Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
      int num2 = 0;
      List<int> intList = new List<int>();
      string str = source1.FirstOrDefault<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (update => !update.Success))?.UpdateResult?.Exception?.Message ?? string.Empty;
      foreach (WorkItemUpdateState workItemUpdateState in source1)
      {
        IEnumerable<\u003C\u003Ef__AnonymousType44<FieldEntry>> source2 = (workItemUpdateState.Update.Fields ?? Enumerable.Empty<KeyValuePair<string, object>>()).Select(f => new
        {
          Field = witRequestContext.FieldDictionary.GetFieldByNameOrId(f.Key)
        });
        stringList.Add(string.Join(",", source2.Select(f => f.Field.Name)));
        if (workItemUpdateState.FieldData != null)
        {
          if (workItemUpdateState.HasWorkItemTypeChanged)
          {
            string fieldValue = workItemUpdateState.FieldData.GetFieldValue(witRequestContext, 25, true) as string;
            string workItemType = workItemUpdateState.FieldData.WorkItemType;
            int num3;
            if (workItemUpdateState.Success)
            {
              ++num1;
              dictionary1[fieldValue] = !dictionary1.TryGetValue(fieldValue, out num3) ? 1 : num3 + 1;
              dictionary2[workItemType] = !dictionary2.TryGetValue(workItemType, out num3) ? 1 : num3 + 1;
            }
            else
            {
              intList.Add(workItemUpdateState.Update.Id);
              ++num2;
              dictionary3[fieldValue] = !dictionary3.TryGetValue(fieldValue, out num3) ? 1 : num3 + 1;
              dictionary4[workItemType] = !dictionary4.TryGetValue(workItemType, out num3) ? 1 : num3 + 1;
            }
          }
        }
        else
        {
          intList.Add(workItemUpdateState.Update.Id);
          ++num2;
        }
      }
      this.ClientTraceData.Add("Count-All", (object) source1.Count<WorkItemUpdateState>());
      this.ClientTraceData.Add("Count-Success", (object) num1);
      this.ClientTraceData.Add("Count-Failed", (object) num2);
      this.ClientTraceData.Add("Failed-Ids", (object) intList);
      if (!string.IsNullOrEmpty(str))
        this.ClientTraceData.Add("Failure-Message", (object) str);
      this.ClientTraceData.Add("WorkItemChangedSourceTypeSuccess", (object) dictionary1);
      this.ClientTraceData.Add("WorkItemChangedDestinationTypeSuccess", (object) dictionary2);
      this.ClientTraceData.Add("WorkItemChangedSourceTypeFail", (object) dictionary3);
      this.ClientTraceData.Add("WorkItemChangedDestinationTypeFail", (object) dictionary4);
      this.ClientTraceData.Add("Fields", (object) stringList);
    }
  }
}
