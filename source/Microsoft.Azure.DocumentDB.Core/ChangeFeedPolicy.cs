// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ChangeFeedPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class ChangeFeedPolicy : JsonSerializable, ICloneable
  {
    [JsonProperty(PropertyName = "retentionDuration")]
    public TimeSpan RetentionDuration
    {
      get => TimeSpan.FromMinutes((double) this.GetValue<int>("retentionDuration"));
      set
      {
        TimeSpan timeSpan = value;
        this.SetValue("retentionDuration", (object) ((int) timeSpan.TotalMinutes + (timeSpan.Seconds > 0 ? 1 : 0)));
      }
    }

    public object Clone() => (object) new ChangeFeedPolicy()
    {
      RetentionDuration = this.RetentionDuration
    };
  }
}
