// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ParallelExecutionInputBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public abstract class ParallelExecutionInputBase : ExecutionInput
  {
    [DataMember]
    public int MaxNumberOfAgents { get; set; }

    [DataMember]
    public bool ContinueOnError { get; set; }

    [IgnoreDataMember]
    public bool Parallel { get; private set; }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    protected ParallelExecutionInputBase(
      ParallelExecutionTypes type,
      int maxNumberOfAgents,
      bool continueOnError = true)
      : base(type)
    {
      this.MaxNumberOfAgents = Math.Max(1, maxNumberOfAgents);
      this.ContinueOnError = continueOnError;
      this.Parallel = true;
    }

    public override bool Equals(ExecutionInput other) => other is ParallelExecutionInputBase executionInputBase && this.MaxNumberOfAgents == executionInputBase.MaxNumberOfAgents && this.ContinueOnError == executionInputBase.ContinueOnError && base.Equals(other);
  }
}
