// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRegistryConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public static class FeedRegistryConstants
  {
    public const string FeedMruPath = "Packaging/MruFeedId";
    public const string MaxUpstreamSourceCount = "/Configuration/Feed/MaxUpstreamSourceCount";
    public const string FeedServiceWarmUpLookback = "/Configuration/Feed/FeedServiceWarmUpLookback";
    public const string PackagesProtocolBackgroundColor = "/Configuration/Feed/Badges/Packages/ProtocolBackgroundColor";
    public const string PackagesVersionBackgroundColor = "/Configuration/Feed/Badges/Packages/VersionBackgroundColor";
    public const string SpecialFeedMessagePath = "/Configuration/Feed/FeedMessage/{0}";

    public static RegistryQuery PerFeedMaxUpstreamSourceCount(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => (RegistryQuery) string.Format("/Configuration/Feed/MaxUpstreamSourceCount/{0}", (object) feed.Id);
  }
}
