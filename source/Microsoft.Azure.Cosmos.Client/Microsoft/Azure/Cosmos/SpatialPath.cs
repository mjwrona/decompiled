// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SpatialPath
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos
{
  public sealed class SpatialPath
  {
    private Collection<SpatialType> spatialTypesInternal;

    [JsonProperty(PropertyName = "path")]
    public string Path { get; set; }

    [JsonProperty(PropertyName = "types", ItemConverterType = typeof (StringEnumConverter))]
    public Collection<SpatialType> SpatialTypes
    {
      get
      {
        if (this.spatialTypesInternal == null)
          this.spatialTypesInternal = new Collection<SpatialType>();
        return this.spatialTypesInternal;
      }
      internal set => this.spatialTypesInternal = value ?? throw new ArgumentNullException();
    }

    [JsonProperty(PropertyName = "boundingBox", NullValueHandling = NullValueHandling.Ignore)]
    public BoundingBoxProperties BoundingBox { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
