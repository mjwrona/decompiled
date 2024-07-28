// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ExpressionTraceWriter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class ExpressionTraceWriter : Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter
  {
    private readonly ITraceWriter m_trace;

    public ExpressionTraceWriter(ITraceWriter trace) => this.m_trace = trace;

    public void Info(string message) => this.m_trace.Info("{0}", (object) message);

    public void Verbose(string message) => this.m_trace.Verbose("{0}", (object) message);
  }
}
