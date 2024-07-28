// Decompiled with JetBrains decompiler
// Type: Nest.ActionsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class ActionsFormatter : IJsonFormatter<Actions>, IJsonFormatter
  {
    private static readonly ActionsInterfaceFormatter ActionsInterfaceFormatter = new ActionsInterfaceFormatter();
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "throttle_period",
        0
      },
      {
        "throttle_period_in_millis",
        0
      },
      {
        "email",
        1
      },
      {
        "webhook",
        2
      },
      {
        "index",
        3
      },
      {
        "logging",
        4
      },
      {
        "slack",
        5
      },
      {
        "pagerduty",
        6
      },
      {
        "foreach",
        7
      },
      {
        "transform",
        8
      },
      {
        "condition",
        9
      },
      {
        "max_iterations",
        10
      }
    };

    public Actions Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      Dictionary<string, IAction> actions = new Dictionary<string, IAction>();
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        string key = reader.ReadPropertyName();
        int count2 = 0;
        Time time = (Time) null;
        IAction action = (IAction) null;
        string str = (string) null;
        int? nullable = new int?();
        TransformContainer transformContainer = (TransformContainer) null;
        ConditionContainer conditionContainer = (ConditionContainer) null;
        while (reader.ReadIsInObject(ref count2))
        {
          ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
          int num;
          if (ActionsFormatter.Fields.TryGetValue(bytes, out num))
          {
            switch (num)
            {
              case 0:
                time = formatterResolver.GetFormatter<Time>().Deserialize(ref reader, formatterResolver);
                continue;
              case 1:
                action = (IAction) formatterResolver.GetFormatter<EmailAction>().Deserialize(ref reader, formatterResolver);
                continue;
              case 2:
                action = (IAction) formatterResolver.GetFormatter<WebhookAction>().Deserialize(ref reader, formatterResolver);
                continue;
              case 3:
                action = (IAction) formatterResolver.GetFormatter<IndexAction>().Deserialize(ref reader, formatterResolver);
                continue;
              case 4:
                action = (IAction) formatterResolver.GetFormatter<LoggingAction>().Deserialize(ref reader, formatterResolver);
                continue;
              case 5:
                action = (IAction) formatterResolver.GetFormatter<SlackAction>().Deserialize(ref reader, formatterResolver);
                continue;
              case 6:
                action = (IAction) formatterResolver.GetFormatter<PagerDutyAction>().Deserialize(ref reader, formatterResolver);
                continue;
              case 7:
                str = reader.ReadString();
                continue;
              case 8:
                transformContainer = formatterResolver.GetFormatter<TransformContainer>().Deserialize(ref reader, formatterResolver);
                continue;
              case 9:
                conditionContainer = formatterResolver.GetFormatter<ConditionContainer>().Deserialize(ref reader, formatterResolver);
                continue;
              case 10:
                nullable = new int?(reader.ReadInt32());
                continue;
              default:
                continue;
            }
          }
          else
            reader.ReadNextBlock();
        }
        if (action != null)
        {
          action.Name = key;
          action.ThrottlePeriod = time;
          action.Foreach = str;
          action.MaxIterations = nullable;
          action.Transform = transformContainer;
          action.Condition = conditionContainer;
          actions.Add(key, action);
        }
      }
      return new Actions((IDictionary<string, IAction>) actions);
    }

    public void Serialize(
      ref JsonWriter writer,
      Actions value,
      IJsonFormatterResolver formatterResolver)
    {
      ActionsFormatter.ActionsInterfaceFormatter.Serialize(ref writer, (IActions) value, formatterResolver);
    }
  }
}
