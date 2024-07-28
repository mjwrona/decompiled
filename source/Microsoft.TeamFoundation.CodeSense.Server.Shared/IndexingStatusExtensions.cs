// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.IndexingStatusExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class IndexingStatusExtensions
  {
    private const string IndexCompleteRegistryKey = "/Service/CodeSense/IndexingStatus/IndexComplete";
    private const string IndexCompleteToRegistryKey = "/Service/CodeSense/IndexingStatus/IndexCompleteTo";

    public static void UpdateIndexingStatus(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      ITfsVersionControlClient tfsVersionControlClient,
      int changesetId,
      bool indexComplete)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      DateTime dateTime = DateTime.MinValue;
      using (new CodeSenseTraceWatch(requestContext, 1026000, TraceLayer.ExternalTfvc, "Query changeset {0}", new object[1]
      {
        (object) changesetId
      }))
      {
        TfvcChangeset changeset = tfsVersionControlClient.GetChangeset(requestContext, changesetId, false, false, true, 1);
        if (changeset != null)
          dateTime = changeset.CreatedDate.ToUniversalTime();
      }
      if (!(dateTime != DateTime.MinValue))
        return;
      using (new CodeSenseTraceWatch(requestContext, 1026005, TraceLayer.ExternalFramework, "Update indexing status: {0} {1}", new object[2]
      {
        (object) indexComplete,
        (object) dateTime
      }))
      {
        registryService.SetValue<bool>(requestContext, "/Service/CodeSense/IndexingStatus/IndexComplete", indexComplete);
        registryService.SetValue<DateTime>(requestContext, "/Service/CodeSense/IndexingStatus/IndexCompleteTo", dateTime);
      }
    }

    public static void GetIndexingStatus(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      out bool indexComplete,
      out DateTime indexCompleteTo)
    {
      using (new CodeSenseTraceWatch(requestContext, 1026010, true, TraceLayer.ExternalFramework, "Get indexing status", Array.Empty<object>()))
      {
        indexComplete = registryService.GetOrDefault<bool>(requestContext, "/Service/CodeSense/IndexingStatus/IndexComplete");
        indexCompleteTo = registryService.GetOrDefault<DateTime>(requestContext, "/Service/CodeSense/IndexingStatus/IndexCompleteTo", DateTime.UtcNow);
        indexCompleteTo = DateTime.SpecifyKind(indexCompleteTo, DateTimeKind.Utc);
      }
    }

    public static void ClearIndexingStatus(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      using (new CodeSenseTraceWatch(requestContext, 1026015, TraceLayer.ExternalFramework, "Clear indexing status", Array.Empty<object>()))
        registryService.DeleteEntries(requestContext, "/Service/CodeSense/IndexingStatus/IndexComplete", "/Service/CodeSense/IndexingStatus/IndexCompleteTo");
    }
  }
}
