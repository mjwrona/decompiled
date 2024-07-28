// Decompiled with JetBrains decompiler
// Type: Nest.ValidateDetectorRequestFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class ValidateDetectorRequestFormatter : 
    IJsonFormatter<IValidateDetectorRequest>,
    IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IValidateDetectorRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        formatterResolver.GetFormatter<IDetector>().Serialize(ref writer, value.Detector, formatterResolver);
    }

    public IValidateDetectorRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (IValidateDetectorRequest) formatterResolver.GetFormatter<ValidateDetectorRequest>().Deserialize(ref reader, formatterResolver);
    }
  }
}
