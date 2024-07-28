// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.Edm.ITableEntityReader
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Documents.Interop.Common.Schema.Edm
{
  internal interface ITableEntityReader : IDisposable
  {
    string CurrentName { get; }

    Microsoft.Azure.Documents.Interop.Common.Schema.DataType CurrentType { get; }

    void Start();

    void End();

    bool MoveNext();

    string ReadString();

    byte[] ReadBinary();

    bool? ReadBoolean();

    DateTime? ReadDateTime();

    double? ReadDouble();

    Guid? ReadGuid();

    int? ReadInt32();

    long? ReadInt64();
  }
}
