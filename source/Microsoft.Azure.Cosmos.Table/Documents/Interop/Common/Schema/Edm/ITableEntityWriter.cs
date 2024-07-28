// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.Edm.ITableEntityWriter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Documents.Interop.Common.Schema.Edm
{
  internal interface ITableEntityWriter : IDisposable
  {
    void Close();

    void Flush();

    void Start();

    void End();

    void WriteName(string name);

    void WriteString(string value);

    void WriteBinary(byte[] value);

    void WriteBoolean(bool? value);

    void WriteDateTime(DateTime? value);

    void WriteDouble(double? value);

    void WriteGuid(Guid? value);

    void WriteInt32(int? value);

    void WriteInt64(long? value);
  }
}
