// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphGroupVstsCreationContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphGroupVstsCreationContext : GraphGroupCreationContext
  {
    [DataMember(IsRequired = true)]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    public SubjectDescriptor Descriptor { get; set; }

    [DataMember(Name = "Descriptor", IsRequired = false, EmitDefaultValue = false)]
    private string DescriptorString
    {
      get => this.Descriptor.ToString();
      set => this.Descriptor = SubjectDescriptor.FromString(value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool CrossProject { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool RestrictedVisibility { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SpecialGroupType { get; set; }
  }
}
