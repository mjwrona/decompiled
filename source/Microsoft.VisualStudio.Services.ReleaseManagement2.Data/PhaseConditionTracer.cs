// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.PhaseConditionTracer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public class PhaseConditionTracer : ITraceWriter
  {
    private readonly IVssRequestContext requestContext;
    private StringBuilder info = new StringBuilder();

    public PhaseConditionTracer(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public string Message => this.info.ToString();

    public void Info(string message)
    {
      this.requestContext.Trace(1980009, TraceLevel.Info, "ReleaseManagementService", "Pipeline", message);
      this.info.AppendLine(message);
    }

    public void Verbose(string message) => this.requestContext.Trace(1980009, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", message);
  }
}
