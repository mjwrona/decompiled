// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonEnumerator
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class JsonEnumerator : IEnumerator<JsonProperty>, IDisposable, IEnumerator
  {
    private readonly StreamReader streamReader;
    private readonly JsonTextReader jsonReader;
    private readonly Stack<JsonProperty> PathParents = new Stack<JsonProperty>();
    private string lastPropertyName;

    public JsonEnumerator(Stream stream)
    {
      this.streamReader = new StreamReader(stream, StrictEncodingWithBOM.UTF8);
      this.jsonReader = new JsonTextReader((TextReader) this.streamReader);
    }

    public static IConcurrentIterator<IEnumerable<JsonProperty>> ReadConcurrentIterator(
      Stream stream,
      int pageSize = 1000)
    {
      return (IConcurrentIterator<IEnumerable<JsonProperty>>) new ConcurrentIterator<IEnumerable<JsonProperty>>((IEnumerator<IEnumerable<JsonProperty>>) new PagedEnumerator<JsonProperty>((IEnumerator<JsonProperty>) new JsonEnumerator(stream), pageSize));
    }

    public void Dispose() => this.streamReader.Dispose();

    public bool MoveNext()
    {
      while (this.jsonReader.Read())
      {
        switch (this.jsonReader.TokenType)
        {
          case JsonToken.StartConstructor:
          case JsonToken.EndConstructor:
            throw new NotImplementedException();
          case JsonToken.PropertyName:
            this.lastPropertyName = (string) this.jsonReader.Value;
            continue;
          case JsonToken.EndObject:
          case JsonToken.EndArray:
            this.lastPropertyName = this.PathParents.Pop().Key;
            break;
        }
        this.Current = new JsonProperty(this.PathParents.Count > 0 ? this.PathParents.Peek() : (JsonProperty) null, this.lastPropertyName, this.jsonReader.TokenType, this.jsonReader.Value);
        switch (this.jsonReader.TokenType)
        {
          case JsonToken.StartObject:
          case JsonToken.StartArray:
            this.PathParents.Push(this.Current);
            break;
        }
        return true;
      }
      return false;
    }

    public void Reset() => throw new NotSupportedException();

    public JsonProperty Current { get; private set; }

    object IEnumerator.Current => (object) this.Current;
  }
}
