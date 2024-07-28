// Decompiled with JetBrains decompiler
// Type: Nest.PutPrivilegesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class PutPrivilegesFormatter : IJsonFormatter<IPutPrivilegesRequest>, IJsonFormatter
  {
    public IPutPrivilegesRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      AppPrivileges appPrivileges = formatterResolver.GetFormatter<AppPrivileges>().Deserialize(ref reader, formatterResolver);
      return (IPutPrivilegesRequest) new PutPrivilegesRequest()
      {
        Applications = (IAppPrivileges) appPrivileges
      };
    }

    public void Serialize(
      ref JsonWriter writer,
      IPutPrivilegesRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        formatterResolver.GetFormatter<IDictionary<string, IPrivileges>>().Serialize(ref writer, (IDictionary<string, IPrivileges>) value.Applications, formatterResolver);
    }
  }
}
