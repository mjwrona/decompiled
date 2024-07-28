// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DocumentPropertyDefinition
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class DocumentPropertyDefinition
  {
    public string PropertyName { get; }

    public bool Optional { get; }

    public bool ReadProcessed { get; private set; }

    public bool WriteProcessed { get; private set; }

    public Action<JsonReader> ReaderAction { private get; set; }

    public Action<JsonWriter> WriterAction { private get; set; }

    public DocumentPropertyDefinition(string propertyName, bool optional)
    {
      this.PropertyName = propertyName;
      this.Optional = optional;
    }

    public void ProcessRead(JsonReader reader)
    {
      this.ReaderAction(reader);
      this.ReadProcessed = true;
    }

    public void ProcessWrite(JsonWriter writer)
    {
      writer.WritePropertyName(this.PropertyName);
      this.WriterAction(writer);
      this.WriteProcessed = true;
    }

    public void WritePropertyAndValue(JsonWriter writer, object value)
    {
      if (!this.Optional || value != null)
      {
        writer.WritePropertyName(this.PropertyName);
        writer.WriteValue(value);
      }
      this.WriteProcessed = true;
    }
  }
}
