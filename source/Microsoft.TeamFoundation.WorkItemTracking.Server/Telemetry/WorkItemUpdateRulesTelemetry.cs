// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry.WorkItemUpdateRulesTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry
{
  internal class WorkItemUpdateRulesTelemetry
  {
    private const string Feature = "WorkItemUpdateRule";
    private static readonly Regex FieldRulesOpenTagRegex = new Regex("^<field-rules field-id=\"-?\\d+\">");

    public static void Publish(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      RuleEngine ruleEngine,
      IDictionary<int, object> oldFieldDataUpdates,
      WorkItemUpdateRulesTelemetry.RuleEvalContextSnapshot oldRuleEvalContext)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      WorkItemUpdateRulesTelemetry.PublishRulesContext(service, witRequestContext, updateState, oldFieldDataUpdates, oldRuleEvalContext);
      WorkItemUpdateRulesTelemetry.PublishRules(service, witRequestContext, ruleEngine);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("FieldRules", (object) ruleEngine.GetFieldRules().ToDictionary<KeyValuePair<int, WorkItemFieldRule>, string, string>((Func<KeyValuePair<int, WorkItemFieldRule>, string>) (pair => WorkItemUpdateRulesTelemetry.MakeFieldReadable(fieldDictionary, pair.Key)), (Func<KeyValuePair<int, WorkItemFieldRule>, string>) (pair => WorkItemUpdateRulesTelemetry.SerializeWorkItemFieldRule(pair.Value))));
      service.Publish(requestContext, "WorkItemService", "WorkItemUpdateRule", properties);
    }

    private static string MakeFieldReadable(IFieldTypeDictionary fieldDictionary, int fieldId)
    {
      FieldEntry field;
      return !fieldDictionary.TryGetField(fieldId, out field) ? string.Format("???:{0}", (object) fieldId) : string.Format("{0}:{1}", (object) field.Name, (object) fieldId);
    }

    private static IDictionary<string, TValue> MakeReadableFieldDictionary<TValue>(
      IFieldTypeDictionary fieldDictionary,
      IDictionary<int, TValue> dict)
    {
      return (IDictionary<string, TValue>) dict.ToDictionary<KeyValuePair<int, TValue>, string, TValue>((Func<KeyValuePair<int, TValue>, string>) (pair => WorkItemUpdateRulesTelemetry.MakeFieldReadable(fieldDictionary, pair.Key)), (Func<KeyValuePair<int, TValue>, TValue>) (pair => pair.Value));
    }

    private static string SerializeWorkItemFieldRule(WorkItemFieldRule fieldRule)
    {
      try
      {
        string input = TeamFoundationSerializationUtility.SerializeToString<WorkItemFieldRule>(fieldRule, new XmlRootAttribute("field-rules")).Replace(" rule-id=\"00000000-0000-0000-0000-000000000000\"", "").Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "").Replace("</field-rules>", "");
        return WorkItemUpdateRulesTelemetry.FieldRulesOpenTagRegex.Replace(input, "");
      }
      catch
      {
        return fieldRule.Serialize<WorkItemFieldRule>();
      }
    }

    public static WorkItemUpdateRulesTelemetry.RuleEvalContextSnapshot CaptureRuleEvalContext(
      IFieldTypeDictionary fieldDictionary,
      IRuleEvaluationContext ruleEvalContext)
    {
      return new WorkItemUpdateRulesTelemetry.RuleEvalContextSnapshot()
      {
        FirstInvalidFieldId = ruleEvalContext.FirstInvalidFieldId,
        FirstFieldRequiresPendingCheck = ruleEvalContext.FirstFieldRequiresPendingCheck,
        RuleEvaluationStatuses = WorkItemUpdateRulesTelemetry.CaptureRuleEvalStatuses(fieldDictionary, ruleEvalContext.RuleEvaluationStatuses)
      };
    }

    public static WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot CaptureRuleEvalStatuses(
      IFieldTypeDictionary fieldDictionary,
      IDictionary<int, FieldRuleEvalutionStatus> ruleEvalStatuses)
    {
      return new WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot()
      {
        Statuses = ruleEvalStatuses.ToDictionary<KeyValuePair<int, FieldRuleEvalutionStatus>, string, Dictionary<string, object>>((Func<KeyValuePair<int, FieldRuleEvalutionStatus>, string>) (pair => WorkItemUpdateRulesTelemetry.MakeFieldReadable(fieldDictionary, pair.Key)), (Func<KeyValuePair<int, FieldRuleEvalutionStatus>, Dictionary<string, object>>) (pair =>
        {
          Dictionary<string, object> dictionary = new Dictionary<string, object>();
          dictionary.Add("Value", pair.Value.Value);
          FieldRuleEvalutionStatus ruleEvalutionStatus = pair.Value;
          // ISSUE: variable of a boxed type
          __Boxed<int> flags = (ValueType) (int) ruleEvalutionStatus.Flags;
          ruleEvalutionStatus = pair.Value;
          string str = ruleEvalutionStatus.Flags.ToString();
          dictionary.Add("Flags", (object) string.Format("{0} {1}", (object) flags, (object) str));
          return dictionary;
        }))
      };
    }

    private static WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot DiffRuleEvaluationStatuses(
      WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot oldContext,
      WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot newContext)
    {
      Dictionary<string, Dictionary<string, object>> dictionary1 = new Dictionary<string, Dictionary<string, object>>();
      foreach (KeyValuePair<string, Dictionary<string, object>> statuse in newContext.Statuses)
      {
        Dictionary<string, object> dictionary2;
        string str;
        if (oldContext.Statuses.TryGetValue(statuse.Key, out dictionary2))
        {
          if (!(dictionary2.Serialize<Dictionary<string, object>>() == statuse.Value.Serialize<Dictionary<string, object>>()))
            str = "Changed";
          else
            continue;
        }
        else
          str = "Added";
        dictionary1[statuse.Key] = new Dictionary<string, object>((IDictionary<string, object>) statuse.Value)
        {
          ["Delta"] = (object) str
        };
      }
      foreach (KeyValuePair<string, Dictionary<string, object>> statuse in oldContext.Statuses)
      {
        if (!newContext.Statuses.ContainsKey(statuse.Key))
          dictionary1[statuse.Key] = new Dictionary<string, object>((IDictionary<string, object>) statuse.Value)
          {
            ["Delta"] = (object) "Removed"
          };
      }
      return new WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot()
      {
        Statuses = dictionary1
      };
    }

    private static void PublishRulesContext(
      CustomerIntelligenceService ciService,
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      IDictionary<int, object> oldFieldDataUpdates,
      WorkItemUpdateRulesTelemetry.RuleEvalContextSnapshot oldRuleEvalContext)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      WorkItemUpdateRulesTelemetry.RuleEvalContextSnapshot evalContextSnapshot = WorkItemUpdateRulesTelemetry.CaptureRuleEvalContext(fieldDictionary, updateState.RuleEvalContext);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("FieldUpdates", (object) updateState.FieldUpdates.Select(fu => new
      {
        Field = WorkItemUpdateRulesTelemetry.MakeFieldReadable(fieldDictionary, fu.Key),
        Old = updateState.FieldData.GetFieldValue<object>(requestContext, fu.Key, true),
        New = fu.Value
      }));
      properties.Add("OldFieldDataUpdates", (object) WorkItemUpdateRulesTelemetry.MakeReadableFieldDictionary<object>(fieldDictionary, oldFieldDataUpdates));
      properties.Add("NewFieldDataUpdates", (object) WorkItemUpdateRulesTelemetry.MakeReadableFieldDictionary<object>(fieldDictionary, (IDictionary<int, object>) updateState.FieldData.Updates));
      properties.Add("OldRuleEvalContext", (object) oldRuleEvalContext);
      evalContextSnapshot.RuleEvaluationStatuses = WorkItemUpdateRulesTelemetry.DiffRuleEvaluationStatuses(oldRuleEvalContext.RuleEvaluationStatuses, evalContextSnapshot.RuleEvaluationStatuses);
      properties.Add("NewRuleEvalContext", (object) evalContextSnapshot);
      ciService.Publish(requestContext, "WorkItemService", "WorkItemUpdateRule", properties);
    }

    private static void PublishRules(
      CustomerIntelligenceService ciService,
      WorkItemTrackingRequestContext witRequestContext,
      RuleEngine ruleEngine)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = ruleEngine.GetFieldRules().Select<KeyValuePair<int, WorkItemFieldRule>, KeyValuePair<string, string>>((Func<KeyValuePair<int, WorkItemFieldRule>, KeyValuePair<string, string>>) (pair => new KeyValuePair<string, string>(WorkItemUpdateRulesTelemetry.MakeFieldReadable(fieldDictionary, pair.Key), WorkItemUpdateRulesTelemetry.SerializeWorkItemFieldRule(pair.Value))));
      List<KeyValuePair<string, string>> currentBatch = new List<KeyValuePair<string, string>>();
      int num = 0;
      foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
      {
        if (num > 0 && num + keyValuePair.Value.Length >= 20000)
        {
          WorkItemUpdateRulesTelemetry.FlushRulesBatch(ciService, requestContext, currentBatch);
          num = 0;
        }
        num += keyValuePair.Value.Length;
        currentBatch.Add(keyValuePair);
      }
      if (num <= 0)
        return;
      WorkItemUpdateRulesTelemetry.FlushRulesBatch(ciService, requestContext, currentBatch);
    }

    private static void FlushRulesBatch(
      CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      List<KeyValuePair<string, string>> currentBatch)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("FieldRules", (object) currentBatch.Select(pair => new
      {
        Field = pair.Key,
        Rules = pair.Value
      }));
      ciService.Publish(requestContext, "WorkItemService", "WorkItemUpdateRule", properties);
      currentBatch.Clear();
    }

    public class RuleEvalContextSnapshot
    {
      public int? FirstInvalidFieldId { get; set; }

      public int? FirstFieldRequiresPendingCheck { get; set; }

      public WorkItemUpdateRulesTelemetry.RuleEvalStatusesSnapshot RuleEvaluationStatuses { get; set; }
    }

    public class RuleEvalStatusesSnapshot
    {
      public Dictionary<string, Dictionary<string, object>> Statuses { get; set; }
    }
  }
}
