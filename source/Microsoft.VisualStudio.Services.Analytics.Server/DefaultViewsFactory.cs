// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DefaultViewsFactory
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class DefaultViewsFactory : IDefaultViewsFactory
  {
    public IFactory GetFactory(
      IVssRequestContext requestContext,
      DefaultViewsFactoryOptions defaultViewsFactoryOptions)
    {
      return defaultViewsFactoryOptions.ViewType == AnalyticsViewType.WorkItems ? (IFactory) new WorkItemDefaultViewsFactory() : (IFactory) new NoOpFactory();
    }

    public List<IFactory> GetFactories(IVssRequestContext requestContext) => new List<IFactory>()
    {
      (IFactory) new WorkItemDefaultViewsFactory()
    };
  }
}
