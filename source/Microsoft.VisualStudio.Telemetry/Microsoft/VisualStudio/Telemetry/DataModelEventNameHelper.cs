// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.DataModelEventNameHelper
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  public static class DataModelEventNameHelper
  {
    private const char Separator = '/';

    public static void SetProductFeatureEntityName(OperationEvent operationEvent) => DataModelEventNameHelper.SetProductFeatureEntityName(operationEvent.Name, operationEvent.ReservedProperties);

    public static void SetProductFeatureEntityName(FaultEvent faultEvent) => DataModelEventNameHelper.SetProductFeatureEntityName(faultEvent.Name, faultEvent.ReservedProperties);

    public static void SetProductFeatureEntityName(AssetEvent assetEvent) => DataModelEventNameHelper.SetProductFeatureEntityName(assetEvent.Name, assetEvent.ReservedProperties);

    private static void SetProductFeatureEntityName(
      string eventName,
      TelemetryPropertyBags.PrefixedNotConcurrent<object> reservedProperties)
    {
      DataModelEventNameHelper.ValidateEventName(eventName);
      int length = eventName.IndexOf('/');
      int num = eventName.LastIndexOf('/');
      reservedProperties.AddPrefixed("Reserved.DataModel.ProductName", (object) eventName.Substring(0, length));
      reservedProperties.AddPrefixed("Reserved.DataModel.FeatureName", (object) eventName.Substring(length + 1, num - length - 1));
      reservedProperties.AddPrefixed("Reserved.DataModel.EntityName", (object) eventName.Substring(num + 1));
    }

    private static void ValidateEventName(string eventName)
    {
      int num = 0;
      eventName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (eventName));
      if (eventName[0] == '/' || eventName[eventName.Length - 1] == '/')
        throw new ArgumentException("Event name '" + eventName + "' shouldn't start or end with slash.", nameof (eventName));
      for (int index = 0; index < eventName.Length; ++index)
      {
        if (eventName[index] == ' ')
          throw new ArgumentException("Event name '" + eventName + "' shouldn't have whitespaces.", nameof (eventName));
        if (eventName[index] == '/')
        {
          ++num;
          if (index > 0 && eventName[index - 1] == '/')
            throw new ArgumentException("Event name '" + eventName + "' shouldn't have successive slashes.", nameof (eventName));
        }
      }
      if (num < 2)
        throw new ArgumentException("Event name '" + eventName + "' should have at least 3 parts separated by slash.", nameof (eventName));
    }
  }
}
