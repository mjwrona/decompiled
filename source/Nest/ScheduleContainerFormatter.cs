// Decompiled with JetBrains decompiler
// Type: Nest.ScheduleContainerFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class ScheduleContainerFormatter : IJsonFormatter<IScheduleContainer>, IJsonFormatter
  {
    private static readonly AutomataDictionary ScheduleTypes = new AutomataDictionary()
    {
      {
        "cron",
        0
      },
      {
        "hourly",
        1
      },
      {
        "daily",
        2
      },
      {
        "weekly",
        3
      },
      {
        "monthly",
        4
      },
      {
        "yearly",
        5
      },
      {
        "interval",
        6
      }
    };

    public IScheduleContainer Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IScheduleContainer) null;
      reader.ReadNext();
      ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
      IScheduleContainer scheduleContainer = (IScheduleContainer) null;
      int num;
      if (ScheduleContainerFormatter.ScheduleTypes.TryGetValue(bytes, out num))
      {
        switch (num)
        {
          case 0:
            scheduleContainer = this.GetCronExpressionContainer(ref reader, formatterResolver);
            break;
          case 1:
            scheduleContainer = this.GetScheduleContainer<IHourlySchedule>(ref reader, formatterResolver);
            break;
          case 2:
            scheduleContainer = this.GetScheduleContainer<IDailySchedule>(ref reader, formatterResolver);
            break;
          case 3:
            scheduleContainer = this.GetScheduleContainer<IWeeklySchedule>(ref reader, formatterResolver);
            break;
          case 4:
            scheduleContainer = this.GetScheduleContainer<IMonthlySchedule>(ref reader, formatterResolver);
            break;
          case 5:
            scheduleContainer = this.GetScheduleContainer<IYearlySchedule>(ref reader, formatterResolver);
            break;
          case 6:
            scheduleContainer = this.GetScheduleContainer<Interval>(ref reader, formatterResolver);
            break;
        }
      }
      else
        reader.ReadNextBlock();
      reader.ReadNext();
      return scheduleContainer;
    }

    private IScheduleContainer GetScheduleContainer<T>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return !(formatterResolver.GetFormatter<T>().Deserialize(ref reader, formatterResolver) is ScheduleBase schedule) ? (IScheduleContainer) null : (IScheduleContainer) new ScheduleContainer(schedule);
    }

    private IScheduleContainer GetCronExpressionContainer(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginArray:
          List<CronExpression> expressions = new List<CronExpression>();
          int count = 0;
          while (reader.ReadIsInArray(ref count))
          {
            if (reader.GetCurrentJsonToken() == JsonToken.String)
            {
              string expression = reader.ReadString();
              expressions.Add(new CronExpression(expression));
            }
          }
          return (IScheduleContainer) new ScheduleContainer((ScheduleBase) new CronExpressions((IEnumerable<CronExpression>) expressions));
        case JsonToken.String:
          string expression1 = reader.ReadString();
          return !string.IsNullOrEmpty(expression1) ? (IScheduleContainer) new ScheduleContainer((ScheduleBase) new CronExpression(expression1)) : throw new Exception("Trigger schedule string value should not be null or empty");
        default:
          throw new Exception("Unexpected JSON in trigger schedule");
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IScheduleContainer value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteBeginObject();
      if (value.CronExpressions != null)
      {
        writer.WritePropertyName("cron");
        formatterResolver.GetFormatter<ICronExpressions>().Serialize(ref writer, value.CronExpressions, formatterResolver);
      }
      else if ((object) value.Cron != null)
      {
        writer.WritePropertyName("cron");
        formatterResolver.GetFormatter<CronExpression>().Serialize(ref writer, value.Cron, formatterResolver);
      }
      else if (value.Hourly != null)
      {
        writer.WritePropertyName("hourly");
        formatterResolver.GetFormatter<IHourlySchedule>().Serialize(ref writer, value.Hourly, formatterResolver);
      }
      else if (value.Daily != null)
      {
        writer.WritePropertyName("daily");
        formatterResolver.GetFormatter<IDailySchedule>().Serialize(ref writer, value.Daily, formatterResolver);
      }
      else if (value.Weekly != null)
      {
        writer.WritePropertyName("weekly");
        formatterResolver.GetFormatter<IWeeklySchedule>().Serialize(ref writer, value.Weekly, formatterResolver);
      }
      else if (value.Monthly != null)
      {
        writer.WritePropertyName("monthly");
        formatterResolver.GetFormatter<IMonthlySchedule>().Serialize(ref writer, value.Monthly, formatterResolver);
      }
      else if (value.Yearly != null)
      {
        writer.WritePropertyName("yearly");
        formatterResolver.GetFormatter<IYearlySchedule>().Serialize(ref writer, value.Yearly, formatterResolver);
      }
      else if ((object) value.Interval != null)
      {
        writer.WritePropertyName("interval");
        formatterResolver.GetFormatter<Interval>().Serialize(ref writer, value.Interval, formatterResolver);
      }
      writer.WriteEndObject();
    }

    private static class Parser
    {
      public const string Cron = "cron";
      public const string Hourly = "hourly";
      public const string Daily = "daily";
      public const string Weekly = "weekly";
      public const string Monthly = "monthly";
      public const string Yearly = "yearly";
      public const string Interval = "interval";
    }
  }
}
