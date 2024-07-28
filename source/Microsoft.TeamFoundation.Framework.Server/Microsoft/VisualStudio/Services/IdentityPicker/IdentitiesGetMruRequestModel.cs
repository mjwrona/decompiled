// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.IdentitiesGetMruRequestModel
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [DataContract]
  public sealed class IdentitiesGetMruRequestModel
  {
    [DataMember(EmitDefaultValue = false, Name = "operationScopes")]
    public IList<string> OperationScopes { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "properties")]
    public IList<string> Properties { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "filterByAncestorEntityIds")]
    public IList<string> FilterByAncestorEntityIds { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "filterByEntityIds")]
    public IList<string> FilterByEntityIds { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "maxItemsCount")]
    public int? MaxItemsCount { get; set; }
  }
}
