// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionDataComponent
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionDataComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<ExtensionDataComponent>(1),
      (IComponentCreator) new ComponentCreator<ExtensionDataComponent2>(2),
      (IComponentCreator) new ComponentCreator<ExtensionDataComponent3>(3),
      (IComponentCreator) new ComponentCreator<ExtensionDataComponent3>(4)
    }, "ExtensionData");
    public const int CollectionNameMaxSize = 100;
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1660000,
        new SqlExceptionFactory(typeof (DocumentExistsException))
      },
      {
        1660001,
        new SqlExceptionFactory(typeof (DocumentDoesNotExistException))
      },
      {
        1660002,
        new SqlExceptionFactory(typeof (DocumentCollectionDoesNotExistException))
      },
      {
        1660003,
        new SqlExceptionFactory(typeof (InvalidDocumentVersionException))
      }
    };
    private const string s_area = "ExtensionDataComponent";

    public ExtensionDataComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ExtensionDataComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (ExtensionDataComponent);

    public virtual ExtensionDataDocument CreateDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string documentValue,
      string publisherName = null,
      string extensionName = null)
    {
      try
      {
        this.TraceEnter(70210, nameof (CreateDocument));
        this.PrepareStoredProcedure("Extension.prc_CreateDocument");
        this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@documentValue", documentValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@version", 1);
        if (this is ExtensionDataComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
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

    public virtual void DeleteDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string publisherName = null,
      string extensionName = null)
    {
      try
      {
        this.TraceEnter(70210, nameof (DeleteDocument));
        this.PrepareStoredProcedure("Extension.prc_DeleteDocument");
        this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        if (this is ExtensionDataComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
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

    public virtual ExtensionDataDocument GetDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string publisherName = null,
      string extensionName = null)
    {
      try
      {
        this.TraceEnter(70210, nameof (GetDocument));
        this.PrepareStoredProcedure("Extension.prc_GetDocument");
        this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        if (this is ExtensionDataComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
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

    public virtual List<ExtensionDataDocument> GetDocuments(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      int maxNumberOfRecords,
      string publisherName = null,
      string extensionName = null)
    {
      try
      {
        this.TraceEnter(70210, nameof (GetDocuments));
        this.PrepareStoredProcedure("Extension.prc_GetDocuments");
        this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        if (this is ExtensionDataComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
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

    public virtual ExtensionDataDocument SetDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string documentValue,
      int version,
      string publisherName = null,
      string extensionName = null)
    {
      try
      {
        this.TraceEnter(70210, nameof (SetDocument));
        this.PrepareStoredProcedure("Extension.prc_SetDocument");
        this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@documentValue", documentValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@version", version);
        if (this is ExtensionDataComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
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

    public virtual ExtensionDataDocument UpdateDocument(
      string collectionName,
      Guid scopeType,
      byte[] scopeValue,
      byte[] documentId,
      string documentValue,
      int version,
      string publisherName = null,
      string extensionName = null)
    {
      try
      {
        this.TraceEnter(70210, nameof (UpdateDocument));
        this.PrepareStoredProcedure("Extension.prc_UpdateDocument");
        this.BindNullableGuid("@extensionId", Guid.Empty);
        this.BindString("@collectionName", collectionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@scopeType", scopeType);
        this.BindBinary("@scopeValue", scopeValue, SqlDbType.Binary);
        this.BindBinary("@documentId", documentId, SqlDbType.Binary);
        this.BindString("@documentValue", documentValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@version", version);
        if (this is ExtensionDataComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
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

    protected virtual ExtensionDataColumns GetExtensionDataColumns() => new ExtensionDataColumns(this.RequestContext);
  }
}
