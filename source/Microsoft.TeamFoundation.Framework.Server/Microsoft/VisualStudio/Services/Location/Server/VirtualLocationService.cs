// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.VirtualLocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class VirtualLocationService : LocationService
  {
    protected internal override ILocationDataProvider CreateLocalDataProvider(
      IVssRequestContext requestContext)
    {
      return (ILocationDataProvider) new VirtualLocalLocationDataProvider(requestContext, this.LocationCache);
    }

    protected override void NotifyLocationDataChanged(
      IVssRequestContext requestContext,
      LocationDataKind kind)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.LocationDataChanged, LocationService.ToEventData(kind));
    }
  }
}
