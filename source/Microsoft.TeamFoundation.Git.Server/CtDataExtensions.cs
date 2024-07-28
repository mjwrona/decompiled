// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CtDataExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Telemetry;
using Microsoft.TeamFoundation.Server.Core.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class CtDataExtensions
  {
    public static void LogElapsedMs(
      this ClientTraceData ctData,
      string ctKey,
      Action action,
      Stopwatch timer = null)
    {
      if (timer == null)
        timer = Stopwatch.StartNew();
      try
      {
        action();
      }
      finally
      {
        ctData.Add(ctKey, (object) timer.ElapsedMilliseconds);
      }
    }

    public static void AddGitPushCtData(
      this ClientTraceData eventData,
      ITfsGitRepository repo,
      Action action,
      Stopwatch timer = null)
    {
      eventData.Add("Action", (object) "Push");
      eventData.AddGitOperationCtData(repo.Key, repo.Name, action, timer);
    }

    public static void AddGitOperationCtData(
      this ClientTraceData eventData,
      RepoKey repoKey,
      string repoName,
      Action action,
      Stopwatch timer = null)
    {
      eventData.LogElapsedMs("ElapsedTimeMs", (Action) (() =>
      {
        eventData.Add("RepositoryId", (object) repoKey.RepoId);
        eventData.Add("RepositoryName", (object) repoName);
        eventData.Add("OdbId", (object) repoKey.OdbId);
        action();
      }), timer);
    }

    public static void AddGvfsPrefetchCtData(
      this ClientTraceData eventData,
      ITfsGitRepository repo,
      long? lastTimestamp,
      long newTimestamp,
      int numHaves,
      int numWants,
      int numToSend)
    {
      eventData.Add("Action", (object) "GvfsGetPrefetch");
      eventData.Add("RepositoryId", (object) repo.Key.RepoId.ToString());
      eventData.Add("RepositoryName", (object) repo.Name);
      eventData.Add("LastTimestamp", (object) lastTimestamp);
      eventData.Add("NumberOfHaves", (object) numHaves);
      eventData.Add("NumberOfWants", (object) numWants);
      eventData.Add("NumberOfObjectsToSend", (object) numToSend);
    }

    public static void AddIOWaitData(
      this ClientTraceData eventData,
      TimeMeasuredStream inputStream,
      TimeMeasuredStream outputStream)
    {
      long timeInMs1 = GetTimeInMs(inputStream);
      long timeInMs2 = GetTimeInMs(outputStream);
      eventData.Add("IOWaitMs", (object) (timeInMs1 + timeInMs2));
      eventData.Add("IOBytes", (object) (inputStream.Bytes + outputStream.Bytes));

      static long GetTimeInMs(TimeMeasuredStream stream) => (long) Math.Round((double) stream.ElapsedTicks / (double) Stopwatch.Frequency * 1000.0);
    }

    public static void PublishGvfsPrefetchCtData(
      this ClientTraceData eventData,
      IVssRequestContext requestContext)
    {
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "Gvfs", eventData);
    }

    public static void PublishObjectMetadataGetSizesCtData(
      this ClientTraceData eventData,
      IVssRequestContext requestContext,
      OdbId odbId,
      int dbMisses,
      string feature)
    {
      ClientTraceService service = requestContext.GetService<ClientTraceService>();
      eventData.Add("Action", (object) "GetObjectsSizes");
      eventData.Add("OdbId", (object) odbId.ToString());
      eventData.Add("NumberOfDbMisses", (object) dbMisses);
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      ClientTraceData properties = eventData;
      service.Publish(requestContext1, "Microsoft.TeamFoundation.Git.Server", feature1, properties);
    }

    public static void PublishProtocolCtData(
      this ClientTraceData eventData,
      IVssRequestContext requestContext)
    {
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "Protocol", eventData);
    }

    public static void PublishAppInsightsTelemetry(
      this ClientTraceData eventData,
      IVssRequestContext requestContext)
    {
      requestContext.PublishAppInsightsTelemetry("TFS/Git/Push", eventData.ToCiData(), (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "PushId",
          "TFS.SourceControl.PushId"
        },
        {
          "RepositoryId",
          "TFS.SourceControl.RepositoryId"
        }
      });
    }

    public static CustomerIntelligenceData ToCiData(this ClientTraceData ctData)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) ctData.GetData())
        ciData.Add(keyValuePair.Key, keyValuePair.Value);
      return ciData;
    }

    public static void PublishContentPolicyCtData(
      this ClientTraceData eventData,
      IVssRequestContext requestContext,
      Guid repoId,
      Guid policyId,
      Action action,
      Stopwatch timer = null)
    {
      try
      {
        eventData.LogElapsedMs("ElapsedTimeMs", (Action) (() =>
        {
          eventData.Add("RepositoryId", (object) repoId);
          action();
        }), timer);
      }
      finally
      {
        eventData.Add("GitPushContentPolicyId", (object) policyId);
        requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "Protocol", eventData);
      }
    }
  }
}
