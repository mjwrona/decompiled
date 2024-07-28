// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SnapshotResponse : ResponseBase
  {
    private bool _accepted;

    [DataMember(Name = "accepted")]
    public bool Accepted
    {
      get => !this._accepted ? this.Snapshot != null : this._accepted;
      internal set => this._accepted = value;
    }

    [DataMember(Name = "snapshot")]
    public Snapshot Snapshot { get; set; }
  }
}
