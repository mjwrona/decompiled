// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserPreferences
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public sealed class UserPreferences : BasePreferences
  {
    private const string DefaultTheme = "Default";
    private CultureInfo m_culture;
    private string m_datePattern;
    private string m_timePattern;
    private string m_calendar;
    private string m_webAccessTheme;

    internal UserPreferences Clone()
    {
      UserPreferences bp = new UserPreferences();
      this.CopyTo((BasePreferences) bp);
      bp.Calendar = this.Calendar;
      bp.DatePattern = this.DatePattern;
      bp.TimePattern = this.TimePattern;
      bp.WebAccessTheme = "Default";
      bp.TypeAheadDisabled = this.TypeAheadDisabled;
      bp.WorkItemFormChromeBorder = this.WorkItemFormChromeBorder;
      return bp;
    }

    [DataMember(IsRequired = false)]
    public override CultureInfo Culture
    {
      get
      {
        if (this.m_culture == null && base.Culture != null)
        {
          this.m_culture = (CultureInfo) base.Culture.Clone();
          if (!string.IsNullOrEmpty(this.Calendar))
          {
            DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo) this.m_culture.DateTimeFormat.Clone();
            foreach (System.Globalization.Calendar optionalCalendar in this.m_culture.OptionalCalendars)
            {
              dateTimeFormatInfo.Calendar = optionalCalendar;
              if (string.Equals(dateTimeFormatInfo.NativeCalendarName, this.Calendar))
              {
                this.m_culture.DateTimeFormat.Calendar = optionalCalendar;
                break;
              }
            }
          }
          if (!string.IsNullOrEmpty(this.DatePattern))
            this.m_culture.DateTimeFormat.ShortDatePattern = this.DatePattern;
          if (!string.IsNullOrEmpty(this.TimePattern) && (!string.IsNullOrEmpty(this.m_culture.DateTimeFormat.AMDesignator) || !string.IsNullOrEmpty(this.m_culture.DateTimeFormat.PMDesignator) || this.TimePattern.IndexOf("tt", StringComparison.OrdinalIgnoreCase) < 0))
            this.m_culture.DateTimeFormat.ShortTimePattern = this.TimePattern;
        }
        return this.m_culture;
      }
      set
      {
        base.Culture = value;
        this.m_culture = (CultureInfo) null;
      }
    }

    [DataMember(IsRequired = false)]
    public string DatePattern
    {
      get => this.m_datePattern;
      set
      {
        this.m_datePattern = value;
        this.m_culture = (CultureInfo) null;
      }
    }

    [DataMember(IsRequired = false)]
    public string TimePattern
    {
      get => this.m_timePattern;
      set
      {
        this.m_timePattern = value;
        this.m_culture = (CultureInfo) null;
      }
    }

    [DataMember(IsRequired = false)]
    public string Calendar
    {
      get => this.m_calendar;
      set
      {
        this.m_calendar = value;
        this.m_culture = (CultureInfo) null;
      }
    }

    [DataMember(IsRequired = false)]
    public string WebAccessTheme
    {
      get => "Default";
      set => this.m_webAccessTheme = value;
    }

    [DataMember(IsRequired = false)]
    public bool TypeAheadDisabled { get; set; }

    [DataMember(IsRequired = false)]
    public bool WorkItemFormChromeBorder { set; get; }
  }
}
