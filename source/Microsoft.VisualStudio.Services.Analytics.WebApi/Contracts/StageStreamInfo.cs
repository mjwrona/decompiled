// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts.StageStreamInfo
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts
{
  [DataContract]
  public class StageStreamInfo
  {
    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int StreamId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string Watermark { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int Priority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Current { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? LatestContentVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? InitialContentVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool KeysOnly { get; set; }
  }
}
