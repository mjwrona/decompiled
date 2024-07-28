// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.WorkItemRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public abstract class WorkItemRule : IEquatable<WorkItemRule>, ICloneable, IRuleVisitable
  {
    private RuleAccessMode m_accessMode;
    private WorkItemRuleName m_Name;
    private Guid m_Id;
    private string m_FriendlyName;
    private bool m_IsDisabled;
    private bool? m_IsSystem;
    private string m_For;
    private Guid m_ForVsId;
    private string m_Not;
    private Guid m_NotVsId;

    [XmlIgnore]
    public RuleAccessMode AccessMode => this.m_accessMode;

    internal virtual RuleEnginePhase Phase => RuleEnginePhase.OtherRules;

    protected internal virtual int RuleWeight => int.MaxValue;

    [XmlIgnore]
    public WorkItemRuleName Name
    {
      get => this.m_Name;
      protected set
      {
        this.m_Name = value;
        this.ReportRulesModification(nameof (Name));
      }
    }

    [XmlAttribute("rule-id")]
    public Guid Id
    {
      get => this.m_Id;
      set
      {
        this.m_Id = value;
        this.ReportRulesModification(nameof (Id));
      }
    }

    [XmlAttribute("rule-friendly-name")]
    public string FriendlyName
    {
      get => this.m_FriendlyName;
      set
      {
        this.m_FriendlyName = value;
        this.ReportRulesModification(nameof (FriendlyName));
      }
    }

    [XmlAttribute("is-disabled")]
    [DefaultValue(false)]
    public bool IsDisabled
    {
      get => this.m_IsDisabled;
      set
      {
        this.m_IsDisabled = value;
        this.ReportRulesModification(nameof (IsDisabled));
      }
    }

    [XmlIgnore]
    public bool? IsSystem
    {
      get => this.m_IsSystem;
      set
      {
        this.m_IsSystem = value;
        this.ReportRulesModification(nameof (IsSystem));
      }
    }

    [XmlAttribute("for")]
    public string For
    {
      get => this.m_For;
      set
      {
        this.m_For = value;
        this.ReportRulesModification(nameof (For));
      }
    }

    [XmlAttribute("forVsId")]
    public Guid ForVsId
    {
      get => this.m_ForVsId;
      set
      {
        this.m_ForVsId = value;
        this.ReportRulesModification(nameof (ForVsId));
      }
    }

    [XmlAttribute("not")]
    public string Not
    {
      get => this.m_Not;
      set
      {
        this.m_Not = value;
        this.ReportRulesModification(nameof (Not));
      }
    }

    [XmlAttribute("notVsId")]
    public Guid NotVsId
    {
      get => this.m_NotVsId;
      set
      {
        this.m_NotVsId = value;
        this.ReportRulesModification(nameof (NotVsId));
      }
    }

    public WorkItemRule() => this.m_accessMode = RuleAccessMode.ReadWrite;

    public void SetAccessModeToReadOnly() => this.Walk((Func<WorkItemRule, bool>) (rule =>
    {
      rule.m_accessMode = RuleAccessMode.ReadOnly;
      return true;
    }));

    public bool CanEditRule() => this.Walk((WorkItemRule) null, (RuleVisitor) ((current, parent) => current.m_accessMode != 0), (RuleVisitor) null);

    public bool IsConditional() => this is ConditionalBlockRule || this.ForVsId != Guid.Empty || this.NotVsId != Guid.Empty;

    private void SetAccessModeToReadWrite() => this.Walk((Func<WorkItemRule, bool>) (rule =>
    {
      rule.m_accessMode = RuleAccessMode.ReadWrite;
      return true;
    }));

    protected void ReportRulesModification(string modifiedAttribute)
    {
      if (this.AccessMode != RuleAccessMode.ReadOnly || !TeamFoundationTracingService.IsRawTracingEnabled(911323, TraceLevel.Info, "Rule", "RuleMetadata", (string[]) null))
        return;
      StackFrame[] frames = new StackTrace(true).GetFrames();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("Exception: {0}, AttributeChanged {1}", (object) "RuleAccessModeException", (object) modifiedAttribute)).AppendLine();
      foreach (StackFrame stackFrame in frames)
      {
        string str = string.Format("Method: {0}, Line {1}, File {2}", (object) stackFrame.GetMethod(), (object) stackFrame.GetFileLineNumber(), (object) stackFrame.GetFileName());
        stringBuilder.Append(str);
        stringBuilder.AppendLine();
      }
      TeamFoundationTracingService.TraceRaw(911323, TraceLevel.Info, "Rule", "RuleMetadata", stringBuilder.ToString());
    }

    public virtual IEnumerable<T> SelectRules<T>() where T : WorkItemRule
    {
      if (this is T obj)
        yield return obj;
    }

    public virtual IEnumerable<T> GetUnconditionalRules<T>() where T : WorkItemRule
    {
      if (this is T unconditionalRule)
        yield return unconditionalRule;
    }

    public virtual IEnumerable<RuleFieldDependency> GetDependencies() => Enumerable.Empty<RuleFieldDependency>();

    public virtual bool Equals(WorkItemRule other) => this.Equals(other, true);

    public virtual bool Equals(WorkItemRule other, bool deep) => other != null && !(this.GetType() != other.GetType()) && StringComparer.OrdinalIgnoreCase.Equals(this.For, other.For) && StringComparer.OrdinalIgnoreCase.Equals(this.Not, other.Not);

    public bool IsApplicable(IWorkItemRuleFilter ruleFilter)
    {
      bool isApplicable = true;
      this.Walk((Func<WorkItemRule, bool>) (rule =>
      {
        isApplicable = isApplicable && ruleFilter.IsApplicable(rule);
        return true;
      }));
      return isApplicable;
    }

    public virtual WorkItemRule GetMergableClone() => this.Clone(true);

    public WorkItemRule Clone() => this.Clone(false);

    public virtual WorkItemRule Clone(bool mergeable)
    {
      if (mergeable)
        return this;
      WorkItemRule workItemRule = this.MemberwiseClone() as WorkItemRule;
      workItemRule.SetAccessModeToReadWrite();
      return workItemRule;
    }

    object ICloneable.Clone() => (object) this.Clone(false);

    public virtual void Validate(IRuleValidationContext validationHelper)
    {
      foreach (RuleFieldDependency dependency in this.GetDependencies())
      {
        if (!string.IsNullOrEmpty(dependency.FieldReferenceName))
        {
          if (!validationHelper.IsValidField(dependency.FieldReferenceName))
            throw new InvalidRuleException();
        }
        else
        {
          if (dependency.FieldId == 0)
            throw new InvalidRuleException();
          if (!validationHelper.IsValidField(dependency.FieldId))
            throw new InvalidRuleException();
        }
      }
    }

    public virtual void Accept(IRuleVisitor visitor) => visitor.Visit(this);

    public virtual void FixFieldReferences(IRuleValidationContext validationHelper)
    {
    }

    public virtual bool Walk(WorkItemRule parent, RuleVisitor before, RuleVisitor after)
    {
      if (before == null || !before(this, parent))
        return false;
      return after == null || after(this, parent);
    }

    public virtual void Walk(Func<WorkItemRule, bool> action) => this.Walk((WorkItemRule) null, (RuleVisitor) ((currentRule, parentRule) => action(currentRule)), (RuleVisitor) null);

    public bool ContainsAny(HashSet<Guid> ruleIds) => !this.Walk((WorkItemRule) null, (RuleVisitor) ((current, parent) => !ruleIds.Contains(current.Id)), (RuleVisitor) null);

    internal virtual IEnumerable<string> ExtractConstants() => Enumerable.Empty<string>();

    protected IEnumerable<string> GetNonEmptyConstants(params IEnumerable<string>[] constants)
    {
      IEnumerable<string>[] stringsArray = constants;
      for (int index = 0; index < stringsArray.Length; ++index)
      {
        IEnumerable<string> source = stringsArray[index];
        if (source != null)
        {
          foreach (string nonEmptyConstant in source.Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))))
            yield return nonEmptyConstant;
        }
      }
      stringsArray = (IEnumerable<string>[]) null;
    }
  }
}
