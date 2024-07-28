// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules.RuleValueValidator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules
{
  public static class RuleValueValidator
  {
    private const string CurrentUser = "$currentUser";

    public static RuleValueValidationResult ValidateAndTransformRuleValue(
      IVssRequestContext requestContext,
      FieldEntry field,
      string value,
      WorkItemFieldRule fieldRule,
      RuleValueFrom valueFrom = RuleValueFrom.Value,
      bool isIdentity = false,
      Guid identityVsid = default (Guid),
      Guid processId = default (Guid),
      string witRefName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FieldEntry>(field, nameof (field));
      object internalValue = (object) null;
      switch (valueFrom)
      {
        case RuleValueFrom.Value:
          FieldStatusFlags fieldStatusFlags = FieldValueHelpers.TryConvertValueToInternal((object) value, field.FieldType, out internalValue);
          if ((fieldStatusFlags & FieldStatusFlags.InvalidMask) != FieldStatusFlags.None)
          {
            RuleValueValidationStatus status = RuleValueValidationStatus.InvalidValueForDataType;
            if ((fieldStatusFlags & FieldStatusFlags.InvalidIdentityField) != FieldStatusFlags.None)
              status = RuleValueValidationStatus.InvalidIdentity;
            else if ((fieldStatusFlags & FieldStatusFlags.InvalidSpecialChars) != FieldStatusFlags.None)
              status = RuleValueValidationStatus.InvalidCharacters;
            else if ((fieldStatusFlags & FieldStatusFlags.InvalidTooLong) != FieldStatusFlags.None)
              status = RuleValueValidationStatus.InvalidTooLong;
            else if ((fieldStatusFlags & FieldStatusFlags.InvalidDate) != FieldStatusFlags.None)
              status = RuleValueValidationStatus.InvalidDateBeforeMinSqlDate;
            return new RuleValueValidationResult(status, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) value, (object) field.FieldType.ToString()));
          }
          if (field.FieldType == InternalFieldType.Double)
          {
            value = Convert.ToDouble(internalValue).ToString();
            break;
          }
          break;
        case RuleValueFrom.CurrentUser:
          return field.SystemType != typeof (string) ? new RuleValueValidationResult(RuleValueValidationStatus.InvalidValueForDataType, "$currentUser", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) "$currentUser", (object) field.FieldType.ToString())) : new RuleValueValidationResult(RuleValueValidationStatus.Valid, value, (string) null);
        case RuleValueFrom.Clock:
          if (field.SystemType != typeof (DateTime))
            return new RuleValueValidationResult(RuleValueValidationStatus.InvalidValueForDataType, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) valueFrom, (object) field.FieldType.ToString()));
          break;
      }
      if (field.FieldType == InternalFieldType.Boolean)
      {
        if (!(internalValue is bool flag))
          return new RuleValueValidationResult(RuleValueValidationStatus.InvalidValueForDataType, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) value, (object) field.FieldType.ToString()));
        value = flag ? "1" : "0";
      }
      if (field.SystemType == typeof (DateTime))
        value = internalValue != null || valueFrom != RuleValueFrom.Value ? (valueFrom == RuleValueFrom.Clock ? (string) null : ((DateTime) internalValue).ToString("O")) : "";
      if (field.SystemType == typeof (string))
      {
        if (isIdentity)
        {
          if (identityVsid == Guid.Empty)
            return new RuleValueValidationResult(RuleValueValidationStatus.InvalidIdentity, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) identityVsid.ToString(), (object) field.FieldType.ToString()));
          if (requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecords(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            identityVsid
          }).FirstOrDefault<ConstantRecord>() == null)
            return new RuleValueValidationResult(RuleValueValidationStatus.InvalidIdentity, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) identityVsid.ToString(), (object) field.FieldType.ToString()));
          AllowedValuesRule allowedValuesRule = fieldRule != null ? fieldRule.GetUnconditionalRules<AllowedValuesRule>().FirstOrDefault<AllowedValuesRule>() : (AllowedValuesRule) null;
          if (allowedValuesRule != null && ((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).Any<ConstantSetReference>((Func<ConstantSetReference, bool>) (r => r.Id != -1)))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
            {
              identityVsid
            }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identity == null || identity.IsContainer)
              return new RuleValueValidationResult(RuleValueValidationStatus.InvalidAllowedValue, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.DefaultRuleInvalidIdentityAllowedValue((object) value, (object) field.Name));
          }
        }
        else if (value == null)
          return new RuleValueValidationResult(RuleValueValidationStatus.InvalidNull, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRuleInvalidValue((object) value, (object) field.FieldType.ToString()));
      }
      if (!field.IsHtml && value != null && value.Length > (int) byte.MaxValue)
        return new RuleValueValidationResult(RuleValueValidationStatus.InvalidTooLong, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemTypeRuleDefaultStringValueInvalid((object) field.Name, (object) (int) byte.MaxValue));
      if (value != null)
      {
        IEnumerable<string> allowedValues = RuleValueValidator.GetAllowedValues(requestContext, field, fieldRule, processId, witRefName);
        if (allowedValues != null && value != "" && !allowedValues.Any<string>((Func<string, bool>) (v => TFStringComparer.AllowedValue.Equals(v, value))))
          return new RuleValueValidationResult(RuleValueValidationStatus.InvalidAllowedValue, value, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.DefaultRuleInvalidAllowedValue((object) value, (object) field.Name, (object) string.Join(", ", allowedValues)));
      }
      return new RuleValueValidationResult(RuleValueValidationStatus.Valid, value, (string) null);
    }

    private static IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      FieldEntry field,
      WorkItemFieldRule fieldRule,
      Guid processId = default (Guid),
      string witRefName = null)
    {
      IReadOnlyCollection<string> allowedValues = (IReadOnlyCollection<string>) null;
      if (field.IsPicklist)
      {
        WorkItemPickList list = requestContext.GetService<IWorkItemPickListService>().GetList(requestContext, field.PickListId.Value, true);
        return list.IsSuggested(requestContext, Guid.Empty, 0) ? (IEnumerable<string>) null : (IEnumerable<string>) list.Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value)).ToList<string>();
      }
      if (processId != Guid.Empty && witRefName != null)
      {
        ComposedWorkItemType workItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(requestContext, processId, witRefName);
        if (workItemType == null)
          throw new ProcessWorkItemTypeDoesNotExistException(witRefName, processId.ToString("D"));
        return !requestContext.GetService<IWorkItemTrackingProcessService>().TryGetAllowedValues(requestContext, processId, field.FieldId, (IEnumerable<string>) new string[1]
        {
          workItemType.Name
        }, out allowedValues) ? (IEnumerable<string>) null : (IEnumerable<string>) allowedValues;
      }
      return !WorkItemTrackingProcessService.TryGetAllowedValuesFromRules(requestContext, field.FieldId, fieldRule, out allowedValues) ? (IEnumerable<string>) null : (IEnumerable<string>) allowedValues;
    }
  }
}
