// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeletFieldRuleProperties
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeletFieldRuleProperties
  {
    public static readonly HashSet<WorkItemRuleName> RulesAffectedByFieldRuleProp = new HashSet<WorkItemRuleName>((IEnumerable<WorkItemRuleName>) new WorkItemRuleName[6]
    {
      WorkItemRuleName.ReadOnly,
      WorkItemRuleName.Required,
      WorkItemRuleName.Default,
      WorkItemRuleName.AllowedValues,
      WorkItemRuleName.AllowExistingValue,
      WorkItemRuleName.SuggestedValues
    });

    public WorkItemTypeletFieldRuleProperties(
      bool isRequired,
      bool isReadOnly,
      string defaultValue,
      RuleValueFrom defaultValueFrom,
      bool? allowGroups,
      string[] allowedValues)
    {
      this.IsRequired = isRequired;
      this.IsReadOnly = isReadOnly;
      this.DefaultValue = defaultValue;
      this.DefaultValueFrom = defaultValueFrom;
      this.AllowGroups = allowGroups;
      this.AllowedValues = allowedValues;
    }

    public bool IsRequired { get; private set; }

    public bool IsReadOnly { get; private set; }

    public string DefaultValue { get; private set; }

    public RuleValueFrom DefaultValueFrom { get; private set; }

    public bool? AllowGroups { get; private set; }

    public string[] AllowedValues { get; set; }
  }
}
