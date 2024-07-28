// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.TeamFoundationJobDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class TeamFoundationJobDefinitionExtensions
  {
    public static bool WasLastRunFaulted(
      this TeamFoundationJobDefinition jobDefinition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<TeamFoundationJobDefinition>(jobDefinition, nameof (jobDefinition));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IEnumerable<TeamFoundationJobHistoryEntry> source = (IEnumerable<TeamFoundationJobHistoryEntry>) null;
      using (new CodeSenseTraceWatch(requestContext, 1025620, TraceLayer.ExternalFramework, "Getting latest job history for '{0}'", new object[1]
      {
        (object) jobDefinition.Name
      }))
        source = (IEnumerable<TeamFoundationJobHistoryEntry>) requestContext.GetService<TeamFoundationJobService>().QueryLatestJobHistory(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobDefinition.JobId
        });
      TeamFoundationJobHistoryEntry foundationJobHistoryEntry = source != null ? source.SingleOrDefault<TeamFoundationJobHistoryEntry>() : (TeamFoundationJobHistoryEntry) null;
      return foundationJobHistoryEntry != null && foundationJobHistoryEntry.Result != TeamFoundationJobResult.Succeeded && foundationJobHistoryEntry.Result != TeamFoundationJobResult.Disabled;
    }
  }
}
