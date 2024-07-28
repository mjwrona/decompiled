// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.Common.SearchClientTraceLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5EF41D26-5C57-4D41-AC16-607E07E24DBC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;

namespace Microsoft.TeamFoundation.Search.Platform.Common
{
  public class SearchClientTraceLoggerService : ISearchClientTraceLoggerService, IVssFrameworkService
  {
    private ISearchClientTraceLoggerService m_clientTraceLogger;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_clientTraceLogger = SearchPlatformHelper.GetExtension<ISearchClientTraceLoggerService>(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_clientTraceLogger == null)
        return;
      if (this.m_clientTraceLogger is IDisposable clientTraceLogger)
      {
        clientTraceLogger.Dispose();
        this.m_clientTraceLogger = (ISearchClientTraceLoggerService) null;
      }
      this.m_clientTraceLogger = (ISearchClientTraceLoggerService) null;
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      ClientTraceData properties)
    {
      this.m_clientTraceLogger.Publish(requestContext, area, feature, properties);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Level level,
      string message)
    {
      this.m_clientTraceLogger.Publish(requestContext, area, layer, level, message);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Exception exception)
    {
      this.m_clientTraceLogger.Publish(requestContext, area, layer, exception);
    }
  }
}
