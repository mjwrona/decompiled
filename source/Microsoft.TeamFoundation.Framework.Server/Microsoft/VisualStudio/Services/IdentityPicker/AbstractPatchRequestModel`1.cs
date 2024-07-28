// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.AbstractPatchRequestModel`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [DataContract]
  public abstract class AbstractPatchRequestModel<ValueType>
  {
    [DataMember(EmitDefaultValue = false, Name = "op")]
    public string Op { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "path")]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "value")]
    public IList<ValueType> Value { get; set; }
  }
}
