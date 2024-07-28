// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.PolicyEvaluationObserver
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal class PolicyEvaluationObserver : IObserver<GitPushPolicyEvaluationProgress>
  {
    private readonly string[] spinnerComponents = new string[4]
    {
      "|",
      "/",
      "-",
      "\\"
    };
    private int spinnerIndex;
    private readonly Action<string> m_writeOutput;
    private readonly Throttler m_objectReportThrottler;

    public PolicyEvaluationObserver(Action<string> writeOutput)
    {
      this.m_writeOutput = writeOutput;
      this.m_objectReportThrottler = new Throttler(100);
    }

    public void OnNext(GitPushPolicyEvaluationProgress progress)
    {
      if (!this.m_objectReportThrottler.IsAfterThreshold() && !progress.ContentScanPolicyPolicyEvaluationComplete)
        return;
      this.m_writeOutput("\r" + progress.DisplayText + "... ");
      if (progress.TotalObjectsToApplyContentScanPolicies > 0)
        this.m_writeOutput(string.Format("({0}/{1}) ", (object) progress.ObjectsContentScanPolicyApplied, (object) progress.TotalObjectsToApplyContentScanPolicies));
      else if (progress.ContentScanPolicyPolicyEvaluationComplete)
      {
        this.m_writeOutput(" ");
      }
      else
      {
        this.m_writeOutput(this.spinnerComponents[this.spinnerIndex]);
        this.spinnerIndex = (this.spinnerIndex + 1) % this.spinnerComponents.Length;
      }
      if (!progress.ContentScanPolicyPolicyEvaluationComplete)
        return;
      this.m_writeOutput(string.Format("done ({0} ms)\n", (object) this.m_objectReportThrottler.TotalElapsedMilliseconds));
    }

    public void OnError(Exception error) => throw new NotImplementedException();

    public void OnCompleted() => throw new NotImplementedException();
  }
}
