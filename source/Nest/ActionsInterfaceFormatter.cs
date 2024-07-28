// Decompiled with JetBrains decompiler
// Type: Nest.ActionsInterfaceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class ActionsInterfaceFormatter : IJsonFormatter<IActions>, IJsonFormatter
  {
    private static readonly ActionsFormatter ActionsFormatter = new ActionsFormatter();

    public void Serialize(
      ref JsonWriter writer,
      IActions value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteBeginObject();
      if (value != null)
      {
        int num1 = 0;
        foreach (KeyValuePair<string, IAction> keyValuePair in value.Where<KeyValuePair<string, IAction>>((Func<KeyValuePair<string, IAction>, bool>) (kv => kv.Value != null)))
        {
          if (num1 > 0)
            writer.WriteValueSeparator();
          IAction action = keyValuePair.Value;
          writer.WritePropertyName(keyValuePair.Key);
          writer.WriteBeginObject();
          if (action.ThrottlePeriod != (Time) null)
          {
            writer.WritePropertyName("throttle_period");
            formatterResolver.GetFormatter<Time>().Serialize(ref writer, action.ThrottlePeriod, formatterResolver);
            writer.WriteValueSeparator();
          }
          if (!string.IsNullOrEmpty(action.Foreach))
          {
            writer.WritePropertyName("foreach");
            writer.WriteString(action.Foreach);
            writer.WriteValueSeparator();
          }
          int? maxIterations = action.MaxIterations;
          if (maxIterations.HasValue)
          {
            writer.WritePropertyName("max_iterations");
            ref JsonWriter local = ref writer;
            maxIterations = action.MaxIterations;
            int num2 = maxIterations.Value;
            local.WriteInt32(num2);
            writer.WriteValueSeparator();
          }
          if (action.Transform != null)
          {
            writer.WritePropertyName("transform");
            formatterResolver.GetFormatter<TransformContainer>().Serialize(ref writer, action.Transform, formatterResolver);
            writer.WriteValueSeparator();
          }
          if (action.Condition != null)
          {
            writer.WritePropertyName("condition");
            formatterResolver.GetFormatter<ConditionContainer>().Serialize(ref writer, action.Condition, formatterResolver);
            writer.WriteValueSeparator();
          }
          writer.WritePropertyName(keyValuePair.Value.ActionType.GetStringValue());
          switch (action.ActionType)
          {
            case ActionType.Email:
              ActionsInterfaceFormatter.Serialize<IEmailAction>(ref writer, action, formatterResolver);
              break;
            case ActionType.Webhook:
              ActionsInterfaceFormatter.Serialize<IWebhookAction>(ref writer, action, formatterResolver);
              break;
            case ActionType.Index:
              ActionsInterfaceFormatter.Serialize<IIndexAction>(ref writer, action, formatterResolver);
              break;
            case ActionType.Logging:
              ActionsInterfaceFormatter.Serialize<ILoggingAction>(ref writer, action, formatterResolver);
              break;
            case ActionType.Slack:
              ActionsInterfaceFormatter.Serialize<ISlackAction>(ref writer, action, formatterResolver);
              break;
            case ActionType.PagerDuty:
              ActionsInterfaceFormatter.Serialize<IPagerDutyAction>(ref writer, action, formatterResolver);
              break;
            default:
              formatterResolver.GetFormatter<IAction>().Serialize(ref writer, action, formatterResolver);
              break;
          }
          writer.WriteEndObject();
          ++num1;
        }
      }
      writer.WriteEndObject();
    }

    private static void Serialize<TAction>(
      ref JsonWriter writer,
      IAction value,
      IJsonFormatterResolver formatterResolver)
      where TAction : class, IAction
    {
      formatterResolver.GetFormatter<TAction>().Serialize(ref writer, value as TAction, formatterResolver);
    }

    public IActions Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => (IActions) ActionsInterfaceFormatter.ActionsFormatter.Deserialize(ref reader, formatterResolver);
  }
}
