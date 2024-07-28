// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Server.ITeamFoundationAnalyticsService
// Assembly: Microsoft.TeamFoundation.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7A426D2C-9BEF-4A84-9FA2-D9A32F46BD7E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Analytics.Server.dll

using Microsoft.TeamFoundation.Analytics.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Analytics.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationAnalyticsService))]
  public interface ITeamFoundationAnalyticsService : IVssFrameworkService
  {
    void UpdateAnalyticsState(IVssRequestContext requestContext, AnalyticsState state);
  }
}
