// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationTraceWriter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class EvaluationTraceWriter : ITraceWriter
  {
    private readonly ISecretMasker m_secretMasker;
    private readonly ITraceWriter m_trace;

    public EvaluationTraceWriter(ITraceWriter trace, ISecretMasker secretMasker)
    {
      ArgumentUtility.CheckForNull<ISecretMasker>(secretMasker, nameof (secretMasker));
      this.m_trace = trace;
      this.m_secretMasker = secretMasker;
    }

    public void Info(string message)
    {
      if (this.m_trace == null)
        return;
      message = this.m_secretMasker.MaskSecrets(message);
      this.m_trace.Info(message);
    }

    public void Verbose(string message)
    {
      if (this.m_trace == null)
        return;
      message = this.m_secretMasker.MaskSecrets(message);
      this.m_trace.Verbose(message);
    }
  }
}
