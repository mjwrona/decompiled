// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionConstraintSet
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionConstraintSet
  {
    private const string s_area = "ContributionService";
    private const string s_layer = "ContributionConstraintSet";
    private const string c_evaluatedConstraintsKey = "evaluatedConstraints";
    private IEnumerable<ContributionConstraint> m_constraints;
    private IDictionary<string, ContributionConstraint> m_sharedConstraints;
    private List<List<ParsedContributionConstraint>> m_groupedConstraints;
    private bool m_hasSharedConstraintReference;
    private object m_processConstraintsLock = new object();

    internal ContributionConstraintSet(
      IEnumerable<ContributionConstraint> constraints,
      IDictionary<string, ContributionConstraint> sharedConstraints)
    {
      this.m_constraints = constraints;
      this.m_sharedConstraints = sharedConstraints;
    }

    public bool Evaluate(
      IVssRequestContext requestContext,
      Contribution contribution,
      string relationship,
      ICollection<EvaluatedCondition> evaluatedConditions,
      HashSet<string> userClaims,
      Func<ContributionConstraint, Contribution, bool?> evaluationCallback = null)
    {
      return this.Evaluate(requestContext, contribution, (string) null, relationship, evaluatedConditions, userClaims, evaluationCallback);
    }

    public bool Evaluate(
      IVssRequestContext requestContext,
      Contribution contribution,
      string phase,
      string relationship,
      ICollection<EvaluatedCondition> evaluatedConditions,
      HashSet<string> userClaims,
      Func<ContributionConstraint, Contribution, bool?> evaluationCallback = null)
    {
      if (userClaims != null && contribution.RestrictedTo != null && !this.EvaluateRestrictedClaims(requestContext, contribution, userClaims))
        return false;
      if (this.m_constraints == null || !this.m_constraints.Any<ContributionConstraint>())
        return true;
      if (this.m_groupedConstraints == null)
      {
        lock (this.m_processConstraintsLock)
        {
          if (this.m_groupedConstraints == null)
            this.m_groupedConstraints = this.GetGroupedConstraints(requestContext);
        }
      }
      IDictionary<string, bool> dictionary = (IDictionary<string, bool>) null;
      if (this.m_hasSharedConstraintReference)
      {
        object obj;
        if (requestContext.Items.TryGetValue("evaluatedConstraints", out obj))
        {
          dictionary = (IDictionary<string, bool>) obj;
        }
        else
        {
          dictionary = (IDictionary<string, bool>) new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          requestContext.Items["evaluatedConstraints"] = (object) dictionary;
        }
      }
      foreach (List<ParsedContributionConstraint> groupedConstraint in this.m_groupedConstraints)
      {
        bool flag1 = true;
        foreach (ParsedContributionConstraint contributionConstraint in (IEnumerable<ParsedContributionConstraint>) groupedConstraint)
        {
          if ((string.IsNullOrEmpty(contributionConstraint.Phase) || contributionConstraint.Phase.Equals(phase)) && (contributionConstraint.Relationships == null || contributionConstraint.Relationships.Contains(relationship)))
          {
            EvaluatedConstraint evaluatedConstraint = (EvaluatedConstraint) null;
            EvaluatedCondition evaluatedCondition = (EvaluatedCondition) null;
            if (evaluatedConditions != null)
            {
              evaluatedConstraint = new EvaluatedConstraint()
              {
                Constraint = contributionConstraint.Constraint
              };
              evaluatedCondition = new EvaluatedCondition()
              {
                Details = (object) evaluatedConstraint
              };
              evaluatedConditions.Add(evaluatedCondition);
            }
            bool? nullable;
            bool flag2 = evaluationCallback == null || !(nullable = evaluationCallback(contributionConstraint.Constraint, contribution)).HasValue ? contributionConstraint.Evaluate(requestContext, dictionary, contribution, evaluatedConstraint) : nullable.Value;
            if (evaluatedCondition != null)
            {
              evaluatedCondition.Evaluation = flag2;
              evaluatedCondition.Filtered = !flag2;
            }
            if (!flag2)
            {
              flag1 = false;
              break;
            }
          }
        }
        if (flag1)
          return true;
      }
      return false;
    }

    private bool EvaluateRestrictedClaims(
      IVssRequestContext requestContext,
      Contribution contribution,
      HashSet<string> userClaims)
    {
      List<List<string>> restrictedClaimGroups = ContributionRestriction.ParseRestrictedClaimGroups(contribution);
      bool restrictedClaims = false;
      foreach (List<string> stringList in restrictedClaimGroups)
      {
        List<string> restrictedToValues = stringList;
        requestContext.TraceConditionally(10025003, TraceLevel.Info, "ContributionService", nameof (ContributionConstraintSet), (Func<string>) (() => string.Join(" | ", restrictedToValues.Select<string, string>((Func<string, string>) (ext => ext ?? "")))));
        restrictedClaims = restrictedToValues.All<string>((Func<string, bool>) (restrictionClaim => userClaims.Contains(restrictionClaim)));
        if (restrictedClaims)
          break;
      }
      return restrictedClaims;
    }

    private List<List<ParsedContributionConstraint>> GetGroupedConstraints(
      IVssRequestContext requestContext)
    {
      List<List<ParsedContributionConstraint>> groupedConstraints = new List<List<ParsedContributionConstraint>>();
      ContributionFilterPluginService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ContributionFilterPluginService>();
      foreach (IEnumerable<ContributionConstraint> contributionConstraints in (IEnumerable<IEnumerable<ContributionConstraint>>) this.m_constraints.GroupBy<ContributionConstraint, int>((Func<ContributionConstraint, int>) (c => c.Group)))
      {
        List<ParsedContributionConstraint> contributionConstraintList = new List<ParsedContributionConstraint>();
        foreach (ContributionConstraint constraint in contributionConstraints)
        {
          ContributionConstraint sharedConstraint = (ContributionConstraint) null;
          IContributionFilter plugin = (IContributionFilter) null;
          if (!string.IsNullOrEmpty(constraint.Id))
          {
            this.m_hasSharedConstraintReference = true;
            if (this.m_sharedConstraints.TryGetValue(constraint.Id, out sharedConstraint))
              plugin = service.GetPlugin(requestContext, sharedConstraint.Name);
          }
          else
            plugin = service.GetPlugin(requestContext, constraint.Name);
          ParsedContributionConstraint contributionConstraint = new ParsedContributionConstraint(constraint, sharedConstraint, plugin);
          contributionConstraintList.Add(contributionConstraint);
        }
        groupedConstraints.Add(contributionConstraintList);
      }
      return groupedConstraints;
    }
  }
}
