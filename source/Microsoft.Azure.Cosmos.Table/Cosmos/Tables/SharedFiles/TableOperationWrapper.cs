// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.TableOperationWrapper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal class TableOperationWrapper
  {
    public bool IsTableEntity { get; set; }

    public TableOperationType OperationType { get; set; }
  }
}
