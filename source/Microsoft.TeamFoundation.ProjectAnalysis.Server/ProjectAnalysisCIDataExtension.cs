// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectAnalysisCIDataExtension
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public static class ProjectAnalysisCIDataExtension
  {
    public static void PublishJobQueued(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      Guid jobId,
      string submittedFrom,
      RepositoryType repositoryType,
      string jobData,
      bool noPreexistingRecord,
      int? lastFileCount,
      DateTime? lastUpdatedTime)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", "LanguageJobQueued");
      properties.Add("JobId", (object) jobId);
      properties.Add("SubmittedFrom", submittedFrom);
      properties.Add("RepositoryType", repositoryType.ToString());
      properties.Add("JobData", jobData);
      properties.Add("NoPreexistingRecord", noPreexistingRecord);
      properties.Add("LastFileCount", (object) lastFileCount);
      properties.Add("LastUpdatedTime", (object) lastUpdatedTime);
      ciService.Publish(requestContext, "Microsoft.TeamFoundation.ProjectAnalysis.Server", "Language", properties);
    }

    public static void PublishJobReceived(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      Guid jobId,
      string jobData)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", "LanguageJobStarted");
      properties.Add("JobId", (object) jobId);
      properties.Add("JobData", jobData);
      ciService.Publish(requestContext, "Microsoft.TeamFoundation.ProjectAnalysis.Server", "Language", properties);
    }

    public static void PublishJobStarted(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      Guid jobId,
      RepositoryType repositoryType,
      Guid repositoryId)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", "LanguageJobStarted");
      properties.Add("JobId", (object) jobId);
      properties.Add("RepositoryType", repositoryType.ToString());
      properties.Add("RepositoryId", (object) repositoryId);
      ciService.Publish(requestContext, "Microsoft.TeamFoundation.ProjectAnalysis.Server", "Language", properties);
    }

    public static void PublishJobFinished(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      Guid jobId,
      RepositoryType repositoryType,
      Guid repositoryId,
      long timeElapsed,
      int fileCount)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", "LanguageJobFinished");
      properties.Add("JobId", (object) jobId);
      properties.Add("RepositoryType", repositoryType.ToString());
      properties.Add("RepositoryId", (object) repositoryId);
      properties.Add("TimeElapsedInMilliseconds", (double) timeElapsed);
      properties.Add("FileCount", (double) fileCount);
      ciService.Publish(requestContext, "Microsoft.TeamFoundation.ProjectAnalysis.Server", "Language", properties);
    }

    public static void PublishJobStopped(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      Guid jobId,
      RepositoryType repositoryType,
      Guid repositoryId)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", "LanguageJobStopped");
      properties.Add("JobId", (object) jobId);
      properties.Add("RepositoryType", repositoryType.ToString());
      properties.Add("RepositoryId", (object) repositoryId);
      ciService.Publish(requestContext, "Microsoft.TeamFoundation.ProjectAnalysis.Server", "Language", properties);
    }

    public static void PublishHistoricalRecord(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      LanguageMetadataRecord record)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Action", "SaveHistoricalData");
      properties.Add("RepositoryType", record.RepositoryType.ToString());
      properties.Add("RepositoryId", (object) record.RepositoryId);
      properties.Add("FileCount", (double) record.FileCount);
      properties.Add("CommitId", record.CommitId.ToString());
      properties.Add("Branch", record.Branch);
      properties.Add("ChangeSetId", (object) record.ChangesetId);
      properties.Add("LanguageBreakdown", record.LanguageBreakdown.Serialize<List<LanguageStatistics>>(true));
      ciService.Publish(requestContext, "Microsoft.TeamFoundation.ProjectAnalysis.Server", "Language", properties);
    }
  }
}
