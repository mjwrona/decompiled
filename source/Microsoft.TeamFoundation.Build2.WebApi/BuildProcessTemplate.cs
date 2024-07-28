// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildProcessTemplate
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class BuildProcessTemplate
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TeamProject { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ServerPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason SupportedReasons { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ProcessTemplateType TemplateType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Key]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Parameters { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool FileExists { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Version { get; set; }
  }
}
