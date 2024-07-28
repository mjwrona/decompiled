// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckRunUpdate
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  public class CheckRunUpdate
  {
    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    [ClientInternalUseOnly(true)]
    public Guid CheckSuiteId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    [ClientInternalUseOnly(true)]
    public Guid CheckRunId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public CheckRunStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public DateTime ModifiedOn { get; set; }
  }
}
