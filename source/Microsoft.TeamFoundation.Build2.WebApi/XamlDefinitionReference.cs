// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.XamlDefinitionReference
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class XamlDefinitionReference : ShallowReference
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new int Id
    {
      get => base.Id;
      set => base.Id = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Name
    {
      get => base.Name;
      set => base.Name = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Url
    {
      get => base.Url;
      set => base.Url = value;
    }
  }
}
