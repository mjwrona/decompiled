// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionHierarchy
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class ContributionHierarchy : IContributionProvider
  {
    private static Contribution[] s_emptyContributionList = Array.Empty<Contribution>();
    private Dictionary<string, IContributionProvider> m_contributionProviders = new Dictionary<string, IContributionProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ContributionType> m_contributionTypes = new Dictionary<string, ContributionType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, Contribution> m_contributions = new Dictionary<string, Contribution>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, Contribution[]> m_contributionsByChild = new Dictionary<string, Contribution[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, Contribution[]> m_contributionsByParent = new Dictionary<string, Contribution[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, Contribution[]> m_contributionsByType = new Dictionary<string, Contribution[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ContributionConstraint> m_constraints = new Dictionary<string, ContributionConstraint>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ContributionConstraintSet> m_constraintSets = new Dictionary<string, ContributionConstraintSet>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private ContributionData m_contributionData;

    public ContributionHierarchy(
      IEnumerable<IContributionProvider> contributionPoviders)
    {
      Dictionary<string, List<Contribution>> dictionary1 = new Dictionary<string, List<Contribution>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<Contribution>> dictionary2 = new Dictionary<string, List<Contribution>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<Contribution>> dictionary3 = new Dictionary<string, List<Contribution>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<Contribution> contributionsWithIncludes = new List<Contribution>();
      foreach (IContributionProvider contributionPovider in contributionPoviders)
        this.CompileContributions(contributionPovider, dictionary1, dictionary2, dictionary3, contributionsWithIncludes);
      this.PostCompileContributions(dictionary1, dictionary2, dictionary3, contributionsWithIncludes);
      this.ValidateContributions(dictionary1, dictionary2, dictionary3);
      this.m_contributionsByChild = this.ConvertToArrayBasedDictionary(dictionary1);
      this.m_contributionsByParent = this.ConvertToArrayBasedDictionary(dictionary2);
      this.m_contributionsByType = this.ConvertToArrayBasedDictionary(dictionary3);
      this.m_contributionData = new ContributionData()
      {
        Contributions = (IEnumerable<Contribution>) this.m_contributions.Values,
        ContributionTypes = (IEnumerable<ContributionType>) this.m_contributionTypes.Values,
        Constraints = (IEnumerable<ContributionConstraint>) this.m_constraints.Values
      };
    }

    public string ProviderName => string.Empty;

    public string ProviderDisplayName => string.Empty;

    public ContributionData QueryContributionData(IVssRequestContext requestContext) => this.m_contributionData;

    public ContributionProviderDetails QueryProviderDetails(IVssRequestContext requestContext) => (ContributionProviderDetails) null;

    public IEnumerable<Contribution> Contributions => (IEnumerable<Contribution>) this.m_contributions.Values;

    public IEnumerable<ContributionType> ContributionTypes => (IEnumerable<ContributionType>) this.m_contributionTypes.Values;

    public IEnumerable<ContributionConstraint> Constraints => (IEnumerable<ContributionConstraint>) this.m_constraints.Values;

    internal ContributionConstraintSet GetContributionConstraints(string contributionId)
    {
      ContributionConstraintSet contributionConstraints;
      this.m_constraintSets.TryGetValue(contributionId, out contributionConstraints);
      return contributionConstraints;
    }

    public Contribution QueryContribution(string contributionId)
    {
      Contribution contribution;
      this.m_contributions.TryGetValue(contributionId, out contribution);
      return contribution;
    }

    public IContributionProvider QueryContributionProvider(string contributionId)
    {
      IContributionProvider contributionProvider = (IContributionProvider) null;
      int num;
      if (!string.IsNullOrEmpty(contributionId) && (num = contributionId.IndexOf(".")) > 0)
      {
        int length;
        if ((length = contributionId.IndexOf(".", num + 1)) == -1)
          length = contributionId.Length;
        this.m_contributionProviders.TryGetValue(contributionId.Substring(0, length), out contributionProvider);
      }
      return contributionProvider;
    }

    public IEnumerable<Contribution> QueryContributionsForChild(string contributionId)
    {
      Contribution[] contributionList;
      if (!this.m_contributionsByChild.TryGetValue(contributionId, out contributionList))
        contributionList = ContributionHierarchy.s_emptyContributionList;
      return (IEnumerable<Contribution>) contributionList;
    }

    public IEnumerable<Contribution> QueryContributionsForTarget(string contributionId)
    {
      Contribution[] contributionList;
      if (!this.m_contributionsByParent.TryGetValue(contributionId, out contributionList))
        contributionList = ContributionHierarchy.s_emptyContributionList;
      return (IEnumerable<Contribution>) contributionList;
    }

    public IEnumerable<Contribution> QueryContributionsForType(string contributionTypeId)
    {
      Contribution[] contributionList;
      if (!this.m_contributionsByType.TryGetValue(contributionTypeId, out contributionList))
        contributionList = ContributionHierarchy.s_emptyContributionList;
      return (IEnumerable<Contribution>) contributionList;
    }

    public IEnumerable<Contribution> QueryContributions(
      IEnumerable<string> contributionIds = null,
      HashSet<string> allowedContributionTypes = null,
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeSelf,
      Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions> filterCallback = null,
      ContributionDiagnostics diagnostics = null)
    {
      Dictionary<string, Contribution> matchedContributions = new Dictionary<string, Contribution>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<long> matchedRelationships = new HashSet<long>();
      if ((queryOptions & ContributionQueryOptions.IncludeAll) == ContributionQueryOptions.None)
        queryOptions |= ContributionQueryOptions.IncludeSelf;
      if (contributionIds != null)
      {
        foreach (string contributionId in contributionIds)
        {
          Contribution contribution;
          if (this.m_contributions.TryGetValue(contributionId, out contribution))
            this.QueryContributionsInternal(contribution, queryOptions, filterCallback, matchedRelationships, matchedContributions, (Dictionary<string, ContributionNode>) null, (ContributionNode) null, (string) null, diagnostics);
        }
      }
      else
      {
        ContributionQueryOptions queryOptions1 = ContributionQueryOptions.IncludeSelf | queryOptions & ~ContributionQueryOptions.IncludeAll;
        foreach (Contribution contribution in this.m_contributions.Values)
          this.QueryContributionsInternal(contribution, queryOptions1, filterCallback, matchedRelationships, matchedContributions, (Dictionary<string, ContributionNode>) null, (ContributionNode) null, (string) null, diagnostics);
      }
      if (allowedContributionTypes != null && allowedContributionTypes.Comparer != StringComparer.OrdinalIgnoreCase)
        throw new ArgumentException("AllowedContributionTypes HashSet must use case-insensitive comparison.");
      List<string> stringList = (List<string>) null;
      foreach (string key in matchedContributions.Keys)
      {
        if (matchedContributions[key] == null || allowedContributionTypes != null && !allowedContributionTypes.Contains(matchedContributions[key].Type))
        {
          if (stringList == null)
            stringList = new List<string>();
          stringList.Add(key);
        }
      }
      if (stringList != null)
      {
        foreach (string key in stringList)
          matchedContributions.Remove(key);
      }
      return (IEnumerable<Contribution>) matchedContributions.Values;
    }

    public IDictionary<string, ContributionNode> QueryContributions(
      IEnumerable<string> contributionIds,
      Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions> filterCallback = null,
      ContributionDiagnostics diagnostics = null)
    {
      Dictionary<string, ContributionNode> matchedContributionNodes = new Dictionary<string, ContributionNode>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<long> matchedRelationships = new HashSet<long>();
      foreach (string contributionId in contributionIds)
      {
        Contribution contribution;
        if (!string.IsNullOrEmpty(contributionId) && this.m_contributions.TryGetValue(contributionId, out contribution))
          this.QueryContributionsInternal(contribution, ContributionQueryOptions.IncludeAll, filterCallback, matchedRelationships, (Dictionary<string, Contribution>) null, matchedContributionNodes, (ContributionNode) null, (string) null, diagnostics);
      }
      return (IDictionary<string, ContributionNode>) matchedContributionNodes;
    }

    private Dictionary<string, Contribution[]> ConvertToArrayBasedDictionary(
      Dictionary<string, List<Contribution>> listBasedDictionary)
    {
      Dictionary<string, Contribution[]> arrayBasedDictionary = new Dictionary<string, Contribution[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, List<Contribution>> listBased in listBasedDictionary)
        arrayBasedDictionary[listBased.Key] = listBased.Value.ToArray();
      return arrayBasedDictionary;
    }

    private void AddRelatedContribution(
      Dictionary<string, List<Contribution>> contributionIndex,
      Contribution contribution,
      string relatedContributionId)
    {
      List<Contribution> contributionList;
      if (!contributionIndex.TryGetValue(relatedContributionId, out contributionList))
      {
        contributionList = new List<Contribution>();
        contributionIndex.Add(relatedContributionId, contributionList);
      }
      if (contributionList.Contains(contribution))
        return;
      contributionList.Add(contribution);
    }

    private void CompileContributions(
      IContributionProvider contributionProvider,
      Dictionary<string, List<Contribution>> contributionsByChild,
      Dictionary<string, List<Contribution>> contributionsByParent,
      Dictionary<string, List<Contribution>> contributionsByType,
      List<Contribution> contributionsWithIncludes)
    {
      if (contributionProvider.ContributionTypes != null)
      {
        foreach (ContributionType contributionType in contributionProvider.ContributionTypes)
          this.m_contributionTypes[contributionType.Id] = contributionType;
      }
      if (contributionProvider.Constraints != null)
      {
        foreach (ContributionConstraint constraint in contributionProvider.Constraints)
          this.m_constraints[constraint.Id] = constraint;
      }
      if (contributionProvider.Contributions == null)
        return;
      this.m_contributionProviders[contributionProvider.ProviderName] = contributionProvider;
      foreach (Contribution contribution in contributionProvider.Contributions)
      {
        this.m_contributions[contribution.Id] = contribution;
        if (contribution.Type == null)
          contribution.Type = string.Empty;
        List<Contribution> contributionList;
        if (!contributionsByType.TryGetValue(contribution.Type, out contributionList))
        {
          contributionList = new List<Contribution>();
          contributionsByType[contribution.Type] = contributionList;
        }
        contributionList.Add(contribution);
        if (contribution.Targets != null)
        {
          for (int index = 0; index < contribution.Targets.Count; ++index)
            this.AddRelatedContribution(contributionsByParent, contribution, contribution.Targets[index]);
        }
        if (contribution.Includes != null)
          contributionsWithIncludes.Add(contribution);
      }
    }

    private bool IsVisibleTo(string contributionId, IEnumerable<string> visibleToRestrictions)
    {
      bool flag = false;
      if (visibleToRestrictions != null)
      {
        foreach (string visibleToRestriction in visibleToRestrictions)
        {
          if (contributionId.Length > visibleToRestriction.Length && contributionId[visibleToRestriction.Length] == '.' && contributionId.StartsWith(visibleToRestriction, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
      }
      else
        flag = true;
      return flag;
    }

    private void PostCompileContributions(
      Dictionary<string, List<Contribution>> contributionsByChild,
      Dictionary<string, List<Contribution>> contributionsByParent,
      Dictionary<string, List<Contribution>> contributionsByType,
      List<Contribution> contributionsWithIncludes)
    {
      foreach (Contribution contributionsWithInclude in contributionsWithIncludes)
      {
        foreach (string include in contributionsWithInclude.Includes)
        {
          Contribution contribution;
          if (this.m_contributions.TryGetValue(include, out contribution))
            this.AddRelatedContribution(contributionsByParent, contribution, contributionsWithInclude.Id);
        }
      }
      foreach (KeyValuePair<string, List<Contribution>> keyValuePair in contributionsByParent)
      {
        Contribution contribution1;
        if (this.m_contributions.TryGetValue(keyValuePair.Key, out contribution1))
        {
          foreach (Contribution contribution2 in keyValuePair.Value)
            this.AddRelatedContribution(contributionsByChild, contribution1, contribution2.Id);
        }
      }
      foreach (Contribution contribution in this.m_contributions.Values)
        this.m_constraintSets[contribution.Id] = new ContributionConstraintSet((IEnumerable<ContributionConstraint>) contribution.Constraints, (IDictionary<string, ContributionConstraint>) this.m_constraints);
    }

    private ContributionNode QueryContributionsInternal(
      Contribution contribution,
      ContributionQueryOptions queryOptions,
      Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions> filterCallback,
      HashSet<long> matchedRelationships,
      Dictionary<string, Contribution> matchedContributions,
      Dictionary<string, ContributionNode> matchedContributionNodes,
      ContributionNode parentNode,
      string parentContributionId,
      ContributionDiagnostics diagnostics)
    {
      List<EvaluatedCondition> evaluatedConditionList = (List<EvaluatedCondition>) null;
      EvaluatedRelationship evaluatedRelationship = (EvaluatedRelationship) null;
      ContributionNode contributionNode = (ContributionNode) null;
      parentContributionId = parentContributionId ?? string.Empty;
      long num = (long) contribution.Id.GetHashCode() + (long) parentContributionId.GetHashCode();
      if (!matchedRelationships.Contains(num))
      {
        if (diagnostics != null)
        {
          diagnostics.Contributions[contribution.Id] = contribution;
          evaluatedConditionList = new List<EvaluatedCondition>();
          evaluatedRelationship = new EvaluatedRelationship()
          {
            ParentContributionId = parentContributionId,
            ChildContributionId = contribution.Id,
            InputOptions = queryOptions
          };
          List<EvaluatedRelationship> evaluatedRelationshipList;
          if (!diagnostics.Relationships.TryGetValue(parentContributionId, out evaluatedRelationshipList))
          {
            evaluatedRelationshipList = new List<EvaluatedRelationship>();
            diagnostics.Relationships[parentContributionId] = evaluatedRelationshipList;
          }
          evaluatedRelationshipList.Add(evaluatedRelationship);
        }
        ContributionQueryOptions contributionQueryOptions = queryOptions;
        if (filterCallback != null)
          contributionQueryOptions = filterCallback(contribution, parentNode?.Contribution, parentContributionId, queryOptions, (ICollection<EvaluatedCondition>) evaluatedConditionList);
        if (diagnostics != null)
        {
          evaluatedRelationship.OutputOptions = contributionQueryOptions;
          if (evaluatedConditionList.Count > 0)
            evaluatedRelationship.Conditions = evaluatedConditionList;
        }
        matchedRelationships.Add(num);
        if ((contributionQueryOptions & ContributionQueryOptions.IncludeSelf) == ContributionQueryOptions.IncludeSelf)
        {
          if (matchedContributions != null)
            matchedContributions[contribution.Id] = contribution;
          if (matchedContributionNodes != null && !matchedContributionNodes.TryGetValue(contribution.Id, out contributionNode))
          {
            contributionNode = new ContributionNode(contribution);
            matchedContributionNodes[contribution.Id] = contributionNode;
          }
        }
        if ((contributionQueryOptions & ContributionQueryOptions.IncludeChildren) == ContributionQueryOptions.IncludeChildren)
        {
          ContributionQueryOptions queryOptions1 = contributionQueryOptions | ContributionQueryOptions.IncludeSelf;
          if ((queryOptions1 & ContributionQueryOptions.IncludeSubTree) != ContributionQueryOptions.IncludeSubTree)
            queryOptions1 &= ~ContributionQueryOptions.IncludeChildren;
          Contribution[] contributionArray;
          if (this.m_contributionsByParent.TryGetValue(contribution.Id, out contributionArray))
          {
            foreach (Contribution contribution1 in contributionArray)
              this.QueryContributionsInternal(contribution1, queryOptions1, filterCallback, matchedRelationships, matchedContributions, matchedContributionNodes, contributionNode, contribution.Id, diagnostics);
          }
        }
      }
      if (contributionNode != null && parentNode != null)
      {
        contributionNode.AddParent(parentNode);
        parentNode.AddChild(contributionNode);
      }
      return contributionNode;
    }

    private void ValidateContributions(
      Dictionary<string, List<Contribution>> contributionsByChild,
      Dictionary<string, List<Contribution>> contributionsByParent,
      Dictionary<string, List<Contribution>> contributionsByType)
    {
      List<Contribution> contributionList1 = new List<Contribution>();
      List<Contribution> contributionList2;
      foreach (Contribution contribution1 in this.m_contributions.Values)
      {
        if (!string.IsNullOrEmpty(contribution1.Type))
        {
          ContributionType contributionType;
          if (this.m_contributionTypes.TryGetValue(contribution1.Type, out contributionType))
          {
            if (!this.IsVisibleTo(contribution1.Id, (IEnumerable<string>) contributionType.VisibleTo))
            {
              contributionList1.Add(contribution1);
              continue;
            }
          }
          else
            continue;
        }
        if (contribution1.Targets != null)
        {
          for (int index1 = 0; index1 < contribution1.Targets.Count; ++index1)
          {
            Contribution contribution2;
            bool flag;
            if (flag = this.m_contributions.TryGetValue(contribution1.Targets[index1], out contribution2))
              flag = this.IsVisibleTo(contribution1.Id, (IEnumerable<string>) contribution2.VisibleTo);
            if (!flag && contributionsByParent.TryGetValue(contribution1.Targets[index1], out contributionList2))
            {
              for (int index2 = 0; index2 < contributionList2.Count; ++index2)
              {
                if (contributionList2[index2].Id.Equals(contribution1.Id, StringComparison.OrdinalIgnoreCase))
                {
                  List<Contribution> contributionList3 = contributionList2;
                  int index3 = index2;
                  int num = index3 - 1;
                  contributionList3.RemoveAt(index3);
                  break;
                }
              }
              if (contributionList2.Count == 0)
                contributionsByParent.Remove(contribution1.Targets[index1]);
            }
          }
        }
      }
      foreach (Contribution contribution3 in contributionList1)
      {
        this.m_contributions.Remove(contribution3.Id);
        if (contributionsByChild.TryGetValue(contribution3.Id, out contributionList2))
        {
          foreach (Contribution contribution4 in contributionList2)
          {
            List<Contribution> contributionList4;
            if (contributionsByParent.TryGetValue(contribution4.Id, out contributionList4))
            {
              for (int index4 = 0; index4 < contributionList4.Count; ++index4)
              {
                if (contributionList4[index4].Id.Equals(contribution3.Id, StringComparison.OrdinalIgnoreCase))
                {
                  List<Contribution> contributionList5 = contributionList4;
                  int index5 = index4;
                  int num = index5 - 1;
                  contributionList5.RemoveAt(index5);
                  break;
                }
              }
            }
          }
          contributionsByChild.Remove(contribution3.Id);
        }
        if (contributionsByParent.TryGetValue(contribution3.Id, out contributionList2))
        {
          foreach (Contribution contribution5 in contributionList2)
          {
            List<Contribution> contributionList6;
            if (contributionsByParent.TryGetValue(contribution5.Id, out contributionList6))
            {
              for (int index6 = 0; index6 < contributionList6.Count; ++index6)
              {
                if (contributionList6[index6].Id.Equals(contribution3.Id, StringComparison.OrdinalIgnoreCase))
                {
                  List<Contribution> contributionList7 = contributionList6;
                  int index7 = index6;
                  int num = index7 - 1;
                  contributionList7.RemoveAt(index7);
                  break;
                }
              }
            }
          }
          contributionsByParent.Remove(contribution3.Id);
        }
        if (contributionsByType.TryGetValue(contribution3.Type, out contributionList2))
        {
          for (int index8 = 0; index8 < contributionList2.Count; ++index8)
          {
            if (contributionList2[index8].Id.Equals(contribution3.Id, StringComparison.OrdinalIgnoreCase))
            {
              List<Contribution> contributionList8 = contributionList2;
              int index9 = index8;
              int num = index9 - 1;
              contributionList8.RemoveAt(index9);
              break;
            }
          }
        }
      }
    }
  }
}
