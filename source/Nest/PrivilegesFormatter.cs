// Decompiled with JetBrains decompiler
// Type: Nest.PrivilegesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class PrivilegesFormatter : IJsonFormatter<IPrivileges>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IPrivileges value,
      IJsonFormatterResolver formatterResolver)
    {
      formatterResolver.GetFormatter<IDictionary<string, IPrivilegesActions>>().Serialize(ref writer, (IDictionary<string, IPrivilegesActions>) value, formatterResolver);
    }

    public IPrivileges Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => (IPrivileges) formatterResolver.GetFormatter<Privileges>().Deserialize(ref reader, formatterResolver);
  }
}
