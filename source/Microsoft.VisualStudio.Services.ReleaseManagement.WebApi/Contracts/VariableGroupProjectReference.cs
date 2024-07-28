// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroupProjectReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class VariableGroupProjectReference : ReleaseManagementSecuredObject
  {
    public VariableGroupProjectReference()
    {
    }

    private VariableGroupProjectReference(
      VariableGroupProjectReference variableGroupReference)
    {
      this.ProjectReference = variableGroupReference.ProjectReference;
      this.Name = variableGroupReference.Name;
      this.Description = variableGroupReference.Description;
    }

    [DataMember(EmitDefaultValue = false)]
    public ProjectReference ProjectReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ProjectReference?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
