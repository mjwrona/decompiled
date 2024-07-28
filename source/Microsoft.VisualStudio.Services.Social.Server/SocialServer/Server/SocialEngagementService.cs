// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialEngagementService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Social.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.SocialEngagement.Server;
using Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class SocialEngagementService : ISocialEngService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<IdentityRef> GetEngagedUsers(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      int top,
      int skip)
    {
      IList<Guid> engagementRecord;
      using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
        engagementRecord = component.GetUsersForSocialEngagementRecord(socialEngagementCreateParameter, top, skip);
      IVssRequestContext elevatedRequestContext = requestContext.Elevate();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = elevatedRequestContext.GetService<IdentityService>().ReadIdentities(elevatedRequestContext, (IList<Guid>) engagementRecord.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      return source == null ? (IEnumerable<IdentityRef>) null : source.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (x => x == null ? (IdentityRef) null : x.ToIdentityRef(elevatedRequestContext, false)));
    }

    public SocialEngagementRecord CreateSocialEngagementRecord(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(socialEngagementCreateParameter.ArtifactType, "ArtifactType");
      ISocialSdkSocialEngagementProvider artifactAndEngagement = this.GetSocialEngagementProvidersInternalForArtifactAndEngagement(requestContext, socialEngagementCreateParameter.ArtifactType, socialEngagementCreateParameter.EngagementType);
      ISecuredObject securedObject = (ISecuredObject) null;
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "SocialEngagement", "ValidateArtifactId"))
      {
        timedCiEvent["ArtifactType"] = (object) socialEngagementCreateParameter.ArtifactType;
        timedCiEvent["EngagementType"] = (object) socialEngagementCreateParameter.EngagementType;
        artifactAndEngagement.ValidateArtifactId(requestContext, socialEngagementCreateParameter.ArtifactId, socialEngagementCreateParameter.ArtifactScope, out securedObject);
      }
      this.PublishActionCi(requestContext, socialEngagementCreateParameter, nameof (CreateSocialEngagementRecord));
      using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
      {
        SocialEngagementRecord engagementRecord = component.CreateSocialEngagementRecord(socialEngagementCreateParameter, requestContext.GetUserId(), artifactAndEngagement.EnableMetricsAggregation()) ?? new SocialEngagementRecord();
        engagementRecord.ArtifactScope = socialEngagementCreateParameter.ArtifactScope;
        if (securedObject != null)
          engagementRecord.SetSecuredObject(securedObject);
        return engagementRecord;
      }
    }

    public SocialEngagementRecord GetSocialEngagementRecord(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter)
    {
      using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
      {
        SocialEngagementRecord engagementRecord = component.GetSocialEngagementRecord(socialEngagementCreateParameter, requestContext.GetUserId()) ?? new SocialEngagementRecord();
        engagementRecord.ArtifactScope = socialEngagementCreateParameter.ArtifactScope;
        return engagementRecord;
      }
    }

    public IEnumerable<SocialEngagementRecord> GetSocialEngagementRecords(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      Guid ownerId,
      string artifactType,
      ISet<string> artifactIds,
      IEnumerable<SocialEngagementType> socialEngagementTypes)
    {
      return requestContext.TraceBlock<IEnumerable<SocialEngagementRecord>>(11000004, 11000005, "SocialEngagement", "Service", nameof (GetSocialEngagementRecords), (Func<IEnumerable<SocialEngagementRecord>>) (() =>
      {
        int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialEngagementRecordsMaxArtifactIdCount", 10000);
        int count = artifactIds.Count;
        ISet<string> artifactIds1 = artifactIds;
        if (count > registryValue)
        {
          requestContext.Trace(11000006, TraceLevel.Error, "SocialEngagement", "Service", string.Format("Number of artifactIds: {0} passed to {1} exceeds the maximum limit: {2}", (object) count, (object) nameof (GetSocialEngagementRecords), (object) registryValue));
          artifactIds1 = (ISet<string>) artifactIds.Take<string>(registryValue).ToHashSet<string>();
        }
        using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
          return component.GetSocialEngagementRecords(artifactScope, ownerId, artifactType, artifactIds1, socialEngagementTypes);
      }));
    }

    public List<KeyValuePair<SocialEngagementType, string>> GetSocialEngagementProviders(
      IVssRequestContext requestContext)
    {
      List<KeyValuePair<SocialEngagementType, string>> engagementProviders = new List<KeyValuePair<SocialEngagementType, string>>();
      foreach (ISocialSdkSocialEngagementProvider engagementProvider in this.GetSocialSdkSocialEngagementProviders(requestContext))
        engagementProviders.AddRange((IEnumerable<KeyValuePair<SocialEngagementType, string>>) engagementProvider.GetSupportedSocialEngagement(requestContext));
      return engagementProviders;
    }

    public SocialEngagementAggregateMetric GetSocialEngagementAggregateMetric(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter)
    {
      using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
      {
        SocialEngagementAggregateMetric engagementAggregateMetric = component.GetGetSocialEngagementAggregateMetric(socialEngagementCreateParameter);
        engagementAggregateMetric.ArtifactType = socialEngagementCreateParameter.ArtifactType;
        engagementAggregateMetric.ArtifactId = socialEngagementCreateParameter.ArtifactId;
        engagementAggregateMetric.ArtifactScope = socialEngagementCreateParameter.ArtifactScope;
        engagementAggregateMetric.EngagementType = socialEngagementCreateParameter.EngagementType;
        return engagementAggregateMetric;
      }
    }

    public int DeleteOldAggregatedSocialEngagementMetrics(IVssRequestContext requestContext)
    {
      int num = 0;
      int maxHoursToRetain = this.GetMaxHoursToRetain(requestContext);
      foreach (ISocialSdkSocialEngagementProvider engagementProvider in this.GetSocialSdkSocialEngagementProviders(requestContext))
      {
        using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
        {
          IDictionary<SocialEngagementType, string> socialEngagement = engagementProvider.GetSupportedSocialEngagement(requestContext);
          int retainAggregation = engagementProvider.HoursToRetainAggregation();
          int hoursToRetain = retainAggregation > maxHoursToRetain ? maxHoursToRetain : retainAggregation;
          foreach (KeyValuePair<SocialEngagementType, string> keyValuePair in (IEnumerable<KeyValuePair<SocialEngagementType, string>>) socialEngagement)
            num += component.DeleteOldAggregatedSocialEngagementMetrics(hoursToRetain, keyValuePair.Key, keyValuePair.Value);
        }
      }
      return num;
    }

    public SocialEngagementRecord DeleteSocialEngagementRecord(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter)
    {
      ISocialSdkSocialEngagementProvider artifactAndEngagement = this.GetSocialEngagementProvidersInternalForArtifactAndEngagement(requestContext, socialEngagementCreateParameter.ArtifactType, socialEngagementCreateParameter.EngagementType);
      AggregationType aggregationType = artifactAndEngagement.EnableMetricsAggregation();
      ISecuredObject securedObject = (ISecuredObject) null;
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "SocialEngagement", "ValidateArtifactId"))
      {
        timedCiEvent["ArtifactType"] = (object) socialEngagementCreateParameter.ArtifactType;
        timedCiEvent["EngagementType"] = (object) socialEngagementCreateParameter.EngagementType;
        artifactAndEngagement.ValidateArtifactId(requestContext, socialEngagementCreateParameter.ArtifactId, socialEngagementCreateParameter.ArtifactScope, out securedObject);
      }
      this.PublishActionCi(requestContext, socialEngagementCreateParameter, nameof (DeleteSocialEngagementRecord));
      using (SocialEngagementSdkSqlResourceComponent component = requestContext.CreateComponent<SocialEngagementSdkSqlResourceComponent>())
      {
        SocialEngagementRecord engagementRecord = component.DeleteSocialEngagement(socialEngagementCreateParameter, requestContext.GetUserId(), aggregationType) ?? new SocialEngagementRecord();
        if (securedObject != null)
          engagementRecord.SetSecuredObject(securedObject);
        return engagementRecord;
      }
    }

    private int GetMaxHoursToRetain(IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialEngagement/SocialAggregateMetricsMaximumHoursToRetain", 720);

    private void PublishActionCi(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      string actionName)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ArtifactType", socialEngagementCreateParameter.ArtifactType);
      intelligenceData.Add("ArtifactId", socialEngagementCreateParameter.ArtifactId);
      intelligenceData.Add("EngagementType", (object) socialEngagementCreateParameter.EngagementType);
      intelligenceData.Add("UserId", (object) requestContext.GetUserId());
      intelligenceData.Add("ActionName", actionName);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "SocialEngagement", "UserAction", properties);
    }

    private ISocialSdkSocialEngagementProvider GetSocialEngagementProvidersInternalForArtifactAndEngagement(
      IVssRequestContext requestContext,
      string artifactType,
      SocialEngagementType engagementType)
    {
      ISocialSdkSocialEngagementProvider artifactAndEngagement = this.GetSocialSdkSocialEngagementProviders(requestContext).FirstOrDefault<ISocialSdkSocialEngagementProvider>((Func<ISocialSdkSocialEngagementProvider, bool>) (x => x.GetSupportedSocialEngagement(requestContext).ContainsKey(engagementType) && x.GetSupportedSocialEngagement(requestContext)[engagementType].Equals(artifactType)));
      if (artifactAndEngagement != null)
        return artifactAndEngagement;
      string str = string.Join(" , ", this.GetSocialSdkSocialEngagementProviders(requestContext).SelectMany<ISocialSdkSocialEngagementProvider, string>((Func<ISocialSdkSocialEngagementProvider, IEnumerable<string>>) (x => (IEnumerable<string>) x.GetSupportedSocialEngagement(requestContext).Values)).Distinct<string>());
      throw new InvalidArtifactTypeException(SocialEngagementServerResource.InvalidArtifactTypeExceptionMessage((object) artifactType, (object) str));
    }

    private List<ISocialSdkSocialEngagementProvider> GetSocialSdkSocialEngagementProviders(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<SocialSdkEngagementProviderService>().GetSocialSdkSocialEngagementProviders(vssRequestContext);
    }
  }
}
