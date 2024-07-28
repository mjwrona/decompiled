// Decompiled with JetBrains decompiler
// Type: Nest.DynamicTemplatesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class DynamicTemplatesFormatter : IJsonFormatter<DynamicTemplateContainer>, IJsonFormatter
  {
    public DynamicTemplateContainer Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicTemplateContainer templateContainer = new DynamicTemplateContainer();
      int count1 = 0;
      IJsonFormatter<DynamicTemplate> formatter = formatterResolver.GetFormatter<DynamicTemplate>();
      while (reader.ReadIsInArray(ref count1))
      {
        int count2 = 0;
        string name = (string) null;
        IDynamicTemplate dynamicTemplate = (IDynamicTemplate) null;
        while (reader.ReadIsInObject(ref count2))
        {
          name = reader.ReadPropertyName();
          dynamicTemplate = (IDynamicTemplate) formatter.Deserialize(ref reader, formatterResolver);
        }
        templateContainer.Add(name, dynamicTemplate);
      }
      return templateContainer;
    }

    public void Serialize(
      ref JsonWriter writer,
      DynamicTemplateContainer value,
      IJsonFormatterResolver formatterResolver)
    {
      formatterResolver.GetFormatter<IDynamicTemplateContainer>().Serialize(ref writer, (IDynamicTemplateContainer) value, formatterResolver);
    }
  }
}
