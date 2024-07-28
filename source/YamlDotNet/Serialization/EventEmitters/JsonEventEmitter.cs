// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.EventEmitters.JsonEventEmitter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.EventEmitters
{
  public sealed class JsonEventEmitter : ChainedEventEmitter
  {
    public JsonEventEmitter(IEventEmitter nextEmitter)
      : base(nextEmitter)
    {
    }

    public override void Emit(AliasEventInfo eventInfo, IEmitter emitter) => throw new NotSupportedException("Aliases are not supported in JSON");

    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
      eventInfo.IsPlainImplicit = true;
      eventInfo.Style = ScalarStyle.Plain;
      TypeCode typeCode = eventInfo.Source.Value != null ? eventInfo.Source.Type.GetTypeCode() : TypeCode.Empty;
      switch (typeCode)
      {
        case TypeCode.Empty:
          eventInfo.RenderedValue = "null";
          break;
        case TypeCode.Boolean:
          eventInfo.RenderedValue = YamlFormatter.FormatBoolean(eventInfo.Source.Value);
          break;
        case TypeCode.Char:
        case TypeCode.String:
          eventInfo.RenderedValue = eventInfo.Source.Value.ToString();
          eventInfo.Style = ScalarStyle.DoubleQuoted;
          break;
        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
          eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
          break;
        case TypeCode.DateTime:
          eventInfo.RenderedValue = YamlFormatter.FormatDateTime(eventInfo.Source.Value);
          break;
        default:
          if ((object) eventInfo.Source.Type == (object) typeof (TimeSpan))
          {
            eventInfo.RenderedValue = YamlFormatter.FormatTimeSpan(eventInfo.Source.Value);
            break;
          }
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", new object[1]
          {
            (object) typeCode
          }));
      }
      base.Emit(eventInfo, emitter);
    }

    public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
    {
      eventInfo.Style = MappingStyle.Flow;
      base.Emit(eventInfo, emitter);
    }

    public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
    {
      eventInfo.Style = SequenceStyle.Flow;
      base.Emit(eventInfo, emitter);
    }
  }
}
