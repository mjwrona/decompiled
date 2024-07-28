// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ClientCultureInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal class ClientCultureInfo
  {
    private const int sm_eraIndex = 0;
    private const int sm_eraName = 1;
    private const int sm_eraStart = 2;
    private const int sm_eraYearOffset = 3;
    private static Dictionary<Type, Action<ClientCultureInfo>> sm_adjustments = new Dictionary<Type, Action<ClientCultureInfo>>();

    private int Adjustment { get; set; }

    private string AdjustmentScriptName { get; set; }

    public string Name { get; private set; }

    public DateTimeFormatInfo DateTimeFormat { get; private set; }

    public NumberFormatInfo NumberFormat { get; private set; }

    public object[] Eras { get; private set; }

    public NumberShortFormInfo NumberShortForm { get; private set; }

    static ClientCultureInfo()
    {
      ClientCultureInfo.sm_adjustments[typeof (JapaneseCalendar)] = new Action<ClientCultureInfo>(ClientCultureInfo.AdjustJapaneseCalendar);
      ClientCultureInfo.sm_adjustments[typeof (KoreanCalendar)] = new Action<ClientCultureInfo>(ClientCultureInfo.AdjustKoreanCalendar);
      ClientCultureInfo.sm_adjustments[typeof (TaiwanCalendar)] = new Action<ClientCultureInfo>(ClientCultureInfo.AdjustTaiwanCalendar);
      ClientCultureInfo.sm_adjustments[typeof (ThaiBuddhistCalendar)] = new Action<ClientCultureInfo>(ClientCultureInfo.AdjustThaiBuddhistCalendar);
      ClientCultureInfo.sm_adjustments[typeof (HijriCalendar)] = new Action<ClientCultureInfo>(ClientCultureInfo.AdjustHijriCalendar);
      ClientCultureInfo.sm_adjustments[typeof (UmAlQuraCalendar)] = new Action<ClientCultureInfo>(ClientCultureInfo.AdjustUmAlQuraCalendar);
    }

    private ClientCultureInfo(CultureInfo cultureInfo)
    {
      this.Name = cultureInfo.Name;
      this.DateTimeFormat = cultureInfo.DateTimeFormat;
      this.NumberFormat = cultureInfo.NumberFormat;
      this.NumberShortForm = new NumberShortFormInfo(cultureInfo);
      this.AdjustCalendar(cultureInfo.DateTimeFormat);
    }

    private void AdjustCalendar(DateTimeFormatInfo dateTimeFormat)
    {
      Calendar calendar = dateTimeFormat == null ? (Calendar) null : dateTimeFormat.Calendar;
      Action<ClientCultureInfo> action;
      if (calendar == null || !ClientCultureInfo.sm_adjustments.TryGetValue(calendar.GetType(), out action))
        return;
      this.PopulateEras(dateTimeFormat);
      action(this);
    }

    private void PopulateEras(DateTimeFormatInfo dateTimeFormat)
    {
      this.Eras = new object[dateTimeFormat.Calendar.Eras.Length * 4];
      int index = 0;
      for (int era = 0; era < dateTimeFormat.Calendar.Eras.Length; ++era)
      {
        this.Eras[index] = (object) era;
        this.Eras[index + 1] = (object) dateTimeFormat.GetEraName(era);
        this.Eras[index + 3] = (object) 0;
        index += 4;
      }
    }

    public JObject ToJson()
    {
      JObject json = JObject.FromObject((object) new OrderedDictionary()
      {
        [(object) "name"] = (object) this.Name,
        [(object) "numberFormat"] = (object) this.NumberFormat,
        [(object) "dateTimeFormat"] = (object) this.DateTimeFormat,
        [(object) "numberShortForm"] = (object) this.NumberShortForm,
        [(object) "eras"] = (object) this.Eras
      });
      if (this.Adjustment != 0)
      {
        JToken jtoken = json.SelectToken("dateTimeFormat.Calendar");
        if (jtoken != null)
          jtoken[(object) "_adjustment"] = (JToken) this.Adjustment;
      }
      return json;
    }

    public void AppendAdjustmentScript(StringBuilder sbScriptBuilder)
    {
      if (string.IsNullOrEmpty(this.AdjustmentScriptName))
        return;
      Assembly assembly = typeof (ScriptManager).Assembly;
      string name = ((IEnumerable<string>) assembly.GetManifestResourceNames()).FirstOrDefault<string>((Func<string, bool>) (s => s.Contains(this.AdjustmentScriptName)));
      if (string.IsNullOrEmpty(name))
        return;
      using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(name)))
      {
        string end = streamReader.ReadToEnd();
        int startIndex = end.IndexOf("__cultureInfo", StringComparison.OrdinalIgnoreCase);
        if (startIndex < 0)
          return;
        sbScriptBuilder.AppendLine();
        sbScriptBuilder.Append(end.Substring(startIndex));
      }
    }

    private static void AdjustJapaneseCalendar(ClientCultureInfo cultureInfo)
    {
      cultureInfo.Eras[2] = (object) 60022080000L;
      cultureInfo.Eras[3] = (object) 1988;
      cultureInfo.Eras[6] = (object) -1357603200000L;
      cultureInfo.Eras[7] = (object) 1925;
      cultureInfo.Eras[10] = (object) -1812153600000L;
      cultureInfo.Eras[11] = (object) 1911;
      cultureInfo.Eras[15] = (object) 1867;
    }

    private static void AdjustKoreanCalendar(ClientCultureInfo cultureInfo) => cultureInfo.Eras[3] = (object) -2333;

    private static void AdjustTaiwanCalendar(ClientCultureInfo cultureInfo) => cultureInfo.Eras[3] = (object) 1911;

    private static void AdjustThaiBuddhistCalendar(ClientCultureInfo cultureInfo) => cultureInfo.Eras[3] = (object) -543;

    private static void AdjustHijriCalendar(ClientCultureInfo cultureInfo)
    {
      if (!(cultureInfo.DateTimeFormat.Calendar is HijriCalendar calendar))
        return;
      cultureInfo.Adjustment = calendar.HijriAdjustment;
      cultureInfo.AdjustmentScriptName = "Date.HijriCalendar.js";
    }

    private static void AdjustUmAlQuraCalendar(ClientCultureInfo cultureInfo) => cultureInfo.AdjustmentScriptName = "Date.UmAlQuraCalendar.js";

    public static ClientCultureInfo GetClientCulture(CultureInfo cultureInfo) => new ClientCultureInfo(cultureInfo);
  }
}
