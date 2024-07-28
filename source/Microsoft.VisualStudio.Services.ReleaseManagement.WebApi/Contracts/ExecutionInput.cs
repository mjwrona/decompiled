// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ExecutionInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (NoneExecutionInput))]
  [KnownType(typeof (MultiConfigInput))]
  [KnownType(typeof (MultiMachineInput))]
  [JsonConverter(typeof (ParallelExecutionInputJsonConverter))]
  [DataContract]
  public abstract class ExecutionInput : ReleaseManagementSecuredObject, IEquatable<ExecutionInput>
  {
    [DataMember]
    public ParallelExecutionTypes ParallelExecutionType { get; protected set; }

    protected ExecutionInput(ParallelExecutionTypes type) => this.ParallelExecutionType = type;

    public virtual bool Equals(ExecutionInput other) => other != null && this.ParallelExecutionType == other.ParallelExecutionType;

    public abstract object Clone();
  }
}
