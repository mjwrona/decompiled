// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamIterationsService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamIterationsService : ITeamIterationsService, IVssFrameworkService
  {
    private string m_iterationSubscribedTeamCacheKey = "{0}/Agile_IterationSubscribedTeamCacheKey";

    public SortedIterationSubscriptions GetSortedTeamIterations(
      IVssRequestContext context,
      Guid projectId,
      ITeamIterationsCollection teamIterations)
    {
      return context.TraceBlock<SortedIterationSubscriptions>(290903, 290904, "Agile", nameof (TeamIterationsService), "GetSortedTeamIterations.SingleTeam", (Func<SortedIterationSubscriptions>) (() =>
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForNull<ITeamIterationsCollection>(teamIterations, nameof (teamIterations));
        List<Guid> list = teamIterations.Select<ITeamIteration, Guid>((Func<ITeamIteration, Guid>) (i => i.IterationId)).ToList<Guid>();
        WorkItemTrackingTreeService service = context.GetService<WorkItemTrackingTreeService>();
        Func<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> selector = (Func<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) (i => service.GetTreeNode(context, projectId, i, false));
        return this.GetSortedIterationSubscriptions(context, list.Select<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(selector).Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (n => n != null)).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>());
      }));
    }

    public IDictionary<Guid, SortedIterationSubscriptions> GetSortedTeamIterations(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<WebApiTeam> teams)
    {
      return context.TraceBlock<IDictionary<Guid, SortedIterationSubscriptions>>(290905, 290906, "Agile", nameof (TeamIterationsService), nameof (GetSortedTeamIterations), (Func<IDictionary<Guid, SortedIterationSubscriptions>>) (() =>
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForNull<IEnumerable<WebApiTeam>>(teams, nameof (teams));
        IDictionary<Guid, IEnumerable<Guid>> iterationSubscriptions = this.GetTeamIterationSubscriptions(context, projectId, teams);
        IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> iterations = this.GetIterations(context, projectId, (IEnumerable<Guid>) iterationSubscriptions.Values.SelectMany<IEnumerable<Guid>, Guid>((Func<IEnumerable<Guid>, IEnumerable<Guid>>) (i => i)).Distinct<Guid>().ToList<Guid>());
        IDictionary<Guid, SortedIterationSubscriptions> sortedTeamIterations = (IDictionary<Guid, SortedIterationSubscriptions>) new Dictionary<Guid, SortedIterationSubscriptions>();
        foreach (Guid key1 in (IEnumerable<Guid>) iterationSubscriptions.Keys)
        {
          List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> teamIterationNodes = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
          foreach (Guid key2 in iterationSubscriptions[key1])
          {
            if (iterations.ContainsKey(key2))
              teamIterationNodes.Add(iterations[key2]);
          }
          sortedTeamIterations[key1] = this.GetSortedIterationSubscriptions(context, teamIterationNodes);
        }
        return sortedTeamIterations;
      }));
    }

    public IReadOnlyCollection<WebApiTeam> GetTeamsWithIterations(
      IVssRequestContext context,
      ProjectInfo project)
    {
      return context.TraceBlock<IReadOnlyCollection<WebApiTeam>>(290907, 290908, "Agile", nameof (TeamIterationsService), nameof (GetTeamsWithIterations), (Func<IReadOnlyCollection<WebApiTeam>>) (() =>
      {
        ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
        return this.FilterToTeamsWithIterations(context, project, (IEnumerable<WebApiTeam>) context.GetService<ITeamService>().QueryTeamsInProject(context, project.Id));
      }));
    }

    public IReadOnlyCollection<WebApiTeam> FilterToTeamsWithIterations(
      IVssRequestContext context,
      ProjectInfo project,
      IEnumerable<WebApiTeam> teams)
    {
      return context.TraceBlock<IReadOnlyCollection<WebApiTeam>>(290909, 290910, "Agile", nameof (TeamIterationsService), nameof (FilterToTeamsWithIterations), (Func<IReadOnlyCollection<WebApiTeam>>) (() =>
      {
        ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
        ArgumentUtility.CheckForNull<IEnumerable<WebApiTeam>>(teams, nameof (teams));
        string key = string.Format(this.m_iterationSubscribedTeamCacheKey, (object) project.Id);
        IReadOnlyCollection<WebApiTeam> teamsWithIterations1;
        if (context.Items.TryGetValue<IReadOnlyCollection<WebApiTeam>>(key, out teamsWithIterations1))
          return teamsWithIterations1;
        HashSet<Guid> teamsWithIterations = new HashSet<Guid>(context.GetService<ITeamConfigurationService>().GetTeamsWithSubscribedIterations(context, project.Id));
        IReadOnlyCollection<WebApiTeam> teamsWithIterations2 = !teamsWithIterations.Any<Guid>() ? (IReadOnlyCollection<WebApiTeam>) new List<WebApiTeam>(0) : (IReadOnlyCollection<WebApiTeam>) teams.Where<WebApiTeam>((Func<WebApiTeam, bool>) (t => teamsWithIterations.Contains(t.Id))).OrderBy<WebApiTeam, string>((Func<WebApiTeam, string>) (t => t.Name), (IComparer<string>) TFStringComparer.TeamNameUI).ToList<WebApiTeam>();
        context.Items[key] = (object) teamsWithIterations2;
        return teamsWithIterations2;
      }));
    }

    private SortedIterationSubscriptions GetSortedIterationSubscriptions(
      IVssRequestContext context,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> teamIterationNodes)
    {
      teamIterationNodes.Sort(new Comparison<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(this.NodeComparison));
      int currentIterationIndex = this.GetCurrentIterationIndex((IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) teamIterationNodes, context.GetCollectionTimeZone());
      SortedIterationSubscriptions iterationSubscriptions = new SortedIterationSubscriptions()
      {
        CurrentIterationIndex = currentIterationIndex,
        Iterations = (IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) teamIterationNodes
      };
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> treeNodeList1 = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> treeNodeList2 = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
      iterationSubscriptions.PreviousIterations = (IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) treeNodeList1;
      iterationSubscriptions.FutureIterations = (IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) treeNodeList2;
      if (currentIterationIndex != -1)
      {
        for (int index = 0; index < teamIterationNodes.Count; ++index)
        {
          if (index < currentIterationIndex)
            treeNodeList1.Add(teamIterationNodes[index]);
          else if (index > currentIterationIndex)
            treeNodeList2.Add(teamIterationNodes[index]);
        }
        iterationSubscriptions.CurrentIteration = teamIterationNodes[currentIterationIndex];
        iterationSubscriptions.PreviousIterations = (IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) treeNodeList1;
        iterationSubscriptions.FutureIterations = (IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) treeNodeList2;
      }
      return iterationSubscriptions;
    }

    private IDictionary<Guid, IEnumerable<Guid>> GetTeamIterationSubscriptions(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<WebApiTeam> teams)
    {
      return context.TraceBlock<IDictionary<Guid, IEnumerable<Guid>>>(290911, 290912, "Agile", nameof (TeamIterationsService), nameof (GetTeamIterationSubscriptions), (Func<IDictionary<Guid, IEnumerable<Guid>>>) (() =>
      {
        IDictionary<Guid, IEnumerable<Guid>> iterationSubscriptions = (IDictionary<Guid, IEnumerable<Guid>>) new Dictionary<Guid, IEnumerable<Guid>>();
        List<WebApiTeam> list = teams.Where<WebApiTeam>((Func<WebApiTeam, bool>) (t => t != null)).ToList<WebApiTeam>();
        if (!list.Any<WebApiTeam>())
          return iterationSubscriptions;
        ITeamConfigurationService service = context.GetService<ITeamConfigurationService>();
        if (service.GetComponentVersion(context) < 15)
        {
          foreach (WebApiTeam team in list)
          {
            ITeamSettings teamSettings = service.GetTeamSettings(context, team, false, false);
            iterationSubscriptions[team.Id] = (IEnumerable<Guid>) teamSettings.Iterations.Select<ITeamIteration, Guid>((Func<ITeamIteration, Guid>) (i => i.IterationId)).ToList<Guid>();
            context.TraceAlways(1530025, TraceLevel.Error, "Agile", nameof (TeamIterationsService), string.Format("Should not hit this path anymore with iterations {0}", (object) teamSettings.Iterations.Count<ITeamIteration>()));
          }
        }
        else
        {
          iterationSubscriptions = service.GetIterationSubscriptionsForTeams(context, projectId, (IEnumerable<Guid>) list.Select<WebApiTeam, Guid>((Func<WebApiTeam, Guid>) (t => t.Id)).ToList<Guid>());
          foreach (WebApiTeam webApiTeam in list)
          {
            if (!iterationSubscriptions.ContainsKey(webApiTeam.Id))
              iterationSubscriptions[webApiTeam.Id] = Enumerable.Empty<Guid>();
            context.TraceAlways(1530022, TraceLevel.Info, "Agile", nameof (TeamIterationsService), string.Format("team iterations count {0}", (object) iterationSubscriptions.Values.SelectMany<IEnumerable<Guid>, Guid>((Func<IEnumerable<Guid>, IEnumerable<Guid>>) (i => i)).Distinct<Guid>().Count<Guid>()));
          }
        }
        return iterationSubscriptions;
      }));
    }

    private IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> GetIterations(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<Guid> iterations)
    {
      return (IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) context.TraceBlock<Dictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>(290913, 290914, "Agile", nameof (TeamIterationsService), nameof (GetIterations), (Func<Dictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>) (() =>
      {
        ArgumentUtility.CheckForNull<IEnumerable<Guid>>(iterations, nameof (iterations));
        WorkItemTrackingTreeService service = context.GetService<WorkItemTrackingTreeService>();
        return iterations.Select<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) (i => service.GetTreeNode(context, projectId, i, false))).Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (n => n != null)).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, Guid>) (n => n.CssNodeId), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) (n => n));
      }));
    }

    private int NodeComparison(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode x, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode y)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(x, nameof (x));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(y, nameof (y));
      int num = DateTime.Compare(this.DateComparisonValue(x.StartDate), this.DateComparisonValue(y.StartDate));
      if (num == 0)
      {
        num = DateTime.Compare(this.DateComparisonValue(x.FinishDate), this.DateComparisonValue(y.FinishDate));
        if (num == 0)
          num = TFStringComparer.CssTreePathName.Compare(x.RelativePath, y.RelativePath);
      }
      return num;
    }

    private int GetCurrentIterationIndex(IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> iterations, TimeZoneInfo timeZone)
    {
      ArgumentUtility.CheckForNull<IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>(iterations, nameof (iterations));
      ArgumentUtility.CheckForNull<TimeZoneInfo>(timeZone, nameof (timeZone));
      return this.GetDateIterationIndex(iterations, TimeZoneInfo.ConvertTime(DateTime.Now, timeZone));
    }

    private DateTime DateComparisonValue(DateTime? date) => !date.HasValue ? DateTime.MaxValue.Date : date.Value.Date;

    private int GetDateIterationIndex(IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> iterations, DateTime date)
    {
      ArgumentUtility.CheckForNull<IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>(iterations, nameof (iterations));
      if (iterations.Count == 0)
        return -1;
      int dateIterationIndex = 0;
      for (int index = 0; index < iterations.Count; ++index)
      {
        DateTime? nullable = iterations[index].StartDate;
        if (nullable.HasValue)
        {
          nullable = iterations[index].StartDate;
          if (!(nullable.Value.Date > date.Date))
          {
            nullable = iterations[index].FinishDate;
            if (nullable.Value.Date >= date.Date)
            {
              dateIterationIndex = index;
              break;
            }
            dateIterationIndex = index;
          }
          else
            break;
        }
        else
          break;
      }
      return dateIterationIndex;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
