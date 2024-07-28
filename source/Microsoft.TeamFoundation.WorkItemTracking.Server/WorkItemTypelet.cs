// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypelet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class WorkItemTypelet : BaseWorkItemType
  {
    protected WorkItemFieldRule[] m_executableRules;
    protected IEnumerable<int> m_referencedFieldsByRules;
    protected IEnumerable<int> m_referencedFields;

    public virtual Guid Id { get; protected set; }

    public DateTime LastChangedDate { get; set; }

    public virtual IEnumerable<WorkItemTypeExtensionFieldEntry> Fields { get; protected set; }

    public virtual IEnumerable<WorkItemFieldRule> FieldRules { get; protected set; }

    public virtual Layout Form { get; protected set; }

    public bool IsDeleted { get; protected set; }

    public virtual IEnumerable<WorkItemFieldRule> ExecutableRules
    {
      get
      {
        if (this.m_executableRules == null)
          this.m_executableRules = RuleEngine.PrepareRulesForExecution(this.FieldRules).ToArray<WorkItemFieldRule>();
        return (IEnumerable<WorkItemFieldRule>) this.m_executableRules;
      }
    }

    public IEnumerable<int> GetReferencedFieldsByRules()
    {
      if (this.m_referencedFieldsByRules == null)
        this.m_referencedFieldsByRules = (IEnumerable<int>) this.ExecutableRules.SelectMany<WorkItemFieldRule, int>((Func<WorkItemFieldRule, IEnumerable<int>>) (fr => fr.GetDependencies().Select<RuleFieldDependency, int>((Func<RuleFieldDependency, int>) (frd => frd.FieldId)).Concat<int>((IEnumerable<int>) new int[1]
        {
          fr.FieldId
        }))).ToArray<int>();
      return this.m_referencedFieldsByRules;
    }

    public virtual IEnumerable<int> GetReferencedFields()
    {
      if (this.m_referencedFields == null)
        this.m_referencedFields = (IEnumerable<int>) this.GetReferencedFieldsByRules().Concat<int>(this.Fields.Select<WorkItemTypeExtensionFieldEntry, int>((Func<WorkItemTypeExtensionFieldEntry, int>) (efe => efe.Field.FieldId))).Distinct<int>().ToArray<int>();
      return this.m_referencedFields;
    }
  }
}
