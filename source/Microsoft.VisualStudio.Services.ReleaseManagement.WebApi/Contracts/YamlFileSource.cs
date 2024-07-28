// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlFileSource
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [ClientInternalUseOnly(false)]
  [DataContract]
  internal class YamlFileSource : ReleaseManagementSecuredObject
  {
    private IDictionary<string, YamlSourceReference> sourceFileReference;

    [JsonConstructor]
    public YamlFileSource(YamlFileSourceTypes type) => this.Type = type;

    [DataMember]
    public YamlFileSourceTypes Type { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [XmlIgnore]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public IDictionary<string, YamlSourceReference> SourceReference
    {
      get
      {
        if (this.sourceFileReference == null)
          this.sourceFileReference = (IDictionary<string, YamlSourceReference>) new Dictionary<string, YamlSourceReference>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.sourceFileReference;
      }
      set => this.sourceFileReference = value;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IDictionary<string, YamlSourceReference> sourceReference = this.SourceReference;
      if (sourceReference == null)
        return;
      sourceReference.ForEach<KeyValuePair<string, YamlSourceReference>>((Action<KeyValuePair<string, YamlSourceReference>>) (i => i.Value.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
