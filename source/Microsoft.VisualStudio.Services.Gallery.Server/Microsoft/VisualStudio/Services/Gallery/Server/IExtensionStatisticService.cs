// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IExtensionStatisticService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ExtensionStatisticService))]
  public interface IExtensionStatisticService : IVssFrameworkService
  {
    void UpdateStatistics(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionStatisticUpdate> statistics);

    bool ShouldUpdateStatCount(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string statType,
      string targetPlatform = null);

    void UpdateWeightedRating(
      IVssRequestContext requestContext,
      string product,
      IEnumerable<string> installationTargets);

    void IncrementStatCount(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string statisticType);
  }
}
