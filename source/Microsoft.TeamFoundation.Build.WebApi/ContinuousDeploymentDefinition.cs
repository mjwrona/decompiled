// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.ContinuousDeploymentDefinition
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class ContinuousDeploymentDefinition
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SubscriptionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RepositoryId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamProjectReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string HostedServiceName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StorageAccountName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Webspace { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Website { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string GitBranch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Definition { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public WebApiConnectedServiceRef ConnectedService { get; set; }
  }
}
