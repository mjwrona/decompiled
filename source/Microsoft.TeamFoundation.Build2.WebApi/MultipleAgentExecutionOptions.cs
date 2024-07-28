// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.MultipleAgentExecutionOptions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class MultipleAgentExecutionOptions : AgentTargetExecutionOptions
  {
    public MultipleAgentExecutionOptions()
      : this((ISecuredObject) null)
    {
    }

    internal MultipleAgentExecutionOptions(ISecuredObject securedObject)
      : base(2, securedObject)
    {
      this.MaxConcurrency = 1;
    }

    [DataMember(EmitDefaultValue = true)]
    [DefaultValue(1)]
    public int MaxConcurrency { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ContinueOnError { get; set; }
  }
}
