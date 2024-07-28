// Decompiled with JetBrains decompiler
// Type: Nest.PercentilesMethodDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PercentilesMethodDescriptor : 
    DescriptorBase<PercentilesMethodDescriptor, IPercentilesMethod>,
    IPercentilesMethod
  {
    public IPercentilesMethod HDRHistogram(
      Func<HDRHistogramMethodDescriptor, IHDRHistogramMethod> hdrSelector = null)
    {
      return (IPercentilesMethod) hdrSelector.InvokeOrDefault<HDRHistogramMethodDescriptor, IHDRHistogramMethod>(new HDRHistogramMethodDescriptor());
    }

    public IPercentilesMethod TDigest(
      Func<TDigestMethodDescriptor, ITDigestMethod> tdigestSelector = null)
    {
      return (IPercentilesMethod) tdigestSelector.InvokeOrDefault<TDigestMethodDescriptor, ITDigestMethod>(new TDigestMethodDescriptor());
    }
  }
}
