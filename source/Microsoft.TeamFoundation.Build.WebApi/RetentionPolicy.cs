// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.RetentionPolicy
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class RetentionPolicy
  {
    public RetentionPolicy()
    {
      this.BuildReason = BuildReason.All;
      this.DeleteOptions = DeleteOptions.All;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason BuildReason { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildStatus BuildStatus { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int NumberToKeep { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DeleteOptions DeleteOptions { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal string DefinitionUri { get; set; }
  }
}
