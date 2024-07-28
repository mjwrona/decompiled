// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ChangeFeedPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
