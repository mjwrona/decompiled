// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQuality.IDataQualityService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.DataQuality
{
  [DefaultServiceImplementation(typeof (DataQualityService))]
  public interface IDataQualityService : IVssFrameworkService
  {
    IReadOnlyCollection<DataQualityResult> CheckDataQuality(
      IVssRequestContext requestContext,
      DataQualityDefinition definition,
      int latencyExclusionSeconds,
      DateTime previousEndDate);

    IReadOnlyCollection<DataQualityResult> CheckDataQuality(
      IVssRequestContext requestContext,
      IEnumerable<DataQualityDefinition> definitions = null);

    IReadOnlyCollection<DataQualityResult> GetLatestDataQualityResults(
      IVssRequestContext requestContext);

    IReadOnlyCollection<DataQualityResult> GetCachedLatestDataQualityResults(
      IVssRequestContext requestContext);

    void ExpireCachedLatestDataQualityResults(IVssRequestContext requestContext);

    IReadOnlyCollection<string> GetDataQualityWarnings(
      IVssRequestContext requestContext,
      IEnumerable<DataQualityResult> dataQualityResults);

    CleanupDataQualityResult CleanupDataQualityResults(IVssRequestContext requestContext);

    void NotifyDataChange(IVssRequestContext requestContext);

    void InitializeModelReady(IVssRequestContext requestContext);
  }
}
