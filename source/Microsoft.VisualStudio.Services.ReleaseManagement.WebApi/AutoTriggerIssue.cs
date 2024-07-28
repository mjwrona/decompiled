// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  [KnownType(typeof (ContinuousDeploymentTriggerIssue))]
  [JsonConverter(typeof (AutoTriggerIssueJsonConverter))]
  public abstract class AutoTriggerIssue : ReleaseManagementSecuredObject
  {
    [DataMember]
    public ReleaseTriggerType ReleaseTriggerType { get; set; }

    [DataMember]
    public ProjectReference Project { get; set; }

    [DataMember]
    public ReleaseDefinitionShallowReference ReleaseDefinitionReference { get; set; }

    [DataMember]
    public IssueSource IssueSource { get; set; }

    [DataMember]
    public Issue Issue { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Project?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.Issue?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
