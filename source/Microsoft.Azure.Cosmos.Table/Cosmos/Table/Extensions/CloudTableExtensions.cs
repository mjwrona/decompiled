// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.CloudTableExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents.Client;
using System;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class CloudTableExtensions
  {
    public static Uri GetCollectionUri(this CloudTable table) => UriFactory.CreateDocumentCollectionUri("TablesDB", table.Name);
  }
}
