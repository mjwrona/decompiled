// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SourceVersionHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class SourceVersionHelper
  {
    private const string c_layer = "SourceVersionHelper";

    public static Change FindChange(
      IVssRequestContext requestContext,
      IBuildSourceProvider sourceProvider,
      List<Change> changes,
      string sourceVersion)
    {
      int changeIndex = SourceVersionHelper.FindChangeIndex(requestContext, sourceProvider, changes, sourceVersion);
      return changeIndex <= -1 ? (Change) null : changes[changeIndex];
    }

    public static int FindChangeIndex(
      IVssRequestContext requestContext,
      IBuildSourceProvider sourceProvider,
      List<Change> changes,
      string sourceVersion)
    {
      string changeId;
      if (sourceProvider.TryConvertSourceVersionToChangeId(requestContext, sourceVersion, out changeId))
        return changes.FindIndex((Predicate<Change>) (c => string.Equals(c?.Id, changeId, StringComparison.OrdinalIgnoreCase)));
      requestContext.TraceError(nameof (SourceVersionHelper), "Can't convert source version '" + sourceVersion + "' to change Id.");
      return -1;
    }
  }
}
