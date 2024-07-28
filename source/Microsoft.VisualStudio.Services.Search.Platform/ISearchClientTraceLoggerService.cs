// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.Common.ISearchClientTraceLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5EF41D26-5C57-4D41-AC16-607E07E24DBC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;

namespace Microsoft.TeamFoundation.Search.Platform.Common
{
  [DefaultServiceImplementation(typeof (SearchClientTraceLoggerService))]
  public interface ISearchClientTraceLoggerService : IVssFrameworkService
  {
    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      ClientTraceData properties);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Level level,
      string message);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Exception exception);
  }
}
