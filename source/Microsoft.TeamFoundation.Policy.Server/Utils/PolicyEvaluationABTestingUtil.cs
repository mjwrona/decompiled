// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.Utils.PolicyEvaluationABTestingUtil
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Policy.Server.Utils
{
  internal class PolicyEvaluationABTestingUtil
  {
    private IVssRequestContext m_requestContext;
    private const string c_layer = "PolicyService";

    public PolicyEvaluationABTestingUtil(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public void SafeTestResultsAndReportTrace(
      IEnumerable<PolicyEvaluationRecord> oldResults,
      IEnumerable<PolicyEvaluationRecord> newResults,
      Guid projectId,
      ArtifactId artifactId,
      Type artifactTargetType)
    {
      try
      {
        if (!this.CheckCount(oldResults, newResults) || !this.CheckEvaluationIds(oldResults, newResults) || !this.m_requestContext.IsFeatureEnabled("Policy.EnableCachedPolicyABTestingWithSerialization"))
          return;
        this.CheckEntireRecord(oldResults, newResults);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceAlways(1390145, TraceLevel.Warning, "Policy", "PolicyService", string.Format("EnableCachedPolicyABTesting failed for project:{0}, artifact:{1}, ", (object) projectId, (object) artifactId.ToolSpecificId) + string.Format("targetType:{0}, error:{1}", (object) artifactTargetType.Name, (object) ex));
      }
    }

    private bool CheckCount(
      IEnumerable<PolicyEvaluationRecord> oldResults,
      IEnumerable<PolicyEvaluationRecord> newResults)
    {
      if (oldResults.Count<PolicyEvaluationRecord>() != newResults.Count<PolicyEvaluationRecord>())
        throw new Exception(string.Format("Old version has {0} records but new version has {1}, expected to be equal", (object) oldResults.Count<PolicyEvaluationRecord>(), (object) newResults.Count<PolicyEvaluationRecord>()));
      return true;
    }

    private bool CheckEvaluationIds(
      IEnumerable<PolicyEvaluationRecord> oldResults,
      IEnumerable<PolicyEvaluationRecord> newResults)
    {
      HashSet<Guid> source = new HashSet<Guid>(oldResults.Select<PolicyEvaluationRecord, Guid>((Func<PolicyEvaluationRecord, Guid>) (x => x.EvaluationId)));
      HashSet<Guid> other = new HashSet<Guid>(newResults.Select<PolicyEvaluationRecord, Guid>((Func<PolicyEvaluationRecord, Guid>) (x => x.EvaluationId)));
      source.SymmetricExceptWith((IEnumerable<Guid>) other);
      if (source.Count > 0)
        throw new Exception(string.Format("Results are different and have {0} not intersected EvaluationIds. Top10 are: \r\n", (object) source.Count) + string.Join<Guid>(", ", source.Take<Guid>(10)));
      return true;
    }

    private bool CheckEntireRecord(
      IEnumerable<PolicyEvaluationRecord> oldResults,
      IEnumerable<PolicyEvaluationRecord> newResults)
    {
      Dictionary<Guid, PolicyEvaluationRecord> dedupedDictionary1 = oldResults.ToDedupedDictionary<PolicyEvaluationRecord, Guid, PolicyEvaluationRecord>((Func<PolicyEvaluationRecord, Guid>) (x => x.EvaluationId), (Func<PolicyEvaluationRecord, PolicyEvaluationRecord>) (y => y));
      Dictionary<Guid, PolicyEvaluationRecord> dedupedDictionary2 = newResults.ToDedupedDictionary<PolicyEvaluationRecord, Guid, PolicyEvaluationRecord>((Func<PolicyEvaluationRecord, Guid>) (x => x.EvaluationId), (Func<PolicyEvaluationRecord, PolicyEvaluationRecord>) (y => y));
      foreach (KeyValuePair<Guid, PolicyEvaluationRecord> keyValuePair in dedupedDictionary1)
      {
        PolicyEvaluationRecord evaluationRecord;
        if (!dedupedDictionary2.TryGetValue(keyValuePair.Key, out evaluationRecord))
          throw new Exception(string.Format("Never expected exception: EvaluationId {0} is missing in new results", (object) keyValuePair.Key));
        string str1 = JsonConvert.SerializeObject((object) keyValuePair.Value);
        string str2 = JsonConvert.SerializeObject((object) evaluationRecord);
        if (!str1.Equals(str2, StringComparison.OrdinalIgnoreCase))
          throw new Exception(string.Format("Different serialized objects detected for EvaluationId {0}.\r\n", (object) keyValuePair.Key) + "oldObject: " + str1 + "\r\nnewObject: " + str2 + "\r\n");
      }
      return true;
    }
  }
}
