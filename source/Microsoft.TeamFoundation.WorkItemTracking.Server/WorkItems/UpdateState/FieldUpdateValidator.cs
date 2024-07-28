// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState.FieldUpdateValidator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState
{
  public static class FieldUpdateValidator
  {
    internal static void Validate(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      WorkItemUpdateMode updateMode)
    {
      long? nullable1 = new long?();
      bool flag1 = ruleExecutionMode == WorkItemUpdateRuleExecutionMode.Bypass;
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      bool flag2 = witRequestContext.RequestContext.IsFeatureEnabled("WorkItemTracking.Server.ReadOnlyFields");
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        if (updateState.Update != null && updateState.Update.Fields != null)
        {
          foreach (KeyValuePair<string, object> field in updateState.Update.Fields)
          {
            FieldEntry fieldByNameOrId = fieldDictionary.GetFieldByNameOrId(field.Key);
            bool isMarkdownFieldUpdated = false;
            object internalValue;
            FieldStatusFlags status = FieldValueHelpers.TryConvertValueToInternal(field.Value, fieldByNameOrId.FieldType, out internalValue);
            switch (status)
            {
              case FieldStatusFlags.None:
                if (fieldByNameOrId.IsLongText)
                {
                  if (!nullable1.HasValue)
                    nullable1 = new long?((long) witRequestContext.ServerSettings.MaxLongTextSize);
                  string str = internalValue as string;
                  if (!string.IsNullOrEmpty(str))
                  {
                    long length = (long) str.Length;
                    long? nullable2 = nullable1;
                    long valueOrDefault = nullable2.GetValueOrDefault();
                    if (length > valueOrDefault & nullable2.HasValue)
                    {
                      updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldMaxLongTextSizeExceededException(updateState.Id, fieldByNameOrId.ReferenceName, nullable1.Value));
                      goto label_56;
                    }
                    else
                      break;
                  }
                  else
                    break;
                }
                else
                  break;
              case FieldStatusFlags.InvalidIdentityField:
                string message;
                if (field.Value is UnknownIdentity)
                {
                  UnknownIdentity unknownIdentity = field.Value as UnknownIdentity;
                  string displayName = unknownIdentity.DisplayName;
                  if (string.IsNullOrWhiteSpace(unknownIdentity.DisplayName))
                    displayName = unknownIdentity.Guid.ToString();
                  message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldUnknownIdentity((object) displayName, (object) field.Key);
                }
                else
                {
                  AmbiguousIdentity ambiguousIdentity = (AmbiguousIdentity) field.Value;
                  string str = ((IEnumerable<string>) ambiguousIdentity.AccountNames).Aggregate<string>((Func<string, string, string>) ((i, j) => i + ";" + j));
                  message = string.Format(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldAmbiguousIdentity((object) ambiguousIdentity.DisplayName, (object) field.Key, (object) str));
                }
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, message));
                break;
              default:
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, field.Key, status));
                goto label_56;
            }
            switch (fieldByNameOrId.FieldId)
            {
              case -4:
              case 32:
                if (!flag1)
                {
                  if (flag2)
                  {
                    updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CannotModifyFieldWithoutBypassingRules((object) fieldByNameOrId.ReferenceName)));
                    continue;
                  }
                  continue;
                }
                break;
              case 9:
              case 33:
                if (!flag1 && (field.Value == null || !FieldUpdateValidator.UserMatchesRequestIdentity(witRequestContext, field.Value.ToString())))
                {
                  if (flag2)
                  {
                    updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CannotModifyFieldWithoutBypassingRules((object) fieldByNameOrId.ReferenceName)));
                    continue;
                  }
                  continue;
                }
                break;
              case 90:
              case 91:
              case 92:
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, field.Key, FieldStatusFlags.ReadOnly));
                continue;
            }
            if (!fieldByNameOrId.IsLongText && !fieldByNameOrId.IsUpdatable)
            {
              switch (fieldByNameOrId.FieldId)
              {
                case -404:
                case -105:
                case -42:
                case -7:
                  break;
                case -6:
                case -1:
                case 3:
                  continue;
                default:
                  witRequestContext.RequestContext.GetService<CustomerIntelligenceService>().Publish(witRequestContext.RequestContext, CustomerIntelligenceArea.WorkItemTracking, CustomerIntelligenceFeature.WorkItemUpdate, CustomerIntelligenceProperty.Action, "AttemptToUpdateNonUpdateableField");
                  continue;
              }
            }
            switch (fieldByNameOrId.FieldId)
            {
              case -404:
                switch (updateMode)
                {
                  case WorkItemUpdateMode.Delete:
                    if (updateState.Update.Fields.Count<KeyValuePair<string, object>>() != 1)
                    {
                      updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemDeleteOrRestoreWithOtherUpdatesException(updateState.Id));
                      break;
                    }
                    break;
                }
                break;
              case -105:
                int treeNodeIdFromPath1 = witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(witRequestContext.RequestContext, Convert.ToString(field.Value), TreeStructureType.Iteration);
                if (treeNodeIdFromPath1 <= 0)
                  updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeNameException(updateState.Id, field.Key));
                fieldByNameOrId = fieldDictionary.GetFieldByNameOrId("System.IterationId");
                internalValue = (object) treeNodeIdFromPath1;
                break;
              case -104:
              case -2:
                if (!witRequestContext.TreeService.LegacyTryGetTreeNode(Convert.ToInt32(field.Value), out TreeNode _))
                {
                  updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeIdException(updateState.Id, field.Key));
                  break;
                }
                break;
              case -42:
                if (WorkItemTrackingFeatureFlags.IsTeamProjectMoveEnabled(witRequestContext.RequestContext))
                {
                  if (field.Value.GetType() != typeof (string))
                  {
                    updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, field.Key, FieldStatusFlags.InvalidFormat));
                    break;
                  }
                  string str = Convert.ToString(field.Value);
                  TreeNode treeNode;
                  if (witRequestContext.TreeService.TryGetNodeFromPath(witRequestContext.RequestContext, str, TreeStructureType.None, out treeNode))
                  {
                    if (treeNode.TypeId != -42)
                    {
                      updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetProjectDoesNotExistException(str));
                      break;
                    }
                    break;
                  }
                  updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetProjectDoesNotExistException(str));
                  break;
                }
                break;
              case -7:
                int treeNodeIdFromPath2 = witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(witRequestContext.RequestContext, Convert.ToString(field.Value), TreeStructureType.Area);
                if (treeNodeIdFromPath2 <= 0)
                  updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeNameException(updateState.Id, field.Key));
                fieldByNameOrId = fieldDictionary.GetFieldByNameOrId("System.AreaId");
                internalValue = (object) treeNodeIdFromPath2;
                break;
              case 54:
                isMarkdownFieldUpdated = field.Value is WorkItemCommentUpdate itemCommentUpdate && itemCommentUpdate.IsMarkdown;
                break;
            }
            updateState.AddFieldUpdate(fieldByNameOrId.FieldId, internalValue, isMarkdownFieldUpdated);
          }
