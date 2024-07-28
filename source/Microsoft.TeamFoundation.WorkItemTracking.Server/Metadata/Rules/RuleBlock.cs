// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.RuleBlock
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("block")]
  public class RuleBlock : WorkItemRule
  {
    private List<WorkItemRule> m_subRules;
    private RuleEnginePhase? m_phase;

    public RuleBlock() => this.Name = WorkItemRuleName.Block;

    internal override RuleEnginePhase Phase
    {
      get
      {
        if (!this.m_phase.HasValue)
        {
          this.m_phase = new RuleEnginePhase?(RuleEnginePhase.None);
          if (this.m_subRules != null)
          {
            foreach (WorkItemRule subRule in this.m_subRules)
            {
              if (subRule != null)
              {
                RuleEnginePhase? phase1 = this.m_phase;
                RuleEnginePhase phase2 = subRule.Phase;
                this.m_phase = phase1.HasValue ? new RuleEnginePhase?(phase1.GetValueOrDefault() | phase2) : new RuleEnginePhase?();
              }
            }
          }
        }
        return this.m_phase.Value;
      }
    }

    [XmlElement(typeof (RuleBlock))]
    [XmlElement(typeof (RequiredRule))]
    [XmlElement(typeof (ReadOnlyRule))]
    [XmlElement(typeof (HideFieldRule))]
    [XmlElement(typeof (EmptyRule))]
    [XmlElement(typeof (FrozenRule))]
    [XmlElement(typeof (CannotLoseValueRuleRule))]
    [XmlElement(typeof (SameAsRule))]
    [XmlElement(typeof (ValidUserRule))]
    [XmlElement(typeof (AllowExistingValueRule))]
    [XmlElement(typeof (MatchRule))]
    [XmlElement(typeof (AllowedValuesRule))]
    [XmlElement(typeof (SuggestedValuesRule))]
    [XmlElement(typeof (ProhibitedValuesRule))]
    [XmlElement(typeof (DefaultRule))]
    [XmlElement(typeof (CopyRule))]
    [XmlElement(typeof (ServerDefaultRule))]
    [XmlElement(typeof (MapRule))]
    [XmlElement(typeof (WhenRule))]
    [XmlElement(typeof (WhenWasRule))]
    [XmlElement(typeof (WhenChangedRule))]
    [XmlElement(typeof (WhenBecameNonEmptyRule))]
    [XmlElement(typeof (WhenRemainedNonEmptyRule))]
    [XmlElement(typeof (ComputedRule))]
    [XmlElement(typeof (TriggerRule))]
    [XmlElement(typeof (HelpTextRule))]
    [XmlElement(typeof (CollectionScopedRules))]
    [XmlElement(typeof (ProjectScopedRules))]
    [XmlElement(typeof (WorkItemTypeScopedRules))]
    public WorkItemRule[] SubRules
    {
      get => this.m_subRules == null ? Array.Empty<WorkItemRule>() : this.m_subRules.ToArray();
      set
      {
        this.m_subRules = new List<WorkItemRule>((IEnumerable<WorkItemRule>) (value ?? Array.Empty<WorkItemRule>()));
        this.ReportRulesModification(nameof (SubRules));
      }
    }

    public T AddRule<T>(T rule) where T : WorkItemRule
    {
      if ((object) rule == null)
        return rule;
      this.m_phase = new RuleEnginePhase?();
      if (this.m_subRules != null)
      {
        T obj = this.m_subRules.OfType<T>().FirstOrDefault<T>((Func<T, bool>) (r => r.Equals((WorkItemRule) rule, false)));
        if ((object) obj != null)
        {
          if (rule is RuleBlock ruleBlock1 && ruleBlock1.m_subRules != null && obj is RuleBlock ruleBlock2)
          {
            foreach (WorkItemRule subRule in ruleBlock1.m_subRules)
              ruleBlock2.AddRule<WorkItemRule>(subRule);
          }
          return obj;
        }
      }
      else
        this.m_subRules = new List<WorkItemRule>();
      this.m_subRules.Add((WorkItemRule) rule);
      this.ReportRulesModification("SubRules");
      return rule;
    }

    public bool TryFindDuplicateRuleId<T>(T ruleToFind, out Guid duplicateRuleId) where T : WorkItemRule
    {
      if (this.m_subRules != null && this.m_subRules.Count > 0)
      {
        T obj = this.m_subRules.OfType<T>().FirstOrDefault<T>((Func<T, bool>) (r => r.Equals((WorkItemRule) ruleToFind, false)));
        if ((object) obj != null)
        {
          if (ruleToFind is RuleBlock ruleBlock1)
          {
            if (ruleBlock1.m_subRules != null && ruleBlock1.m_subRules.Count > 0 && obj is RuleBlock ruleBlock)
            {
              foreach (WorkItemRule subRule in ruleBlock1.m_subRules)
              {
                if (ruleBlock.TryFindDuplicateRuleId<WorkItemRule>(subRule, out duplicateRuleId))
                  return true;
              }
            }
            duplicateRuleId = Guid.Empty;
            return false;
          }
          duplicateRuleId = obj.Id;
          return true;
        }
      }
      duplicateRuleId = Guid.Empty;
      return false;
    }

    public void RemoveRules(ISet<Guid> ruleIdsToRemove)
    {
      ArgumentUtility.CheckForNull<ISet<Guid>>(ruleIdsToRemove, nameof (ruleIdsToRemove));
      if (this.m_subRules == null)
        return;
      this.m_subRules = this.m_subRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !ruleIdsToRemove.Contains(r.Id))).ToList<WorkItemRule>();
      this.ReportRulesModification("SubRules");
      List<WorkItemRule> second = new List<WorkItemRule>();
      foreach (WorkItemRule subRule in this.m_subRules)
      {
        if (subRule is RuleBlock)
        {
          RuleBlock ruleBlock = subRule as RuleBlock;
          ruleBlock.RemoveRules(ruleIdsToRemove);
          if (!((IEnumerable<WorkItemRule>) ruleBlock.SubRules).Any<WorkItemRule>())
            second.Add(subRule);
        }
      }
      this.m_subRules = this.m_subRules.Except<WorkItemRule>((IEnumerable<WorkItemRule>) second).ToList<WorkItemRule>();
    }

    public void RemoveRulesByObjects(ISet<WorkItemRule> rulesToRemove)
    {
      ArgumentUtility.CheckForNull<ISet<WorkItemRule>>(rulesToRemove, nameof (rulesToRemove));
      if (this.m_subRules == null)
        return;
      this.m_subRules = this.m_subRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !rulesToRemove.Contains(r))).ToList<WorkItemRule>();
      this.ReportRulesModification("SubRules");
      List<WorkItemRule> second = new List<WorkItemRule>();
      foreach (WorkItemRule subRule in this.m_subRules)
      {
        if (subRule is RuleBlock)
        {
          RuleBlock ruleBlock = subRule as RuleBlock;
          ruleBlock.RemoveRulesByObjects(rulesToRemove);
          if (!((IEnumerable<WorkItemRule>) ruleBlock.SubRules).Any<WorkItemRule>())
            second.Add(subRule);
        }
      }
      this.m_subRules = this.m_subRules.Except<WorkItemRule>((IEnumerable<WorkItemRule>) second).ToList<WorkItemRule>();
    }

    public void RemoveRule(Guid ruleIdToRemove)
    {
      ArgumentUtility.CheckForEmptyGuid(ruleIdToRemove, nameof (ruleIdToRemove));
      this.RemoveRules((ISet<Guid>) new HashSet<Guid>()
      {
        ruleIdToRemove
      });
    }

    public override IEnumerable<T> SelectRules<T>()
    {
      RuleBlock ruleBlock = this;
      if (ruleBlock is T)
        yield return ruleBlock as T;
      if (ruleBlock.m_subRules != null)
      {
        foreach (T obj in ruleBlock.m_subRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null)).SelectMany<WorkItemRule, T>((Func<WorkItemRule, IEnumerable<T>>) (r => r.SelectRules<T>())))
          yield return obj;
      }
    }

    public IEnumerable<WorkItemRule> GetAllUnconditionalRules()
    {
      List<WorkItemRule> unconditionalRules = new List<WorkItemRule>();
      if (this.m_subRules != null)
      {
        foreach (WorkItemRule subRule in this.m_subRules)
        {
          if (!subRule.IsConditional())
          {
            if (subRule is RuleBlock)
              unconditionalRules.AddRange((subRule as RuleBlock).GetAllUnconditionalRules());
            else if (subRule != null)
              unconditionalRules.Add(subRule);
          }
        }
      }
      return (IEnumerable<WorkItemRule>) unconditionalRules;
    }

    public override IEnumerable<T> GetUnconditionalRules<T>() => (IEnumerable<T>) this.GetAllUnconditionalRules().OfType<T>().ToList<T>();

    public override IEnumerable<RuleFieldDependency> GetDependencies()
    {
      IEnumerable<RuleFieldDependency> first = base.GetDependencies();
      if (this.m_subRules != null)
        first = first.Concat<RuleFieldDependency>(this.m_subRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null)).SelectMany<WorkItemRule, RuleFieldDependency>((Func<WorkItemRule, IEnumerable<RuleFieldDependency>>) (r => r.GetDependencies())));
      return first;
    }

    public IEnumerable<WorkItemRule> GetOrderedRules(IWorkItemRuleFilter ruleFilter)
    {
      IEnumerable<WorkItemRule> source = (IEnumerable<WorkItemRule>) this.m_subRules;
      if (ruleFilter != null)
      {
        if (ruleFilter.IsApplicable((WorkItemRule) this))
        {
          if (source != null)
            source = source.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null && ruleFilter.IsApplicable(r)));
        }
        else
          source = (IEnumerable<WorkItemRule>) null;
      }
      return source != null ? (IEnumerable<WorkItemRule>) source.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null)).OrderBy<WorkItemRule, int>((Func<WorkItemRule, int>) (r => r.RuleWeight)) : Enumerable.Empty<WorkItemRule>();
    }

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (!base.Equals(other, deep))
        return false;
      if (!deep)
        return true;
      RuleBlock ruleBlock = other as RuleBlock;
      return this.m_subRules != null && this.m_subRules.Any<WorkItemRule>() ? ruleBlock.m_subRules != null && ruleBlock.m_subRules.Any<WorkItemRule>() && this.m_subRules.SequenceEqual<WorkItemRule>((IEnumerable<WorkItemRule>) ruleBlock.m_subRules) : ruleBlock.m_subRules == null || !ruleBlock.m_subRules.Any<WorkItemRule>();
    }

    public override WorkItemRule Clone(bool mergable)
    {
      RuleBlock ruleBlock = base.Clone(false) as RuleBlock;
      if (this.m_subRules != null)
        ruleBlock.m_subRules = new List<WorkItemRule>(this.m_subRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null)).Select<WorkItemRule, WorkItemRule>((Func<WorkItemRule, WorkItemRule>) (r => r.Clone(mergable))));
      return (WorkItemRule) ruleBlock;
    }

    public override void Accept(IRuleVisitor visitor)
    {
      base.Accept(visitor);
      if (this.m_subRules == null)
        return;
      foreach (WorkItemRule subRule in this.m_subRules)
        subRule?.Accept(visitor);
    }

    public override void FixFieldReferences(IRuleValidationContext validationHelper)
    {
      base.FixFieldReferences(validationHelper);
      if (this.m_subRules == null)
        return;
      foreach (WorkItemRule subRule in this.m_subRules)
        subRule?.FixFieldReferences(validationHelper);
    }

    public override bool Walk(WorkItemRule parent, RuleVisitor before, RuleVisitor after) => base.Walk(parent, before, (RuleVisitor) ((c, p) =>
    {
      if (this.m_subRules != null)
      {
        foreach (WorkItemRule subRule in this.m_subRules)
        {
          if (subRule != null && !subRule.Walk((WorkItemRule) this, before, after))
            return false;
        }
      }
      return after == null || after((WorkItemRule) this, parent);
    }));

    public override void Walk(Func<WorkItemRule, bool> action) => this.Walk((WorkItemRule) null, (RuleVisitor) ((currentRule, parentRule) => action(currentRule)), (RuleVisitor) null);

    internal void ClearSubRules()
    {
      if (this.m_subRules == null)
        return;
      this.m_subRules.Clear();
    }

    internal override IEnumerable<string> ExtractConstants()
    {
      IEnumerable<string>[] stringsArray = new IEnumerable<string>[2];
      List<WorkItemRule> subRules = this.m_subRules;
      stringsArray[0] = subRules != null ? subRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null)).SelectMany<WorkItemRule, string>((Func<WorkItemRule, IEnumerable<string>>) (subRule => subRule?.ExtractConstants())) : (IEnumerable<string>) null;
      stringsArray[1] = base.ExtractConstants();
      return this.GetNonEmptyConstants(stringsArray);
    }
  }
}
