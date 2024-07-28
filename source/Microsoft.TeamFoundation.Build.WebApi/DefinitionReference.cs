// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DefinitionReference
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class DefinitionReference : ShallowReference
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionType DefinitionType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionQueueStatus QueueStatus { get; set; }
  }
}
