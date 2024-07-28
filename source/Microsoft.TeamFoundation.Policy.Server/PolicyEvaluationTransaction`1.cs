// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyEvaluationTransaction`1
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public sealed class PolicyEvaluationTransaction<TPolicy> : IDisposable where TPolicy : class, ITeamFoundationPolicy
  {
    private readonly ActivePolicyEvaluationSet<TPolicy> m_policyEvaluationSet;
    private readonly Func<IVssRequestContext, IPolicyComponent> m_componentFactory;
    private bool m_isDisposed;
    private bool m_resultsAreSaved;
    private readonly StackTrace m_constructorStackTrace;

    internal PolicyEvaluationTransaction(
      ActivePolicyEvaluationSet<TPolicy> policyEvaluationSet,
      Func<IVssRequestContext, IPolicyComponent> componentFactory)
    {
      this.m_policyEvaluationSet = policyEvaluationSet;
      this.m_componentFactory = componentFactory;
      this.m_isDisposed = false;
      this.m_resultsAreSaved = false;
      if (!TeamFoundationTracingService.IsRawTracingEnabled(1801898970, TraceLevel.Info, "Policy", nameof (PolicyEvaluationTransaction<TPolicy>), (string[]) null))
        return;
      this.m_constructorStackTrace = new StackTrace(1);
    }

    public void Finalize(
      IVssRequestContext requestContext,
      Func<TPolicy, PolicyEvaluationStatus, TeamFoundationPolicyEvaluationRecordContext, TeamFoundationPolicyEvaluationRecordContext> commitHandler,
      ClientTraceData ctData = null)
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
      if (this.m_resultsAreSaved)
        throw new InvalidOperationException();
      this.m_policyEvaluationSet.Notify(requestContext, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult>) ((p, status, context) =>
      {
        TeamFoundationPolicyEvaluationRecordContext context1 = commitHandler(p, status.Value, context);
        return new PolicyActionResult(status.Value, context1);
      }));
      this.m_policyEvaluationSet.SavePolicyEvaluationRecords(requestContext, this.m_componentFactory, ctData);
    }

    public void Save(IVssRequestContext requestContext, ClientTraceData ctData = null)
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
      if (this.m_resultsAreSaved)
        throw new InvalidOperationException();
      this.m_policyEvaluationSet.SavePolicyEvaluationRecords(requestContext, this.m_componentFactory, ctData);
      this.m_resultsAreSaved = true;
    }

    public void Discard()
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
      this.m_resultsAreSaved = !this.m_resultsAreSaved ? true : throw new InvalidOperationException();
    }

    public void Dispose() => this.m_isDisposed = true;

    ~PolicyEvaluationTransaction()
    {
      if (this.m_isDisposed)
        return;
      if (this.m_constructorStackTrace != null)
        TeamFoundationTracingService.TraceRaw(423690590, TraceLevel.Error, "Policy", nameof (PolicyEvaluationTransaction<TPolicy>), "PolicyEvaluationTransaction finalizer without dispose - call stack: {0}", (object) this.m_constructorStackTrace);
      else
        TeamFoundationTracingService.TraceRaw(423690590, TraceLevel.Error, "Policy", nameof (PolicyEvaluationTransaction<TPolicy>), "PolicyEvaluationTransaction finalizer without dispose");
    }
  }
}
