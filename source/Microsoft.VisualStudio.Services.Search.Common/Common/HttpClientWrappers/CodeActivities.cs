// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.CodeActivities
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class CodeActivities
  {
    public List<Tuple<DateTime, int>> RecentActivities { get; private set; }

    public int RecentActivityCount1Day { get; private set; }

    public int RecentActivityCount7Days { get; private set; }

    public int RecentActivityCount30Days { get; private set; }

    public DateTime? LastUpdated { get; private set; }

    public CodeActivities(
      TfsContributionsHttpClientWrapper.CodeMetrics metrics)
    {
      TfsContributionsHttpClientWrapper.GitMetrics gitMetrics = metrics != null ? metrics.GitMetrics : throw new ArgumentNullException(nameof (metrics));
      TfsContributionsHttpClientWrapper.TfvcMetrics tfvcMetrics = metrics.TfvcMetrics;
      bool flag1 = gitMetrics?.CommitsTrend != null;
      bool flag2 = tfvcMetrics?.ChangesetsTrend != null;
      this.RecentActivities = new List<Tuple<DateTime, int>>();
      if (flag1 & flag2 && gitMetrics.CommitsTrend.Length != tfvcMetrics.ChangesetsTrend.Length)
        throw new SearchServiceException("Invalid metrics length got from dataprovider");
      int num1 = flag1 ? gitMetrics.CommitsTrend.Length : (flag2 ? tfvcMetrics.ChangesetsTrend.Length : 0);
      for (int index = num1 - 1; index >= 0; --index)
      {
        int num2 = num1 - index;
        int num3 = 0;
        DateTime dateTime1 = DateTime.UtcNow.Date.AddDays((double) (-1 * num2));
        if (flag1)
          num3 += gitMetrics.CommitsTrend[index];
        if (flag2)
          num3 += tfvcMetrics.ChangesetsTrend[index];
        if (num3 > 0)
        {
          DateTime? lastUpdated = this.LastUpdated;
          if (lastUpdated.HasValue)
          {
            DateTime dateTime2 = dateTime1;
            lastUpdated = this.LastUpdated;
            if ((lastUpdated.HasValue ? (dateTime2 > lastUpdated.GetValueOrDefault() ? 1 : 0) : 0) == 0)
              goto label_13;
          }
          this.LastUpdated = new DateTime?(dateTime1);
        }
label_13:
        this.RecentActivities.Add(new Tuple<DateTime, int>(dateTime1, num3));
        if (num2 <= 1)
          this.RecentActivityCount1Day += num3;
        if (num2 <= 7)
          this.RecentActivityCount7Days += num3;
        this.RecentActivityCount30Days += num3;
      }
    }

    public CodeActivities(ProjectActivityMetrics projectActivityMetrics, DateTime tillTime)
    {
      if (projectActivityMetrics == null)
        throw new ArgumentNullException(nameof (projectActivityMetrics));
      if (projectActivityMetrics.CodeChangesTrend == null)
        throw new SearchServiceException("Invalid projectActivityMetrics received from TFS");
      this.PopulateFromCodeChangeTrends(projectActivityMetrics.CodeChangesTrend, tillTime);
    }

    public CodeActivities(RepositoryActivityMetrics repositoryMetrics, DateTime tillTime)
    {
      if (repositoryMetrics == null)
        throw new ArgumentNullException(nameof (repositoryMetrics));
      if (repositoryMetrics.CodeChangesTrend == null)
        throw new SearchServiceException("Invalid repositoryActivityMetrics received from TFS");
      this.PopulateFromCodeChangeTrends(repositoryMetrics.CodeChangesTrend, tillTime);
    }

    private void PopulateFromCodeChangeTrends(
      IList<CodeChangeTrendItem> codeChangesTrend,
      DateTime tillTime)
    {
      this.RecentActivities = codeChangesTrend.GroupBy<CodeChangeTrendItem, DateTime>((Func<CodeChangeTrendItem, DateTime>) (x => x.Time.ToUniversalTime().Date)).Select<IGrouping<DateTime, CodeChangeTrendItem>, Tuple<DateTime, int>>((Func<IGrouping<DateTime, CodeChangeTrendItem>, Tuple<DateTime, int>>) (x => new Tuple<DateTime, int>(x.Key, x.Sum<CodeChangeTrendItem>((Func<CodeChangeTrendItem, int>) (y => y.Value))))).ToList<Tuple<DateTime, int>>();
      foreach (CodeChangeTrendItem codeChangeTrendItem in (IEnumerable<CodeChangeTrendItem>) codeChangesTrend)
      {
        if (tillTime.Subtract(codeChangeTrendItem.Time.ToUniversalTime()) <= new TimeSpan(1, 1, 0, 0))
          this.RecentActivityCount1Day += codeChangeTrendItem.Value;
        if (tillTime.Date.Subtract(codeChangeTrendItem.Time.ToUniversalTime().Date) <= new TimeSpan(7, 0, 0, 0))
          this.RecentActivityCount7Days += codeChangeTrendItem.Value;
        if (tillTime.Date.Subtract(codeChangeTrendItem.Time.ToUniversalTime().Date) <= new TimeSpan(30, 0, 0, 0))
          this.RecentActivityCount30Days += codeChangeTrendItem.Value;
        if (codeChangeTrendItem.Value > 0)
        {
          DateTime? lastUpdated = this.LastUpdated;
          if (lastUpdated.HasValue)
          {
            DateTime time = codeChangeTrendItem.Time;
            lastUpdated = this.LastUpdated;
            if ((lastUpdated.HasValue ? (time > lastUpdated.GetValueOrDefault() ? 1 : 0) : 0) == 0)
              continue;
          }
          this.LastUpdated = new DateTime?(codeChangeTrendItem.Time);
        }
      }
    }
  }
}
