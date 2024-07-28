// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemUpdateTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemUpdateTelemetry : WorkItemTrackingTelemetry
  {
    public const string ThresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemUpdateAboveThreshold";
    public const int DefaultThresholdTimeMs = 700;

    public WorkItemUpdateTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemUpdateAboveThreshold", 700)
    {
    }

    public static string Feature { get; } = "WorkItemUpdate";

    public override void AddData(params object[] param)
    {
      if (param.Length != 2)
        return;
      WorkItemTrackingRequestContext witRequestContext = param[0] as WorkItemTrackingRequestContext;
      IEnumerable<WorkItemUpdateState> source1 = param[1] as IEnumerable<WorkItemUpdateState>;
      if (witRequestContext == null || source1 == null)
        return;
      int num = 0;
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      List<string> stringList3 = new List<string>();
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      List<int> intList3 = new List<int>();
      List<int> intList4 = new List<int>();
      List<int> intList5 = new List<int>();
      List<int> intList6 = new List<int>();
      List<AssignedToCode> source2 = new List<AssignedToCode>();
      foreach (WorkItemUpdateState state in source1)
      {
        IEnumerable<\u003C\u003Ef__AnonymousType45<FieldEntry, object>> source3 = (state.Update.Fields ?? Enumerable.Empty<KeyValuePair<string, object>>()).Select(f => new
        {
          Field = witRequestContext.FieldDictionary.GetFieldByNameOrId(f.Key),
          Value = f.Value
        });
        num = state.UpdateResult.Rev == 1 ? num + 1 : num;
        stringList1.Add(source3.Any() ? string.Join(",", source3.Select(f => f.Field.Name)) : "none");
        if (source3.Any(f => f.Field.FieldId == 2 && (string) f.Value == "Resolved"))
        {
          string str = source3.Where(f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.Field.ReferenceName, "Microsoft.VSTS.Common.ResolvedReason")).FirstOrDefault() != null ? "Set" : "Empty";
          stringList2.Add(str);
        }
        else
          stringList2.Add("N/A");
        stringList3.Add(state.FieldData != null ? state.FieldData.WorkItemType : "unknown");
        intList1.Add(state.Id);
        intList2.Add(state.UpdateResult.Rev);
        intList3.Add(state.HasLinkUpdates ? state.Update.LinkUpdates.Count<WorkItemLinkUpdate>() : 0);
        intList4.Add(state.HasResourceLinkUpdates ? state.Update.ResourceLinkUpdates.Count<WorkItemResourceLinkUpdate>() : 0);
        intList5.Add(state.AddedTags == null || !state.AddedTags.Any<TagDefinition>() ? 0 : state.AddedTags.Count<TagDefinition>());
        intList6.Add(state.RemovedTags == null || !state.RemovedTags.Any<TagDefinition>() ? 0 : state.RemovedTags.Count<TagDefinition>());
        source2.Add(WorkItemUpdateTelemetry.ComputeAssignedToCode(state));
      }
      this.ClientTraceData.Add("Count-All", (object) source1.Count<WorkItemUpdateState>());
      this.ClientTraceData.Add("Count-New", (object) num);
      this.ClientTraceData.Add("Ids", (object) intList1);
      this.ClientTraceData.Add("Revs", (object) intList2);
      this.ClientTraceData.Add("Fields", (object) stringList1);
      this.ClientTraceData.Add("ResolvedReasons", (object) stringList2);
      this.ClientTraceData.Add("Types", (object) stringList3);
      this.ClientTraceData.Add("LinkUpdates", (object) intList3);
      this.ClientTraceData.Add("ResourceLinkUpdates", (object) intList4);
      this.ClientTraceData.Add("AddedTags", (object) intList5);
      this.ClientTraceData.Add("RemovedTags", (object) intList6);
      this.ClientTraceData.Add("AssignedToCode", (object) string.Join(", ", source2.Select<AssignedToCode, string>((Func<AssignedToCode, string>) (code => code.ToString()))));
      this.ClientTraceData.Add("Timings", (object) witRequestContext.RequestContext.GetTraceTimingAsString());
      if (!witRequestContext.RequestContext.Items.ContainsKey("WorkItemTracking.SqlTimings"))
        return;
      IDictionary<string, IEnumerable<KeyValuePair<string, int>>> dictionary = witRequestContext.RequestContext.Items["WorkItemTracking.SqlTimings"] as IDictionary<string, IEnumerable<KeyValuePair<string, int>>>;
      if (!dictionary.ContainsKey("prc_UpdateWorkItems"))
        return;
      string updateWorkItemsSqlTimings = "";
      dictionary["prc_UpdateWorkItems"].ToList<KeyValuePair<string, int>>().ForEach((Action<KeyValuePair<string, int>>) (kv => updateWorkItemsSqlTimings = updateWorkItemsSqlTimings + kv.ToString() + " "));
      this.ClientTraceData.Add("prc_UpdateWorkItems_SqlTimings", (object) updateWorkItemsSqlTimings);
    }

    private static AssignedToCode ComputeAssignedToCode(WorkItemUpdateState state)
    {
      object obj1 = (object) null;
      bool flag = false;
      foreach (KeyValuePair<int, object> fieldUpdate in state.FieldUpdates)
      {
        if (fieldUpdate.Key == 24)
        {
          obj1 = fieldUpdate.Value;
          flag = true;
          break;
        }
      }
      if (obj1 == null)
      {
        if (flag)
          return AssignedToCode.ActiveNobody;
        state.DBFieldUpdates.TryGetValue(24, out obj1);
        if (obj1 == null)
          return AssignedToCode.AlreadyNobody;
      }
      object obj2;
      return state.DBFieldUpdates.TryGetValue(9, out obj2) && string.Equals(obj1?.ToString(), obj2?.ToString(), StringComparison.CurrentCultureIgnoreCase) ? (!flag ? AssignedToCode.AlreadySelf : AssignedToCode.ActiveSelf) : (!flag ? AssignedToCode.AlreadyOthers : AssignedToCode.ActiveOthers);
    }
  }
}
