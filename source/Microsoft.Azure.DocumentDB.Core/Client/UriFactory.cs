// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.UriFactory
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Client
{
  public static class UriFactory
  {
    public static Uri CreateDatabaseUri(string databaseId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) "dbs", (object) Uri.EscapeUriString(databaseId)), UriKind.Relative);

    [Obsolete("CreateCollectionUri method is deprecated, please use CreateDocumentCollectionUri method instead.")]
    public static Uri CreateCollectionUri(string databaseId, string collectionId) => UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

    public static Uri CreateDocumentCollectionUri(string databaseId, string collectionId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId)), UriKind.Relative);

    internal static Uri CreateClientEncryptionKeyUri(
      string databaseId,
      string clientEncryptionKeyId)
    {
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "clientencryptionkeys", (object) Uri.EscapeUriString(clientEncryptionKeyId)), UriKind.Relative);
    }

    public static Uri CreateUserUri(string databaseId, string userId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "users", (object) Uri.EscapeUriString(userId)), UriKind.Relative);

    internal static Uri CreateUserDefinedTypeUri(string databaseId, string userDefinedTypeId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "udts", (object) Uri.EscapeUriString(userDefinedTypeId)), UriKind.Relative);

    public static Uri CreateDocumentUri(string databaseId, string collectionId, string documentId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "docs", (object) Uri.EscapeUriString(documentId)), UriKind.Relative);

    public static Uri CreatePermissionUri(string databaseId, string userId, string permissionId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "users", (object) Uri.EscapeUriString(userId), (object) "permissions", (object) Uri.EscapeUriString(permissionId)), UriKind.Relative);

    public static Uri CreateStoredProcedureUri(
      string databaseId,
      string collectionId,
      string storedProcedureId)
    {
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "sprocs", (object) Uri.EscapeUriString(storedProcedureId)), UriKind.Relative);
    }

    internal static Uri CreateStoredProcedureUri(
      string documentCollectionLink,
      string storedProcedureId)
    {
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) documentCollectionLink, (object) "sprocs", (object) Uri.EscapeUriString(storedProcedureId)), UriKind.Relative);
    }

    public static Uri CreateTriggerUri(string databaseId, string collectionId, string triggerId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "triggers", (object) Uri.EscapeUriString(triggerId)), UriKind.Relative);

    public static Uri CreateUserDefinedFunctionUri(
      string databaseId,
      string collectionId,
      string udfId)
    {
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "udfs", (object) Uri.EscapeUriString(udfId)), UriKind.Relative);
    }

    public static Uri CreateConflictUri(string databaseId, string collectionId, string conflictId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "conflicts", (object) Uri.EscapeUriString(conflictId)), UriKind.Relative);

    public static Uri CreateAttachmentUri(
      string databaseId,
      string collectionId,
      string documentId,
      string attachmentId)
    {
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "docs", (object) Uri.EscapeUriString(documentId), (object) "attachments", (object) Uri.EscapeUriString(attachmentId)), UriKind.Relative);
    }

    public static Uri CreatePartitionKeyRangesUri(string databaseId, string collectionId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "pkranges"), UriKind.Relative);

    internal static Uri CreateSchemaUri(string databaseId, string collectionId, string schemaId) => new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", (object) "dbs", (object) Uri.EscapeUriString(databaseId), (object) "colls", (object) Uri.EscapeUriString(collectionId), (object) "schemas", (object) Uri.EscapeUriString(schemaId)), UriKind.Relative);
  }
}
