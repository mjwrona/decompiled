// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CalendarModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class CalendarModel
  {
    public CalendarModel(DateTimeFormatInfo dateTimeFormatInfo, Calendar calendar, DateTime now)
    {
      DateTimeFormatInfo provider = (DateTimeFormatInfo) dateTimeFormatInfo.Clone();
      provider.Calendar = calendar;
      this.DisplayName = provider.NativeCalendarName;
      string[] dateTimePatterns1 = provider.GetAllDateTimePatterns('d');
      this.DateFormats = new List<PatternModel>(dateTimePatterns1.Length);
      foreach (string str1 in dateTimePatterns1)
      {
        try
        {
          string format = str1.Replace("/", "'/'");
          string str2 = now.ToString(format);
          this.DateFormats.Add(new PatternModel()
          {
            Format = format,
            DisplayFormat = string.Format((IFormatProvider) provider, "{0:" + str1 + "} ({1})", (object) str2, (object) str1)
          });
        }
        catch (FormatException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, nameof (CalendarModel), (Exception) ex);
        }
      }
      string[] dateTimePatterns2 = provider.GetAllDateTimePatterns('t');
      this.TimeFormats = new List<PatternModel>(dateTimePatterns2.Length);
      foreach (string str in dateTimePatterns2)
      {
        try
        {
          this.TimeFormats.Add(new PatternModel()
          {
            Format = str,
            DisplayFormat = string.Format((IFormatProvider) provider, "{0:" + str + "} ({1})", (object) now, (object) str)
          });
        }
        catch (FormatException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, nameof (CalendarModel), (Exception) ex);
        }
      }
    }

    public string DisplayName { get; private set; }

    public List<PatternModel> DateFormats { get; private set; }

    public List<PatternModel> TimeFormats { get; private set; }
  }
}
