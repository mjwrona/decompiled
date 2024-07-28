// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckRun
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CheckRun : CheckRunResult
  {
    private List<CheckRunUpdate> m_resultUpdates;

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public CheckEvaluationOrder EvaluationOrder { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CompletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CheckConfigurationRef CheckConfigurationRef { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [IgnoreDataMember]
    public CheckSuiteRef CheckSuiteRef { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [IgnoreDataMember]
    public Guid? TimeoutJobId { get; set; }

    public bool IsEvaluationInProgress => (this.Status & (CheckRunStatus.Queued | CheckRunStatus.Running)) != 0;

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public List<CheckRunUpdate> ResultUpdates
    {
      get => this.m_resultUpdates ?? (this.m_resultUpdates = new List<CheckRunUpdate>());
      set => this.m_resultUpdates = value;
    }

    public string ToStringWithCheckConfigurationExtended()
    {
      try
      {
        return JsonUtility.ToString((object) new
        {
          CheckRunId = this.Id,
          CheckRunStatus = this.Status,
          CheckRunResultMessage = this.ResultMessage,
          RunCreatedDate = this.CreatedDate,
          RunCompletedDate = this.CompletedDate,
          ConfigurationId = this.CheckConfigurationRef?.Id,
          Type = this.CheckConfigurationRef?.Type?.Name,
          ResourceName = this.CheckConfigurationRef?.Resource?.Name,
          ResourceType = this.CheckConfigurationRef?.Resource?.Type,
          CheckSuiteId = this.CheckSuiteRef?.Id,
          IsEvaluationInProgress = this.IsEvaluationInProgress,
          EvaluationOrder = this.EvaluationOrder
        });
      }
      catch (Exception ex)
      {
        return string.Format("{0}() encountered {1}", (object) "ToString", (object) ex.GetType());
      }
    }

    public string ToStringExtended()
    {
      try
      {
        return JsonUtility.ToString((object) new
        {
          CheckRunId = this.Id,
          CheckRunStatus = this.Status,
          CheckRunResultMessage = this.ResultMessage,
          RunCreatedDate = this.CreatedDate,
          RunCompletedDate = this.CompletedDate,
          CheckSuiteId = this.CheckSuiteRef?.Id,
          IsEvaluationInProgress = this.IsEvaluationInProgress,
          EvaluationOrder = this.EvaluationOrder
        });
      }
      catch (Exception ex)
      {
        return string.Format("{0}() encountered {1}", (object) "ToString", (object) ex.GetType());
      }
    }
  }
}
