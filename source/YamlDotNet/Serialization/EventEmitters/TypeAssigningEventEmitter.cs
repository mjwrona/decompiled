// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.EventEmitters.TypeAssigningEventEmitter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
  public sealed class TypeAssigningEventEmitter : ChainedEventEmitter
  {
    private readonly bool requireTagWhenStaticAndActualTypesAreDifferent;
    private IDictionary<Type, string> tagMappings;

    public TypeAssigningEventEmitter(
      IEventEmitter nextEmitter,
      bool requireTagWhenStaticAndActualTypesAreDifferent,
      IDictionary<Type, string> tagMappings)
      : base(nextEmitter)
    {
      this.requireTagWhenStaticAndActualTypesAreDifferent = requireTagWhenStaticAndActualTypesAreDifferent;
      this.tagMappings = tagMappings;
    }

    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
      ScalarStyle scalarStyle = ScalarStyle.Plain;
      TypeCode typeCode = eventInfo.Source.Value != null ? eventInfo.Source.Type.GetTypeCode() : TypeCode.Empty;
      switch (typeCode)
      {
        case TypeCode.Empty:
          eventInfo.Tag = "tag:yaml.org,2002:null";
          eventInfo.RenderedValue = "";
          break;
        case TypeCode.Boolean:
          eventInfo.Tag = "tag:yaml.org,2002:bool";
          eventInfo.RenderedValue = YamlFormatter.FormatBoolean(eventInfo.Source.Value);
          break;
        case TypeCode.Char:
        case TypeCode.String:
          eventInfo.Tag = "tag:yaml.org,2002:str";
          eventInfo.RenderedValue = eventInfo.Source.Value.ToString();
          scalarStyle = ScalarStyle.Any;
          break;
        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
          eventInfo.Tag = "tag:yaml.org,2002:int";
          eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
          break;
        case TypeCode.Single:
          eventInfo.Tag = "tag:yaml.org,2002:float";
          eventInfo.RenderedValue = YamlFormatter.FormatNumber((float) eventInfo.Source.Value);
          break;
        case TypeCode.Double:
          eventInfo.Tag = "tag:yaml.org,2002:float";
          eventInfo.RenderedValue = YamlFormatter.FormatNumber((double) eventInfo.Source.Value);
          break;
        case TypeCode.Decimal:
          eventInfo.Tag = "tag:yaml.org,2002:float";
          eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
          break;
        case TypeCode.DateTime:
          eventInfo.Tag = "tag:yaml.org,2002:timestamp";
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
      eventInfo.IsPlainImplicit = true;
      if (eventInfo.Style == ScalarStyle.Any)
        eventInfo.Style = scalarStyle;
      base.Emit(eventInfo, emitter);
    }

    public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
    {
      this.AssignTypeIfNeeded((ObjectEventInfo) eventInfo);
      base.Emit(eventInfo, emitter);
    }

    public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
    {
      this.AssignTypeIfNeeded((ObjectEventInfo) eventInfo);
      base.Emit(eventInfo, emitter);
    }

    private void AssignTypeIfNeeded(ObjectEventInfo eventInfo)
    {
      string str;
      if (this.tagMappings.TryGetValue(eventInfo.Source.Type, out str))
        eventInfo.Tag = str;
      else if (this.requireTagWhenStaticAndActualTypesAreDifferent && eventInfo.Source.Value != null && (object) eventInfo.Source.Type != (object) eventInfo.Source.StaticType)
        throw new YamlException("Cannot serialize type '" + eventInfo.Source.Type.FullName + "' where a '" + eventInfo.Source.StaticType.FullName + "' was expected because no tag mapping has been registered for '" + eventInfo.Source.Type.FullName + "', which means that it won't be possible to deserialize the document.\nRegister a tag mapping using the SerializerBuilder.WithTagMapping method.\n\nE.g: builder.WithTagMapping(\"!" + eventInfo.Source.Type.Name + "\", typeof(" + eventInfo.Source.Type.FullName + "));");
    }
  }
}
