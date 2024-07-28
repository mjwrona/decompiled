// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemMoveTelemetry
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
  internal class WorkItemMoveTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_workItemMove = "WorkItemMove";
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemMoveAboveThreshold";
    private const int c_defaultThresholdTime = 0;

    public WorkItemMoveTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemMoveAboveThreshold", 0)
    {
    }

    public static string Feature => "WorkItemMove";

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
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      int num2 = 0;
      List<int> intList = new List<int>();
      WorkItemUpdateState workItemUpdateState1 = source1.FirstOrDefault<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (update => !update.Success));
      string str = "";
      if (workItemUpdateState1 != null && workItemUpdateState1.UpdateResult != null && workItemUpdateState1.UpdateResult.Exception != null)
        str = workItemUpdateState1.UpdateResult.Exception.Message;
      foreach (WorkItemUpdateState workItemUpdateState2 in source1)
      {
        IEnumerable<\u003C\u003Ef__AnonymousType45<FieldEntry, object>> source2 = (workItemUpdateState2.Update.Fields ?? Enumerable.Empty<KeyValuePair<string, object>>()).Select(f => new
        {
          Field = witRequestContext.FieldDictionary.GetFieldByNameOrId(f.Key),
          Value = f.Value
        });
        stringList.Add(string.Join(",", source2.Select(f => f.Field.Name)));
        if (workItemUpdateState2.FieldData != null)
        {
          if (workItemUpdateState2.HasTeamProjectChanged)
          {
            string workItemType = workItemUpdateState2.FieldData.WorkItemType;
            int num3;
            dictionary[workItemType] = !dictionary.TryGetValue(workItemType, out num3) ? 1 : num3 + 1;
            if (workItemUpdateState2.Success)
            {
              ++num1;
            }
            else
            {
              intList.Add(workItemUpdateState2.Update.Id);
              ++num2;
            }
          }
          else
          {
            intList.Add(workItemUpdateState2.Update.Id);
            ++num2;
          }
        }
        else
        {
          intList.Add(workItemUpdateState2.Update.Id);
          ++num2;
        }
      }
      this.ClientTraceData.Add("Count-All", (object) source1.Count<WorkItemUpdateState>());
      this.ClientTraceData.Add("Count-Success", (object) num1);
      this.ClientTraceData.Add("Count-Failed", (object) num2);
      this.ClientTraceData.Add("Failed-Ids", (object) intList);
      if (!str.Equals(""))
        this.ClientTraceData.Add("Failure-Message", (object) str);
      this.ClientTraceData.Add("WorkItemsMovedOfType", (object) dictionary);
      this.ClientTraceData.Add("Fields", (object) stringList);
    }
  }
}
