// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.JsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Serializers;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class JsonExtensions
  {
    public static JsObject ToJson(this CommonProjectConfiguration settings)
    {
      ArgumentUtility.CheckForNull<CommonProjectConfiguration>(settings, nameof (settings));
      JsObject json = new JsObject();
      json["requirementsWorkItems"] = (object) JsonExtensions.ToJson(settings.RequirementWorkItems);
      json["taskWorkItems"] = (object) JsonExtensions.ToJson(settings.TaskWorkItems);
      json["feedbackRequestWorkItems"] = (object) JsonExtensions.ToJson(settings.FeedbackRequestWorkItems);
      json["feedbackResponseWorkItems"] = (object) JsonExtensions.ToJson(settings.FeedbackResponseWorkItems);
      json["feedbackWorkItems"] = (object) JsonExtensions.ToJson(settings.FeedbackWorkItems);
      json["bugWorkItems"] = (object) JsonExtensions.ToJson(settings.BugWorkItems);
      if (settings.TypeFields != null)
        json["typeFields"] = (object) ((IEnumerable<TypeField>) settings.TypeFields).Select<TypeField, JsObject>((Func<TypeField, JsObject>) (tf => JsonExtensions.ToJson(tf))).ToArray<JsObject>();
      json["weekends"] = (object) settings.Weekends;
      return json;
    }

    public static JsObject ToJson(
      this ProjectProcessConfiguration settings,
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(settings, nameof (settings));
      JsObject json = new JsObject();
      json["portfolioBacklogs"] = (object) ((IEnumerable<BacklogCategoryConfiguration>) settings.PortfolioBacklogs).Select<BacklogCategoryConfiguration, JsObject>((Func<BacklogCategoryConfiguration, JsObject>) (x => JsonExtensions.ToJson(x))).ToArray<JsObject>();
      json["requirementBacklog"] = (object) JsonExtensions.ToJson(settings.RequirementBacklog);
      json["taskBacklog"] = (object) JsonExtensions.ToJson(settings.TaskBacklog);
      json["bugWorkItems"] = (object) settings.BugWorkItems.ToJson();
      if (settings.TypeFields != null)
        json["typeFields"] = (object) ((IEnumerable<TypeField>) settings.TypeFields).Select<TypeField, JsObject>((Func<TypeField, JsObject>) (tf => JsonExtensions.ToJson(tf))).ToArray<JsObject>();
      json["weekends"] = (object) settings.Weekends;
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      if (settings.AllBacklogs != null)
      {
        IEnumerable<BacklogCategoryConfiguration> source = ((IEnumerable<BacklogCategoryConfiguration>) settings.AllBacklogs).Where<BacklogCategoryConfiguration>((Func<BacklogCategoryConfiguration, bool>) (x => x != null && !string.IsNullOrWhiteSpace(x.CategoryReferenceName)));
        List<string> list = source.Select<BacklogCategoryConfiguration, string>((Func<BacklogCategoryConfiguration, string>) (x => x.CategoryReferenceName)).ToList<string>();
        if (settings.BugWorkItems != null)
          list.Add(settings.BugWorkItems.CategoryReferenceName);
        json["workItemCategories"] = (object) service.GetWorkItemTypeMappingsForCategories(requestContext, projectName, (IEnumerable<string>) list);
        json["defaultWorkItemTypes"] = (object) source.ToDictionary<BacklogCategoryConfiguration, string, string>((Func<BacklogCategoryConfiguration, string>) (backlog => backlog.CategoryReferenceName), (Func<BacklogCategoryConfiguration, string>) (backlog => backlog.GetDefaultWorkItemTypeName(requestContext, projectName, backlog.CategoryReferenceName)));
      }
      if (settings.WorkItemColors != null)
        json["workItemColors"] = (object) ((IEnumerable<WorkItemColor>) settings.WorkItemColors).Select<WorkItemColor, JsObject>((Func<WorkItemColor, JsObject>) (color => color.ToJson())).ToArray<JsObject>();
      return json;
    }

    public static JsObject ToJson(this FieldDefinition field)
    {
      JsObject json = new JsObject();
      json["id"] = (object) field.Id;
      json["name"] = (object) field.Name;
      json["referenceName"] = (object) field.ReferenceName;
      json["type"] = (object) (int) field.FieldType;
      json["flags"] = (object) (int) field.Flags;
      json["usages"] = (object) (int) field.Usages;
      json["isIdentity"] = (object) field.IsIdentity;
      json["isHistoryEnabled"] = (object) field.IsHistoryEnabled;
      return json;
    }

    public static JsObject ToJson(this WorkItemColor workItemColor)
    {
      JsObject json = (JsObject) null;
      if (workItemColor != null)
      {
        json = new JsObject();
        json["primary"] = (object) workItemColor.PrimaryColor;
        json["secondary"] = (object) workItemColor.SecondaryColor;
        json["name"] = (object) workItemColor.WorkItemTypeName;
      }
      return json;
    }

    public static JsObject ToJson(this CategoryConfiguration category)
    {
      JsObject json = (JsObject) null;
      if (category != null)
      {
        json = new JsObject();
        json["plural"] = (object) category.PluralName;
        json["singular"] = (object) category.SingularName;
        json[nameof (category)] = (object) category.CategoryReferenceName;
        if (category.States != null)
          json["states"] = (object) ((IEnumerable<State>) category.States).Select<State, JsObject>((Func<State, JsObject>) (s => JsonExtensions.ToJson(s))).ToArray<JsObject>();
      }
      return json;
    }

    public static JsObject ToJson(this BacklogCategoryConfiguration category)
    {
      JsObject json = (JsObject) null;
      if (category != null)
      {
        json = new JsObject();
        json["plural"] = (object) category.PluralName;
        json["singular"] = (object) category.SingularName;
        json[nameof (category)] = (object) category.CategoryReferenceName;
        json["parentCategory"] = (object) category.ParentCategoryReferenceName;
        json["workItemCountLimit"] = (object) category.WorkItemCountLimit;
        if (category.States != null)
          json["states"] = (object) ((IEnumerable<State>) category.States).Select<State, JsObject>((Func<State, JsObject>) (s => JsonExtensions.ToJson(s))).ToArray<JsObject>();
      }
      return json;
    }

    public static JsObject ToJson(this Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration)
    {
      JsObject json = (JsObject) null;
      if (backlogConfiguration != null)
      {
        json = new JsObject();
        json["projectId"] = (object) backlogConfiguration.ProjectId.ToString();
        json["workItemTypeStates"] = (object) backlogConfiguration.WorkItemTypeMappedStates.Select<WorkItemTypeStateInfo, JsObject>((Func<WorkItemTypeStateInfo, JsObject>) (s => s.ToJson())).ToArray<JsObject>();
        json["backlogFields"] = (object) backlogConfiguration.BacklogFields.TypeFields.ToJson();
        json["taskBacklog"] = (object) backlogConfiguration.TaskBacklog.ToJson();
        json["requirementBacklog"] = (object) backlogConfiguration.RequirementBacklog.ToJson();
        json["portfolioBacklogs"] = (object) backlogConfiguration.PortfolioBacklogs.Select<BacklogLevelConfiguration, JsObject>((Func<BacklogLevelConfiguration, JsObject>) (b => b.ToJson())).ToArray<JsObject>();
        json["hiddenBacklogs"] = (object) backlogConfiguration.HiddenBacklogs.ToArray<string>();
      }
      return json;
    }

    public static IEnumerable<object> ToJson(
      this WorkItemRule rule,
      HashSet<string> globalListReferences,
      IWorkItemRuleFilter ruleFilter,
      StringComparer stringComparer,
      FieldEntry field,
      IVssRequestContext requestContext,
      bool isHostedXMLProject,
      bool getSuggestedAsAllowedValuesForIdentity = false)
    {
      if (!(rule is HideFieldRule) && (ruleFilter == null || ruleFilter.IsApplicable(rule)))
      {
        RuleBlock block = rule as RuleBlock;
        ConditionalBlockRule conditionalBlockRule = rule as ConditionalBlockRule;
        if (block != null && conditionalBlockRule == null)
        {
          foreach (object obj in block.GetOrderedRules(ruleFilter).SelectMany<WorkItemRule, object>((Func<WorkItemRule, IEnumerable<object>>) (r => r.ToJson(globalListReferences, ruleFilter, stringComparer, field, requestContext, isHostedXMLProject, getSuggestedAsAllowedValuesForIdentity))))
            yield return obj;
        }
        else
        {
          if (getSuggestedAsAllowedValuesForIdentity)
          {
            if (field != null && field.IsIdentity && rule is ProhibitedValuesRule)
              yield break;
            else
              yield return (object) (field == null || !field.IsIdentity || !(rule is AllowedValuesRule) ? rule.Name : WorkItemRuleName.SuggestedValues).ToString();
          }
          else if (field.IsIdentity && rule is AllowedValuesRule && !isHostedXMLProject)
            yield return (object) "ScopedIdentity";
          else if (field.IsIdentity && (rule is ProhibitedValuesRule || rule is AllowedValuesRule || rule is SuggestedValuesRule))
            yield break;
          else
            yield return (object) rule.Name.ToString();
          List<object> objectList = new List<object>(JsonExtensions.GetRuleArguments(requestContext, rule, globalListReferences, stringComparer, field.IsIdentity));
          if (block != null)
            objectList.Add((object) block.GetOrderedRules(ruleFilter).SelectMany<WorkItemRule, object>((Func<WorkItemRule, IEnumerable<object>>) (r => r.ToJson(globalListReferences, ruleFilter, stringComparer, field, requestContext, isHostedXMLProject, getSuggestedAsAllowedValuesForIdentity))));
          yield return (object) objectList;
        }
        block = (RuleBlock) null;
      }
    }

    public static JsObject ToJson(
      this WorkItemTypeExtensionFieldEntry extensionField,
      string friendlyFieldName = null)
    {
      JsObject json = new JsObject();
      json["localName"] = (object) extensionField.LocalName;
      json["localReferenceName"] = (object) extensionField.LocalReferenceName;
      json["extensionScoped"] = (object) extensionField.ExtensionScoped;
      json["field"] = (object) extensionField.Field.ToJson(friendlyFieldName);
      return json;
    }

    public static JsObject ToJson(this FieldEntry field, string friendlyFieldName = null)
    {
      JsObject json = new JsObject();
      json["id"] = (object) field.FieldId;
      json["name"] = string.IsNullOrEmpty(friendlyFieldName) ? (object) field.Name : (object) friendlyFieldName;
      json["referenceName"] = (object) field.ReferenceName;
      json["type"] = (object) (int) field.FieldType;
      json["flags"] = (object) (int) field.Flags;
      json["usages"] = (object) (int) field.Usage;
      json["isIdentity"] = (object) field.IsIdentity;
      json["isHistoryEnabled"] = (object) field.IsHistoryEnabled;
      json["isInCurrentProject"] = (object) (field.IsInCurrentProject ?? new bool?());
      json["isHtml"] = (object) field.IsHtml;
      json["isQueryable"] = (object) field.IsQueryable;
      json["isIgnored"] = (object) field.IsIgnored;
      json["isTextQuerySupported"] = (object) field.SupportsTextQuery;
      return json;
    }

    public static JsObject ToJson(
      this WorkItemUpdateResult updateResult,
      IVssRequestContext requestContext,
      DateTime loadTime)
    {
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      JsObject json = new JsObject();
      json["id"] = (object) updateResult.Id;
      if (updateResult.UpdateId < 0)
        json["tempId"] = (object) updateResult.UpdateId;
      json["rev"] = (object) updateResult.Rev;
      json[nameof (loadTime)] = (object) loadTime;
      if (updateResult.Exception != null)
      {
        json["state"] = (object) "Error";
        WorkItemTrackingAggregateException exception1 = updateResult.Exception as WorkItemTrackingAggregateException;
        TeamFoundationServiceException exception2 = updateResult.Exception;
        if (exception1 != null)
          exception2 = exception1.LeadingException;
        WorkItemFieldInvalidException exception3 = exception2 as WorkItemFieldInvalidException;
        RuleValidationException exception4 = exception2 as RuleValidationException;
        if (exception3 != null)
          json["error"] = (object) exception3.ToJson();
        else if (exception4 != null)
          json["error"] = (object) exception4.ToJson();
        else
          json["error"] = (object) exception2.ToJson();
      }
      else
        json["state"] = (object) "Success";
      json["fields"] = (object) updateResult.Fields.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<string, object>, object>) (pair => JsonExtensions.SerializePotentialWorkItemIdentity(pair.Value)));
      List<JsObject> jsObjectList1 = new List<JsObject>();
      List<JsObject> jsObjectList2 = new List<JsObject>();
      List<JsObject> jsObjectList3 = new List<JsObject>();
      if (updateResult.LinkUpdates != null)
      {
        jsObjectList1.AddRange(updateResult.LinkUpdates.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (wl => wl.UpdateType == LinkUpdateType.Add)).Select<WorkItemLinkUpdateResult, JsObject>((Func<WorkItemLinkUpdateResult, JsObject>) (wl => wl.ToJson())));
        jsObjectList2.AddRange(updateResult.LinkUpdates.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (wl => wl.UpdateType == LinkUpdateType.Update)).Select<WorkItemLinkUpdateResult, JsObject>((Func<WorkItemLinkUpdateResult, JsObject>) (wl => wl.ToJson())));
        jsObjectList3.AddRange(updateResult.LinkUpdates.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (wl => wl.UpdateType == LinkUpdateType.Delete)).Select<WorkItemLinkUpdateResult, JsObject>((Func<WorkItemLinkUpdateResult, JsObject>) (wl => wl.ToJson())));
      }
      if (updateResult.ResourceLinkUpdates != null)
      {
        jsObjectList1.AddRange(updateResult.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rl => rl.UpdateType == LinkUpdateType.Add)).Select<WorkItemResourceLinkUpdateResult, JsObject>((Func<WorkItemResourceLinkUpdateResult, JsObject>) (rl => rl.ToJson())));
        jsObjectList2.AddRange(updateResult.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rl => rl.UpdateType == LinkUpdateType.Update)).Select<WorkItemResourceLinkUpdateResult, JsObject>((Func<WorkItemResourceLinkUpdateResult, JsObject>) (rl => rl.ToJson())));
        jsObjectList3.AddRange(updateResult.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rl => rl.UpdateType == LinkUpdateType.Delete)).Select<WorkItemResourceLinkUpdateResult, JsObject>((Func<WorkItemResourceLinkUpdateResult, JsObject>) (rl => rl.ToJson())));
      }
      json["addedLinks"] = (object) jsObjectList1;
      json["deletedLinks"] = (object) jsObjectList3;
      json["updatedLinks"] = (object) jsObjectList2;
      json["currentExtensions"] = (object) updateResult.CurrentExtensions;
      json["attachedExtensions"] = (object) updateResult.AttachedExtensions;
      json["detachedExtensions"] = (object) updateResult.DetachedExtensions;
      json["isCommentingAvailable"] = (object) updateResult.IsCommentingAvailable;
      return json;
    }

    public static JsObject ToJson(this WorkItemResourceLinkUpdateResult link)
    {
      JsObject json = new JsObject();
      json["ExtID"] = (object) link.ResourceId;
      json["FilePath"] = (object) link.Location;
      return json;
    }

    public static JsObject ToJson(this WorkItemLinkUpdateResult link)
    {
      JsObject json = new JsObject();
      json["SourceID"] = (object) link.SourceWorkItemId;
      json["TargetID"] = (object) link.TargetWorkItemId;
      json["LinkType"] = (object) link.LinkType;
      json["ChangedDate"] = (object) link.ChangedDate;
      json["ChangedBy"] = (object) link.ChangeBy;
      json["RemoteHostId"] = (object) link.RemoteHostId;
      json["RemoteProjectId"] = (object) link.RemoteProjectId;
      json["RemoteStatus"] = (object) link.RemoteStatus;
      return json;
    }

    private static object SerializePotentialWorkItemIdentity(object value) => value is WorkItemIdentity workItemIdentity ? (object) workItemIdentity.ToWitIdentityRef().ToJson() : value;

    private static IEnumerable<object> GetRuleArguments(
      IVssRequestContext requestContext,
      WorkItemRule rule,
      HashSet<string> globalListReferences,
      StringComparer stringComparer,
      bool isIdentity)
    {
      switch (rule)
      {
        case ConditionalBlockRule _:
          ConditionalBlockRule conditionalRule = (ConditionalBlockRule) rule;
          yield return (object) conditionalRule.Inverse;
          yield return (object) conditionalRule.FieldId;
          if (rule is WhenRule || rule is WhenWasRule)
          {
            yield return (object) conditionalRule.Value;
            yield return (object) conditionalRule.ValueFrom;
          }
          conditionalRule = (ConditionalBlockRule) null;
          break;
        case DependentRule _:
          yield return (object) ((DependentRule) rule).FieldId;
          switch (rule)
          {
            case SameAsRule _:
              if (((SameAsRule) rule).Inverse)
                yield return (object) "not-same-as";
              else
                yield return (object) "same-as";
              yield return (object) ((SameAsRule) rule).CheckOriginalValue;
              yield break;
            case MapRule _:
              MapRule mapRule = (MapRule) rule;
              yield return (object) mapRule.Inverse;
              yield return (object) ((IEnumerable<MapCase>) mapRule.Cases).Select<MapCase, JsObject>((Func<MapCase, JsObject>) (mc => JsonExtensions.ToJson(mc))).ToArray<JsObject>();
              if (mapRule.Else != null)
                yield return (object) mapRule.Else.ToJson();
              mapRule = (MapRule) null;
              yield break;
            default:
              yield break;
          }
        case ListRule _:
          ListRule listRule = (ListRule) rule;
          if (listRule.Values != null)
            yield return (object) listRule.Values.OrderBy<string, string>((Func<string, string>) (v => v), (IComparer<string>) stringComparer).ToArray<string>();
          else
            yield return (object) null;
          if (listRule.Sets != null && ((IEnumerable<ConstantSetReference>) listRule.Sets).Any<ConstantSetReference>())
          {
            List<ExtendedConstantSetRef> extendedSets = ((IEnumerable<ConstantSetReference>) listRule.Sets).Select<ConstantSetReference, ExtendedConstantSetRef>((Func<ConstantSetReference, ExtendedConstantSetRef>) (s => s as ExtendedConstantSetRef)).Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => cset != null)).ToList<ExtendedConstantSetRef>();
            if (isIdentity && extendedSets.Any<ExtendedConstantSetRef>())
            {
              List<ExtendedConstantSetRef> list1 = extendedSets.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => !string.IsNullOrWhiteSpace(cset.EntityId))).ToList<ExtendedConstantSetRef>();
              List<ExtendedConstantSetRef> list2 = list1.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => !cset.IsGroup)).ToList<ExtendedConstantSetRef>();
              List<ExtendedConstantSetRef> nonexpandingGroupSets = list1.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => cset.IsGroup && cset.Direct)).ToList<ExtendedConstantSetRef>();
              List<ExtendedConstantSetRef> expandingGroupSets = list1.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => cset.IsGroup && !cset.Direct)).ToList<ExtendedConstantSetRef>();
              yield return (object) list2.Union<ExtendedConstantSetRef>(nonexpandingGroupSets.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => !cset.ExcludeGroups))).Select<ExtendedConstantSetRef, string>((Func<ExtendedConstantSetRef, string>) (s => s.EntityId)).Distinct<string>();
              yield return (object) expandingGroupSets.Select<ExtendedConstantSetRef, string>((Func<ExtendedConstantSetRef, string>) (s => s.EntityId)).Distinct<string>();
              List<ExtendedConstantSetRef> list3 = extendedSets.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (s => !string.IsNullOrEmpty(s.DisplayName))).ToList<ExtendedConstantSetRef>();
              yield return (object) list3.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (s =>
              {
                if (!s.IsGroup)
                  return false;
                return s.Direct && !s.ExcludeGroups || !s.Direct;
              })).OrderByDescending<ExtendedConstantSetRef, bool>((Func<ExtendedConstantSetRef, bool>) (s => s.IdentityDescriptor != (IdentityDescriptor) null)).Concat<ExtendedConstantSetRef>(list3.Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (s => !s.IsGroup)).OrderByDescending<ExtendedConstantSetRef, bool>((Func<ExtendedConstantSetRef, bool>) (s => s.IdentityDescriptor != (IdentityDescriptor) null)).Take<ExtendedConstantSetRef>(11)).Select<ExtendedConstantSetRef, string>((Func<ExtendedConstantSetRef, string>) (s => s.DisplayName));
              yield return (object) (bool) (expandingGroupSets.Any<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => cset.ExcludeGroups)) ? 1 : (nonexpandingGroupSets.Any<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (cset => cset.ExcludeGroups)) ? 1 : 0));
              nonexpandingGroupSets = (List<ExtendedConstantSetRef>) null;
              expandingGroupSets = (List<ExtendedConstantSetRef>) null;
            }
            else
            {
              IEnumerable<string> strings = ((IEnumerable<ConstantSetReference>) listRule.Sets).Select<ConstantSetReference, string>((Func<ConstantSetReference, string>) (cset => cset.ToString()));
              globalListReferences?.UnionWith(strings);
              yield return (object) strings.ToArray<string>();
            }
            extendedSets = (List<ExtendedConstantSetRef>) null;
          }
          listRule = (ListRule) null;
          break;
        case ActionRule _:
          ActionRule copyRule = (ActionRule) rule;
          yield return (object) copyRule.ValueFrom;
          switch (copyRule.ValueFrom)
          {
            case RuleValueFrom.Value:
            case RuleValueFrom.OtherFieldCurrentValue:
            case RuleValueFrom.OtherFieldOriginalValue:
              yield return (object) copyRule.Value;
              break;
          }
          copyRule = (ActionRule) null;
          break;
        case ServerDefaultRule _:
          yield return (object) ((ServerDefaultRule) rule).From;
          break;
        case TriggerRule _:
          yield return (object) ((IEnumerable<int>) (((TriggerRule) rule).FieldIds ?? Array.Empty<int>())).ToArray<int>();
          break;
      }
    }

    private static JsObject ToJson(this MapValues mapValues)
    {
      JsObject json = new JsObject();
      json["default"] = (object) mapValues.Default;
      json["values"] = (object) mapValues.Values;
      return json;
    }

    private static JsObject ToJson(this WorkItemTypeStateInfo workItemTypeState)
    {
      JsObject json = (JsObject) null;
      if (workItemTypeState != null)
      {
        json = new JsObject();
        json["workItemTypeName"] = (object) workItemTypeState.WorkItemTypeName;
        json["states"] = (object) workItemTypeState.States.ToJson();
      }
      return json;
    }

    private static JsObject ToJson(
      this IReadOnlyDictionary<string, WorkItemStateCategory> states)
    {
      JsObject json = (JsObject) null;
      if (states != null)
      {
        json = new JsObject();
        foreach (KeyValuePair<string, WorkItemStateCategory> state in (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) states)
          json[state.Key] = (object) (int) state.Value;
      }
      return json;
    }

    private static JsObject ToJson(
      this IReadOnlyDictionary<FieldTypeEnum, string> backlogFields)
    {
      JsObject json = (JsObject) null;
      if (backlogFields != null)
      {
        json = new JsObject();
        foreach (KeyValuePair<FieldTypeEnum, string> backlogField in (IEnumerable<KeyValuePair<FieldTypeEnum, string>>) backlogFields)
          json[backlogField.Key.ToString()] = (object) backlogField.Value;
      }
      return json;
    }

    public static JsObject ToJson(
      this WorkItemTypeExtension extension,
      IVssRequestContext requestContext)
    {
      StringComparer serverComparer = requestContext.WitContext().ServerSettings.ServerStringComparer;
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      JsObject json = new JsObject();
      json["id"] = (object) extension.Id;
      json["projectId"] = (object) extension.ProjectId;
      json["ownerId"] = (object) extension.OwnerId;
      json["name"] = (object) extension.Name;
      json["description"] = (object) extension.Description;
      json["markerField"] = (object) extension.MarkerField.ToJson();
      if (extension.Fields != null)
        json["fields"] = (object) extension.Fields.Select<WorkItemTypeExtensionFieldEntry, JsObject>((Func<WorkItemTypeExtensionFieldEntry, JsObject>) (f => f.ToJson())).ToArray<JsObject>();
      if (extension.FieldRules != null)
      {
        RuleMembershipFilter rulefilter = new RuleMembershipFilter(requestContext);
        HashSet<string> globals = new HashSet<string>();
        json["fieldRules"] = (object) extension.ExecutableRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => rulefilter.IsApplicable((WorkItemRule) fr))).ToDictionary<WorkItemFieldRule, string, IEnumerable<object>>((Func<WorkItemFieldRule, string>) (fr => fr.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<WorkItemFieldRule, IEnumerable<object>>) (fr => fr.ToJson(globals, (IWorkItemRuleFilter) rulefilter, serverComparer, fieldDict.GetField(fr.FieldId), requestContext, WorkItemTrackingFeatureFlags.IsHostedXMLProject(requestContext, extension.ProjectId))));
        json["globals"] = (object) globals;
        json["form"] = (object) extension.Form;
      }
      return json;
    }

    public static JsObject ToJson(this BacklogLevelConfiguration backlogLevel)
    {
      JsObject json = (JsObject) null;
      if (backlogLevel != null)
      {
        json = new JsObject();
        json["id"] = (object) backlogLevel.Id;
        json["name"] = (object) backlogLevel.Name;
        json["rank"] = (object) backlogLevel.Rank;
        json["workItemCountLimit"] = (object) backlogLevel.WorkItemCountLimit;
        json["workItemTypes"] = (object) backlogLevel.WorkItemTypes.ToArray<string>();
        json["addPanelFields"] = (object) backlogLevel.AddPanelFields;
        json["color"] = (object) backlogLevel.Color;
        json["columnFields"] = (object) backlogLevel.ColumnFields;
        json["defaultWorkItemType"] = (object) backlogLevel.DefaultWorkItemType;
      }
      return json;
    }

    public static JsObject ToJson(
      this IEnumerable<WorkItemStateColor> states,
      string workItemTypeName)
    {
      JsObject json = (JsObject) null;
      if (states != null && workItemTypeName != null)
      {
        json = new JsObject();
        json[nameof (workItemTypeName)] = (object) workItemTypeName;
        json["stateColors"] = (object) states.Select<WorkItemStateColor, JsObject>((Func<WorkItemStateColor, JsObject>) (s => s.ToJson())).ToArray<JsObject>();
      }
      return json;
    }

    public static JsObject ToJson(this WorkItemStateColor stateColor)
    {
      JsObject json = (JsObject) null;
      if (stateColor != null)
      {
        json = new JsObject();
        json["name"] = (object) stateColor.Name;
        json["color"] = (object) stateColor.Color;
        json["category"] = (object) stateColor.Category;
      }
      return json;
    }

    public static JsObject ToJson(this WorkItemModel workItem)
    {
      JsObject json = new JsObject();
      json["fields"] = (object) workItem.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (pair => pair.Value != null)).ToDictionary<KeyValuePair<int, object>, string, object>((Func<KeyValuePair<int, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<int, object>, object>) (pair => JsonExtensions.SerializePotentialWitIdentityRef(pair.Value)));
      json["revisions"] = (object) workItem.RevisionData.Select<IReadOnlyDictionary<int, object>, Dictionary<string, object>>((Func<IReadOnlyDictionary<int, object>, Dictionary<string, object>>) (dict => dict.ToDictionary<KeyValuePair<int, object>, string, object>((Func<KeyValuePair<int, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<int, object>, object>) (pair => JsonExtensions.SerializePotentialWitIdentityRef(pair.Value)))));
      json["files"] = (object) workItem.Files;
      json["relations"] = (object) workItem.Relations;
      json["relationRevisions"] = (object) workItem.RelationRevisions;
      json["projectId"] = (object) workItem.ProjectId;
      json["referencedPersons"] = (object) workItem.ReferencedPersons.ToDictionary<KeyValuePair<int, WitIdentityRef>, string, JsObject>((Func<KeyValuePair<int, WitIdentityRef>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<int, WitIdentityRef>, JsObject>) (pair => pair.Value.ToJson()));
      json["tags"] = (object) workItem.Tags.Select(tag => new
      {
        tagId = tag.TagId,
        name = tag.Name
      });
      json["currentExtensions"] = (object) workItem.CurrentExtensions;
      json["loadTime"] = (object) workItem.LoadTime;
      json["referencedNodes"] = (object) workItem.ReferencedNodes.ToJson();
      json["isReadOnly"] = (object) workItem.IsReadOnly;
      json["commentVersions"] = (object) workItem.CommentVersions;
      json["isCommentingAvailable"] = (object) workItem.isCommentingAvailable;
      return json;
    }

    public static JsObject ToJson(this ReferencedNodes nodes)
    {
      JsObject json = new JsObject();
      IEnumerable<ExtendedTreeNode> areaNodes = nodes.AreaNodes;
      json["areaNodes"] = (object) (areaNodes != null ? areaNodes.Select<ExtendedTreeNode, JsObject>((Func<ExtendedTreeNode, JsObject>) (n => n.ToJson())) : (IEnumerable<JsObject>) null);
      IEnumerable<ExtendedTreeNode> iterationNodes = nodes.IterationNodes;
      json["iterationNodes"] = (object) (iterationNodes != null ? iterationNodes.Select<ExtendedTreeNode, JsObject>((Func<ExtendedTreeNode, JsObject>) (n => n.ToJson())) : (IEnumerable<JsObject>) null);
      return json;
    }

    public static JsObject ToJson(this ExtendedTreeNode node)
    {
      JsObject json = new JsObject();
      json["id"] = (object) node.Id;
      json["parentId"] = (object) node.ParentId;
      json["name"] = (object) node.Name;
      json["guid"] = (object) node.Guid;
      json["type"] = (object) node.Type;
      json["structure"] = (object) node.StructureType;
      json["startDate"] = (object) node.StartDate;
      json["finishDate"] = (object) node.FinishDate;
      json["path"] = (object) node.Path;
      return json;
    }

    public static JsObject ToJson(this WitIdentityRef witIdentityRef)
    {
      JsObject json = new JsObject();
      json["distinctDisplayName"] = (object) witIdentityRef.DistinctDisplayName;
      json["identityRef"] = (object) witIdentityRef.IdentityRef.ToJson();
      return json;
    }

    public static JsObject ToJson(this IdentityRef identity)
    {
      if (identity == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("id", (object) identity.Id);
      json.Add("displayName", (object) identity.DisplayName);
      json.Add("uniqueName", (object) identity.UniqueName);
      json.Add("isContainer", (object) identity.IsContainer);
      json.Add("isAadIdentity", (object) identity.IsAadIdentity);
      json.Add("descriptor", (object) identity.Descriptor.ToString());
      json.Add("url", (object) identity.Url);
      json.Add("imageUrl", (object) identity.ImageUrl);
      json.Add("_links", (object) identity.Links?.Links);
      return json;
    }

    public static JsObject ToJson(
      this IWorkItemType workItemType,
      IVssRequestContext requestContext,
      Guid projectId,
      bool getSuggestedAsAllowedValuesForIdentity = false)
    {
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      StringComparer serverStringComparer = trackingRequestContext.ServerSettings.ServerStringComparer;
      IFieldTypeDictionary fieldDictionary = trackingRequestContext.FieldDictionary;
      JsObject json = new JsObject();
      json["name"] = (object) workItemType.Name;
      if (workItemType.Id.HasValue)
        json["id"] = (object) workItemType.Id;
      json["referenceName"] = (object) workItemType.ReferenceName;
      json["fields"] = (object) workItemType.GetFields(requestContext).Select<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id)).OrderBy<int, int>((Func<int, int>) (id => id));
      json["rules"] = (object) workItemType.GetExtendedProperties(requestContext).ToJson(workItemType, projectId, (IWorkItemRuleFilter) new RuleMembershipFilter(requestContext), serverStringComparer, fieldDictionary, requestContext, getSuggestedAsAllowedValuesForIdentity);
      json["color"] = (object) workItemType.Color;
      List<string> orderedStates = workItemType.GetOrderedStates(requestContext);
      if (orderedStates != null)
        json["orderedStates"] = (object) orderedStates;
      return json;
    }

    public static JsObject ToJson(this QueryItem queryItem, bool includeQueryText)
    {
      JsObject json = new JsObject();
      json.Add("id", (object) queryItem.Id);
      json.Add("name", (object) queryItem.Name);
      json.Add("parent", (object) queryItem.ParentId);
      json.Add("folder", (object) queryItem.IsFolder);
      json.Add("query", includeQueryText ? (object) queryItem.QueryText : (object) (string) null);
      return json;
    }

    public static JsObject ToJson(this HtmlQueryResult queryResult)
    {
      JsObject json = new JsObject();
      json.Add("Html", (object) queryResult.Html);
      json.Add("MaxWorkItemCount", (object) queryResult.MaxWorkItemCount);
      json.Add("WorkItemCount", (object) queryResult.WorkItemCount);
      return json;
    }

    public static JsObject ToJson(this ResourceLink resourceLink)
    {
      JsObject json = new JsObject();
      json["AddedDate"] = (object) resourceLink.AddedDate;
      json["Comment"] = (object) resourceLink.Comment;
      json["CreationDate"] = (object) resourceLink.CreationDate;
      json["ExtId"] = (object) resourceLink.ExtId;
      json["FilePath"] = (object) resourceLink.FilePath;
      json["FilePathHash"] = (object) resourceLink.FilePathHash;
      json["FldId"] = (object) resourceLink.FldId;
      json["WorkItemId"] = (object) resourceLink.WorkItemId;
      json["LastWriteDate"] = (object) resourceLink.LastWriteDate;
      json["Length"] = (object) resourceLink.Length;
      json["OriginalName"] = (object) resourceLink.OriginalName;
      json["RemovedDate"] = (object) resourceLink.RemovedDate;
      return json;
    }

    public static JsObject ToJson(this ProcessMigrationResult migrationResult)
    {
      JsObject json = new JsObject();
      json.Add("projectId", (object) migrationResult.Project.Id);
      json.Add("projectName", (object) migrationResult.Project.Name);
      json.Add("exception", (object) migrationResult.MigrationException.ToJson());
      return json;
    }

    private static JsObject ToJson(
      this AdditionalWorkItemTypeProperties rules,
      IWorkItemType workItemType,
      Guid projectId,
      IWorkItemRuleFilter ruleFilter,
      StringComparer stringComparer,
      IFieldTypeDictionary fieldDict,
      IVssRequestContext requestContext,
      bool getSuggestedAsAllowedValuesForIdentity = false)
    {
      JsObject json = new JsObject();
      json["transitions"] = (object) rules.Transitions;
      WorkItemFieldRule[] source = ruleFilter == null ? rules.FieldRules.ToArray<WorkItemFieldRule>() : rules.FieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => ruleFilter.IsApplicable((WorkItemRule) fr))).ToArray<WorkItemFieldRule>();
      json["triggerList"] = (object) ((IEnumerable<WorkItemFieldRule>) source).Select<WorkItemFieldRule, int>((Func<WorkItemFieldRule, int>) (fr => fr.FieldId)).ToArray<int>();
      ProcessDescriptor descriptor = (ProcessDescriptor) null;
      requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, projectId, out descriptor);
      HashSet<string> globals = new HashSet<string>();
      json["fieldRules"] = (object) ((IEnumerable<WorkItemFieldRule>) source).Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => fieldDict.TryGetField(fr.FieldId, out FieldEntry _))).ToDictionary<WorkItemFieldRule, string, IEnumerable<object>>((Func<WorkItemFieldRule, string>) (fr => fr.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<WorkItemFieldRule, IEnumerable<object>>) (fr => fr.ToJson(globals, ruleFilter, stringComparer, fieldDict.GetField(fr.FieldId), requestContext, descriptor != null && descriptor.IsCustom, getSuggestedAsAllowedValuesForIdentity)));
      json["fieldHelpTexts"] = (object) rules.FieldHelpTexts.ToDictionary<KeyValuePair<int, string>, string, string>((Func<KeyValuePair<int, string>, string>) (p => p.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<int, string>, string>) (p => p.Value));
      json["globals"] = (object) globals;
      Guid empty = Guid.Empty;
      if (descriptor != null)
        json["processId"] = (object) descriptor.TypeId;
      try
      {
        IWorkItemMetadataFacadeService service = requestContext.GetService<IWorkItemMetadataFacadeService>();
        json["stateColors"] = (object) service.GetWorkItemStateColors(requestContext, projectId, workItemType.Name).Select<WorkItemStateColor, JsObject>((Func<WorkItemStateColor, JsObject>) (sc => sc.ToJson()));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290728, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
      Layout layout1 = rules.GetFormLayout(requestContext).Clone();
      FormExtensionsTransformer.ResolveFormContributions(layout1, requestContext);
      LayoutSerializationHelper.InjectLinkTypes(requestContext, layout1, descriptor);
      Layout layout2 = rules.WorkItemType.ApplyHideFieldRuleOnLayout(requestContext, ruleFilter, layout1);
      json["form"] = (object) JsonConvert.SerializeObject((object) layout2).ToString();
      return json;
    }

    private static JsObject ToJson(WorkItemCategory category)
    {
      JsObject json = (JsObject) null;
      if (category != null)
      {
        json = new JsObject();
        json[nameof (category)] = (object) category.CategoryName;
        json["plural"] = (object) category.PluralName;
        if (category.States != null)
          json["states"] = (object) ((IEnumerable<State>) category.States).Select<State, JsObject>((Func<State, JsObject>) (s => JsonExtensions.ToJson(s))).ToArray<JsObject>();
      }
      return json;
    }

    private static JsObject ToJson(State state)
    {
      JsObject json = new JsObject();
      json.Add("value", (object) state.Value);
      json.Add("type", (object) state.Type);
      return json;
    }

    private static JsObject ToJson(TypeField field)
    {
      JsObject jsObject = new JsObject();
      jsObject.Add("name", (object) field.Name);
      jsObject.Add("type", (object) field.Type);
      JsObject json = jsObject;
      if (field.Format != null)
        json["format"] = (object) field.Format;
      if (field.TypeFieldValues != null)
        json["typeFieldValues"] = (object) ((IEnumerable<TypeFieldValue>) field.TypeFieldValues).Select<TypeFieldValue, JsObject>((Func<TypeFieldValue, JsObject>) (tfv => JsonExtensions.ToJson(tfv))).ToArray<JsObject>();
      return json;
    }

    private static JsObject ToJson(TypeFieldValue typeFieldValue)
    {
      JsObject json = new JsObject();
      json.Add("type", (object) typeFieldValue.Type);
      json.Add("value", (object) typeFieldValue.Value);
      return json;
    }

    private static object SerializePotentialWitIdentityRef(object value) => value is WitIdentityRef witIdentityRef ? (object) witIdentityRef.ToJson() : value;

    private static JsObject ToJson(this MapCase mapCase)
    {
      JsObject json = JsonExtensions.ToJson(mapCase);
      json["case"] = (object) mapCase.Value;
      return json;
    }
  }
}
