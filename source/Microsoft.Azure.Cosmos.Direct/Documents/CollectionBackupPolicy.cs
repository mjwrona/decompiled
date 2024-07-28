// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CollectionBackupPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class CollectionBackupPolicy : JsonSerializable, ICloneable
  {
    public CollectionBackupPolicy() => this.CollectionBackupType = CollectionBackupType.Invalid;

    [JsonProperty(PropertyName = "type")]
    public CollectionBackupType CollectionBackupType
    {
      get => (CollectionBackupType) this.GetValue<int>("type");
      set => this.SetValue("type", (object) (int) value);
    }

    public object Clone() => (object) new CollectionBackupPolicy()
    {
      CollectionBackupType = this.CollectionBackupType
    };
  }
}
