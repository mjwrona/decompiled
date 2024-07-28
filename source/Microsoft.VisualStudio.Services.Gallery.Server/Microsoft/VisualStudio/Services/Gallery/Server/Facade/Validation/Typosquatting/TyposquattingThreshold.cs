// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.Typosquatting.TyposquattingThreshold
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.Typosquatting
{
  public class TyposquattingThreshold
  {
    public int LowerBound { get; }

    public int UpperBound { get; }

    public int ThresholdValue { get; }

    public TyposquattingThreshold(int lowerBound, int upperBound, int thresholdValue)
    {
      this.LowerBound = lowerBound;
      this.UpperBound = upperBound;
      this.ThresholdValue = thresholdValue;
    }

    public static IList<TyposquattingThreshold> LoadThresholds(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntryCollection,
      string thresholdStringPathInRegistry,
      string thresholdStringDefaultValue)
    {
      requestContext.TraceEnter(12062112, "gallery", nameof (TyposquattingThreshold), nameof (LoadThresholds));
      string valueFromPath = registryEntryCollection.GetValueFromPath<string>(thresholdStringPathInRegistry, thresholdStringDefaultValue);
      requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (LoadThresholds), "Parsing the threshold string {0} from Registry: {1} with a default value {2}.", (object) valueFromPath, (object) thresholdStringPathInRegistry, (object) thresholdStringDefaultValue);
      IList<TyposquattingThreshold> thresholds;
      if (TyposquattingThreshold.TryParseThresholdString(requestContext, valueFromPath, out thresholds))
      {
        requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (LoadThresholds), "Parsed the threshold string {0} successfully.", (object) valueFromPath);
      }
      else
      {
        TyposquattingThreshold.TryParseThresholdString(requestContext, thresholdStringDefaultValue, out thresholds);
        requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (LoadThresholds), "Failed to parse the threshold string {0} and used the default value {1} instead.", (object) valueFromPath, (object) thresholdStringDefaultValue);
      }
      requestContext.TraceLeave(12062112, "gallery", nameof (TyposquattingThreshold), nameof (LoadThresholds));
      return thresholds;
    }

    internal static bool TryParseThresholdString(
      IVssRequestContext requestContext,
      string thresholdString,
      out IList<TyposquattingThreshold> thresholds)
    {
      requestContext.TraceEnter(12062112, "gallery", nameof (TyposquattingThreshold), nameof (TryParseThresholdString));
      thresholds = (IList<TyposquattingThreshold>) new List<TyposquattingThreshold>();
      bool thresholdString1 = true;
      if (string.IsNullOrWhiteSpace(thresholdString))
      {
        thresholdString1 = false;
      }
      else
      {
        string[] strArray1 = thresholdString.Split(';');
        requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (TryParseThresholdString), "The threshold string {0} has {1} entries.", (object) thresholdString, (object) strArray1.Length);
        foreach (string str in strArray1)
        {
          if (string.IsNullOrWhiteSpace(str))
          {
            thresholdString1 = false;
            requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (TryParseThresholdString), "Failed to parse the threshold entry {0}.", (object) str);
            break;
          }
          string[] strArray2 = str.Split(':');
          int result1;
          if (strArray2.Length != 2 || string.IsNullOrWhiteSpace(strArray2[0]) || string.IsNullOrWhiteSpace(strArray2[1]) || !int.TryParse(strArray2[1], out result1))
          {
            thresholdString1 = false;
            requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (TryParseThresholdString), "Failed to parse the threshold entry {0}.", (object) str);
            break;
          }
          string[] strArray3 = strArray2[0].Split('/');
          int result2;
          int result3;
          if (strArray3.Length != 2 || string.IsNullOrWhiteSpace(strArray3[0]) || string.IsNullOrWhiteSpace(strArray3[1]) || !int.TryParse(strArray3[0], out result2) || !int.TryParse(strArray3[1], out result3) || result2 > result3)
          {
            thresholdString1 = false;
            requestContext.TraceAlways(12062112, TraceLevel.Info, "gallery", nameof (TryParseThresholdString), "Failed to parse the threshold entry {0}.", (object) str);
            break;
          }
          thresholds.Add(new TyposquattingThreshold(result2, result3, result1));
        }
      }
      requestContext.TraceLeave(12062112, "gallery", nameof (TyposquattingThreshold), nameof (TryParseThresholdString));
      return thresholdString1;
    }

    public static int GetThreshold(string displayName, IList<TyposquattingThreshold> thresholds)
    {
      foreach (TyposquattingThreshold threshold in (IEnumerable<TyposquattingThreshold>) thresholds)
      {
        if (displayName.Length >= threshold.LowerBound && displayName.Length < threshold.UpperBound)
          return threshold.ThresholdValue;
      }
      return thresholds.Last<TyposquattingThreshold>().ThresholdValue;
    }
  }
}
