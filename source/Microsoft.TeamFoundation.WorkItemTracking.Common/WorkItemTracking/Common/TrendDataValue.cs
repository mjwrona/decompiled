// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.TrendDataValue
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TrendDataValue
  {
    private static TrendDataValue s_increase;
    private static TrendDataValue s_decrease;

    public static TrendDataValue Increase
    {
      get
      {
        if (TrendDataValue.s_increase == null)
          TrendDataValue.s_increase = new TrendDataValue();
        return TrendDataValue.s_increase;
      }
    }

    public static TrendDataValue Decrease
    {
      get
      {
        if (TrendDataValue.s_decrease == null)
          TrendDataValue.s_decrease = new TrendDataValue();
        return TrendDataValue.s_decrease;
      }
    }

    private TrendDataValue()
    {
    }
  }
}
