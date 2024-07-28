// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.Common.SearchCiLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5EF41D26-5C57-4D41-AC16-607E07E24DBC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Search.Platform.Common
{
  public class SearchCiLoggerService : ISearchCiLoggerService, IVssFrameworkService
  {
    private ISearchCiLoggerService m_ciLogger;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_ciLogger = SearchPlatformHelper.GetExtension<ISearchCiLoggerService>(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_ciLogger == null)
        return;
      if (this.m_ciLogger is IDisposable ciLogger)
      {
        ciLogger.Dispose();
        this.m_ciLogger = (ISearchCiLoggerService) null;
      }
      this.m_ciLogger = (ISearchCiLoggerService) null;
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      CustomerIntelligenceData properties)
    {
      this.m_ciLogger.Publish(requestContext, area, feature, properties);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      bool value)
    {
      this.m_ciLogger.Publish(requestContext, area, feature, property, value);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      double value)
    {
      this.m_ciLogger.Publish(requestContext, area, feature, property, value);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      string value)
    {
      this.m_ciLogger.Publish(requestContext, area, feature, property, value);
    }

    public void PublishOnPremisesEvent(
      IVssRequestContext requestContext,
      string eventName,
      Dictionary<string, string> value)
    {
      this.m_ciLogger.PublishOnPremisesEvent(requestContext, eventName, value);
    }
  }
}
