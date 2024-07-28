// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process.FieldRulePropertiesFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process
{
  public class FieldRulePropertiesFactory
  {
    public IFieldRuleProperties Create(
      IEnumerable<WorkItemRule> rules,
      FieldType fieldType,
      Func<Guid, IdentityRef> vsidToIdentity)
    {
      if (rules == null || !rules.Any<WorkItemRule>())
        return (IFieldRuleProperties) new FieldRulePropertiesFactory.FieldRuleProperties()
        {
          ReadOnly = false,
          Required = false,
          AllowGroups = new bool?(),
          DefaultValue = (object) null
        };
      WorkItemTypeletFieldRuleProperties propertiesFromRules = ProcessWorkItemTypeService.ExtractPropertiesFromRules(rules, fieldType == FieldType.Identity);
      return (IFieldRuleProperties) new FieldRulePropertiesFactory.FieldRuleProperties()
      {
        ReadOnly = propertiesFromRules.IsReadOnly,
        Required = propertiesFromRules.IsRequired,
        AllowGroups = propertiesFromRules.AllowGroups,
        DefaultValue = FieldRulePropertiesFactory.GetDefaultValue(propertiesFromRules, fieldType, vsidToIdentity)
      };
    }

    private static object GetDefaultValue(
      WorkItemTypeletFieldRuleProperties ruleProps,
      FieldType fieldType,
      Func<Guid, IdentityRef> vsidToIdentity)
    {
      object defaultValue = (object) ruleProps.DefaultValue;
      switch (ruleProps.DefaultValueFrom)
      {
        case RuleValueFrom.Value:
          Guid result;
          if (vsidToIdentity != null && fieldType == FieldType.Identity && Guid.TryParse(defaultValue.ToString(), out result))
          {
            defaultValue = (object) vsidToIdentity(result);
            break;
          }
          break;
        case RuleValueFrom.CurrentUser:
          defaultValue = (object) "$currentUser";
          break;
        case RuleValueFrom.Clock:
          defaultValue = (object) "$currentDateTime";
          break;
      }
      return defaultValue;
    }

    private class FieldRuleProperties : IFieldRuleProperties
    {
      public bool? AllowGroups { get; set; }

      public object DefaultValue { get; set; }

      public bool ReadOnly { get; set; }

      public bool Required { get; set; }
    }
  }
}
