// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DataExport.ReferenceExporterPluginRegistration
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DataExport
{
  public static class ReferenceExporterPluginRegistration
  {
    internal static List<ReferenceExporterBase> GetRegisteredExporters(
      IVssRequestContext requestContext,
      JobParameters runParameters)
    {
      return new List<ReferenceExporterBase>()
      {
        (ReferenceExporterBase) new PackagingReferenceExporter(requestContext, runParameters),
        (ReferenceExporterBase) new SymbolReferenceExporter(requestContext, runParameters)
      };
    }

    private static IEnumerable<ReferenceExporterBase> GetEnabledExporters(
      IVssRequestContext requestContext,
      JobParameters runParameters)
    {
      return ReferenceExporterPluginRegistration.GetRegisteredExporters(requestContext, runParameters).Where<ReferenceExporterBase>((Func<ReferenceExporterBase, bool>) (x => x.IsEnabled));
    }

    public static IEnumerable<IBlobIdReferenceProcessor> GetEnabledFileExporters(
      IVssRequestContext requestContext,
      JobParameters runParameters)
    {
      return (IEnumerable<IBlobIdReferenceProcessor>) ReferenceExporterPluginRegistration.GetEnabledExporters(requestContext, runParameters);
    }

    public static IEnumerable<IDedupMetadataEntryProcessor> GetEnabledChunkExporters(
      IVssRequestContext requestContext,
      JobParameters runParameters)
    {
      return (IEnumerable<IDedupMetadataEntryProcessor>) ReferenceExporterPluginRegistration.GetEnabledExporters(requestContext, runParameters);
    }
  }
}
