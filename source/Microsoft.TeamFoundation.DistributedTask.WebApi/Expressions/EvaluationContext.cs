// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvaluationContext
  {
    private readonly Dictionary<ExpressionNode, string> m_traceResults = new Dictionary<ExpressionNode, string>();
    private readonly MemoryCounter m_traceMemory;

    internal EvaluationContext(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options,
      ExpressionNode node)
    {
      ArgumentUtility.CheckForNull<ITraceWriter>(trace, nameof (trace));
      ArgumentUtility.CheckForNull<ISecretMasker>(secretMasker, nameof (secretMasker));
      this.Trace = trace;
      this.SecretMasker = secretMasker;
      this.State = state;
      options = new EvaluationOptions(options);
      if (options.MaxMemory == 0)
        options.MaxMemory = 1048576;
      this.Options = options;
      this.Memory = new EvaluationMemory(options.MaxMemory, node);
      this.m_traceResults = new Dictionary<ExpressionNode, string>();
      this.m_traceMemory = new MemoryCounter((ExpressionNode) null, new int?(options.MaxMemory));
    }

    public ITraceWriter Trace { get; }

    public ISecretMasker SecretMasker { get; }

    public object State { get; }

    internal EvaluationMemory Memory { get; }

    internal EvaluationOptions Options { get; }

    internal void SetTraceResult(ExpressionNode node, EvaluationResult result)
    {
      string str1;
      if (this.m_traceResults.TryGetValue(node, out str1))
      {
        this.m_traceMemory.Remove(str1);
        this.m_traceResults.Remove(node);
      }
      string str2 = ExpressionUtil.FormatValue(this.SecretMasker, result);
      if (!this.m_traceMemory.TryAdd(str2))
        return;
      this.m_traceResults[node] = str2;
    }

    internal bool TryGetTraceResult(ExpressionNode node, out string value) => this.m_traceResults.TryGetValue(node, out value);
  }
}
