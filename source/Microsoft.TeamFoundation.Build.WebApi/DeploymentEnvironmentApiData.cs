// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DeploymentEnvironmentApiData
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class DeploymentEnvironmentApiData
  {
    public DeploymentEnvironmentApiData() => this.DisconnectSubscription = false;

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string ProjectName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string SubscriptionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string SubscriptionName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string DeploymentName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string UserName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Password { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Cert { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool DisconnectSubscription { get; set; }
  }
}
