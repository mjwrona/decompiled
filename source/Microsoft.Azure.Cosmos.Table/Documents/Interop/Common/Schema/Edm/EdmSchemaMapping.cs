// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.Edm.EdmSchemaMapping
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Interop.Common.Schema.Edm
{
  internal static class EdmSchemaMapping
  {
    public static readonly string EntityName = "entity";
    public static readonly string SystemPropertiesPrefix = "$";
    public static readonly Dictionary<string, string> SystemPropertiesMapping = new Dictionary<string, string>();

    static EdmSchemaMapping()
    {
      EdmSchemaMapping.SystemPropertiesMapping.Add("PartitionKey", "$pk");
      EdmSchemaMapping.SystemPropertiesMapping.Add("RowKey", "$id");
      EdmSchemaMapping.SystemPropertiesMapping.Add("Etag", "_etag");
      EdmSchemaMapping.SystemPropertiesMapping.Add("Timestamp", "_ts");
    }

    public static bool IsDocumentDBProperty(string name) => name == "_rid" || name == "_self" || name == "_etag" || name == "_attachments" || name == "_ts" || name == "id";

    public static class Table
    {
      public const string RowKey = "RowKey";
      public const string PartitionKey = "PartitionKey";
      public const string Etag = "Etag";
      public const string Timestamp = "Timestamp";
    }

    public static class DocumentDB
    {
      public const string Id = "id";
      public const string Etag = "_etag";
      public const string Timestamp = "_ts";
      public const string Rid = "_rid";
      public const string Self = "_self";
      public const string Attachments = "_attachments";
      public const string PartitionKey = "$pk";
      public const string IndexedId = "$id";
    }
  }
}
