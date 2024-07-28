// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosSerializationUtil
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.Cosmos
{
  internal static class CosmosSerializationUtil
  {
    private static readonly CamelCaseNamingStrategy camelCaseNamingStrategy = new CamelCaseNamingStrategy();

    internal static string ToCamelCase(string name) => CosmosSerializationUtil.camelCaseNamingStrategy.GetPropertyName(name, false);

    internal static string GetStringWithPropertyNamingPolicy(
      CosmosLinqSerializerOptions options,
      string name)
    {
      return options != null && options.PropertyNamingPolicy == CosmosPropertyNamingPolicy.CamelCase ? CosmosSerializationUtil.ToCamelCase(name) : name;
    }
  }
}
