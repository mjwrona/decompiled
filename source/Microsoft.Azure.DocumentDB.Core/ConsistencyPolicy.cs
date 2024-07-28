// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConsistencyPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  public sealed class ConsistencyPolicy : JsonSerializable
  {
    private const ConsistencyLevel defaultDefaultConsistencyLevel = ConsistencyLevel.Session;
    internal const int DefaultMaxStalenessInterval = 5;
    internal const int DefaultMaxStalenessPrefix = 100;
    internal const int MaxStalenessIntervalInSecondsMinValue = 5;
    internal const int MaxStalenessIntervalInSecondsMaxValue = 86400;
    internal const int MaxStalenessPrefixMinValue = 10;
    internal const int MaxStalenessPrefixMaxValue = 1000000;

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "defaultConsistencyLevel")]
    public ConsistencyLevel DefaultConsistencyLevel
    {
      get => this.GetValue<ConsistencyLevel>("defaultConsistencyLevel", ConsistencyLevel.Session);
      set => this.SetValue("defaultConsistencyLevel", (object) value.ToString());
    }

    [JsonProperty(PropertyName = "maxStalenessPrefix")]
    public int MaxStalenessPrefix
    {
      get => this.GetValue<int>("maxStalenessPrefix", 100);
      set => this.SetValue("maxStalenessPrefix", (object) value);
    }

    [JsonProperty(PropertyName = "maxIntervalInSeconds")]
    public int MaxStalenessIntervalInSeconds
    {
      get => this.GetValue<int>("maxIntervalInSeconds", 5);
      set => this.SetValue("maxIntervalInSeconds", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateNonNegativeInteger("maxStalenessPrefix", this.MaxStalenessPrefix);
      Helpers.ValidateNonNegativeInteger("maxIntervalInSeconds", this.MaxStalenessIntervalInSeconds);
      if (this.DefaultConsistencyLevel == ConsistencyLevel.BoundedStaleness && (this.MaxStalenessIntervalInSeconds < 5 || this.MaxStalenessIntervalInSeconds > 86400))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidMaxStalenessInterval, (object) 5, (object) 86400));
      if (this.DefaultConsistencyLevel == ConsistencyLevel.BoundedStaleness && (this.MaxStalenessPrefix < 10 || this.MaxStalenessPrefix > 1000000))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidMaxStalenessPrefix, (object) 10, (object) 1000000));
    }
  }
}
