// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksApiConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServiceHooksApiConstants
  {
    public const string AreaId = "6F0D0CB2-7079-41FA-AEEF-4772F7A835F7";
    internal const string InstanceType = "00000003-0000-8888-8000-000000000000";
    public const string AreaName = "hookssvc";
    public static readonly Guid ConsumerActionsLocationId = new Guid("6A1F0102-A266-4CA8-BFE9-F126DF266A37");
    public static readonly Guid ConsumersLocationId = new Guid("5F431332-1A18-43D9-BA45-109EC52C71C7");
    public static readonly Guid EventsLocationId = new Guid("46F7C4D2-97A1-48E6-85F3-5083742752FD");
    public static readonly Guid InputValuesQueryLocationId = new Guid("95784519-7B74-4625-8888-49B294FE46B3");
    public static readonly Guid NotificationsLocationId = new Guid("A8DBBB75-C3CF-4D7B-A61D-C95F5A97CA55");
    public static readonly Guid NotificationsQueryLocationId = new Guid("E5E555F6-94AD-475B-AC98-7F011B48DCD5");
    public static readonly Guid SubscriptionsLocationId = new Guid("4431EE3F-13E3-4B41-B62E-47289E90D3DC");
    public static readonly Guid SubscriptionsQueryLocationId = new Guid("D4A77D7B-9BE6-4060-B53D-1211F34DE619");
    public static readonly Guid TestNotificationsLocationId = new Guid("B906CEDB-0A8E-4E3C-B1B1-2F3D680F035E");
    public static readonly string SelfLink = "self";
    public static readonly string ActionsLink = "actions";
    public static readonly string ConsumerLink = "consumer";
    public static readonly string ConsumerActionLink = "consumerAction";
    public static readonly string EventTypesLink = "eventTypes";
    public static readonly string NotificationsLink = "notifications";
    public static readonly string PublisherLink = "publisher";
  }
}
