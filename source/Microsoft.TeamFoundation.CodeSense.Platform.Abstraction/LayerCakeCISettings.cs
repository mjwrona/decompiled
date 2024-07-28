// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.LayerCakeCISettings
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using System;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public class LayerCakeCISettings
  {
    public bool IsLayerCakeSamplingEnabled { get; set; }

    public int ProbabilityInverse { get; set; }

    public TimeSpan TimeElapsedThresholdForLayerCakeCI { get; set; }
  }
}
