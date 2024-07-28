// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Rules.RuleRunner
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Rules
{
  public class RuleRunner
  {
    private readonly List<JobRule> _rules = new List<JobRule>();

    public RuleRunner RegisterRule(JobRule rule)
    {
      this._rules.Add(rule);
      return this;
    }

    public bool RunRules() => !this._rules.Any<JobRule>() || this._rules.All<JobRule>((Func<JobRule, bool>) (rule => rule.Implement()));
  }
}
