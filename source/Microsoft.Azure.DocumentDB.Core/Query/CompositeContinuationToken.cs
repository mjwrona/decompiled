// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.CompositeContinuationToken
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class CompositeContinuationToken
  {
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("range")]
    [JsonConverter(typeof (RangeJsonConverter))]
    public Microsoft.Azure.Documents.Routing.Range<string> Range { get; set; }

    public object ShallowCopy() => this.MemberwiseClone();
  }
}
