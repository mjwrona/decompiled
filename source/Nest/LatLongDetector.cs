// Decompiled with JetBrains decompiler
// Type: Nest.LatLongDetector
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class LatLongDetector : 
    DetectorBase,
    IGeographicDetector,
    IDetector,
    IByFieldNameDetector,
    IOverFieldNameDetector,
    IPartitionFieldNameDetector,
    IFieldNameDetector
  {
    public LatLongDetector()
      : base(GeographicFunction.LatLong.GetStringValue())
    {
    }

    public Field ByFieldName { get; set; }

    public Field FieldName { get; set; }

    public Field OverFieldName { get; set; }

    public Field PartitionFieldName { get; set; }
  }
}
