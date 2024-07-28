// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.DeliveryTimelineSettings
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  internal class DeliveryTimelineSettings
  {
    internal const int c_maxExpandedTeamsDefault = 20;
    internal const int c_maxUnexpandedTeamsDefault = 0;
    internal const int c_maxTotalTeamFieldsDefault = 200;
    internal const int c_maxDateRangeInDaysDefault = 65;
    internal const int c_maxFullPageWorkItemCountDefault = 400;
    internal const int c_maxPartialPageItemsPerBucketDefault = 10;
    internal const int c_maxNumberOfCardsDefault = 999;
    internal const int c_warningNumberOfCardsDefault = 750;
    internal const string c_maxExpandedTeamsRegistryPath = "/Configuration/DeliveryTimeline/MaxExpandedTeams";
    internal const string c_maxUnexpandedTeamsRegistryPath = "/Configuration/DeliveryTimeline/MaxUnexpandedTeams";
    internal const string c_maxTotalTeamFieldsRegistryPath = "/Configuration/DeliveryTimeline/MaxTotalTeamFields";
    internal const string c_maxDateRangeInDaysRegistryPath = "/Configuration/DeliveryTimeline/MaxDateRangeInDays";
    internal const string c_maxFullPageWorkItemCountRegistryPath = "/Configuration/DeliveryTimeline/MaxFullPageWorkItemCount";
    internal const string c_maxPartialPageItemsPerBucketRegistryPath = "/Configuration/DeliveryTimeline/MaxPartialPageItemsPerBucket";
    internal const string c_maxNumberOfCardsRegistryPath = "/Configuration/DeliveryTimeline/MaxNumberOfCards";
    internal const string c_warningNumberOfCardsRegistryPath = "/Configuration/DeliveryTimeline/WarningNumberOfCards";

    public int MaxExpandedTeams { get; private set; }

    public int MaxUnexpandedTeams { get; private set; }

    public int MaxTotalTeamFields { get; private set; }

    public int MaxDateRangeInDays { get; private set; }

    public int MaxFullPageWorkItemCount { get; private set; }

    public int MaxPartialPageItemsPerBucket { get; private set; }

    public int MaxNumberOfCards { get; private set; }

    public int WarningNumberOfCards { get; private set; }

    public static DeliveryTimelineSettings Create(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return new DeliveryTimelineSettings()
      {
        MaxExpandedTeams = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxExpandedTeams", 20),
        MaxUnexpandedTeams = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxUnexpandedTeams", 0),
        MaxTotalTeamFields = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxTotalTeamFields", 200),
        MaxDateRangeInDays = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxDateRangeInDays", 65),
        MaxFullPageWorkItemCount = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxFullPageWorkItemCount", 400),
        MaxPartialPageItemsPerBucket = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxPartialPageItemsPerBucket", 10),
        MaxNumberOfCards = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/MaxNumberOfCards", 999),
        WarningNumberOfCards = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/DeliveryTimeline/WarningNumberOfCards", 750)
      };
    }

    internal DeliveryTimelineSettings()
    {
    }
  }
}
