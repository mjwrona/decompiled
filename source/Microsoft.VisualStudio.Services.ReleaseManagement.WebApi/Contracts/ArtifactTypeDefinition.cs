// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactTypeDefinition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ArtifactTypeDefinition : ReleaseManagementSecuredObject
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string EndpointTypeId { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string UniqueSourceIdentifier { get; set; }

    [DataMember]
    public string ArtifactType { get; set; }

    [DataMember]
    public bool IsCommitsTraceabilitySupported { get; set; }

    [DataMember]
    public bool IsWorkitemsTraceabilitySupported { get; set; }

    [DataMember]
    public ArtifactTriggerConfiguration ArtifactTriggerConfiguration { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public IList<InputDescriptor> InputDescriptors { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      if (this.InputDescriptors != null && this.InputDescriptors.Any<InputDescriptor>())
      {
        foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) this.InputDescriptors)
          inputDescriptor.SetSecuredObjectProperties(SecurityConstants.ReleaseManagementSecurityNamespaceId, requiredPermissions, token);
      }
      this.ArtifactTriggerConfiguration?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
