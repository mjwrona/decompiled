// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionDataComponent3
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionDataComponent3 : ExtensionDataComponent2
  {
    public override ExtensionDataDocument CreateDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string documentValue,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (CreateDocument));
        this.PrepareStoredProcedure("Extension.prc_CreateDocument");
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@documentValue", documentValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@version", 1);
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionDataDocument>((ObjectBinder<ExtensionDataDocument>) this.GetExtensionDataColumns());
        return resultCollection.GetCurrent<ExtensionDataDocument>().Items[0];
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (CreateDocument));
      }
    }

    public override void DeleteDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (DeleteDocument));
        this.PrepareStoredProcedure("Extension.prc_DeleteDocument");
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (DeleteDocument));
      }
    }

    public override ExtensionDataDocument GetDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (GetDocument));
        this.PrepareStoredProcedure("Extension.prc_GetDocument");
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionDataDocument>((ObjectBinder<ExtensionDataDocument>) this.GetExtensionDataColumns());
        List<ExtensionDataDocument> items = resultCollection.GetCurrent<ExtensionDataDocument>().Items;
        return items.Count > 0 ? items[0] : (ExtensionDataDocument) null;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (GetDocument));
      }
    }

    public override List<ExtensionDataDocument> GetDocuments(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      int maxNumberOfRecords,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (GetDocuments));
        this.PrepareStoredProcedure("Extension.prc_GetDocuments");
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        if (this.Version >= 4)
          this.BindInt("@maxNumberOfRecords", maxNumberOfRecords);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionDataDocument>((ObjectBinder<ExtensionDataDocument>) this.GetExtensionDataColumns());
        return resultCollection.GetCurrent<ExtensionDataDocument>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (GetDocuments));
      }
    }

    public override ExtensionDataDocument SetDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string documentValue,
      int version,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (SetDocument));
        this.PrepareStoredProcedure("Extension.prc_SetDocument");
        if (this == null)
          this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@documentValue", documentValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@version", version);
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionDataDocument>((ObjectBinder<ExtensionDataDocument>) this.GetExtensionDataColumns());
        return resultCollection.GetCurrent<ExtensionDataDocument>().Items[0];
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (SetDocument));
      }
    }

    public override ExtensionDataDocument UpdateDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string documentValue,
      int version,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (UpdateDocument));
        this.PrepareStoredProcedure("Extension.prc_UpdateDocument");
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@documentValue", documentValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@version", version);
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionDataDocument>((ObjectBinder<ExtensionDataDocument>) this.GetExtensionDataColumns());
        return resultCollection.GetCurrent<ExtensionDataDocument>().Items[0];
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (UpdateDocument));
      }
    }
  }
}
