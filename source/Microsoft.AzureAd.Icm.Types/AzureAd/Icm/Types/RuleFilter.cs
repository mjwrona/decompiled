// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.RuleFilter
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class RuleFilter
  {
    [DataMember]
    public Dictionary<string, object> FilterProperties { get; set; }

    public static void ValidateDate(
      Dictionary<string, object> props,
      string fieldName,
      string friendlyName)
    {
      if (!props.ContainsKey(fieldName))
        return;
      object prop = props[fieldName];
      if (prop == null)
        return;
      DateTime dateTime;
      try
      {
        dateTime = Convert.ToDateTime(prop);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case InvalidCastException _:
          case FormatException _:
          case InvalidOperationException _:
            throw new ArgumentException("Invalid date specified: " + friendlyName);
          default:
            throw;
        }
      }
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(dateTime, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, friendlyName, nameof (ValidateDate), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\RuleFilter.cs");
    }

    public static void ValidateString(
      Dictionary<string, object> props,
      string fieldName,
      string friendlyName)
    {
      if (!props.ContainsKey(fieldName))
        return;
      ArgumentCheck.ThrowIfEmptyOrWhiteSpace(props[fieldName] as string, friendlyName, nameof (ValidateString), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\RuleFilter.cs");
    }
  }
}
