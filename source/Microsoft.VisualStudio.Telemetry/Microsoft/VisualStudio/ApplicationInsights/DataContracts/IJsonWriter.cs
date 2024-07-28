// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.IJsonWriter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  public interface IJsonWriter
  {
    void WriteStartArray();

    void WriteStartObject();

    void WriteEndArray();

    void WriteEndObject();

    void WriteComma();

    void WriteProperty(string name, string value);

    void WriteProperty(string name, bool? value);

    void WriteProperty(string name, int? value);

    void WriteProperty(string name, double? value);

    void WriteProperty(string name, TimeSpan? value);

    void WriteProperty(string name, DateTimeOffset? value);

    void WriteProperty(string name, IDictionary<string, double> values);

    void WriteProperty(string name, IDictionary<string, string> values);

    void WriteProperty(string name, IJsonSerializable value);

    void WritePropertyName(string name);

    void WriteRawValue(object value);
  }
}
