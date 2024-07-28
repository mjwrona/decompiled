// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageMetadataAnalyzer
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal class LanguageMetadataAnalyzer : IAnalyzer
  {
    private LanguageStatsDescendingComparer m_comparer = new LanguageStatsDescendingComparer();
    private IFileAccessorFactory m_factory;
    private AnalyzerDescriptor m_identifier;
    public const string c_layer = "LanguageMetadataAnalyzer";

    internal LanguageMetadataAnalyzer(IFileAccessorFactory factory)
    {
      this.m_identifier = new AnalyzerDescriptor()
      {
        Name = "Language",
        Id = new Guid("A9F8CFF2-B58E-4DDE-8D81-75A1DD31C9E7"),
        MajorVersion = 1,
        MinorVersion = 134,
        PatchVersion = 0,
        Description = "Report language breakdown of the repository. The analyzer reports total file counts and total byte sizes in a list sorted by size first. In case of preliminary results, only file counts are returned."
      };
      this.m_factory = factory;
    }

    internal LanguageMetadataAnalyzer()
      : this((IFileAccessorFactory) new DefaultFileAcessorFactory())
    {
    }

    public AnalyzerDescriptor AnalyzerDescriptor => this.m_identifier;

    public bool TryGetAnalytics(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<IRepositoryDescriptor> descriptors,
      out IAnalytics projectLanguageAnalytics)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<IRepositoryDescriptor>>(descriptors, nameof (descriptors));
      try
      {
        using (requestContext.TraceBlock(15280013, 15280014, "ProjectAnalysisService", nameof (LanguageMetadataAnalyzer), MethodBase.GetCurrentMethod().Name))
        {
          ProjectLanguageAnalytics languageAnalytics = new ProjectLanguageAnalytics(projectId)
          {
            RepositoryLanguageAnalytics = (IList<RepositoryLanguageAnalytics>) new List<RepositoryLanguageAnalytics>(),
            ResultPhase = ResultPhase.Full
          };
          Dictionary<Guid, LanguageMetadataRecord> dictionary = new Dictionary<Guid, LanguageMetadataRecord>();
          using (ProjectAnalysisComponent analysisComponent = requestContext.CreateProjectAnalysisComponent())
          {
            List<LanguageMetadataRecord> languageMetadata = analysisComponent.GetLanguageMetadata(projectId);
            dictionary = languageMetadata.Where<LanguageMetadataRecord>((Func<LanguageMetadataRecord, bool>) (r => r.IsUpToDate())).ToDictionary<LanguageMetadataRecord, Guid, LanguageMetadataRecord>((Func<LanguageMetadataRecord, Guid>) (e => e.RepositoryId), (Func<LanguageMetadataRecord, LanguageMetadataRecord>) (e => e));
            HashSet<Guid> guidSet = new HashSet<Guid>(languageMetadata.Where<LanguageMetadataRecord>((Func<LanguageMetadataRecord, bool>) (r => !r.IsUpToDate())).Select<LanguageMetadataRecord, Guid>((Func<LanguageMetadataRecord, Guid>) (r => r.RepositoryId)));
          }
          bool flag = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ProjectAnalysisSecurityConstants.SecurityNamespaceId).HasWritePermission(requestContext, projectId.ToLanguageMetricsToken(), false);
          foreach (IRepositoryDescriptor descriptor in descriptors)
          {
            LanguageMetadataRecord preliminaryRecord;
            if (!dictionary.TryGetValue(descriptor.Id, out preliminaryRecord) & flag)
            {
              preliminaryRecord = this.CreatePreliminaryRecord(descriptor, new List<LanguageStatistics>());
              requestContext.QueueLanguageMetadataAnalyzerJob(descriptor, nameof (LanguageMetadataAnalyzer));
              languageAnalytics.ResultPhase = ResultPhase.Preliminary;
            }
            if (preliminaryRecord != null)
            {
              RepositoryLanguageAnalytics repositoryAnalytics = this.CreateRepositoryAnalytics(requestContext, descriptor.Name, preliminaryRecord);
              languageAnalytics.RepositoryLanguageAnalytics.Add(repositoryAnalytics);
            }
          }
          languageAnalytics.LanguageBreakdown = (IList<LanguageStatistics>) this.AggregateAndTakeTopLanguages(languageAnalytics.RepositoryLanguageAnalytics);
          projectLanguageAnalytics = (IAnalytics) languageAnalytics;
          return true;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15281501, "ProjectAnalysisService", nameof (LanguageMetadataAnalyzer), ex);
        projectLanguageAnalytics = (IAnalytics) null;
        return false;
      }
    }

    private LanguageMetadataRecord CreatePreliminaryRecord(
      IRepositoryDescriptor descriptor,
      List<LanguageStatistics> languageBreakdown)
    {
      int fileCount = languageBreakdown.Sum<LanguageStatistics>((Func<LanguageStatistics, int>) (e => e.Files));
      switch (descriptor.Type)
      {
        case RepositoryType.TfsGit:
          return this.CreateGitPreliminaryRecord(descriptor as TfsGitRepositoryDescriptor, fileCount, languageBreakdown);
        case RepositoryType.Tfvc:
          return this.CreateTfvcPreliminaryRecord(descriptor as TfvcRepositoryDescriptor, fileCount, languageBreakdown);
        default:
          throw new ArgumentException("Type");
      }
    }

    private LanguageMetadataRecord CreateGitPreliminaryRecord(
      TfsGitRepositoryDescriptor descriptor,
      int fileCount,
      List<LanguageStatistics> languageBreakdown)
    {
      return new LanguageMetadataRecord(descriptor.ProjectId, descriptor.Type, descriptor.Id, fileCount, DateTime.Now, new int?(), descriptor.CommitId, descriptor.Branch, languageBreakdown, ResultPhase.Preliminary, 4);
    }

    private LanguageMetadataRecord CreateTfvcPreliminaryRecord(
      TfvcRepositoryDescriptor descriptor,
      int fileCount,
      List<LanguageStatistics> languageBreakdown)
    {
      return new LanguageMetadataRecord(descriptor.ProjectId, descriptor.Type, descriptor.Id, fileCount, DateTime.Now, new int?(descriptor.ChangesetId), new Sha1Id?(), descriptor.TfvcRootFolder, languageBreakdown, ResultPhase.Preliminary, 4);
    }

    private List<LanguageStatistics> AggregateAndTakeTopLanguages(
      IList<RepositoryLanguageAnalytics> repoStats,
      int take = 15)
    {
      Dictionary<string, LanguageStatistics> dictionary = new Dictionary<string, LanguageStatistics>();
      foreach (RepositoryLanguageAnalytics repoStat in (IEnumerable<RepositoryLanguageAnalytics>) repoStats)
      {
        foreach (LanguageStatistics languageStatistics1 in (IEnumerable<LanguageStatistics>) repoStat.LanguageBreakdown)
        {
          LanguageStatistics languageStatistics2;
          if (dictionary.TryGetValue(languageStatistics1.Name, out languageStatistics2))
          {
            languageStatistics2.Files += languageStatistics1.Files;
            LanguageStatistics languageStatistics3 = languageStatistics2;
            long? bytes1 = languageStatistics3.Bytes;
            long? bytes2 = languageStatistics1.Bytes;
            languageStatistics3.Bytes = bytes1.HasValue & bytes2.HasValue ? new long?(bytes1.GetValueOrDefault() + bytes2.GetValueOrDefault()) : new long?();
          }
          else
          {
            LanguageStatistics languageStatistics4 = new LanguageStatistics(repoStat.ProjectId)
            {
              Name = languageStatistics1.Name,
              Files = languageStatistics1.Files,
              Bytes = languageStatistics1.Bytes
            };
            dictionary.Add(languageStatistics1.Name, languageStatistics4);
          }
        }
      }
      List<LanguageStatistics> list = dictionary.Values.ToList<LanguageStatistics>();
      list.Sort((IComparer<LanguageStatistics>) this.m_comparer);
      this.AddPercentage((IList<LanguageStatistics>) list);
      foreach (RepositoryLanguageAnalytics repoStat in (IEnumerable<RepositoryLanguageAnalytics>) repoStats)
      {
        List<LanguageStatistics> source = new List<LanguageStatistics>((IEnumerable<LanguageStatistics>) repoStat.LanguageBreakdown);
        source.Sort((IComparer<LanguageStatistics>) this.m_comparer);
        repoStat.LanguageBreakdown = (IList<LanguageStatistics>) source.Take<LanguageStatistics>(take).ToList<LanguageStatistics>();
      }
      return list.Take<LanguageStatistics>(take).ToList<LanguageStatistics>();
    }

    private RepositoryLanguageAnalytics CreateRepositoryAnalytics(
      IVssRequestContext requestContext,
      string repositoryName,
      LanguageMetadataRecord record)
    {
      List<LanguageStatistics> currentStats = this.ApplyHeuristics(requestContext, (IEnumerable<LanguageStatistics>) record.LanguageBreakdown);
      currentStats.Sort((IComparer<LanguageStatistics>) this.m_comparer);
      this.AddPercentage((IList<LanguageStatistics>) currentStats);
      return new RepositoryLanguageAnalytics(record.ProjectId)
      {
        Name = repositoryName,
        Id = record.RepositoryId,
        ResultPhase = record.ResultPhase,
        UpdatedTime = record.UpdatedTime,
        LanguageBreakdown = (IList<LanguageStatistics>) currentStats
      };
    }

    private List<LanguageStatistics> ApplyHeuristics(
      IVssRequestContext requestContext,
      IEnumerable<LanguageStatistics> languageStatistics)
    {
      Dictionary<string, LanguageStatistics> dictionary = languageStatistics.ToDictionary<LanguageStatistics, string, LanguageStatistics>((Func<LanguageStatistics, string>) (s => s.Name), (Func<LanguageStatistics, LanguageStatistics>) (s => s));
      LanguageStatistics languageStatistics1;
      if (dictionary.TryGetValue(".h", out languageStatistics1))
      {
        List<LanguageStatistics> list = dictionary.Where<KeyValuePair<string, LanguageStatistics>>((Func<KeyValuePair<string, LanguageStatistics>, bool>) (s => s.Key.Equals("C") || s.Key.Equals("C++") || s.Key.Equals("Objective-C"))).Select<KeyValuePair<string, LanguageStatistics>, LanguageStatistics>((Func<KeyValuePair<string, LanguageStatistics>, LanguageStatistics>) (s => s.Value)).ToList<LanguageStatistics>();
        LanguageStatistics dominatingLanguage;
        if (this.TryGetDominatingLanguage(list, out dominatingLanguage))
        {
          requestContext.Trace(15280023, TraceLevel.Verbose, "ProjectAnalysisService", nameof (LanguageMetadataAnalyzer), string.Format("ApplyHeuristics:.h:{0}:{1}:{2}:{3}", (object) dominatingLanguage.Name, (object) dominatingLanguage.Bytes, (object) languageStatistics1.Bytes, (object) list.Count));
          dominatingLanguage.Files += languageStatistics1.Files;
          if (dominatingLanguage.Bytes.HasValue && languageStatistics1.Bytes.HasValue)
          {
            LanguageStatistics languageStatistics2 = dominatingLanguage;
            long? bytes1 = languageStatistics2.Bytes;
            long? bytes2 = languageStatistics1.Bytes;
            languageStatistics2.Bytes = bytes1.HasValue & bytes2.HasValue ? new long?(bytes1.GetValueOrDefault() + bytes2.GetValueOrDefault()) : new long?();
          }
          dictionary.Remove(languageStatistics1.Name);
        }
      }
      return dictionary.Values.ToList<LanguageStatistics>();
    }

    private bool TryGetDominatingLanguage(
      List<LanguageStatistics> languageStats,
      out LanguageStatistics dominatingLanguage)
    {
      dominatingLanguage = (LanguageStatistics) null;
      if (languageStats == null || languageStats.Count == 0)
        return false;
      if (languageStats.Count == 1)
      {
        dominatingLanguage = languageStats.First<LanguageStatistics>();
        return true;
      }
      languageStats.Sort((IComparer<LanguageStatistics>) this.m_comparer);
      LanguageStatistics languageStatistics1 = languageStats.ElementAt<LanguageStatistics>(0);
      LanguageStatistics languageStatistics2 = languageStats.ElementAt<LanguageStatistics>(1);
      if (languageStatistics1.Bytes.HasValue && languageStatistics2.Bytes.HasValue)
      {
        long? bytes1 = languageStatistics1.Bytes;
        long? bytes2 = languageStatistics2.Bytes;
        long num = 100;
        long? nullable = bytes2.HasValue ? new long?(bytes2.GetValueOrDefault() * num) : new long?();
        if (bytes1.GetValueOrDefault() > nullable.GetValueOrDefault() & bytes1.HasValue & nullable.HasValue)
        {
          dominatingLanguage = languageStatistics1;
          return true;
        }
      }
      return false;
    }

    private void AddPercentage(IList<LanguageStatistics> currentStats)
    {
      long num1 = (long) currentStats.Sum<LanguageStatistics>((Func<LanguageStatistics, int>) (s => s.Files));
      long num2 = currentStats.Where<LanguageStatistics>((Func<LanguageStatistics, bool>) (s => this.IsRecognizedLanguage(s.Name))).Sum<LanguageStatistics>((Func<LanguageStatistics, long>) (s => s.Bytes.GetValueOrDefault()));
      foreach (LanguageStatistics currentStat in (IEnumerable<LanguageStatistics>) currentStats)
      {
        currentStat.FilesPercentage = new double?(Math.Round((double) currentStat.Files / (double) num1 * 100.0, 2, MidpointRounding.AwayFromZero));
        if (this.IsRecognizedLanguage(currentStat.Name) && currentStat.Bytes.HasValue && num2 > 0L)
          currentStat.LanguagePercentage = new double?(Math.Round((double) currentStat.Bytes.Value / (double) num2 * 100.0, 2, MidpointRounding.AwayFromZero));
      }
    }

    private bool IsRecognizedLanguage(string s) => !s.StartsWith(".") && !s.Equals("Unknown");
  }
}
