// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.Common.ISearchCiLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5EF41D26-5C57-4D41-AC16-607E07E24DBC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Search.Platform.Common
{
  [DefaultServiceImplementation(typeof (SearchCiLoggerService))]
  public interface ISearchCiLoggerService : IVssFrameworkService
  {
    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      CustomerIntelligenceData properties);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      bool value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      double value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      string value);

    void PublishOnPremisesEvent(
      IVssRequestContext requestContext,
      string eventName,
      Dictionary<string, string> value);
  }
}
