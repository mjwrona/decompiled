// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.PatchDeploymentResource
// Assembly: Microsoft.VisualStudio.Services.DeploymentTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F672626D-7DDA-4A84-9A4F-2205F04CA597
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DeploymentTracking.WebApi
{
  [DataContract]
  public class PatchDeploymentResource
  {
    [DataMember]
    public int ReleaseDefinitionId { get; set; }

    [DataMember]
    public int DefinitionEnvironmentId { get; set; }
  }
}
