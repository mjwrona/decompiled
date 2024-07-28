// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.UpdateKeepUntilReferenceHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class UpdateKeepUntilReferenceHelper
  {
    public const double FuzzMinStepPercent = 0.05;
    public const double ScalingStepPercent = 0.01;
    public static readonly TimeSpan ClockSkewBuffer = AzureStorageConstants.MaxExpectedClockSkew;
    public static readonly TimeSpan MinStep = TimeSpan.FromDays(1.0);
    public static readonly long MaxFuzzTimeInTicks = (long) (0.05 * (double) UpdateKeepUntilReferenceHelper.MinStep.Ticks);

    public static bool TryGetNewKeepUntil(
      DateTime? existingKeepUntil,
      DateTime requestedKeepUntil,
      out DateTime keepUntilToSet)
    {
      bool newKeepUntil;
      if (existingKeepUntil.HasValue)
      {
        DateTime dateTime = requestedKeepUntil + UpdateKeepUntilReferenceHelper.ClockSkewBuffer;
        DateTime? nullable = existingKeepUntil;
        if ((nullable.HasValue ? (dateTime > nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          keepUntilToSet = existingKeepUntil.Value;
          newKeepUntil = false;
          goto label_4;
        }
      }
      TimeSpan timeSpan = TimeSpan.FromTicks((long) (ThreadLocalRandom.Generator.NextDouble() * (double) UpdateKeepUntilReferenceHelper.MaxFuzzTimeInTicks));
      TimeSpan b = !existingKeepUntil.HasValue ? TimeSpan.Zero : TimeSpan.FromTicks((long) ((double) (requestedKeepUntil - existingKeepUntil.Value).Ticks * 0.01));
      TimeSpan maxTimeSpan = UpdateKeepUntilReferenceHelper.GetMaxTimeSpan(UpdateKeepUntilReferenceHelper.MinStep, b);
      keepUntilToSet = requestedKeepUntil + UpdateKeepUntilReferenceHelper.ClockSkewBuffer + maxTimeSpan + timeSpan;
      newKeepUntil = true;
label_4:
      if (keepUntilToSet < requestedKeepUntil)
        throw new InvalidOperationException("Attempting to set a keepUntil that is less than the requested value.");
      if (existingKeepUntil.HasValue && keepUntilToSet < existingKeepUntil.Value)
        throw new InvalidOperationException("Attempting to set a keepUntil that is less than the existing value.");
      return newKeepUntil;
    }

    private static TimeSpan GetMaxTimeSpan(TimeSpan a, TimeSpan b) => a > b ? a : b;
  }
}
