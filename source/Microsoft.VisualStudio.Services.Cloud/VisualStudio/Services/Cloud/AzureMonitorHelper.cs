// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureMonitorHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class AzureMonitorHelper
  {
    public static (DateTime startTime, DateTime endTime) GetStartAndEndDateTime(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      string azureMetricsQueryFormat,
      string name,
      TimeSpan azureMetricsRetentionTimespan)
    {
      DateTime dateTime1 = AzureMonitorHelper.RoundDown(DateTime.Parse(AzureMonitorHelper.GetLastProcessedStartDateTimeString(requestContext, registryService, azureMetricsQueryFormat, name), (IFormatProvider) null, DateTimeStyles.RoundtripKind), TimeSpan.FromMinutes(1.0));
      DateTime dateTime2 = AzureMonitorHelper.RoundDown(DateTime.UtcNow, TimeSpan.FromMinutes(1.0));
      if (dateTime2 - dateTime1 > azureMetricsRetentionTimespan)
        dateTime1 = dateTime2 - azureMetricsRetentionTimespan;
      return (dateTime1, dateTime2);
    }

    public static string GetLastProcessedStartDateTimeString(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      string azureMetricsQueryFormat,
      string name)
    {
      string startDateTimeString = registryService.GetValue(requestContext, (RegistryQuery) string.Format(azureMetricsQueryFormat, (object) name), false, (string) null);
      if (string.IsNullOrEmpty(startDateTimeString))
        startDateTimeString = AzureMonitorHelper.GetDefaultLastProcessedStartDateTimeString();
      return startDateTimeString;
    }

    public static string GetDefaultLastProcessedStartDateTimeString()
    {
      DateTime dateTime = DateTime.UtcNow;
      dateTime = dateTime.AddHours(-1.0);
      return dateTime.ToString("o");
    }

    public static DateTime RoundDown(DateTime timeToBeRoundedDown, TimeSpan timeSpan) => new DateTime(timeToBeRoundedDown.Ticks - timeToBeRoundedDown.Ticks % timeSpan.Ticks);

    public sealed class GenericResourceComparer : IEqualityComparer<GenericResource>
    {
      public bool Equals(GenericResource x, GenericResource y)
      {
        if (x == null && y == null)
          return true;
        return x != null && y != null && string.Equals(((ResourceData) x.Data)?.Name, ((ResourceData) y.Data)?.Name, StringComparison.OrdinalIgnoreCase);
      }

      public int GetHashCode(GenericResource obj)
      {
        string name = ((ResourceData) obj?.Data)?.Name;
        return name == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(name);
      }
    }
  }
}
