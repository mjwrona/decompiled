// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.RuleRecordsTranslator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class RuleRecordsTranslator
  {
    private readonly IDictionary<string, HashSet<string>> m_transitions;
    private readonly IDictionary<int, string> m_helpTexts;
    private readonly Func<int, WorkItemFieldRule> GetFieldRule;
    private readonly Action<string> SetForm;
    private readonly Action<ListRule> GetStateAllowedValuesRule;

    internal RuleRecordsTranslator(
      IDictionary<string, HashSet<string>> Transitions,
      IDictionary<int, string> HelpTexts,
      Action<ListRule> GetStateAllowedValuesRule,
      Func<int, WorkItemFieldRule> GetFieldRule,
      Action<string> SetForm)
    {
      this.m_transitions = Transitions;
      this.m_helpTexts = HelpTexts;
      this.GetStateAllowedValuesRule = GetStateAllowedValuesRule;
      this.GetFieldRule = GetFieldRule;
      this.SetForm = SetForm;
    }

    internal void TranslateRuleRecords(RuleRecord ruleRecord)
    {
      if (this.ReadFormRule(ruleRecord) || this.ReadHelpTextRule(ruleRecord) || this.ReadStatesRule(ruleRecord) || this.ReadDefaultReasonRule(ruleRecord) || this.ReadReasonsRule(ruleRecord) || this.ReadRequiredRule(ruleRecord) || this.ReadValidUserRule(ruleRecord) || this.ReadAllowedValuesRule(ruleRecord) || this.ReadSuggestedValuesRule(ruleRecord) || this.ReadProhibitedValuesRule(ruleRecord) || this.ReadReadOnlyRule(ruleRecord) || this.ReadCopyDefaultRule(ruleRecord) || this.ReadEmptyRule(ruleRecord) || this.ReadCannotLoseValueRule(ruleRecord) || this.ReadOtherFieldRule(ruleRecord) || this.ReadOtherFieldRule2(ruleRecord) || this.ReadTransitionPermissionRule(ruleRecord) || this.ReadMatchRule(ruleRecord) || this.ReadFrozenRule(ruleRecord) || this.ReadServerDefaultRule(ruleRecord))
        return;
      this.ReadSystemServerDefaultRule(ruleRecord);
    }

    private bool ReadFormRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.IfFldID != 25 || ruleRecord.ThenFldID != -14 || (ruleRecord.RuleFlags2 & RuleFlags2.ThenConstLargetext) == RuleFlags2.None || !ruleRecord.Check(RuleFlags.Default, RuleFlags.If | RuleFlags.Then))
        return false;
      this.SetForm(ruleRecord.Form);
      return true;
    }

    private bool ReadHelpTextRule(RuleRecord ruleRecord)
    {
      if (!ruleRecord.Check(RuleFlags.Helptext, RuleFlags.If | RuleFlags.Then) || ruleRecord.PersonID != -1 || ruleRecord.IfFldID != 0 || ruleRecord.If2FldID != 0 || ruleRecord.Fld2ID != 0)
        return false;
      this.m_helpTexts[ruleRecord.ThenFldID] = ruleRecord.Then;
      return true;
    }

    private bool ReadStatesRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.Fld2ID != 0 || ruleRecord.IfFldID != 0 || ruleRecord.ThenConstID <= 0 || ruleRecord.ThenFldID != 2 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel, RuleFlags.If | RuleFlags.ThenNot | RuleFlags.ThenLike | RuleFlags.ThenTwoPlusLevels | RuleFlags.ThenImplicitAlso) || (ruleRecord.RuleFlags2 & (RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged | RuleFlags2.FlowaroundTree)) != RuleFlags2.None)
        return false;
      this.AttachRule(ruleRecord, (WorkItemRule) new RequiredRule());
      AllowedValuesRule rule = new AllowedValuesRule();
      this.AttachListRule(ruleRecord, (ListRule) rule);
      this.GetStateAllowedValuesRule((ListRule) rule);
      return true;
    }

    private bool ReadReasonsRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.Fld2ID != 2 || ruleRecord.Fld2WasConstID == 0 || ruleRecord.Fld3ID != 0 || ruleRecord.ThenConstID <= 0 || ruleRecord.ThenFldID != 22 || ruleRecord.IfFldID != 0 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel, RuleFlags.If | RuleFlags.ThenNot | RuleFlags.ThenLike | RuleFlags.ThenTwoPlusLevels | RuleFlags.ThenImplicitAlso) || (ruleRecord.RuleFlags2 & (RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged | RuleFlags2.FlowaroundTree)) != RuleFlags2.None)
        return false;
      this.DefineTransition(ruleRecord.Fld2Was, ruleRecord.Fld2Is);
      this.AttachRule(ruleRecord, (WorkItemRule) new RequiredRule());
      this.AttachListRule(ruleRecord, (ListRule) new AllowedValuesRule());
      return true;
    }

    private bool ReadRequiredRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID != -10000 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenNot, RuleFlags.ThenAllNodes | RuleFlags.ThenLike))
        return false;
      this.AttachRule(ruleRecord, (WorkItemRule) new RequiredRule());
      return true;
    }

    private bool ReadValidUserRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel | RuleFlags.ThenTwoPlusLevels, RuleFlags.ThenNot | RuleFlags.ThenLike | RuleFlags.ThenImplicitAlso))
        return false;
      if (ruleRecord.ThenConstID != -2)
      {
        string then = ruleRecord.Then;
        if (then == null || Guid.TryParse(then, out Guid _))
          return false;
      }
      if ((ruleRecord.RuleFlags2 & RuleFlags2.ThenImplicitEmpty) == RuleFlags2.None)
        this.AttachRule(ruleRecord, (WorkItemRule) new RequiredRule());
      this.AttachListRule(ruleRecord, (ListRule) new AllowedValuesRule(true));
      return true;
    }

    private bool ReadAllowedValuesRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID <= 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel, RuleFlags.ThenNot | RuleFlags.ThenLike | RuleFlags.ThenImplicitAlso))
        return false;
      if ((ruleRecord.RuleFlags2 & RuleFlags2.ThenImplicitEmpty) == RuleFlags2.None)
        this.AttachRule(ruleRecord, (WorkItemRule) new RequiredRule());
      this.AttachListRule(ruleRecord, (ListRule) new AllowedValuesRule());
      return true;
    }

    private bool ReadSuggestedValuesRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID <= 0 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.Suggestion | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel, RuleFlags.ThenNot | RuleFlags.ThenLike | RuleFlags.ThenImplicitAlso))
        return false;
      this.AttachListRule(ruleRecord, (ListRule) new SuggestedValuesRule());
      return true;
    }

    private bool ReadProhibitedValuesRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID <= 0 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenNot | RuleFlags.ThenOneLevel, RuleFlags.ThenLike | RuleFlags.ThenImplicitAlso))
        return false;
      this.AttachListRule(ruleRecord, (ListRule) new ProhibitedValuesRule());
      return true;
    }

    private void AttachListRule(RuleRecord ruleRecord, ListRule rule)
    {
      bool flag = (ruleRecord.RuleFlags & RuleFlags.ThenInterior) == RuleFlags.None;
      ConstantSetReference constantSetReference = new ConstantSetReference()
      {
        Id = ruleRecord.ThenConstID,
        ExcludeGroups = flag,
        IncludeTop = false,
        Direct = (ruleRecord.RuleFlags & RuleFlags.ThenTwoPlusLevels) == RuleFlags.None
      };
      rule.Sets = new ConstantSetReference[1]
      {
        constantSetReference
      };
      this.AttachRule(ruleRecord, (WorkItemRule) rule);
    }

    private bool ReadReadOnlyRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID != -10001 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless, RuleFlags.Then))
        return false;
      this.AttachRule(ruleRecord, (WorkItemRule) new ReadOnlyRule());
      return true;
    }

    private bool ReadCopyDefaultRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenFldID == 2 || ruleRecord.ThenFldID == 22 || ruleRecord.ThenConstID == -10013 || ruleRecord.ThenConstID == -10026 || ruleRecord.ThenConstID == -10031 || !ruleRecord.Check(RuleFlags.Default, RuleFlags.Then))
        return false;
      bool flag = ((IEnumerable<int>) new int[4]
      {
        ruleRecord.Fld1ID,
        ruleRecord.Fld2ID,
        ruleRecord.Fld3ID,
        ruleRecord.Fld4ID
      }).Contains<int>(ruleRecord.ThenFldID);
      if ((ruleRecord.RuleFlags & RuleFlags.Editable) == RuleFlags.None && ruleRecord.IfFldID != 0 && ruleRecord.IfConstID == -10000)
        flag = true;
      ActionRule rule = !flag ? (ActionRule) new CopyRule() : (ActionRule) new DefaultRule();
      if (ruleRecord.ThenConstID > 0)
        rule.Value = ruleRecord.Then;
      else if (ruleRecord.ThenConstID == -10000)
      {
        rule.Value = string.Empty;
      }
      else
      {
        switch (ruleRecord.ThenConstID)
        {
          case -10028:
            rule.ValueFrom = RuleValueFrom.Clock;
            break;
          case -10025:
            rule.Value = ruleRecord.If2FldID.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
            rule.ValueFrom = RuleValueFrom.OtherFieldCurrentValue;
            break;
          case -10012:
            rule.Value = ruleRecord.If2FldID.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
            rule.ValueFrom = RuleValueFrom.OtherFieldOriginalValue;
            break;
          case -10002:
            rule.ValueFrom = RuleValueFrom.CurrentUser;
            break;
          default:
            return false;
        }
      }
      this.AttachRule(ruleRecord, (WorkItemRule) rule);
      return true;
    }

    private bool ReadEmptyRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID != -10000 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless, RuleFlags.Then))
        return false;
      this.AttachRule(ruleRecord, (WorkItemRule) new EmptyRule());
      return true;
    }

    private bool ReadCannotLoseValueRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.If2FldID != ruleRecord.ThenFldID || ruleRecord.If2ConstID != -10006 || ruleRecord.ThenConstID != -10000 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.If2Not | RuleFlags.ThenNot, RuleFlags.ThenAllNodes | RuleFlags.ThenLike))
        return false;
      this.AttachRule(ruleRecord, (WorkItemRule) new CannotLoseValueRuleRule());
      return true;
    }

    private bool ReadOtherFieldRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.IfConstID != ruleRecord.ThenConstID || ruleRecord.IfConstID != -10025 && ruleRecord.IfConstID != -10012 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless, RuleFlags.ThenAllNodes | RuleFlags.ThenLike))
        return false;
      bool flag1 = (ruleRecord.RuleFlags & RuleFlags.ThenNot) != 0;
      bool flag2 = ruleRecord.ThenConstID == -10012;
      RuleRecord ruleRecord1 = ruleRecord;
      SameAsRule rule = new SameAsRule();
      rule.FieldId = ruleRecord.IfFldID;
      rule.Inverse = flag1;
      rule.CheckOriginalValue = flag2;
      this.AttachRule(ruleRecord1, (WorkItemRule) rule);
      return true;
    }

    private bool ReadOtherFieldRule2(RuleRecord ruleRecord)
    {
      if (ruleRecord.If2ConstID != ruleRecord.ThenConstID || ruleRecord.If2ConstID != -10025 && ruleRecord.If2ConstID != -10012 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless, RuleFlags.ThenAllNodes | RuleFlags.ThenLike))
        return false;
      bool flag1 = (ruleRecord.RuleFlags & RuleFlags.ThenNot) != 0;
      bool flag2 = ruleRecord.ThenConstID == -10012;
      RuleRecord ruleRecord1 = ruleRecord;
      SameAsRule rule = new SameAsRule();
      rule.FieldId = ruleRecord.If2FldID;
      rule.Inverse = flag1;
      rule.CheckOriginalValue = flag2;
      this.AttachRule(ruleRecord1, (WorkItemRule) rule);
      return true;
    }

    private bool ReadDefaultReasonRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenFldID != 22 || ruleRecord.IfFldID != 0 || ruleRecord.If2FldID != 0 || ruleRecord.ThenConstID <= 0 || ruleRecord.Fld2ID != 2 || ruleRecord.Fld2WasConstID == 0 || ruleRecord.Fld2IsConstID == 0 || !ruleRecord.Check(RuleFlags.Default, RuleFlags.If | RuleFlags.Then))
        return false;
      RuleRecord ruleRecord1 = ruleRecord;
      CopyRule rule = new CopyRule();
      rule.Value = ruleRecord.Then;
      this.AttachRule(ruleRecord1, (WorkItemRule) rule);
      return true;
    }

    private bool ReadTransitionPermissionRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenFldID != 2 || ruleRecord.Fld2ID != 2 || ruleRecord.IfFldID != 0 || ruleRecord.If2FldID != 0 || ruleRecord.Fld3ID != 0 || ruleRecord.PersonID == -1 || !ruleRecord.Check(RuleFlags.DenyWrite, RuleFlags.If | RuleFlags.Then | RuleFlags.Unless))
        return false;
      string fld2Was = ruleRecord.Fld2Was;
      string then = ruleRecord.Then;
      WorkItemTypeScopedRules itemTypeScopedRules = this.GetFieldRule(2).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      stringSet.Add(then);
      ProhibitedValuesRule prohibitedValuesRule = new ProhibitedValuesRule();
      prohibitedValuesRule.Values = stringSet;
      ProhibitedValuesRule rule1 = prohibitedValuesRule;
      if ((ruleRecord.RuleFlags & RuleFlags.InversePerson) != RuleFlags.None)
        rule1.Not = ruleRecord.Person;
      else
        rule1.For = ruleRecord.Person;
      WhenWasRule rule2 = new WhenWasRule();
      rule2.FieldId = 2;
      rule2.Value = fld2Was;
      itemTypeScopedRules.AddRule<WhenWasRule>(rule2).AddRule<ProhibitedValuesRule>(rule1);
      return true;
    }

    private bool ReadMatchRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.ThenConstID <= 0 || ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenLike | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel, RuleFlags.ThenNot | RuleFlags.ThenTwoPlusLevels))
        return false;
      if ((ruleRecord.RuleFlags2 & RuleFlags2.ThenImplicitEmpty) == RuleFlags2.None)
        this.AttachRule(ruleRecord, (WorkItemRule) new RequiredRule());
      this.AttachListRule(ruleRecord, (ListRule) new MatchRule());
      return true;
    }

    private bool ReadFrozenRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.If2FldID != ruleRecord.ThenFldID || ruleRecord.If2ConstID != -10000 || ruleRecord.ThenConstID != -10022 || !ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.If2Not, RuleFlags.Then))
        return false;
      this.AttachRule(ruleRecord, (WorkItemRule) new FrozenRule());
      return true;
    }

    private bool ReadServerDefaultRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.DenyWrite, (RuleFlags) 265019392))
        return false;
      ServerDefaultType serverDefaultType;
      switch (ruleRecord.ThenConstID)
      {
        case -10031:
          serverDefaultType = ServerDefaultType.RandomGuid;
          break;
        case -10026:
          serverDefaultType = ServerDefaultType.CallerIdentity;
          break;
        case -10013:
          serverDefaultType = ServerDefaultType.ServerDateTime;
          break;
        default:
          return false;
      }
      this.AttachRule(ruleRecord, (WorkItemRule) new ServerDefaultRule()
      {
        From = serverDefaultType
      });
      return true;
    }

    private bool ReadSystemServerDefaultRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.Default, (RuleFlags) 265019393))
        return false;
      ServerDefaultType serverDefaultType;
      switch (ruleRecord.ThenConstID)
      {
        case -10031:
          serverDefaultType = ServerDefaultType.RandomGuid;
          break;
        case -10026:
          serverDefaultType = ServerDefaultType.CallerIdentity;
          break;
        case -10013:
          serverDefaultType = ServerDefaultType.ServerDateTime;
          break;
        default:
          return false;
      }
      this.AttachRule(ruleRecord, (WorkItemRule) new ServerDefaultRule()
      {
        From = serverDefaultType
      });
      return true;
    }

    private void AttachRule(RuleRecord ruleRecord, WorkItemRule rule)
    {
      WorkItemFieldRule workItemFieldRule = this.GetFieldRule(ruleRecord.ThenFldID);
      if (ruleRecord.PersonID != -1)
      {
        if ((ruleRecord.RuleFlags & RuleFlags.InversePerson) != RuleFlags.None)
        {
          rule.Not = ruleRecord.Person;
          rule.NotVsId = ruleRecord.PersonVsId;
        }
        else
        {
          rule.For = ruleRecord.Person;
          rule.ForVsId = ruleRecord.PersonVsId;
        }
      }
      if (ruleRecord.Fld2ID == 2)
      {
        string fromState = ruleRecord.Fld2Was;
        if (ruleRecord.Fld2WasConstID == -10000)
          fromState = string.Empty;
        string fld2Is = ruleRecord.Fld2Is;
        if (ruleRecord.Fld3ID == 22)
        {
          this.DefineTransition(fromState, fld2Is);
          string fld3Is = ruleRecord.Fld3Is;
          WhenWasRule whenWasRule = new WhenWasRule();
          whenWasRule.FieldId = 2;
          whenWasRule.Value = fromState;
          ConditionalBlockRule conditionalBlockRule1 = (ConditionalBlockRule) whenWasRule;
          ConditionalBlockRule conditionalBlockRule2 = conditionalBlockRule1;
          WhenRule rule1 = new WhenRule();
          rule1.FieldId = 2;
          rule1.Value = fld2Is;
          WhenRule whenRule = conditionalBlockRule2.AddRule<WhenRule>(rule1);
          WhenRule rule2 = new WhenRule();
          rule2.FieldId = 22;
          rule2.Value = fld3Is;
          whenRule.AddRule<WhenRule>(rule2).AddRule<WorkItemRule>(rule);
          rule = (WorkItemRule) conditionalBlockRule1;
        }
        else if (fromState != null)
        {
          WhenWasRule whenWasRule = new WhenWasRule();
          whenWasRule.FieldId = 2;
          whenWasRule.Value = fromState;
          ConditionalBlockRule conditionalBlockRule3 = (ConditionalBlockRule) whenWasRule;
          if (!string.IsNullOrEmpty(fld2Is))
          {
            this.DefineTransition(fromState, fld2Is);
            ConditionalBlockRule conditionalBlockRule4 = conditionalBlockRule3;
            WhenRule rule3 = new WhenRule();
            rule3.FieldId = 2;
            rule3.Value = fld2Is;
            conditionalBlockRule4.AddRule<WhenRule>(rule3).AddRule<WorkItemRule>(rule);
          }
          else
            conditionalBlockRule3.AddRule<WorkItemRule>(rule);
          rule = (WorkItemRule) conditionalBlockRule3;
        }
        else
        {
          WhenRule whenRule = new WhenRule();
          whenRule.FieldId = 2;
          whenRule.Value = fld2Is;
          ConditionalBlockRule conditionalBlockRule = (ConditionalBlockRule) whenRule;
          conditionalBlockRule.AddRule<WorkItemRule>(rule);
          rule = (WorkItemRule) conditionalBlockRule;
        }
      }
      if (ruleRecord.IfFldID != 0)
      {
        ConditionalBlockRule conditionalBlockRule = (ConditionalBlockRule) null;
        bool flag = (ruleRecord.RuleFlags & RuleFlags.IfNot) != 0;
        switch (ruleRecord.IfConstID)
        {
          case -10015:
            WhenRemainedNonEmptyRule remainedNonEmptyRule = new WhenRemainedNonEmptyRule();
            remainedNonEmptyRule.Inverse = flag;
            remainedNonEmptyRule.FieldId = ruleRecord.IfFldID;
            conditionalBlockRule = (ConditionalBlockRule) remainedNonEmptyRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          case -10014:
            WhenBecameNonEmptyRule becameNonEmptyRule = new WhenBecameNonEmptyRule();
            becameNonEmptyRule.Inverse = flag;
            becameNonEmptyRule.FieldId = ruleRecord.IfFldID;
            conditionalBlockRule = (ConditionalBlockRule) becameNonEmptyRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          case -10006:
            WhenWasRule whenWasRule = new WhenWasRule();
            whenWasRule.Inverse = flag;
            whenWasRule.FieldId = ruleRecord.IfFldID;
            whenWasRule.Value = string.Empty;
            conditionalBlockRule = (ConditionalBlockRule) whenWasRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          case -10001:
            WhenChangedRule whenChangedRule = new WhenChangedRule();
            whenChangedRule.Inverse = !flag;
            whenChangedRule.FieldId = ruleRecord.IfFldID;
            conditionalBlockRule = (ConditionalBlockRule) whenChangedRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          case -10000:
            WhenRule whenRule1 = new WhenRule();
            whenRule1.Inverse = flag;
            whenRule1.FieldId = ruleRecord.IfFldID;
            whenRule1.Value = string.Empty;
            conditionalBlockRule = (ConditionalBlockRule) whenRule1;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          default:
            if (ruleRecord.IfConstID > 0)
            {
              WhenRule whenRule2 = new WhenRule();
              whenRule2.Inverse = flag;
              whenRule2.FieldId = ruleRecord.IfFldID;
              whenRule2.Value = ruleRecord.If;
              conditionalBlockRule = (ConditionalBlockRule) whenRule2;
              conditionalBlockRule.AddRule<WorkItemRule>(rule);
              break;
            }
            break;
        }
        if (conditionalBlockRule != null)
          rule = (WorkItemRule) conditionalBlockRule;
      }
      if (ruleRecord.If2FldID != 0)
      {
        ConditionalBlockRule conditionalBlockRule = (ConditionalBlockRule) null;
        bool flag = (ruleRecord.RuleFlags & RuleFlags.If2Not) != 0;
        switch (ruleRecord.If2ConstID)
        {
          case -10006:
            WhenWasRule whenWasRule = new WhenWasRule();
            whenWasRule.Inverse = flag;
            whenWasRule.FieldId = ruleRecord.If2FldID;
            whenWasRule.Value = string.Empty;
            conditionalBlockRule = (ConditionalBlockRule) whenWasRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          case -10001:
            WhenChangedRule whenChangedRule = new WhenChangedRule();
            whenChangedRule.Inverse = !flag;
            whenChangedRule.FieldId = ruleRecord.If2FldID;
            conditionalBlockRule = (ConditionalBlockRule) whenChangedRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
          case -10000:
            WhenRule whenRule = new WhenRule();
            whenRule.Inverse = flag;
            whenRule.FieldId = ruleRecord.If2FldID;
            whenRule.Value = string.Empty;
            conditionalBlockRule = (ConditionalBlockRule) whenRule;
            conditionalBlockRule.AddRule<WorkItemRule>(rule);
            break;
        }
        if (conditionalBlockRule != null)
          rule = (WorkItemRule) conditionalBlockRule;
      }
      RuleBlock ruleBlock = workItemFieldRule.AddRule<RuleBlock>(RuleRecordsTranslator.GetScopeRule(ruleRecord));
      ruleBlock.AddRule<WorkItemRule>(rule);
      if ((ruleRecord.RuleFlags & RuleFlags.DenyWrite) == RuleFlags.None || (ruleRecord.RuleFlags2 & RuleFlags2.ThenImplicitUnchanged) == RuleFlags2.None)
        return;
      ruleBlock.AddRule<AllowExistingValueRule>(new AllowExistingValueRule());
    }

    private static RuleBlock GetScopeRule(RuleRecord ruleRecord)
    {
      if (ruleRecord.AreaID == 0)
        return (RuleBlock) new CollectionScopedRules();
      return ruleRecord.Fld1ID != 25 ? (RuleBlock) new ProjectScopedRules() : (RuleBlock) new WorkItemTypeScopedRules();
    }

    private void DefineTransition(string fromState, string toState)
    {
      if (string.IsNullOrEmpty(toState))
        return;
      if (fromState == null)
        fromState = "";
      HashSet<string> stringSet;
      if (!this.m_transitions.TryGetValue(fromState, out stringSet))
      {
        stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        if (!string.IsNullOrEmpty(fromState))
          stringSet.Add(fromState);
        this.m_transitions.Add(fromState, stringSet);
      }
      stringSet.Add(toState);
    }
  }
}
