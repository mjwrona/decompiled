// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic.BaseJobStep
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic
{
  public class BaseJobStep
  {
    private static readonly string c_traceArea = "Agile";
    private static readonly string c_traceLayer = TfsTraceLayers.Framework;

    protected IDisposable Trace(IVssRequestContext context, int start, int end, [CallerMemberName] string memberName = "") => context.TraceBlock(start, end, BaseJobStep.c_traceArea, BaseJobStep.c_traceLayer, memberName);
  }
}
