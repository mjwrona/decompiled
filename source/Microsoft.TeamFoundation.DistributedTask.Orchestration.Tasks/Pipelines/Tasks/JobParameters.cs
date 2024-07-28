// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.JobParameters
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public sealed class JobParameters
  {
    [DataMember(Name = "AgentIds")]
    private List<int> m_agentIds;

    public JobParameters()
    {
      this.Attempt = 1;
      this.StageAttempt = 1;
      this.PhaseAttempt = 1;
      this.CheckRerunAttempt = 1;
    }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int Attempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int PhaseAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PhaseName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string WorkerName { get; set; }

    public List<int> CandidateAgentIds
    {
      get
      {
        if (this.m_agentIds == null)
          this.m_agentIds = new List<int>();
        return this.m_agentIds;
      }
    }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int CheckRerunAttempt { get; set; }

    public static bool TryParseInstanceName(string instanceName, out JobParameters jobParams)
    {
      jobParams = (JobParameters) null;
      if (string.IsNullOrEmpty(instanceName))
        return false;
      JobParameters jobParameters = new JobParameters();
      string[] tokens = instanceName.Split('.');
      int i = tokens.Length - 1;
      string name = (string) null;
      int attempt = 1;
      if (!JobParameters.TryNextPair(tokens, ref i, ref name, ref attempt))
        return false;
      jobParameters.Name = name;
      jobParameters.Attempt = attempt;
      if (i >= 0)
      {
        if (!JobParameters.TryNextPair(tokens, ref i, ref name, ref attempt))
          return false;
        jobParameters.PhaseName = name;
        jobParameters.PhaseAttempt = attempt;
      }
      else
        jobParameters.PhaseName = PipelineConstants.DefaultJobName;
      if (i >= 0)
      {
        if (!JobParameters.TryNextPair(tokens, ref i, ref name, ref attempt))
          return false;
        jobParameters.StageName = name;
        jobParameters.StageAttempt = attempt;
      }
      else
        jobParameters.StageName = PipelineConstants.DefaultJobName;
      jobParams = jobParameters;
      return true;
    }

    private static bool TryNextPair(string[] tokens, ref int i, ref string name, ref int attempt)
    {
      int result;
      bool flag = int.TryParse(tokens[i], out result);
      if (flag)
      {
        attempt = result;
        --i;
        if (i < 0)
          return false;
        flag = int.TryParse(tokens[i], out int _);
      }
      else
        attempt = 1;
      if (flag)
        return false;
      name = tokens[i];
      --i;
      return true;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<int> agentIds = this.m_agentIds;
      // ISSUE: explicit non-virtual call
      if ((agentIds != null ? (__nonvirtual (agentIds.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_agentIds = (List<int>) null;
    }
  }
}
