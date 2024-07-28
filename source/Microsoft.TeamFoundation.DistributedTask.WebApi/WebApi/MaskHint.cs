// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.MaskHint
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class MaskHint
  {
    public MaskHint()
    {
    }

    private MaskHint(MaskHint maskHintToBeCloned)
    {
      this.Type = maskHintToBeCloned.Type;
      this.Value = maskHintToBeCloned.Value;
    }

    public MaskHint Clone() => new MaskHint(this);

    [DataMember]
    public MaskType Type { get; set; }

    [DataMember]
    public string Value { get; set; }

    public override bool Equals(object obj) => obj is MaskHint maskHint && this.Type == maskHint.Type && string.Equals(this.Value ?? string.Empty, maskHint.Value ?? string.Empty, StringComparison.Ordinal);

    public override int GetHashCode() => this.Type.GetHashCode() ^ (this.Value ?? string.Empty).GetHashCode();
  }
}
