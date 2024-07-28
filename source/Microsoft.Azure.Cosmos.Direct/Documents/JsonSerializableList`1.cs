// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.JsonSerializableList`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal sealed class JsonSerializableList<T> : List<T>
  {
    public JsonSerializableList(IEnumerable<T> list)
      : base(list)
    {
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public static List<T> LoadFrom(string serialized) => serialized != null ? JArray.Parse(serialized).ToObject<List<T>>() : throw new ArgumentNullException(nameof (serialized));
  }
}
