// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic.DeleteBoardsForDeletedBacklogLevelsJobStep
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic
{
  public class DeleteBoardsForDeletedBacklogLevelsJobStep : BaseJobStep
  {
    private const string c_roundTripDatetimeFormat = "O";
    private const string c_lastRunRegistryKey = "/Configuration/DeleteBoardsForDeletedBacklogLevels/LastRun";
    private static readonly DateTime c_minDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

    public JobStepResult Execute(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      JobStepResult stepResult = new JobStepResult(nameof (DeleteBoardsForDeletedBacklogLevelsJobStep));
      stepResult.StartTimer();
      if (!requestContext.IsFeatureEnabled("Agile.Cleanup.DeleteBoardsForDeletedBacklogLevels"))
      {
        stepResult.ExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
        stepResult.Message = "Feature is not enabled";
      }
      else
        this.DeleteBoards(requestContext, stepResult);
      stepResult.StopTimer();
      return stepResult;
    }

    internal RegistryEntry GetLastRunUpdatedValue(IVssRequestContext requestContext, DateTime value) => new RegistryEntry("/Configuration/DeleteBoardsForDeletedBacklogLevels/LastRun", this.SerializeTimestamp(value));

    private JobStepResult DeleteBoards(IVssRequestContext requestContext, JobStepResult stepResult)
    {
      DeleteBoardsForDeletedBacklogLevelsResult backlogLevelsResult = new DeleteBoardsForDeletedBacklogLevelsResult();
      string str = (string) null;
      string dateTimeStr = (string) null;
      DeletedBehaviors refNamesDeletedSince = (DeletedBehaviors) null;
      IDictionary<Guid, string> dictionary = (IDictionary<Guid, string>) null;
      Exception exception = (Exception) null;
      try
      {
        using (this.Trace(requestContext, 290756, 290757, nameof (DeleteBoards)))
        {
          IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
          using (this.Trace(requestContext, 290758, 290759, nameof (DeleteBoards)))
            dateTimeStr = service1.GetValue(requestContext, (RegistryQuery) "/Configuration/DeleteBoardsForDeletedBacklogLevels/LastRun", this.SerializeTimestamp(DeleteBoardsForDeletedBacklogLevelsJobStep.c_minDateTime));
          DateTime deletedSince = this.DeserializeTimestamp(dateTimeStr);
          IBehaviorService service2 = requestContext.GetService<IBehaviorService>();
          using (this.Trace(requestContext, 290760, 290761, nameof (DeleteBoards)))
            refNamesDeletedSince = service2.GetBehaviorReferenceNamesDeletedSince(requestContext, deletedSince);
          IReadOnlyCollection<string> referenceNamesToDelete = this.GetBoardsCategoryReferenceNamesToDelete(requestContext, service2, refNamesDeletedSince);
          if (referenceNamesToDelete.Any<string>())
          {
            BoardService service3 = requestContext.GetService<BoardService>();
            using (this.Trace(requestContext, 290762, 290763, nameof (DeleteBoards)))
              dictionary = service3.DeleteBoardsByCategoryReferenceNames(requestContext, referenceNamesToDelete);
          }
          RegistryEntry lastRunUpdatedValue = this.GetLastRunUpdatedValue(requestContext, refNamesDeletedSince.AsOf);
          using (this.Trace(requestContext, 290764, 290765, nameof (DeleteBoards)))
            service1.UpdateOrDeleteEntries(requestContext, (IEnumerable<RegistryEntry>) new List<RegistryEntry>()
            {
              lastRunUpdatedValue
            });
          str = lastRunUpdatedValue.Value;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290004, TraceLevel.Warning, "Agile", TfsTraceLayers.Framework, ex);
        exception = ex;
      }
      backlogLevelsResult.LastRunOldValue = dateTimeStr;
      backlogLevelsResult.LastRunNewValue = str;
      backlogLevelsResult.ReferenceNamesFound = refNamesDeletedSince;
      backlogLevelsResult.BoardsDeleted = dictionary;
      backlogLevelsResult.ExceptionMsg = exception?.ToString();
      stepResult.ExecutionResult = exception == null ? TeamFoundationJobExecutionResult.Succeeded : TeamFoundationJobExecutionResult.Failed;
      stepResult.Message = JsonConvert.SerializeObject((object) backlogLevelsResult, Formatting.Indented);
      return stepResult;
    }

    private IReadOnlyCollection<string> GetBoardsCategoryReferenceNamesToDelete(
      IVssRequestContext requestContext,
      IBehaviorService behaviorService,
      DeletedBehaviors refNamesDeletedSince)
    {
      return !refNamesDeletedSince.BehaviorsDeleted.Any<KeyValuePair<string, DateTime>>() ? (IReadOnlyCollection<string>) new List<string>() : (IReadOnlyCollection<string>) behaviorService.IsSystemBehavior(requestContext, (IReadOnlyCollection<string>) refNamesDeletedSince.BehaviorsDeleted.Keys.ToList<string>()).Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (kvp => !kvp.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (kvp => kvp.Key)).ToList<string>();
    }

    private string SerializeTimestamp(DateTime dateTime) => dateTime.ToString("O");

    private DateTime DeserializeTimestamp(string dateTimeStr) => DateTime.Parse(dateTimeStr, (IFormatProvider) null, DateTimeStyles.RoundtripKind);
  }
}