label_56:
          if (updateState.Update.IsNew)
          {
            if (!updateState.HasFieldUpdate(-2) && !updateState.HasFieldUpdate(-7) && (updateState.Update == null || updateState.Update.FieldData == null || updateState.Update.FieldData.AreaId <= 0 || string.IsNullOrEmpty(updateState.Update.FieldData.GetAreaPath(witRequestContext.RequestContext))))
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemAreaPathInvalidException(updateState.Id));
              break;
            }
            if (!updateState.HasFieldUpdate(25) && (updateState.Update == null || updateState.Update.FieldData == null || string.IsNullOrEmpty(updateState.Update.FieldData.WorkItemType)))
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, witRequestContext.FieldDictionary.GetField(25).ReferenceName, FieldStatusFlags.InvalidEmpty));
              break;
            }
          }
        }
      }
    }

    private static bool UserMatchesRequestIdentity(
      WorkItemTrackingRequestContext witRequestContext,
      string displayName)
    {
      if (displayName == null || witRequestContext.RequestIdentity == null || witRequestContext.RequestIdentity.DisplayName == null || witRequestContext.RequestIdentity.GetLegacyDistinctDisplayName() == null)
        return false;
      return string.Equals(displayName, witRequestContext.RequestIdentity.DisplayName, StringComparison.OrdinalIgnoreCase) || string.Equals(displayName, witRequestContext.RequestIdentity.GetLegacyDistinctDisplayName(), StringComparison.OrdinalIgnoreCase);
    }
  }
}
