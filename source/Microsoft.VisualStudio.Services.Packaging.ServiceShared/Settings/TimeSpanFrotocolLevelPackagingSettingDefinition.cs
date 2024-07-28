// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.TimeSpanFrotocolLevelPackagingSettingDefinition
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Globalization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class TimeSpanFrotocolLevelPackagingSettingDefinition : 
    IFrotocolLevelPackagingSettingDefinition<TimeSpan>
  {
    public TimeSpanFrotocolLevelPackagingSettingDefinition(
      IFrotocolLevelPackagingSettingDefinition<string?> innerDefinition,
      TimeSpan defaultValue,
      TimeUnit? timeUnitForNumericValues = null)
    {
      this.InnerDefinition = innerDefinition;
      this.TimeUnitForNumericValues = timeUnitForNumericValues;
      this.DefaultValue = defaultValue;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IFrotocolLevelPackagingSettingDefinition<string?> InnerDefinition { get; }

    public TimeUnit? TimeUnitForNumericValues { get; }

    public TimeSpan DefaultValue { get; }

    public IFrotocolLevelPackagingSetting<TimeSpan> Bootstrap(IVssRequestContext requestContext) => (IFrotocolLevelPackagingSetting<TimeSpan>) new TimeSpanFrotocolLevelPackagingSettingDefinition.TimeSpanFrotocolLevelPackagingSetting(this, this.InnerDefinition.Bootstrap(requestContext));

    private class TimeSpanFrotocolLevelPackagingSetting : IFrotocolLevelPackagingSetting<TimeSpan>
    {
      public TimeSpanFrotocolLevelPackagingSetting(
        TimeSpanFrotocolLevelPackagingSettingDefinition definition,
        IFrotocolLevelPackagingSetting<string?> innerSetting)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003Cdefinition\u003EP = definition;
        // ISSUE: reference to a compiler-generated field
        this.\u003CinnerSetting\u003EP = innerSetting;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      public TimeSpan Get(IFeedRequest feedRequest)
      {
        // ISSUE: reference to a compiler-generated field
        string str = this.\u003CinnerSetting\u003EP.Get(feedRequest);
        if (str == null)
        {
          // ISSUE: reference to a compiler-generated field
          return this.\u003Cdefinition\u003EP.DefaultValue;
        }
        TimeSpan result1;
        if (TimeSpan.TryParse(str, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
          return result1;
        // ISSUE: reference to a compiler-generated field
        TimeUnit? forNumericValues = this.\u003Cdefinition\u003EP.TimeUnitForNumericValues;
        double result2;
        if (forNumericValues.HasValue && double.TryParse(str, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
        {
          // ISSUE: reference to a compiler-generated field
          forNumericValues = this.\u003Cdefinition\u003EP.TimeUnitForNumericValues;
          if (forNumericValues.HasValue)
          {
            switch (forNumericValues.GetValueOrDefault())
            {
              case TimeUnit.Minutes:
                return TimeSpan.FromMinutes(result2);
              case TimeUnit.Seconds:
                return TimeSpan.FromSeconds(result2);
            }
          }
          throw new ArgumentOutOfRangeException();
        }
        throw new FormatException("Unable to convert setting value " + str + " to a TimeSpan");
      }
    }
  }
}
