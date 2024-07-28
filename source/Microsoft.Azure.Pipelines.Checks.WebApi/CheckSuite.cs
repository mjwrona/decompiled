// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckSuite
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
  public class CheckSuite : CheckSuiteRef
  {
    private List<CheckRun> m_checkRuns;

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public CheckRunStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Message { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CompletedDate { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public List<CheckRun> CheckRuns
    {
      get => this.m_checkRuns ?? (this.m_checkRuns = new List<CheckRun>());
      set => this.m_checkRuns = value;
    }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public List<Resource> Resources { get; set; }

    public bool isCompleted() => (this.Status & CheckRunStatus.Completed) != 0;

    public bool isDeclined() => (this.Status & CheckRunStatus.Failed) != 0;
  }
}
