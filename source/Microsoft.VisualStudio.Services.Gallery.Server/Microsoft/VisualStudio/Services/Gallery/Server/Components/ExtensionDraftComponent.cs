// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDraftComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDraftComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories;
    private const string s_area = "ExtensionDraftComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ExtensionDraftComponent>(1)
    }, "ExtensionDraft");

    static ExtensionDraftComponent()
    {
      ExtensionDraftComponent.SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      ExtensionDraftComponent.SqlExceptionFactories.Add(270026, new SqlExceptionFactory(typeof (DraftIdDoesNotExistException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ExtensionDraftComponent.SqlExceptionFactories;

    protected override string TraceArea => nameof (ExtensionDraftComponent);

    public virtual void CreateOrUpdateExtensionDraft(ExtensionDraft extensionDraft)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateOrUpdateExtensionDraft");
      this.BindGuid("draftId", extensionDraft.Id);
      this.BindInt("extensionDeploymentType", (int) extensionDraft.Payload.Type);
      this.BindString("publisherName", extensionDraft.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", extensionDraft.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindNullableGuid("extensionId", extensionDraft.ExtensionId);
      this.BindGuid("userId", extensionDraft.UserId);
      this.BindString("payloadFileName", extensionDraft.Payload.FileName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDateTime("lastUpdated", extensionDraft.LastUpdated);
      this.BindDateTime("createdDate", extensionDraft.CreatedDate);
      this.BindNullableDateTime("editReferenceDate", new DateTime?(extensionDraft.EditReferenceDate));
      this.BindInt("draftState", (int) extensionDraft.DraftState);
      this.BindString("product", extensionDraft.Product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual void AddAssetToDraft(
      Guid draftId,
      string assetType,
      string contentType,
      string language,
      int fileId,
      bool isOldAsset,
      bool isPayloadAsset,
      DateTime lastUpdated)
    {
      this.PrepareStoredProcedure("Gallery.prc_AddAssetForExtensionDraft");
      AssetInfo assetInfo = new AssetInfo(assetType, language);
      this.BindGuid(nameof (draftId), draftId);
      this.BindString(nameof (assetType), assetInfo.ToString(), 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (contentType), contentType, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (fileId), fileId);
      this.BindBoolean(nameof (isOldAsset), isOldAsset);
      this.BindBoolean(nameof (isPayloadAsset), isPayloadAsset);
      this.BindDateTime(nameof (lastUpdated), lastUpdated);
      this.ExecuteNonQuery();
    }

    public void DeleteExtensionDraft(Guid draftId)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteExtensionDraft");
      this.BindGuid(nameof (draftId), draftId);
      this.ExecuteNonQuery();
    }

    public virtual IList<ExtensionDraft> GetExtensionDrafts(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetExtensionDrafts");
      if (userId != Guid.Empty)
        this.BindGuid(nameof (userId), userId);
      IList<ExtensionDraft> items1;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetExtensionDrafts", this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionDraft>((ObjectBinder<ExtensionDraft>) new ExtensionDraftBinder());
        items1 = (IList<ExtensionDraft>) resultCollection.GetCurrent<ExtensionDraft>().Items;
        resultCollection.AddBinder<ExtensionDraftAssetRow>((ObjectBinder<ExtensionDraftAssetRow>) new ExtensionDraftAssetRowBinder());
        resultCollection.NextResult();
        IList<ExtensionDraftAssetRow> items2 = (IList<ExtensionDraftAssetRow>) resultCollection.GetCurrent<ExtensionDraftAssetRow>().Items;
        this.MapAssetsToDrafts(items1, items2);
      }
      return items1;
    }

    public void DeleteAssetFromDraft(Guid draftId, string assetType, string language)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteAssetFromExtensionDraft");
      AssetInfo assetInfo = new AssetInfo(assetType, language);
      this.BindGuid(nameof (draftId), draftId);
      this.BindString(nameof (assetType), assetInfo.ToString(), 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    private void MapAssetsToDrafts(
      IList<ExtensionDraft> drafts,
      IList<ExtensionDraftAssetRow> draftAssets)
    {
      IDictionary<Guid, ExtensionDraft> dictionary = (IDictionary<Guid, ExtensionDraft>) new Dictionary<Guid, ExtensionDraft>();
      foreach (ExtensionDraft draft in (IEnumerable<ExtensionDraft>) drafts)
      {
        draft.Assets = new List<ExtensionDraftAsset>();
        dictionary.Add(draft.Id, draft);
      }
      foreach (ExtensionDraftAssetRow draftAsset in (IEnumerable<ExtensionDraftAssetRow>) draftAssets)
      {
        ExtensionDraft extensionDraft;
        if (dictionary.TryGetValue(draftAsset.DraftId, out extensionDraft))
        {
          List<ExtensionDraftAsset> assets = extensionDraft.Assets;
          ExtensionDraftAsset extensionDraftAsset = new ExtensionDraftAsset();
          extensionDraftAsset.AssetType = draftAsset.AssetType;
          extensionDraftAsset.Language = draftAsset.Language;
          extensionDraftAsset.ContentType = draftAsset.ContentType;
          extensionDraftAsset.FileId = draftAsset.FileId;
          extensionDraftAsset.IsOldAsset = draftAsset.IsOldAsset;
          extensionDraftAsset.IsPayloadAsset = draftAsset.IsPayloadAsset;
          assets.Add(extensionDraftAsset);
        }
      }
    }
  }
}
