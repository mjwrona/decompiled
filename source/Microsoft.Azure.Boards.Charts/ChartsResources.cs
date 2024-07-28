// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.ChartsResources
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Boards.Charts
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class ChartsResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ChartsResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (ChartsResources.resourceMan == null)
          ChartsResources.resourceMan = new ResourceManager("Microsoft.Azure.Boards.Charts.Resources.ChartsResources", typeof (ChartsResources).Assembly);
        return ChartsResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => ChartsResources.resourceCulture;
      set => ChartsResources.resourceCulture = value;
    }

    public static string AvailableCapacitySeries => ChartsResources.ResourceManager.GetString(nameof (AvailableCapacitySeries), ChartsResources.resourceCulture);

    public static string Burndown => ChartsResources.ResourceManager.GetString(nameof (Burndown), ChartsResources.resourceCulture);

    public static string BurndownActualTrendSeries => ChartsResources.ResourceManager.GetString(nameof (BurndownActualTrendSeries), ChartsResources.resourceCulture);

    public static Bitmap BurndownChart_ZeroData_Dark => (Bitmap) ChartsResources.ResourceManager.GetObject(nameof (BurndownChart_ZeroData_Dark), ChartsResources.resourceCulture);

    public static Bitmap BurndownChart_ZeroData_Light => (Bitmap) ChartsResources.ResourceManager.GetObject(nameof (BurndownChart_ZeroData_Light), ChartsResources.resourceCulture);

    public static string BurndownIdealTrendSeries => ChartsResources.ResourceManager.GetString(nameof (BurndownIdealTrendSeries), ChartsResources.resourceCulture);

    public static string BurndownRemainingWorkAxisTitle => ChartsResources.ResourceManager.GetString(nameof (BurndownRemainingWorkAxisTitle), ChartsResources.resourceCulture);

    public static string BurndownRemainingWorkSeries => ChartsResources.ResourceManager.GetString(nameof (BurndownRemainingWorkSeries), ChartsResources.resourceCulture);

    public static string BurndownValidation_Dates => ChartsResources.ResourceManager.GetString(nameof (BurndownValidation_Dates), ChartsResources.resourceCulture);

    public static string BurndownWorkItemLimitExceededMessage => ChartsResources.ResourceManager.GetString(nameof (BurndownWorkItemLimitExceededMessage), ChartsResources.resourceCulture);

    public static Bitmap CFDChart_ZeroData_Dark => (Bitmap) ChartsResources.ResourceManager.GetObject(nameof (CFDChart_ZeroData_Dark), ChartsResources.resourceCulture);

    public static Bitmap CFDChart_ZeroData_Light => (Bitmap) ChartsResources.ResourceManager.GetObject(nameof (CFDChart_ZeroData_Light), ChartsResources.resourceCulture);

    public static string CumulativeFlow => ChartsResources.ResourceManager.GetString(nameof (CumulativeFlow), ChartsResources.resourceCulture);

    public static string CumulativeFlowDeletedColumn => ChartsResources.ResourceManager.GetString(nameof (CumulativeFlowDeletedColumn), ChartsResources.resourceCulture);

    public static string CumulativeFlowYAxisTitle => ChartsResources.ResourceManager.GetString(nameof (CumulativeFlowYAxisTitle), ChartsResources.resourceCulture);

    public static string InvalidChartDimensionsMessage => ChartsResources.ResourceManager.GetString(nameof (InvalidChartDimensionsMessage), ChartsResources.resourceCulture);

    public static string IterationHasNoStartOrEndDates => ChartsResources.ResourceManager.GetString(nameof (IterationHasNoStartOrEndDates), ChartsResources.resourceCulture);

    public static string IterationNotFound => ChartsResources.ResourceManager.GetString(nameof (IterationNotFound), ChartsResources.resourceCulture);

    public static string NoIterationsSetForTeamMessage => ChartsResources.ResourceManager.GetString(nameof (NoIterationsSetForTeamMessage), ChartsResources.resourceCulture);

    public static string Today => ChartsResources.ResourceManager.GetString(nameof (Today), ChartsResources.resourceCulture);

    public static string Velocity => ChartsResources.ResourceManager.GetString(nameof (Velocity), ChartsResources.resourceCulture);

    public static Bitmap VelocityChart_ZeroData_Dark => (Bitmap) ChartsResources.ResourceManager.GetObject(nameof (VelocityChart_ZeroData_Dark), ChartsResources.resourceCulture);

    public static Bitmap VelocityChart_ZeroData_Light => (Bitmap) ChartsResources.ResourceManager.GetObject(nameof (VelocityChart_ZeroData_Light), ChartsResources.resourceCulture);

    public static string VelocityCompletedSeries => ChartsResources.ResourceManager.GetString(nameof (VelocityCompletedSeries), ChartsResources.resourceCulture);

    public static string VelocityRemainingSeries => ChartsResources.ResourceManager.GetString(nameof (VelocityRemainingSeries), ChartsResources.resourceCulture);

    public static string VelocityValidation_IterationsMissing => ChartsResources.ResourceManager.GetString(nameof (VelocityValidation_IterationsMissing), ChartsResources.resourceCulture);
  }
}
