// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "By design.")]
  [DataContract]
  public class Artifact : ReleaseManagementSecuredObject
  {
    [DataMember]
    [Obsolete("This property is deprecated use Alias instead and remove all its references")]
    public string SourceId { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Alias { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [XmlIgnore]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public IDictionary<string, ArtifactSourceReference> DefinitionReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsPrimary { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsRetained { get; set; }

    public bool HasAlias()
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(this.Alias))
        flag = true;
      return flag;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IDictionary<string, ArtifactSourceReference> definitionReference = this.DefinitionReference;
      if (definitionReference == null)
        return;
      definitionReference.ForEach<KeyValuePair<string, ArtifactSourceReference>>((Action<KeyValuePair<string, ArtifactSourceReference>>) (i => i.Value?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
