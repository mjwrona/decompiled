// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReplicationPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  public sealed class ReplicationPolicy : JsonSerializable
  {
    private const int DefaultMaxReplicaSetSize = 4;
    private const int DefaultMinReplicaSetSize = 3;
    private const bool DefaultAsyncReplication = false;

    public int MaxReplicaSetSize
    {
      get => this.GetValue<int>("maxReplicasetSize", 4);
      set => this.SetValue("maxReplicasetSize", (object) value);
    }

    public int MinReplicaSetSize
    {
      get => this.GetValue<int>("minReplicaSetSize", 3);
      set => this.SetValue("minReplicaSetSize", (object) value);
    }

    public bool AsyncReplication
    {
      get => this.GetValue<bool>("asyncReplication", false);
      set => this.SetValue("asyncReplication", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateNonNegativeInteger("minReplicaSetSize", this.MinReplicaSetSize);
      Helpers.ValidateNonNegativeInteger("minReplicaSetSize", this.MaxReplicaSetSize);
    }
  }
}
