// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.ConfigurationType
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [JsonConverter(typeof (UnknownEnumJsonConverter))]
  public enum ConfigurationType
  {
    Unknown = 0,
    Yaml = 1,
    [EditorBrowsable(EditorBrowsableState.Never), EnumMember(Value = "designer-json")] DesignerHyphenJson = 2,
    DesignerJson = 2,
    [EditorBrowsable(EditorBrowsableState.Never)] JustInTime = 3,
  }
}
