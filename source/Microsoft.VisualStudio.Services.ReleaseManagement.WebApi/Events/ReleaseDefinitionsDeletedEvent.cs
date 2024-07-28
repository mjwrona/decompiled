// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.ReleaseDefinitionsDeletedEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [ServiceEventObject]
  public class ReleaseDefinitionsDeletedEvent
  {
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public List<ReleaseDefinitionShallowReference> ReleaseDefinitionReferences { get; set; }

    [DataMember]
    public ProjectReference Project { get; set; }
  }
}
