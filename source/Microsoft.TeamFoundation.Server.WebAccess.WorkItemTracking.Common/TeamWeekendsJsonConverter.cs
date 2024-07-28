// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamWeekendsJsonConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class TeamWeekendsJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (TeamWeekends).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      TeamWeekends teamWeekends = (TeamWeekends) value;
      var data = new
      {
        canEditWeekends = teamWeekends.CanEditWeekends,
        days = Array.ConvertAll<DayOfWeek, int>(teamWeekends.Days, (Converter<DayOfWeek, int>) (item => (int) item))
      };
      serializer.Serialize(writer, (object) data);
    }
  }
}
