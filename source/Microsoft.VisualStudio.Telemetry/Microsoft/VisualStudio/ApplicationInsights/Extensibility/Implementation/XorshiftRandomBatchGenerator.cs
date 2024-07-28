// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.XorshiftRandomBatchGenerator
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class XorshiftRandomBatchGenerator : IRandomNumberBatchGenerator
  {
    private const ulong Y = 4477743899113974427;
    private const ulong Z = 2994213561913849757;
    private const ulong W = 9123831478480964153;
    private ulong lastX;
    private ulong lastY;
    private ulong lastZ;
    private ulong lastW;

    public XorshiftRandomBatchGenerator(ulong seed)
    {
      this.lastX = (ulong) ((long) seed * 5073061188973594169L + (long) seed * 8760132611124384359L + (long) seed * 8900702462021224483L + (long) seed * 6807056130438027397L);
      this.lastY = 4477743899113974427UL;
      this.lastZ = 2994213561913849757UL;
      this.lastW = 9123831478480964153UL;
    }

    public void NextBatch(ulong[] buffer, int index, int count)
    {
      ulong num1 = this.lastX;
      ulong num2 = this.lastY;
      ulong num3 = this.lastZ;
      ulong num4 = this.lastW;
      for (int index1 = 0; index1 < count; ++index1)
      {
        ulong num5 = num1 ^ num1 << 11;
        num1 = num2;
        num2 = num3;
        num3 = num4;
        num4 = (ulong) ((long) num4 ^ (long) (num4 >> 19) ^ (long) num5 ^ (long) (num5 >> 8));
        buffer[index + index1] = num4;
      }
      this.lastX = num1;
      this.lastY = num2;
      this.lastZ = num3;
      this.lastW = num4;
    }
  }
}
