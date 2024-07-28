// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  public static class NameResolutionServiceExtensions
  {
    public static NameResolutionEntry QueryEntry(
      this INameResolutionService nameResolutionService,
      IVssRequestContext requestContext,
      string @namespace,
      string name)
    {
      NameResolutionQuery nameResolutionQuery = new NameResolutionQuery(@namespace, name);
      return nameResolutionService.QueryEntries(requestContext, (IReadOnlyList<NameResolutionQuery>) new NameResolutionQuery[1]
      {
        nameResolutionQuery
      }).Single<NameResolutionEntry>();
    }

    public static IReadOnlyList<NameResolutionEntry> QueryEntries(
      this INameResolutionService nameResolutionService,
      IVssRequestContext requestContext,
      IEnumerable<string> namespaces,
      string name)
    {
      NameResolutionQuery[] array = namespaces.Select<string, NameResolutionQuery>((Func<string, NameResolutionQuery>) (ns => new NameResolutionQuery(ns, name))).ToArray<NameResolutionQuery>();
      return nameResolutionService.QueryEntries(requestContext, (IReadOnlyList<NameResolutionQuery>) array);
    }

    public static void SetEntry(
      this INameResolutionService nameResolutionService,
      IVssRequestContext requestContext,
      NameResolutionEntry entry,
      bool overwriteExisting = false)
    {
      nameResolutionService.SetEntries(requestContext, (IEnumerable<NameResolutionEntry>) new NameResolutionEntry[1]
      {
        entry
      }, (overwriteExisting ? 1 : 0) != 0);
    }

    public static void DeleteEntry(
      this INameResolutionService nameResolutionService,
      IVssRequestContext requestContext,
      string @namespace,
      string name)
    {
      NameResolutionEntry nameResolutionEntry = new NameResolutionEntry(@namespace, name);
      nameResolutionService.DeleteEntries(requestContext, (IEnumerable<NameResolutionEntry>) new NameResolutionEntry[1]
      {
        nameResolutionEntry
      });
    }
  }
}
