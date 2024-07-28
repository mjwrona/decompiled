// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobCoverageEstimator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class BlobCoverageEstimator
  {
    private int max;
    private int aug;
    private int prefixLength;
    private int precision;

    public BlobCoverageEstimator(int prefixLength, int precision)
    {
      this.precision = precision;
      this.prefixLength = prefixLength;
      this.max = (int) Math.Pow(16.0, (double) precision);
      this.aug = (int) Math.Pow(10.0, (double) precision);
    }

    public double EstimatePercentage(string blobName) => (double) (int) ((double) Convert.ToUInt32(blobName.Substring(this.prefixLength, this.precision), 16) / (double) this.max * (double) this.aug) / (double) this.aug * 100.0;
  }
}
