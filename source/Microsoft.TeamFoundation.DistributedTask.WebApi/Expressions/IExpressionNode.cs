// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.IExpressionNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IExpressionNode
  {
    T Evaluate<T>(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options = null);

    object Evaluate(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options = null);

    bool EvaluateBoolean(ITraceWriter trace, ISecretMasker secretMasker, object state);

    IEnumerable<T> GetParameters<T>() where T : IExpressionNode;

    [EditorBrowsable(EditorBrowsableState.Never)]
    EvaluationResult EvaluateResult(
      ITraceWriter trace,
      ISecretMasker secretMasker,
      object state,
      EvaluationOptions options);
  }
}
