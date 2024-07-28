// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SpatialSpec
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class SpatialSpec : JsonSerializable
  {
    private Collection<SpatialType> spatialTypes;
    private BoundingBoxSpec boundingBoxSpec;

    [JsonProperty(PropertyName = "path")]
    public string Path
    {
      get => this.GetValue<string>("path");
      set => this.SetValue("path", (object) value);
    }

    [JsonProperty(PropertyName = "types", ItemConverterType = typeof (StringEnumConverter))]
    public Collection<SpatialType> SpatialTypes
    {
      get
      {
        if (this.spatialTypes == null)
        {
          this.spatialTypes = this.GetValue<Collection<SpatialType>>("types");
          if (this.spatialTypes == null)
            this.spatialTypes = new Collection<SpatialType>();
        }
        return this.spatialTypes;
      }
      set
      {
        this.spatialTypes = value != null ? value : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.PropertyCannotBeNull, (object) nameof (SpatialTypes)));
        this.SetValue("types", (object) value);
      }
    }

    [JsonProperty(PropertyName = "boundingBox", NullValueHandling = NullValueHandling.Ignore)]
    public BoundingBoxSpec BoundingBox
    {
      get => this.GetValue<BoundingBoxSpec>("boundingBox");
      set
      {
        this.boundingBoxSpec = value;
        this.SetValue("boundingBox", (object) this.boundingBoxSpec);
      }
    }

    internal object Clone()
    {
      SpatialSpec spatialSpec = new SpatialSpec()
      {
        Path = this.Path
      };
      foreach (SpatialType spatialType in this.SpatialTypes)
        spatialSpec.SpatialTypes.Add(spatialType);
      if (this.boundingBoxSpec != null)
        spatialSpec.boundingBoxSpec = (BoundingBoxSpec) this.boundingBoxSpec.Clone();
      return (object) spatialSpec;
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("path");
      foreach (SpatialType spatialType in this.SpatialTypes)
        Helpers.ValidateEnumProperties<SpatialType>(spatialType);
      if (this.boundingBoxSpec == null)
        return;
      this.boundingBoxSpec.Validate();
    }

    internal override void OnSave()
    {
      if (this.spatialTypes != null)
        this.SetValue("types", (object) this.spatialTypes);
      if (this.boundingBoxSpec == null)
        return;
      this.boundingBoxSpec.OnSave();
      this.SetValue("boundingBox", (object) this.boundingBoxSpec);
    }
  }
}
