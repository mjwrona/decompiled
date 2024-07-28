// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.IJsonWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.Json
{
  [CLSCompliant(false)]
  public interface IJsonWriter
  {
    void StartPaddingFunctionScope();

    void EndPaddingFunctionScope();

    void StartObjectScope();

    void EndObjectScope();

    void StartArrayScope();

    void EndArrayScope();

    void WriteName(string name);

    void WritePaddingFunctionName(string functionName);

    void WriteValue(bool value);

    void WriteValue(int value);

    void WriteValue(float value);

    void WriteValue(short value);

    void WriteValue(long value);

    void WriteValue(double value);

    void WriteValue(Guid value);

    void WriteValue(Decimal value);

    void WriteValue(DateTimeOffset value);

    void WriteValue(TimeSpan value);

    void WriteValue(byte value);

    void WriteValue(sbyte value);

    void WriteValue(string value);

    void WriteValue(byte[] value);

    void WriteValue(Date value);

    void WriteValue(TimeOfDay value);

    void WriteRawValue(string rawValue);

    void Flush();
  }
}
