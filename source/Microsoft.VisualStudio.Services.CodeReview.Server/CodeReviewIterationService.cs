// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewIterationService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server.Utils;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewIterationService : 
    CodeReviewServiceBase,
    ICodeReviewIterationService,
    IVssFrameworkService
  {
    private const string c_registrySaveIterationCommandTimeoutSeconds = "/Service/CodeReview/Settings/SaveIterationCommandTimeoutSeconds";

    public override void ServiceStart(IVssRequestContext systemRequestContext) => base.ServiceStart(systemRequestContext);

    public override void ServiceEnd(IVssRequestContext systemRequestContext) => base.ServiceEnd(systemRequestContext);

    public IList<Iteration> GetIterations(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      bool includeUnpublished)
    {
      IList<Iteration> iterations = (IList<Iteration>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetIterations), 1380221, 1380222, 1380223, (Action) (() =>
      {
        requestContext.Trace(1380224, TraceLevel.Verbose, this.Area, this.Layer, "Getting iterations: review id: '{0}', project id: '{1}', includeUnpublished: '{2}'", (object) reviewId, (object) projectId, (object) includeUnpublished);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int reviewId1 = reviewId;
        bool flag = includeUnpublished;
        int? iterationId = new int?();
        int num = flag ? 1 : 0;
        iterations = this.GetIterationsInternal(requestContext1, projectId1, reviewId1, iterationId, num != 0);
      }));
      return iterations;
    }

    public IList<Iteration> GetRawIterations(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      bool includeUnpublished)
    {
      IList<Iteration> iterations = (IList<Iteration>) null;
      this.ExecuteAndTrace(requestContext, "GetIterations", 1380221, 1380222, 1380223, (Action) (() =>
      {
        requestContext.Trace(1380224, TraceLevel.Verbose, this.Area, this.Layer, "Getting iterations: review id: '{0}', project id: '{1}', includeUnpublished: '{2}'", (object) reviewId, (object) projectId, (object) includeUnpublished);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int reviewId1 = reviewId;
        bool flag = includeUnpublished;
        int? iterationId = new int?();
        int num = flag ? 1 : 0;
        iterations = this.GetIterationsInternal(requestContext1, projectId1, reviewId1, iterationId, num != 0, false);
      }));
      return iterations;
    }

    public Iteration GetIteration(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      IEnumerable<Iteration> iterations = (IEnumerable<Iteration>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetIteration), 1380211, 1380212, 1380213, (Action) (() =>
      {
        requestContext.Trace(1380214, TraceLevel.Verbose, this.Area, this.Layer, "Getting an iteration: review id: '{0}', iteration id: '{1}', project id: '{2}'", (object) reviewId, (object) iterationId, (object) projectId);
        iterations = (IEnumerable<Iteration>) this.GetIterationsInternal(requestContext, projectId, reviewId, new int?(iterationId), true);
      }));
      Iteration iteration = iterations.FirstOrDefault<Iteration>();
      if (iteration != null)
        iteration.AddReferenceLinks(requestContext, projectId, reviewId, iterationId);
      return iteration;
    }

    protected IList<Iteration> GetIterationsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId = null,
      bool includeUnpublished = false,
      bool fetchExtendedProperties = true)
    {
      IList<Iteration> iterations = (IList<Iteration>) null;
      this.ValidateReviewIterationIds(reviewId, iterationId);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        iterations = component.GetIterations(projectId, reviewId, iterationId, includeUnpublished);
      if (fetchExtendedProperties)
        ArtifactPropertyKinds.FetchIterationExtendedProperties(requestContext, projectId, iterations);
      return IdentityHelper.FillAuthorIdentities(requestContext, iterations);
    }

    public virtual Iteration SaveIteration(
      IVssRequestContext requestContext,
      Guid projectId,
      Iteration iteration,
      bool updateIteration)
    {
      Iteration savedIteration = (Iteration) null;
      this.ExecuteAndTrace(requestContext, nameof (SaveIteration), 1380201, 1380202, 1380203, (Action) (() =>
      {
        this.ValidateReviewIterationIds(iteration.ReviewId, iteration.Id);
        ValidationHelper.ValidateCommonIterationArguments(iteration, updateIteration);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        requestContext.Trace(1380204, TraceLevel.Verbose, this.Area, this.Layer, "Saving iteration: review id: '{0}', iteration id: '{1}', project id: '{2}', updateIteration: '{3}'", (object) iteration.ReviewId, (object) iteration.Id, (object) projectId, (object) updateIteration);
        IList<ChangeEntry> changeList1 = iteration.ChangeList;
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, iteration.ReviewId, ReviewScope.ReviewPlusIterations);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
        if (!updateIteration && !iteration.IsUnpublished && changeList1 != null && changeList1.Count<ChangeEntry>() != 0 && !this.ReviewHasCustomStorage(reviewRaw))
          throw new CodeReviewIterationMustBeOfUnpublishedTypeException(iteration.Id.Value);
        iteration = ValidationHelper.SanitizeIterationInput(requestContext, iteration, updateIteration);
        CodeReviewEventNotification crEvent = !updateIteration ? (CodeReviewEventNotification) new IterationAddedNotification(projectId, reviewRaw, iteration, reviewRaw.UpdatedDate) : (CodeReviewEventNotification) new IterationUpdatedNotification(projectId, reviewRaw, iteration, reviewRaw.UpdatedDate);
        EventServiceHelper.PublishDecisionPoint(requestContext, crEvent);
        if (changeList1 != null)
          iteration.ChangeList = this.SanitizeChangeList(requestContext, projectId, reviewRaw, iteration.Id.Value, changeList1);
        int iterationTimeoutSeconds = this.GetSaveIterationTimeoutSeconds(requestContext);
        DateTime? priorReviewUpdatedTimestamp;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        {
          component.CommandTimeout = iterationTimeoutSeconds;
          savedIteration = component.SaveIteration(projectId, iteration, updateIteration, out priorReviewUpdatedTimestamp);
        }
        Iteration iteration1 = savedIteration;
        IList<ChangeEntry> changeList2 = savedIteration.ChangeList;
        List<ChangeEntry> list = changeList2 != null ? changeList2.Select<ChangeEntry, ChangeEntry>((Func<ChangeEntry, ChangeEntry>) (entry => entry.ConvertDatabasePathToUserPath())).ToList<ChangeEntry>() : (List<ChangeEntry>) null;
        iteration1.ChangeList = (IList<ChangeEntry>) list;
        savedIteration = this.SaveIterationProperties(requestContext, projectId, iteration, savedIteration);
        if (savedIteration.Id.HasValue)
        {
          int? id = savedIteration.Id;
          int num = 1;
          if (id.GetValueOrDefault() > num & id.HasValue && !DefaultReviewArtifactId.IsValid(reviewRaw.SourceArtifactId))
            requestContext.GetService<ICodeReviewDiffCacheService>().QueueDiffCacheJob(requestContext, projectId, savedIteration.ReviewId, savedIteration.Id.Value);
        }
        Review review = reviewRaw.ShallowClone();
        review.UpdatedDate = savedIteration.UpdatedDate;
        EventServiceHelper.PublishNotification(requestContext, !(crEvent is IterationUpdatedNotification) ? (CodeReviewEventNotification) new IterationAddedNotification(projectId, review, savedIteration, priorReviewUpdatedTimestamp) : (CodeReviewEventNotification) new IterationUpdatedNotification(projectId, review, savedIteration, priorReviewUpdatedTimestamp), this.Area, this.Layer);
      }));
      if (savedIteration.ChangeList != null)
        savedIteration.ChangeList = (IList<ChangeEntry>) savedIteration.ChangeList.Select<ChangeEntry, ChangeEntry>((Func<ChangeEntry, ChangeEntry>) (entry => entry.PopulateReferenceLinks(requestContext, projectId, iteration.ReviewId))).ToList<ChangeEntry>();
      return IdentityHelper.FillAuthorIdentities(requestContext, (IList<Iteration>) new Iteration[1]
      {
        savedIteration
      }).First<Iteration>();
    }

    public void DeleteIteration(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      this.ExecuteAndTrace(requestContext, nameof (DeleteIteration), 1380231, 1380232, 1380233, (Action) (() =>
      {
        requestContext.Trace(1380234, TraceLevel.Verbose, this.Area, this.Layer, "Deleting an iteration: review id: '{0}', iteration id: '{1}', project id: '{2}'", (object) reviewId, (object) iterationId, (object) projectId);
        this.DeleteIterationsInternal(requestContext, projectId, reviewId, new int?(iterationId));
      }));
    }

    public void DeleteIterations(IVssRequestContext requestContext, Guid projectId, int reviewId) => this.ExecuteAndTrace(requestContext, nameof (DeleteIterations), 1380241, 1380242, 1380243, (Action) (() =>
    {
      requestContext.Trace(1380244, TraceLevel.Verbose, this.Area, this.Layer, "Deleting iterations: review id: '{0}', project id: '{1}'", (object) reviewId, (object) projectId);
      this.DeleteIterationsInternal(requestContext, projectId, reviewId);
    }));

    protected virtual void DeleteIterationsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId = null)
    {
      this.ValidateReviewIterationIds(reviewId, iterationId);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      if (!this.CanDelete(requestContext, projectId, reviewId))
        throw new ArgumentException(CodeReviewResources.CannotDeleteIteration());
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      Iteration iteration = new Iteration()
      {
        Id = new int?(iterationId.HasValue ? iterationId.Value : 0)
      };
      CodeReviewEventNotification crEvent = (CodeReviewEventNotification) new IterationDeletedNotification(projectId, reviewRaw, iteration, reviewRaw.UpdatedDate);
      EventServiceHelper.PublishDecisionPoint(requestContext, crEvent);
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        component.DeleteIterations(projectId, reviewId, iterationId);
      EventServiceHelper.PublishNotification(requestContext, crEvent, this.Area, this.Layer);
    }

    private IList<ChangeEntry> SanitizeChangeList(
      IVssRequestContext requestContext,
      Guid projectId,
      Review review,
      int iterationId,
      IList<ChangeEntry> changeEntries)
    {
      return this.ExecuteAndTrace<IList<ChangeEntry>>(requestContext, 1380601, 1380602, 1380603, (Func<IList<ChangeEntry>>) (() =>
      {
        bool changeTrackingId = FileChangeUtilities.ShouldComputeChangeTrackingId(iterationId, (IEnumerable<ChangeEntry>) changeEntries);
        int trackingComputation = this.GetMaxChangeEntriesForChangeTrackingComputation(requestContext);
        IList<ChangeEntry> changeEntriesToCompute = ValidationHelper.SanitizeChangeEntries(new int?(iterationId), changeEntries, changeTrackingId, trackingComputation);
        if (changeTrackingId)
        {
          requestContext.Trace(1380604, TraceLevel.Verbose, this.Area, this.Layer, "Sanitizing changelist: review id: '{0}', iteration id: '{1}', project id: '{2}'", (object) review.Id, (object) iterationId, (object) projectId);
          Stopwatch stopwatch = Stopwatch.StartNew();
          IList<int> forChangeTracking = this.GetIterationsForChangeTracking((IEnumerable<Iteration>) review.Iterations);
          IList<ChangeEntry> previousChangeEntries = (IList<ChangeEntry>) null;
          if (forChangeTracking.Count > 0)
          {
            previousChangeEntries = (IList<ChangeEntry>) new List<ChangeEntry>();
            using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
            {
              using (VirtualResultCollection<ChangeEntry> changeList = component.GetChangeList(projectId, review.Id, (IEnumerable<int>) forChangeTracking, new int?(10000)))
              {
                foreach (ChangeEntry changeEntry in changeList.GetCurrent<ChangeEntry>())
                  previousChangeEntries.Add(changeEntry);
              }
            }
          }
          new CodeReviewChangeTracker().ComputeChangeTrackingIds(changeEntriesToCompute, previousChangeEntries);
          TelemetryHelper.Publish(requestContext, "Iteration", new List<Tuple<string, string>>()
          {
            new Tuple<string, string>("ReviewId", review.Id.ToString()),
            new Tuple<string, string>("SourceArtifactId", review.SourceArtifactId ?? "Null"),
            new Tuple<string, string>("IterationId", iterationId.ToString()),
            new Tuple<string, string>("ChangesCount", changeEntries.Count<ChangeEntry>().ToString()),
            new Tuple<string, string>("KeyCodeReviewChangeTrackingIdComputationElapsedMs", stopwatch.ElapsedMilliseconds.ToString())
          });
        }
        return changeEntries;
      }), nameof (SanitizeChangeList));
    }

    public IterationChanges GetIterationChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      int? top = null,
      int? skip = null)
    {
      IEnumerable<ChangeEntry> changeList = this.GetChangeList(requestContext, projectId, reviewId, iterationIds, top, skip, new int?());
      int totalChangesCount = changeList.First<ChangeEntry>().TotalChangesCount;
      int? nextTop;
      int? nextSkip;
      ValidationHelper.EvaluateNextTopSkip(top, skip, new int?(totalChangesCount), out nextTop, out nextSkip, 2000);
      IterationChanges iterationChanges1 = new IterationChanges();
      IterationChanges iterationChanges2 = iterationChanges1;
      string[] strArray1;
      if (!nextSkip.HasValue)
        strArray1 = new string[0];
      else
        strArray1 = new string[1]
        {
          nextSkip.Value.ToString()
        };
      iterationChanges2.NextSkip = (IEnumerable<string>) strArray1;
      IterationChanges iterationChanges3 = iterationChanges1;
      string[] strArray2;
      if (!nextTop.HasValue)
        strArray2 = new string[0];
      else
        strArray2 = new string[1]
        {
          nextTop.Value.ToString()
        };
      iterationChanges3.NextTop = (IEnumerable<string>) strArray2;
      iterationChanges1.ChangeEntries = changeList;
      return iterationChanges1;
    }

    public IEnumerable<ChangeEntry> GetChangeList(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      int? top = null,
      int? skip = null,
      int? baseIteration = null)
    {
      return this.ExecuteAndTrace<IEnumerable<ChangeEntry>>(requestContext, 1380611, 1380612, 1380613, (Func<IEnumerable<ChangeEntry>>) (() =>
      {
        int topValue;
        int skipValue;
        ValidationHelper.EvaluateTopSkip(top, skip, out topValue, out skipValue, 2000);
        int baseIteration1 = baseIteration.HasValue ? baseIteration.Value : 0;
        if ((iterationIds == null || iterationIds.Count == 0) && baseIteration1 > 0)
          throw new ArgumentException(CodeReviewResources.BaseIterationRequiresIterations(), nameof (baseIteration));
        if (requestContext.IsTracing(1380614, TraceLevel.Verbose, this.Area, this.Layer))
        {
          string str = string.Join("/", iterationIds.Select<int, string>((Func<int, string>) (n => n.ToString())).ToArray<string>());
          requestContext.Trace(1380614, TraceLevel.Verbose, this.Area, this.Layer, "Getting changelist: review id: '{0}', iteration ids: '{1}', baseIteration '{2}', project id: '{3}', top: '{4}', skip: '{5}'", (object) reviewId, (object) str, (object) baseIteration1, (object) projectId, (object) top, (object) skip);
        }
        return this.GetChangeListInternal(requestContext, projectId, reviewId, iterationIds, topValue, skipValue, baseIteration1);
      }), nameof (GetChangeList));
    }

    private IEnumerable<ChangeEntry> GetChangeListInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      int top,
      int skip,
      int baseIteration)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) iterationIds, nameof (iterationIds));
      foreach (int iterationId in iterationIds)
        ArgumentUtility.CheckForOutOfRange(iterationId, "iterationId", 1);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      List<ChangeEntry> dbChanges = new List<ChangeEntry>();
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
      {
        using (VirtualResultCollection<ChangeEntry> changeList = component.GetChangeList(projectId, reviewId, (IEnumerable<int>) iterationIds, new int?(top), new int?(skip), new int?(baseIteration)))
        {
          foreach (ChangeEntry changeEntry in changeList.GetCurrent<ChangeEntry>())
          {
            if (changeEntry.Base != null || changeEntry.Modified != null)
              dbChanges.Add(changeEntry);
          }
        }
      }
      List<ChangeEntry> results;
      if (baseIteration > 0)
      {
        results = new List<ChangeEntry>();
        foreach (int iterationId in iterationIds)
          this.CombineIterationCompareChangeEntries(dbChanges, iterationId, baseIteration, results);
      }
      else
        results = dbChanges;
      List<ChangeEntry> changeListInternal = new List<ChangeEntry>();
      foreach (ChangeEntry entry in results)
      {
        entry.PopulateReferenceLinksAndNormalizeOutputs(requestContext, projectId, reviewId);
        changeListInternal.Add(entry);
      }
      return (IEnumerable<ChangeEntry>) changeListInternal;
    }

    private void CombineIterationCompareChangeEntries(
      List<ChangeEntry> dbChanges,
      int iterationId,
      int baseIteration,
      List<ChangeEntry> results)
    {
      ChangeEntry[] allChangesArray = dbChanges.Where<ChangeEntry>((Func<ChangeEntry, bool>) (change =>
      {
        int? iterationId1 = change.IterationId;
        int num1 = iterationId;
        if (iterationId1.GetValueOrDefault() == num1 & iterationId1.HasValue)
          return true;
        int? iterationId2 = change.IterationId;
        int num2 = baseIteration;
        return iterationId2.GetValueOrDefault() == num2 & iterationId2.HasValue;
      })).ToArray<ChangeEntry>();
      Dictionary<string, int> fileIndicies = new Dictionary<string, int>();
      Dictionary<string, int> addsAndPartialEdits = new Dictionary<string, int>();
      HashSet<int> intSet = new HashSet<int>();
      for (int index1 = 0; index1 < allChangesArray.Length; ++index1)
      {
        ChangeEntry changeEntry1 = allChangesArray[index1];
        int index2 = -1;
        int index3 = -1;
        string b = (string) null;
        if (!string.IsNullOrEmpty(changeEntry1.Base?.Path))
        {
          b = changeEntry1.Base.Path;
          int num;
          if (fileIndicies.TryGetValue(changeEntry1.Base.Path, out num))
            index2 = num;
          else
            fileIndicies.Add(changeEntry1.Base.Path, index1);
        }
        if (index2 < 0 && !string.IsNullOrEmpty(changeEntry1.Modified?.Path) && (b == null || !string.Equals(changeEntry1.Modified.Path, b, StringComparison.Ordinal)))
        {
          int num;
          if (fileIndicies.TryGetValue(changeEntry1.Modified.Path, out num))
            index2 = num;
          else
            fileIndicies.Add(changeEntry1.Modified.Path, index1);
        }
        if (changeEntry1.Type == ChangeType.Add && (changeEntry1.Base != null || changeEntry1.Modified != null))
        {
          string key = changeEntry1.Base != null ? changeEntry1.Base.SHA1Hash : changeEntry1.Modified.SHA1Hash;
          int num;
          if (addsAndPartialEdits.TryGetValue(key, out num))
            index3 = num;
          else
            addsAndPartialEdits.Add(key, index1);
        }
        if (changeEntry1.Type == ChangeType.Edit && changeEntry1.Base == null && changeEntry1.Modified != null)
        {
          int num;
          if (addsAndPartialEdits.TryGetValue(changeEntry1.Modified.SHA1Hash, out num))
            index3 = num;
          else
            addsAndPartialEdits.Add(changeEntry1.Modified.SHA1Hash, index1);
        }
        if (index2 >= 0 || index3 >= 0)
        {
          bool flag = true;
          int num;
          ChangeEntry changeEntry2;
          if (index2 >= 0)
          {
            num = index2;
            changeEntry2 = allChangesArray[index2];
            if (changeEntry2.Type == ChangeType.Add && changeEntry1.Type == ChangeType.Delete)
            {
              if (changeEntry2.Modified?.SHA1Hash != changeEntry1.Base?.SHA1Hash)
                results.Add(changeEntry2);
            }
            else if (changeEntry2.Type == ChangeType.Delete && changeEntry1.Type == ChangeType.Add)
            {
              if (changeEntry2.Base?.SHA1Hash != changeEntry1.Modified?.SHA1Hash)
                results.Add(changeEntry1);
            }
            else if (changeEntry2.Type == ChangeType.Add && changeEntry1.Type.HasFlag((Enum) ChangeType.Rename))
              results.Add(new ChangeEntry()
              {
                Base = changeEntry2.Base,
                ChangeId = changeEntry2.ChangeId,
                ChangeTrackingId = changeEntry2.ChangeTrackingId,
                IterationId = changeEntry2.IterationId,
                Modified = changeEntry1.Modified,
                Type = ChangeType.Edit
              });
            else if (changeEntry2.Type.HasFlag((Enum) ChangeType.Rename) && changeEntry1.Type == ChangeType.Add)
              results.Add(new ChangeEntry()
              {
                Base = changeEntry1.Base,
                ChangeId = changeEntry1.ChangeId,
                ChangeTrackingId = changeEntry1.ChangeTrackingId,
                IterationId = changeEntry1.IterationId,
                Modified = changeEntry2.Modified,
                Type = ChangeType.Edit
              });
            else if (changeEntry2.Type == ChangeType.Edit && changeEntry1.Type == ChangeType.Edit)
            {
              if (changeEntry2.Base != null)
                results.Add(changeEntry2);
              else
                results.Add(changeEntry1);
            }
            else
              flag = false;
          }
          else
          {
            num = index3;
            changeEntry2 = allChangesArray[index3];
            if (changeEntry2.Type == ChangeType.Edit && changeEntry1.Type == ChangeType.Add && changeEntry1.Base != null && changeEntry1.Modified == null)
              results.Add(new ChangeEntry()
              {
                Base = changeEntry1.Base,
                ChangeId = changeEntry1.ChangeId,
                ChangeTrackingId = changeEntry1.ChangeTrackingId,
                IterationId = changeEntry1.IterationId,
                Modified = changeEntry2.Modified,
                Type = ChangeType.Rename
              });
            else if (changeEntry2.Type == ChangeType.Add && changeEntry1.Type == ChangeType.Edit && changeEntry2.Base != null && changeEntry2.Modified == null)
              results.Add(new ChangeEntry()
              {
                Base = changeEntry2.Base,
                ChangeId = changeEntry2.ChangeId,
                ChangeTrackingId = changeEntry2.ChangeTrackingId,
                IterationId = changeEntry2.IterationId,
                Modified = changeEntry1.Modified,
                Type = ChangeType.Rename
              });
            else if (changeEntry2.Type == ChangeType.Add && changeEntry1.Type == ChangeType.Add && changeEntry2.Base == null && changeEntry2.Modified != null && changeEntry1.Base != null && changeEntry1.Modified == null)
              results.Add(new ChangeEntry()
              {
                Base = changeEntry1.Base,
                ChangeId = changeEntry1.ChangeId,
                ChangeTrackingId = changeEntry1.ChangeTrackingId,
                IterationId = changeEntry1.IterationId,
                Modified = changeEntry2.Modified,
                Type = ChangeType.Rename
              });
            else if (changeEntry2.Type == ChangeType.Add && changeEntry1.Type == ChangeType.Add && changeEntry2.Base != null && changeEntry2.Modified == null && changeEntry1.Base == null && changeEntry1.Modified != null)
              results.Add(new ChangeEntry()
              {
                Base = changeEntry2.Base,
                ChangeId = changeEntry2.ChangeId,
                ChangeTrackingId = changeEntry2.ChangeTrackingId,
                IterationId = changeEntry2.IterationId,
                Modified = changeEntry1.Modified,
                Type = ChangeType.Rename
              });
            else
              flag = false;
          }
          if (flag)
          {
            intSet.Add(index1);
            intSet.Add(num);
            CodeReviewIterationService.MarkFileHandled(fileIndicies, addsAndPartialEdits, changeEntry1.Base);
            CodeReviewIterationService.MarkFileHandled(fileIndicies, addsAndPartialEdits, changeEntry1.Modified);
            CodeReviewIterationService.MarkFileHandled(fileIndicies, addsAndPartialEdits, changeEntry2.Base);
            CodeReviewIterationService.MarkFileHandled(fileIndicies, addsAndPartialEdits, changeEntry2.Modified);
          }
        }
      }
      for (int changeIndex = 0; changeIndex < allChangesArray.Length; ++changeIndex)
      {
        if (!intSet.Contains(changeIndex) && !results.Any<ChangeEntry>((Func<ChangeEntry, bool>) (x => x.Modified?.SHA1Hash == allChangesArray[changeIndex].Modified?.SHA1Hash)))
          results.Add(allChangesArray[changeIndex]);
      }
    }

    private static void MarkFileHandled(
      Dictionary<string, int> fileIndicies,
      Dictionary<string, int> addsAndPartialEdits,
      ChangeEntryFileInfo fileInfo)
    {
      if (fileInfo == null)
        return;
      if (!string.IsNullOrEmpty(fileInfo.Path))
        fileIndicies.Remove(fileInfo.Path);
      addsAndPartialEdits.Remove(fileInfo.SHA1Hash);
    }

    public ChangeEntryStream ReadChangeEntryFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId,
      int changeId,
      ChangeEntryFileType fileType)
    {
      return this.ExecuteAndTrace<ChangeEntryStream>(requestContext, 1380621, 1380622, 1380623, (Func<ChangeEntryStream>) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        ArgumentUtility.CheckForOutOfRange(iterationId, nameof (iterationId), 1);
        ArgumentUtility.CheckForOutOfRange(changeId, nameof (changeId), 1);
        requestContext.Trace(1380624, TraceLevel.Verbose, this.Area, this.Layer, "Reading change entry file: review id: '{0}', iteration id: '{1}', project id: '{2}', change id: '{3}', fileType: '{4}'", (object) reviewId, (object) iterationId, (object) projectId, (object) changeId, (object) fileType);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, this.GetReviewRaw(requestContext, projectId, reviewId).SourceArtifactId);
        ContentAccessMetadata changeEntryAccessInfo = this.GetChangeEntryAccessInfo(requestContext, projectId, reviewId, iterationId, changeId, fileType);
        string fileName = changeEntryAccessInfo.FileId > 0 ? FileChangeUtilities.GetDownloadableFileName(changeEntryAccessInfo.FilePath) : throw new FileNotFoundException(CodeReviewResources.FileNotFoundForChangeIdException((object) fileType.ToString(), (object) changeId));
        return ReviewFilesUtility.GetFileContentStream(requestContext, (ReviewFileContentInfo) new ChangeEntryFileInfo()
        {
          ReviewId = reviewId,
          Path = changeEntryAccessInfo.FilePath,
          FileServiceFileId = changeEntryAccessInfo.FileId
        }, fileName);
      }), nameof (ReadChangeEntryFile));
    }

    public PushStreamContent GetIterationFilesContent(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      ChangeEntryFileType? fileType,
      out bool needsNextPage,
      int? top = null,
      int? skip = null)
    {
      PushStreamContent iterationFilesContent = (PushStreamContent) null;
      bool nextPageExists = false;
      this.ExecuteAndTrace(requestContext, 1380631, 1380632, 1380633, (Action) (() =>
      {
        int topValue;
        int skipValue;
        ValidationHelper.EvaluateTopSkip(top, skip, out topValue, out skipValue);
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) iterationIds, nameof (iterationIds));
        foreach (int iterationId in iterationIds)
          ArgumentUtility.CheckForOutOfRange(iterationId, "iterationId", 1);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        this.TraceGetIterationFilesContent(requestContext, projectId, reviewId, iterationIds, fileType, top, skip);
        IReviewContentProvider contentExtension = this.GetReviewContentExtension(reviewRaw.SourceArtifactId);
        iterationFilesContent = ReviewFilesUtility.GetZipPushStreamContent(requestContext, projectId, reviewRaw, iterationIds, fileType, topValue, skipValue, contentExtension, out nextPageExists);
      }), nameof (GetIterationFilesContent));
      needsNextPage = nextPageExists;
      return iterationFilesContent;
    }

    private IList<int> GetIterationsForChangeTracking(IEnumerable<Iteration> priorIterations)
    {
      IOrderedEnumerable<Iteration> orderedEnumerable = priorIterations.OrderByDescending<Iteration, int>((Func<Iteration, int>) (iteration => iteration.Id.Value));
      IList<int> forChangeTracking = (IList<int>) new List<int>();
      int num = 1;
      foreach (Iteration iteration in (IEnumerable<Iteration>) orderedEnumerable)
      {
        if (num <= 5)
        {
          forChangeTracking.Add(iteration.Id.Value);
          ++num;
        }
        else
          break;
      }
      return forChangeTracking;
    }

    public PropertiesCollection GetProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      return this.ExecuteAndTrace<PropertiesCollection>(requestContext, 1383431, 1383432, 1383433, (Func<PropertiesCollection>) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, this.GetReviewRaw(requestContext, projectId, reviewId).SourceArtifactId);
        requestContext.Trace(1383434, TraceLevel.Verbose, this.Area, this.Layer, "Getting iteration properties: project id: '{0}', review id: '{1}', iteration id: '{2}'", (object) projectId, (object) reviewId, (object) iterationId);
        return ArtifactPropertyKinds.GetProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.IterationPropertyKind, ArtifactPropertyKinds.GetIterationMoniker(projectId, reviewId, new int?(iterationId))));
      }), nameof (GetProperties));
    }

    public PropertiesCollection PatchProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId,
      PropertiesCollection properties)
    {
      PropertiesCollection patchedProperties = (PropertiesCollection) null;
      this.ExecuteAndTrace(requestContext, 1383421, 1383422, 1383423, (Action) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        if (!reviewRaw.CanUpdate())
          throw new CodeReviewNotActiveException(reviewId);
        ArgumentUtility.CheckForOutOfRange(iterationId, nameof (iterationId), 1);
        if (requestContext.IsTracing(1383424, TraceLevel.Verbose, this.Area, this.Layer))
          requestContext.Trace(1383424, TraceLevel.Verbose, this.Area, this.Layer, "Patching review properties: project id: '{0}', review id: '{1}', iteration id: '{2}', properties: '{3}'", (object) projectId, (object) reviewId, (object) iterationId, (object) ArtifactPropertyKinds.PreparePatchPropertiesInfo(properties));
        EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) new PropertiesNotification(projectId, reviewId, reviewRaw.SourceArtifactId, reviewRaw.UpdatedDate, reviewRaw.UpdatedDate, ResourceType.Iteration, iterationId));
        patchedProperties = ArtifactPropertyKinds.PatchProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.IterationPropertyKind, ArtifactPropertyKinds.GetIterationMoniker(projectId, reviewId, new int?(iterationId))), properties);
        UpdateTimestamps updateTimestamps;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          updateTimestamps = component.MarkIterationAsModified(projectId, reviewId, iterationId);
        EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) new PropertiesNotification(projectId, reviewId, reviewRaw.SourceArtifactId, new DateTime?(updateTimestamps.Prior), new DateTime?(updateTimestamps.Current), ResourceType.Iteration, iterationId), this.Area, this.Layer);
      }), nameof (PatchProperties));
      return patchedProperties;
    }

    private int GetSaveIterationTimeoutSeconds(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/CodeReview/Settings/SaveIterationCommandTimeoutSeconds", true, InternalDatabaseProperties.DefaultDatabaseRequestTimeout);

    private void ValidateReviewIterationIds(int reviewId, int? iterationId)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      if (!iterationId.HasValue)
        return;
      ArgumentUtility.CheckForOutOfRange(iterationId.Value, "id", 1);
    }

    private bool CanDelete(IVssRequestContext requestContext, Guid projectId, int reviewId) => requestContext.GetService<ICodeReviewService>().IsCurrentUserReviewAuthor(requestContext, projectId, reviewId);

    private ContentAccessMetadata GetChangeEntryAccessInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId,
      int changeId,
      ChangeEntryFileType fileType)
    {
      return this.ExecuteAndTrace<ContentAccessMetadata>(requestContext, 1380611, 1380612, 1380613, (Func<ContentAccessMetadata>) (() =>
      {
        requestContext.Trace(1380614, TraceLevel.Verbose, this.Area, this.Layer, "Getting changelist: review id: '{0}', iteration id: '{1}', project id: '{2}', change id: '{3}', fileType: '{4}'", (object) reviewId, (object) iterationId, (object) projectId, (object) changeId, (object) fileType);
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          return component.ReadChangeEntryAccessMetadata(projectId, reviewId, iterationId, changeId, fileType);
      }), nameof (GetChangeEntryAccessInfo));
    }

    private void TraceGetIterationFilesContent(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      ChangeEntryFileType? fileType,
      int? top = null,
      int? skip = null)
    {
      if (!requestContext.IsTracing(1380634, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (int iterationId in iterationIds)
        stringBuilder.AppendFormat("{0},", (object) iterationId);
      requestContext.Trace(1380634, TraceLevel.Verbose, this.Area, this.Layer, "Getting iteration files content: review id: '{0}', iteration ids: '{1}', project id: '{2}', fileType: '{3}', top: '{4}', skip: '{5}'", (object) reviewId, (object) stringBuilder.ToString().TrimEnd(','), (object) projectId, (object) fileType, (object) top, (object) skip);
    }

    public virtual int GetMaxChangeEntriesForChangeTrackingComputation(
      IVssRequestContext requestContext)
    {
      return 2000;
    }
  }
}
