// Decompiled with JetBrains decompiler
// Type: Nest.HDRHistogramMethodDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HDRHistogramMethodDescriptor : 
    DescriptorBase<HDRHistogramMethodDescriptor, IHDRHistogramMethod>,
    IHDRHistogramMethod,
    IPercentilesMethod
  {
    int? IHDRHistogramMethod.NumberOfSignificantValueDigits { get; set; }

    public HDRHistogramMethodDescriptor NumberOfSignificantValueDigits(int? numDigits) => this.Assign<int?>(numDigits, (Action<IHDRHistogramMethod, int?>) ((a, v) => a.NumberOfSignificantValueDigits = v));
  }
}
