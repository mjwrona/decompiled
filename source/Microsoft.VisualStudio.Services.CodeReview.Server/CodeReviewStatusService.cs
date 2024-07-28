// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewStatusService
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
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewStatusService : 
    CodeReviewServiceBase,
    ICodeReviewStatusService,
    IVssFrameworkService
  {
    public Status GetStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int statusId,
      int? iterationId = null)
    {
      Status status = (Status) null;
      this.ExecuteAndTrace(requestContext, nameof (GetStatus), 1383211, 1383212, 1383212, (Action) (() =>
      {
        requestContext.Trace(1383214, TraceLevel.Verbose, this.Area, this.Layer, "Getting a status: review id: '{0}', status id: '{1}', iteration id: '{2}', project id: '{3}'", (object) reviewId, (object) statusId, (object) iterationId, (object) projectId);
        status = this.GetStatusesInternal(requestContext, projectId, (IEnumerable<int>) new int[1]
        {
          reviewId
        }, iterationId, new int?(statusId), true).FirstOrDefault<Status>();
        if (status == null)
          throw new CodeReviewStatusNotFoundException(statusId, reviewId, iterationId);
      }));
      return status;
    }

    public IEnumerable<Status> GetStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId = null,
      bool includeProperties = false)
    {
      IEnumerable<Status> statuses = (IEnumerable<Status>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetStatuses), 1383221, 1383222, 1383223, (Action) (() =>
      {
        requestContext.Trace(1383224, TraceLevel.Verbose, this.Area, this.Layer, "Getting statuses: review id: '{0}', iteration id: '{1}', project id: '{2}'", (object) reviewId, (object) iterationId, (object) projectId);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int[] reviewIds = new int[1]{ reviewId };
        int? iterationId1 = iterationId;
        bool flag = includeProperties;
        int? statusId = new int?();
        int num = flag ? 1 : 0;
        statuses = this.GetStatusesInternal(requestContext1, projectId1, (IEnumerable<int>) reviewIds, iterationId1, statusId, num != 0);
      }));
      return statuses;
    }

    public ILookup<int, Status> GetStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds)
    {
      IEnumerable<Status> statuses = (IEnumerable<Status>) null;
      this.ExecuteAndTrace(requestContext, "GetStatusesForMultipleReviews", 1383231, 1383232, 1383233, (Action) (() =>
      {
        this.TraceGetStatusesForMultipleReviewsInfo(requestContext, projectId, reviewIds);
        statuses = this.GetStatusesInternal(requestContext, projectId, reviewIds);
      }));
      return statuses.ToLookup<Status, int>((Func<Status, int>) (s => s.ReviewId));
    }

    public IEnumerable<StatusContext> GetLatestStatusContextsBySourceArtifactPrefix(
      IVssRequestContext requestContext,
      Guid projectId,
      string reviewArtifactPrefix)
    {
      IEnumerable<StatusContext> statusContexts = (IEnumerable<StatusContext>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetLatestStatusContextsBySourceArtifactPrefix), 1383241, 1383242, 1383243, (Action) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        int getLatestStatuses1 = this.GetTopStatusesForGetLatestStatuses(requestContext);
        int getLatestStatuses2 = this.GetTopReviewsForGetLatestStatuses(requestContext);
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          statusContexts = component.GetLatestStatusContextsBySourceArtifactPrefix(projectId, reviewArtifactPrefix, getLatestStatuses1, getLatestStatuses2);
      }));
      return statusContexts;
    }

    public IEnumerable<Status> GetLatestStatusesByReviewId(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int top = 500)
    {
      IList<Status> statuses = (IList<Status>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetLatestStatusesByReviewId), 1383261, 1383262, 1383263, (Action) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          statuses = component.GetLatestStatusesByReviewId(projectId, reviewId, top);
        this.AddStatusReferenceLinks(requestContext, projectId, (IEnumerable<Status>) statuses);
        statuses = IdentityHelper.FillAuthorIdentities(requestContext, statuses);
      }));
      return (IEnumerable<Status>) statuses;
    }

    protected IEnumerable<Status> GetStatusesInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds,
      int? iterationId = null,
      int? statusId = null,
      bool includeProperties = false)
    {
      IList<Status> statuses = (IList<Status>) null;
      this.ValidateReviewStatusIterationIds(reviewIds, statusId, iterationId);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      this.FilterReviews(requestContext, projectId, reviewIds);
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        statuses = component.GetStatuses(projectId, reviewIds, iterationId, statusId);
      if (includeProperties)
        this.PopulateStatusProperties(requestContext, (IEnumerable<Status>) statuses);
      this.AddStatusReferenceLinks(requestContext, projectId, (IEnumerable<Status>) statuses);
      return (IEnumerable<Status>) IdentityHelper.FillAuthorIdentities(requestContext, statuses);
    }

    public Status SaveStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Status status,
      int? iterationId = null)
    {
      Status savedStatus = (Status) null;
      this.ExecuteAndTrace(requestContext, nameof (SaveStatus), 1383201, 1383202, 1383203, (Action) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        CodeReviewRequestContextCacheUtil.InvalidateCachedStatuses(requestContext, projectId, reviewId, iterationId);
        bool updateStatus = status.Id > 0;
        this.ValidateStatus(status, updateStatus, iterationId);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        if (updateStatus)
          savedStatus = this.UpdateStatus(requestContext, projectId, reviewId, status, iterationId);
        else
          savedStatus = this.CreateStatus(requestContext, projectId, reviewId, status, iterationId);
      }));
      return savedStatus;
    }

    private Status CreateStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Status status,
      int? iterationId = null)
    {
      Status status1 = (Status) null;
      int? iterationId1 = iterationId;
      if (!iterationId1.HasValue)
        iterationId1 = status.IterationId;
      requestContext.Trace(1383204, TraceLevel.Verbose, this.Area, this.Layer, "Creating status: review id: '{0}', iteration id: '{1}', project id: '{2}', displayName: '{3}'", (object) reviewId, (object) iterationId1, (object) projectId, (object) status.Context?.Name);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckWriteReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
      status.Author = IdentityHelper.GetRequesterIdentityRef(requestContext);
      StatusAddedNotification crEvent1 = new StatusAddedNotification(projectId, reviewRaw, iterationId1, status, reviewRaw.UpdatedDate);
      EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) crEvent1);
      this.SaveStatusProperties(requestContext, status);
      int maxStatusCount = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/Statuses/MaxCodeReviewStatusCount", true, 10000);
      DateTime? priorReviewUpdatedTimestamp;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        status1 = component.SaveStatus(projectId, reviewId, iterationId1, status, false, maxStatusCount, out priorReviewUpdatedTimestamp);
      this.PopulateStatusProperties(requestContext, (IEnumerable<Status>) new Status[1]
      {
        status1
      });
      Review review = reviewRaw.ShallowClone();
      review.UpdatedDate = status1.UpdatedDate;
      StatusAddedNotification crEvent2 = new StatusAddedNotification(projectId, review, status1.IterationId, status1, priorReviewUpdatedTimestamp);
      EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) crEvent2, this.Area, this.Layer);
      status1.AddReferenceLinks(requestContext, projectId, reviewId, status1.Id, status1.IterationId);
      return IdentityHelper.FillAuthorIdentities(requestContext, (IList<Status>) new Status[1]
      {
        status1
      }).Single<Status>();
    }

    private Status UpdateStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Status status,
      int? iterationId = null)
    {
      requestContext.Trace(1383205, TraceLevel.Verbose, this.Area, this.Layer, "Updating status: review id: '{0}', iteration id: '{1}', project id: '{2}', status id: '{3}'", (object) reviewId, (object) iterationId, (object) projectId, (object) status.Id);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckWriteReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
      status.Author = IdentityHelper.GetRequesterIdentityRef(requestContext);
      IEnumerable<Status> statuses;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        statuses = (IEnumerable<Status>) component.GetStatuses(projectId, (IEnumerable<int>) new int[1]
        {
          reviewId
        }, iterationId, new int?(status.Id));
      Status status1 = statuses.FirstOrDefault<Status>();
      if (status1 == null)
        throw new CodeReviewStatusNotFoundException(status.Id, reviewId, iterationId);
      Status status2 = status.ShallowClone();
      status2.Context = status1.Context;
      StatusUpdatedNotification crEvent1 = new StatusUpdatedNotification(projectId, reviewRaw, iterationId, status2, reviewRaw.UpdatedDate);
      EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) crEvent1);
      DateTime? priorReviewUpdatedTimestamp;
      Status status3;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        status3 = component.SaveStatus(projectId, reviewId, iterationId, status, true, 0, out priorReviewUpdatedTimestamp);
      this.UpdateStatusProperties(requestContext, status3.PropertyId, status.Properties);
      this.PopulateStatusProperties(requestContext, (IEnumerable<Status>) new Status[1]
      {
        status3
      });
      Review review = reviewRaw.ShallowClone();
      review.UpdatedDate = status3.UpdatedDate;
      StatusUpdatedNotification crEvent2 = new StatusUpdatedNotification(projectId, review, status3.IterationId, status3, priorReviewUpdatedTimestamp);
      EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) crEvent2, this.Area, this.Layer);
      status3.AddReferenceLinks(requestContext, projectId, reviewId, status3.Id, status3.IterationId);
      return IdentityHelper.FillAuthorIdentities(requestContext, (IList<Status>) new Status[1]
      {
        status3
      }).Single<Status>();
    }

    public void DeleteStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<int> statusIds,
      int? iterationId = null)
    {
      this.ExecuteAndTrace(requestContext, nameof (DeleteStatuses), 1383251, 1383252, 1383253, (Action) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        requestContext.Trace(1383254, TraceLevel.Verbose, this.Area, this.Layer, "Deleting status: review id: '{0}', iteration id: '{1}', status ids: {2}", (object) reviewId, (object) iterationId, (object) projectId, (object) string.Join<int>(", ", statusIds));
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckWriteReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
        DateTime priorReviewUpdatedTimestamp;
        DateTime lastUpdatedTimestamp;
        List<Status> source;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          source = component.DeleteStatuses(projectId, reviewId, iterationId, statusIds, out priorReviewUpdatedTimestamp, out lastUpdatedTimestamp);
        try
        {
          this.DeleteStatusProperties(requestContext, (IEnumerable<Status>) source);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1383255, TraceLevel.Error, this.Area, this.Layer, "Exception occured while deleting status properties. Excpetion: {0}, review id: '{1}', iteration id: '{2}', property ids: '{3}'", (object) ex, (object) reviewId, (object) iterationId, (object) string.Join(",", source.Select<Status, string>((Func<Status, string>) (s =>
          {
            long? propertyId = s.PropertyId;
            ref long? local = ref propertyId;
            return !local.HasValue ? (string) null : local.GetValueOrDefault().ToString();
          }))));
        }
        Review review = reviewRaw.ShallowClone();
        review.UpdatedDate = new DateTime?(lastUpdatedTimestamp);
        EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) new StatusesDeletedNotification(projectId, review, iterationId, statusIds, priorReviewUpdatedTimestamp, lastUpdatedTimestamp), this.Area, this.Layer);
      }));
    }

    private void ValidateReviewStatusIterationIds(
      IEnumerable<int> reviewIds,
      int? statusId,
      int? iterationId)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) reviewIds, nameof (reviewIds));
      foreach (int reviewId in reviewIds)
        ArgumentUtility.CheckForOutOfRange(reviewId, "reviewId", 1);
      if (reviewIds.Count<int>() == 1)
      {
        if (iterationId.HasValue)
          ArgumentUtility.CheckForOutOfRange(iterationId.Value, nameof (iterationId), 1);
        if (!statusId.HasValue)
          return;
        ArgumentUtility.CheckForOutOfRange(statusId.Value, "id", 1);
      }
      else
      {
        ArgumentUtility.EnsureIsNull((object) iterationId, nameof (iterationId));
        ArgumentUtility.EnsureIsNull((object) statusId, "id");
      }
    }

    private void ValidateStatus(Status status, bool updateStatus, int? iterationId)
    {
      if (status.CreatedDate.HasValue)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyCreatedDate(), "createdDate");
      if (status.UpdatedDate.HasValue)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyUpdatedDate(), "updatedDate");
      if (iterationId.HasValue)
        ArgumentUtility.CheckForOutOfRange(iterationId.Value, nameof (iterationId), 1);
      if (!string.IsNullOrEmpty(status.Context?.Name))
      {
        ArgumentUtility.CheckStringLength(status.Context.Name, "Name", 128);
        ArgumentUtility.CheckStringForInvalidCharacters(status.Context.Name, "Name");
      }
      if (!string.IsNullOrEmpty(status.Context?.Genre))
      {
        ArgumentUtility.CheckStringLength(status.Context.Genre, "Genre", 128);
        ArgumentUtility.CheckStringForInvalidCharacters(status.Context.Genre, "Genre");
      }
      if (!string.IsNullOrEmpty(status.TargetUrl))
      {
        ArgumentUtility.CheckStringLength(status.TargetUrl, "TargetUrl", 2048);
        ArgumentUtility.CheckStringForInvalidCharacters(status.TargetUrl, "TargetUrl");
        ArgumentUtility.CheckIsValidURI(status.TargetUrl, UriKind.RelativeOrAbsolute, "TargetUrl");
      }
      if (!string.IsNullOrEmpty(status.Description))
      {
        ArgumentUtility.CheckStringLength(status.Description, "Description", 2048);
        ArgumentUtility.CheckStringForInvalidCharacters(status.Description, "Description");
      }
      if (updateStatus)
      {
        if (status.Context?.Name != null)
          throw new ArgumentException(CodeReviewResources.StatusNameCannotBeUpdated(), "name");
        if (status.Context?.Genre != null)
          throw new ArgumentException(CodeReviewResources.StatusGenreCannotBeUpdated(), "genre");
      }
      else
      {
        ArgumentUtility.CheckStringForNullOrEmpty(status.Context?.Name, "Name");
        int? iterationId1 = status.IterationId;
        if (iterationId1.HasValue)
        {
          iterationId1 = status.IterationId;
          ArgumentUtility.CheckForOutOfRange(iterationId1.Value, nameof (iterationId), 1);
        }
        if (!iterationId.HasValue)
          return;
        iterationId1 = status.IterationId;
        if (!iterationId1.HasValue)
          return;
        int num1 = iterationId.Value;
        iterationId1 = status.IterationId;
        int num2 = iterationId1.Value;
        if (num1 != num2)
          throw new ArgumentException(CodeReviewResources.StatusIterationIdNotConsistent());
      }
    }

    private ArtifactSpec GetStatusPropertiesArtifactSpec(long propertyId)
    {
      byte[] bytes = BitConverter.GetBytes(propertyId);
      if (BitConverter.IsLittleEndian)
        Array.Reverse((Array) bytes);
      return new ArtifactSpec(ServerConstants.ReviewStatusPropertyKind, bytes, 0);
    }

    private long GetStatusPropertyId(ArtifactSpec artifactSpec)
    {
      byte[] destinationArray = new byte[artifactSpec.Id.Length];
      Array.Copy((Array) artifactSpec.Id, (Array) destinationArray, destinationArray.Length);
      if (BitConverter.IsLittleEndian)
        Array.Reverse((Array) destinationArray);
      return BitConverter.ToInt64(destinationArray, 0);
    }

    private void SaveStatusProperties(IVssRequestContext requestContext, Status status)
    {
      ITeamFoundationCounterService service1 = requestContext.GetService<ITeamFoundationCounterService>();
      if (service1.CounterExists(requestContext, "CodeReviewStatusPropertyId"))
        status.PropertyId = new long?(service1.ReserveCounterIds(requestContext, "CodeReviewStatusPropertyId", 1L));
      long? propertyId = status.PropertyId;
      if (!propertyId.HasValue || status.Properties == null || status.Properties.Count <= 0)
        return;
      ITeamFoundationPropertyService service2 = requestContext.GetService<ITeamFoundationPropertyService>();
      if (!service2.GetArtifactKinds(requestContext).Any<ArtifactKind>((Func<ArtifactKind, bool>) (a => a.Kind == ServerConstants.ReviewStatusPropertyKind)))
        return;
      PropertyValue[] array = status.Properties.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (p => new PropertyValue(p.Key, p.Value))).ToArray<PropertyValue>();
      propertyId = status.PropertyId;
      ArtifactSpec propertiesArtifactSpec = this.GetStatusPropertiesArtifactSpec(propertyId.Value);
      service2.SetProperties(requestContext, propertiesArtifactSpec, (IEnumerable<PropertyValue>) array);
    }

    private void UpdateStatusProperties(
      IVssRequestContext requestContext,
      long? statusPropertyId,
      PropertiesCollection updatedProperties)
    {
      if (!statusPropertyId.HasValue)
        return;
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      if (!service.GetArtifactKinds(requestContext).Any<ArtifactKind>((Func<ArtifactKind, bool>) (a => a.Kind == ServerConstants.ReviewStatusPropertyKind)))
        return;
      PropertyValue[] propertyValueArray1;
      if (updatedProperties == null)
      {
        propertyValueArray1 = (PropertyValue[]) null;
      }
      else
      {
        IEnumerable<PropertyValue> source = updatedProperties.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (p => new PropertyValue(p.Key, p.Value)));
        propertyValueArray1 = source != null ? source.ToArray<PropertyValue>() : (PropertyValue[]) null;
      }
      PropertyValue[] propertyValueArray2 = propertyValueArray1;
      if (propertyValueArray2 == null)
        return;
      ArtifactSpec propertiesArtifactSpec = this.GetStatusPropertiesArtifactSpec(statusPropertyId.Value);
      service.SetProperties(requestContext, propertiesArtifactSpec, (IEnumerable<PropertyValue>) propertyValueArray2);
    }

    private void DeleteStatusProperties(
      IVssRequestContext requestContext,
      IEnumerable<Status> statuses)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      if (!service.GetArtifactKinds(requestContext).Any<ArtifactKind>((Func<ArtifactKind, bool>) (a => a.Kind == ServerConstants.ReviewStatusPropertyKind)))
        return;
      ArtifactSpec[] array = statuses.Where<Status>((Func<Status, bool>) (s => s.PropertyId.HasValue)).Select<Status, ArtifactSpec>((Func<Status, ArtifactSpec>) (status => this.GetStatusPropertiesArtifactSpec(status.PropertyId.Value))).ToArray<ArtifactSpec>();
      service.DeleteArtifacts(requestContext, (IEnumerable<ArtifactSpec>) array);
    }

    private void PopulateStatusProperties(
      IVssRequestContext requestContext,
      IEnumerable<Status> statuses)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      if (service.GetArtifactKinds(requestContext).Any<ArtifactKind>((Func<ArtifactKind, bool>) (a => a.Kind == ServerConstants.ReviewStatusPropertyKind)))
      {
        Status[] array1 = statuses.Where<Status>((Func<Status, bool>) (s => s.PropertyId.HasValue)).ToArray<Status>();
        Dictionary<long, Status> dictionary1 = ((IEnumerable<Status>) array1).ToDictionary<Status, long, Status>((Func<Status, long>) (s => s.PropertyId.Value), (Func<Status, Status>) (s => s));
        ArtifactSpec[] array2 = ((IEnumerable<Status>) array1).Select<Status, ArtifactSpec>((Func<Status, ArtifactSpec>) (s => this.GetStatusPropertiesArtifactSpec(s.PropertyId.Value))).ToArray<ArtifactSpec>();
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) array2, (IEnumerable<string>) null))
        {
          foreach (ArtifactPropertyValue artifactPropertyValue in properties)
          {
            long statusPropertyId = this.GetStatusPropertyId(artifactPropertyValue.Spec);
            Status status = dictionary1[statusPropertyId];
            StreamingCollection<PropertyValue> propertyValues = artifactPropertyValue.PropertyValues;
            Dictionary<string, object> dictionary2 = propertyValues != null ? propertyValues.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (pv => pv.PropertyName), (Func<PropertyValue, object>) (pv => pv.Value)) : (Dictionary<string, object>) null;
            if (dictionary2 != null)
              status.Properties = new PropertiesCollection((IDictionary<string, object>) dictionary2);
          }
        }
      }
      else
        requestContext.TraceAlways(1383225, TraceLevel.Info, this.Area, this.Layer, "ReviewStatusPropertyKind doesn't exist.");
    }

    private IEnumerable<int> FilterReviews(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds,
      int? top = null,
      int? skip = null)
    {
      int num = reviewIds.Count<int>();
      int codeReviewId = reviewIds.First<int>();
      List<Review> source = new List<Review>();
      int topValue;
      int skipValue;
      ValidationHelper.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      foreach (int reviewId in reviewIds)
      {
        Review cachedCodeReview = CodeReviewRequestContextCacheUtil.GetCachedCodeReview(requestContext, projectId, reviewId, ReviewScope.ReviewLevelOnly);
        if (cachedCodeReview != null)
          source.Add(cachedCodeReview);
      }
      if (source.Count<Review>() < num)
      {
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          source = component.GetReviews(projectId, reviewIds, topValue, skipValue, false, new DateTime?(), new int?(), ReviewScope.ReviewLevelOnly).ToList<Review>();
      }
      List<Review> list = source.Where<Review>((Func<Review, bool>) (review => ReviewSecurityEvaluator.HasReviewAccess(requestContext, this.SecurityExtensions, projectId, review.Id, review.SourceArtifactId))).ToList<Review>();
      foreach (Review review in list)
        CodeReviewRequestContextCacheUtil.CacheCodeReview(requestContext, review, ReviewScope.ReviewLevelOnly);
      if (num == 1 && list.Count<Review>() == 0)
        throw new CodeReviewNotFoundException(codeReviewId);
      return list.Select<Review, int>((Func<Review, int>) (review => review.Id));
    }

    private void AddStatusReferenceLinks(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Status> statuses)
    {
      if (statuses == null || !statuses.Any<Status>())
        return;
      foreach (Status statuse in statuses)
        statuse.AddReferenceLinks(requestContext, projectId, statuse.ReviewId, statuse.Id, statuse.IterationId);
    }

    private void TraceGetStatusesForMultipleReviewsInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds)
    {
      if (!requestContext.IsTracing(1383234, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (reviewIds != null)
      {
        foreach (int reviewId in reviewIds)
          stringBuilder.Append(string.Format("'{0}',", (object) reviewId));
      }
      requestContext.Trace(1383234, TraceLevel.Verbose, this.Area, this.Layer, "Getting statuses for a list of code reviews: ids: '{0}', project id: '{1}'", (object) stringBuilder.ToString(), (object) projectId);
    }

    private int GetTopReviewsForGetLatestStatuses(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/Statuses/TopReviewsForGetLatest", true, 1000);

    private int GetTopStatusesForGetLatestStatuses(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/Statuses/TopStatusesForGetLatest", true, 2000);
  }
}
