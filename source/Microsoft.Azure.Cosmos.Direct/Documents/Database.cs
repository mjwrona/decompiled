// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Database
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class Database : Resource
  {
    [JsonProperty(PropertyName = "_colls")]
    public string CollectionsLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_colls");

    [JsonProperty(PropertyName = "_users")]
    public string UsersLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_users");

    internal string UserDefinedTypesLink => this.SelfLink.TrimEnd('/') + "/udts/";

    internal override void Validate() => base.Validate();
  }
}
