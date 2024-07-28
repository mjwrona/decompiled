// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CultureInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class CultureInfoModel
  {
    private Dictionary<string, CalendarModel> m_optionalCalendars = new Dictionary<string, CalendarModel>();

    public CultureInfoModel(CultureInfo cultureInfo, DateTime now)
    {
      this.DisplayName = cultureInfo.NativeName;
      this.LCID = new int?(cultureInfo.LCID);
      this.Language = cultureInfo.IetfLanguageTag;
      foreach (Calendar optionalCalendar in cultureInfo.OptionalCalendars)
      {
        CalendarModel calendarModel = new CalendarModel(cultureInfo.DateTimeFormat, optionalCalendar, now);
        if (!this.m_optionalCalendars.ContainsKey(calendarModel.DisplayName))
          this.m_optionalCalendars[calendarModel.DisplayName] = calendarModel;
      }
    }

    public CalendarModel[] OptionalCalendars => this.m_optionalCalendars.Values.ToArray<CalendarModel>();

    public virtual string DisplayName { get; private set; }

    public int? LCID { get; private set; }

    public string Language { get; set; }
  }
}
