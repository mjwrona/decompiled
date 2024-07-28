// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.PackagingCiData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public abstract class PackagingCiData : ICiData
  {
    protected PackagingCiData(
      IVssRequestContext requestContext,
      string featureName,
      IProtocol protocol,
      FeedCore feed)
      : this(featureName)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidRequestContextHostException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CiDataExpectingSpecificHostType((object) TeamFoundationHostType.ProjectCollection));
      CustomerIntelligenceData ciData1 = this.CiData;
      Guid guid = requestContext.ServiceHost.InstanceId;
      string str1 = guid.ToString();
      ciData1.Add("CollectionId", str1);
      this.CiData.Add("CollectionName", requestContext.ServiceHost.Name);
      CustomerIntelligenceData ciData2 = this.CiData;
      guid = requestContext.ServiceHost.ParentServiceHost.InstanceId;
      string str2 = guid.ToString();
      ciData2.Add("OrganizationId", str2);
      this.CiData.Add("OrganizationName", requestContext.ServiceHost.ParentServiceHost.Name);
      this.CiData.Add("Protocol", protocol.ToString());
      CustomerIntelligenceData ciData3 = this.CiData;
      guid = feed.Id;
      string str3 = guid.ToString();
      ciData3.Add("FeedId", str3);
      this.CiData.Add("FeedName", feed.Name);
      CustomerIntelligenceData ciData4 = this.CiData;
      guid = requestContext.ActivityId;
      string str4 = guid.ToString();
      ciData4.Add("ActivityId", str4);
      this.CiData.Add("UserAgent", requestContext.UserAgent);
      CustomerIntelligenceData ciData5 = this.CiData;
      Guid? viewId = feed.ViewId;
      ref Guid? local = ref viewId;
      string str5;
      if (!local.HasValue)
      {
        str5 = (string) null;
      }
      else
      {
        guid = local.GetValueOrDefault();
        str5 = guid.ToString();
      }
      ciData5.Add("ViewId", str5);
    }

    private PackagingCiData(string featureName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(featureName, nameof (featureName));
      this.AreaName = "Packaging";
      this.FeatureName = featureName;
    }

    public string AreaName { get; }

    public string FeatureName { get; }

    public CustomerIntelligenceData CiData { get; } = new CustomerIntelligenceData();
  }
}
