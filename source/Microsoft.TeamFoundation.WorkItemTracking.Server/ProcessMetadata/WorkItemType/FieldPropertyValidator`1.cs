// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemType.FieldPropertyValidator`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemType
{
  public class FieldPropertyValidator<T> where T : WorkItemRule
  {
    public WorkItemFieldRule CustomFieldRules { get; private set; }

    public T CustomFieldProperty { get; private set; }

    public T OutOfBoxFieldProperty { get; private set; }

    public bool RuleExistsInParentWIT() => (object) this.OutOfBoxFieldProperty != null;

    public FieldPropertyValidator(
      WorkItemFieldRule cusotmFieldRules,
      WorkItemFieldRule outOfBoxFieldRules)
    {
      ArgumentUtility.CheckForNull<WorkItemFieldRule>(cusotmFieldRules, nameof (cusotmFieldRules));
      this.CustomFieldRules = cusotmFieldRules;
      this.OutOfBoxFieldProperty = outOfBoxFieldRules != null ? outOfBoxFieldRules.GetUnconditionalRules<T>().FirstOrDefault<T>() : default (T);
      this.CustomFieldProperty = cusotmFieldRules != null ? cusotmFieldRules.GetUnconditionalRules<T>().FirstOrDefault<T>() : default (T);
    }

    public void Validate(
      Func<T, T, bool> ruleComparer,
      Func<bool> oobRuleOverwritable,
      Func<bool> oobRuleRemovable,
      string exceptionMessage,
      ref List<Guid> rulesToDisable,
      ref List<Guid> rulesToEnable,
      bool takeOobFieldPropertyAsDefault = false)
    {
      ArgumentUtility.CheckForNull<Func<T, T, bool>>(ruleComparer, nameof (ruleComparer));
      ArgumentUtility.CheckForNull<Func<bool>>(oobRuleOverwritable, nameof (oobRuleOverwritable));
      ArgumentUtility.CheckForNull<Func<bool>>(oobRuleRemovable, nameof (oobRuleRemovable));
      ArgumentUtility.CheckForNull<List<Guid>>(rulesToDisable, nameof (rulesToDisable));
      ArgumentUtility.CheckForNull<List<Guid>>(rulesToEnable, nameof (rulesToEnable));
      if ((object) this.CustomFieldProperty != null)
      {
        if ((object) this.OutOfBoxFieldProperty != null)
        {
          if (ruleComparer(this.CustomFieldProperty, this.OutOfBoxFieldProperty))
          {
            this.DeleteCustomFieldProperty();
            rulesToEnable.Add(this.OutOfBoxFieldProperty.Id);
          }
          else if (oobRuleOverwritable())
          {
            rulesToDisable.Add(this.OutOfBoxFieldProperty.Id);
          }
          else
          {
            if (!takeOobFieldPropertyAsDefault)
              throw new ProcessWorkItemTypeValidationException(exceptionMessage);
            this.DeleteCustomFieldProperty();
            rulesToEnable.Add(this.OutOfBoxFieldProperty.Id);
          }
        }
        else
        {
          if (oobRuleOverwritable())
            return;
          if (!takeOobFieldPropertyAsDefault)
            throw new ProcessWorkItemTypeValidationException(exceptionMessage);
          this.DeleteCustomFieldProperty();
        }
      }
      else
      {
        if ((object) this.OutOfBoxFieldProperty == null)
          return;
        if (oobRuleRemovable())
        {
          rulesToDisable.Add(this.OutOfBoxFieldProperty.Id);
        }
        else
        {
          if (!takeOobFieldPropertyAsDefault)
            throw new ProcessWorkItemTypeValidationException(exceptionMessage);
          rulesToEnable.Add(this.OutOfBoxFieldProperty.Id);
        }
      }
    }

    public void DeleteCustomFieldProperty()
    {
      RuleBlock ruleBlock = (RuleBlock) this.CustomFieldRules;
      if (!((IEnumerable<WorkItemRule>) this.CustomFieldRules.SubRules).Any<WorkItemRule>() || (object) this.CustomFieldProperty == null)
        return;
      if (((IEnumerable<WorkItemRule>) this.CustomFieldRules.SubRules).First<WorkItemRule>().GetType() == typeof (WorkItemTypeScopedRules))
        ruleBlock = (RuleBlock) (((IEnumerable<WorkItemRule>) this.CustomFieldRules.SubRules).First<WorkItemRule>() as WorkItemTypeScopedRules);
      ruleBlock.SubRules = ((IEnumerable<WorkItemRule>) ruleBlock.SubRules).Except<WorkItemRule>((IEnumerable<WorkItemRule>) new T[1]
      {
        this.CustomFieldProperty
      }).ToArray<WorkItemRule>();
    }

    public void OverrideCurrentRuleWithOobRule() => this.CustomFieldProperty = this.OutOfBoxFieldProperty;
  }
}
