// Decompiled with JetBrains decompiler
// Type: Nest.FieldTypes
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class FieldTypes : IsADictionaryBase<string, FieldCapabilities>
  {
    public FieldCapabilities All
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_all", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Attachment
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("attachment", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Binary
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("binary", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Boolean
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("boolean", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Byte
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("byte", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Completion
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("completion", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Date
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("date", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities DateNanos
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("date_nanos", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities DateRange
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("date_range", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities DenseVector
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("dense_vector", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Double
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("double", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities DoubleRange
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("double_range", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities FieldNames
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_field_names", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Float
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("float", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities FloatRange
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("float_range", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities GeoPoint
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("geo_point", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities GeoShape
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("geo_shape", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities HalfFloat
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("half_float", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Id
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_id", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Index
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_index", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Integer
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("integer", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities IntegerRange
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("integer_range", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Ip
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("ip", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Keyword
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("keyword", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Long
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("long", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities LongRange
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("long_range", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities MatchOnlyText
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("match_only_text", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Murmur3
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("murmur3", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Parent
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_parent", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities ParentJoin
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_parent_join", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Percolator
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("percolator", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Routing
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_routing", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities ScaledFloat
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("scaled_float", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities SearchAsYouType
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("search_as_you_type", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Shape
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("shape", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Short
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("short", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Source
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_source", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Text
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("text", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Tier
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_tier", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities TokenCount
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("token_count", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Type
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_type", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Uid
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_uid", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities Version
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("_version", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }

    public FieldCapabilities VersionField
    {
      get
      {
        FieldCapabilities fieldCapabilities;
        return !this.BackingDictionary.TryGetValue("version", out fieldCapabilities) ? (FieldCapabilities) null : fieldCapabilities;
      }
    }
  }
}
