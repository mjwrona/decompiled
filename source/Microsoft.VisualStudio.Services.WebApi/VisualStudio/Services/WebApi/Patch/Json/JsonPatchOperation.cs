// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi.Patch.Json
{
  [DataContract]
  public class JsonPatchOperation
  {
    [DataMember(Name = "op", IsRequired = true)]
    public Operation Operation { get; set; }

    [DataMember(IsRequired = true)]
    public string Path { get; set; }

    [DataMember]
    public string From { get; set; }

    [DataMember]
    public object Value { get; set; }
  }
}
