// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Social.WebApi.SocialEngagementRestConstants
// Assembly: Microsoft.VisualStudio.Services.Social.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D2A928F-A131-41A8-A9E6-C3C26BFE105A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Social.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SocialEngagementRestConstants
  {
    public const string AreaId = "A71D8DD9-C94D-458E-B1D2-4B595E38C9C4";
    public const string Area = "Social";

    public static class SocialEngagementResource
    {
      public const string Name = "SocialEngagement";
      public const int Version = 1;
      public const string RouteName = "SocialEngagement";
      public const string RouteTemplate = "{area}/{resource}";
      public const string LocationString = "99A61482-7000-4AF0-9D84-DAEACBEA71D1";
      public static Guid LocationId = new Guid("99A61482-7000-4AF0-9D84-DAEACBEA71D1");
    }

    public static class SocialEngagementProvidersResource
    {
      public const string Name = "SocialEngagementProviders";
      public const int Version = 1;
      public const string RouteName = "SocialEngagementProviders";
      public const string RouteTemplate = "{area}/{resource}";
      public static Guid LocationId = new Guid("7DC56847-4EFE-4461-BD12-6C2F31E8144D");
    }

    public static class SocialEngagementUsersResource
    {
      public const string Name = "SocialEngagementUsers";
      public const int Version = 1;
      public const string RouteName = "SocialEngagementUsers";
      public const string RouteTemplate = "{area}/{resource}";
      public static Guid LocationId = new Guid("358536C5-2742-4C3E-9301-B46945BECD73");
    }

    public static class SocialEngagementAggregateMetricResource
    {
      public const string Name = "SocialEngagementAggregateMetric";
      public const int Version = 1;
      public const string RouteName = "SocialEngagementAggregateMetric";
      public const string RouteTemplate = "{area}/{resource}";
      public static Guid LocationId = new Guid("B38448B8-44EC-4470-8328-08FE78EFE297");
    }
  }
}
