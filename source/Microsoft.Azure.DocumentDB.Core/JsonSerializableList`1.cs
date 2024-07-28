// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.JsonSerializableList`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
