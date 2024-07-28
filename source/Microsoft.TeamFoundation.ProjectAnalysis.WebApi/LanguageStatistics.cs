// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.LanguageStatistics
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  [DataContract]
  public class LanguageStatistics : LanguageMetricsSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name;
    [DataMember(EmitDefaultValue = false)]
    public int Files;
    [DataMember(EmitDefaultValue = false)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public double? FilesPercentage;
    [DataMember(EmitDefaultValue = false)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? Bytes;
    [DataMember(EmitDefaultValue = false)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public double? LanguagePercentage;

    public LanguageStatistics(Guid projectId)
      : base(projectId)
    {
    }
  }
}
