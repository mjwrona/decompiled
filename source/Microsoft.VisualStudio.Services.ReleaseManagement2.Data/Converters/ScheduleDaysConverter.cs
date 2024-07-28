// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ScheduleDaysConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ScheduleDaysConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays scheduleDays)
    {
      return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays) Enum.ToObject(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays), Convert.ToInt32((object) scheduleDays, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays scheduleDays)
    {
      return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays) Enum.ToObject(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduleDays), Convert.ToInt32((object) scheduleDays, (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
