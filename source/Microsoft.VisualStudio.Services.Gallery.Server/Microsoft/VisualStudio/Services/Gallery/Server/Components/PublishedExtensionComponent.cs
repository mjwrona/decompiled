// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506: Avoid excessive class coupling", Justification = "Raising a seperate bug 876407 for cleanup")]
  internal class PublishedExtensionComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "PublishedExtensionComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[23]
    {
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent>(1),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent21>(21),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent22>(22),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent23>(23),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent24>(24),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent25>(25),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent26>(26),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent27>(27),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent28>(28),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent29>(29),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent30>(30),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent31>(31),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent32>(32),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent33>(33),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent34>(34),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent35>(35),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent36>(36),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent37>(37),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent38>(38),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent39>(39),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent40>(40),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent41>(41),
      (IComponentCreator) new ComponentCreator<PublishedExtensionComponent42>(42)
    }, "Gallery");

    [SuppressMessage("Microsoft.Maintainability", "CA1506: Avoid excessive class coupling", Justification = "Raising a seperate bug 876407 for cleanup")]
    static PublishedExtensionComponent()
    {
      PublishedExtensionComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270002, new SqlExceptionFactory(typeof (ExtensionExistsException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270003, new SqlExceptionFactory(typeof (ExtensionDoesNotExistException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270015, new SqlExceptionFactory(typeof (ExtensionDoesNotExistException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270006, new SqlExceptionFactory(typeof (ExtensionMustBePrivateException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270016, new SqlExceptionFactory(typeof (ExtensionDoesNotExistException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270001, new SqlExceptionFactory(typeof (PublisherDoesNotExistException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270017, new SqlExceptionFactory(typeof (ExtensionAssetAlreadyExistsException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270024, new SqlExceptionFactory(typeof (ValidationStepInvalidParentIdException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270025, new SqlExceptionFactory(typeof (ValidationStepInvalidStepIdException)));
      PublishedExtensionComponent.s_sqlExceptionFactories.Add(270033, new SqlExceptionFactory(typeof (ExtensionAssetNotFoundException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PublishedExtensionComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (PublishedExtensionComponent);

    public virtual List<ExtensionCategory> QueryCategories(IVssRequestContext requestContext)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryCategories");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryCategories", requestContext))
      {
        resultCollection.AddBinder<ExtensionCategory>((ObjectBinder<ExtensionCategory>) new ExtensionCategoryBinder());
        return resultCollection.GetCurrent<ExtensionCategory>().Items;
      }
    }

    public virtual List<PublishedExtensionUpdate> GetExtensionData(IVssRequestContext requestContext)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetExtensionData");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetExtensionData", requestContext))
      {
        resultCollection.AddBinder<PublishedExtensionUpdate>((ObjectBinder<PublishedExtensionUpdate>) new PublishedExtensionUpdateBinder());
        return resultCollection.GetCurrent<PublishedExtensionUpdate>().Items;
      }
    }

    public virtual void UpdateExtensionInstallationTargets(
      string publisherName,
      string extensionName,
      bool isExtensionPublic,
      IEnumerable<InstallationTarget> InstallationTargets)
    {
      throw new NotImplementedException();
    }

    public virtual void AddAssetsForExtensionVersion(
      PublishedExtension extension,
      Guid validationId,
      IEnumerable<ExtensionFile> assets)
    {
      throw new NotImplementedException();
    }

    public virtual ExtensionVersionValidationStep CreateValidationStep(
      IVssRequestContext requestContext,
      Guid stepId,
      Guid parentValidationId,
      int stepType,
      int stepStatus,
      DateTime startTime)
    {
      throw new NotImplementedException();
    }

    public virtual ExtensionValidationResult GetValidationResult(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version)
    {
      throw new NotImplementedException();
    }

    public virtual List<ExtensionVersionValidationStep> GetAllValidationSteps(
      IVssRequestContext requestContext,
      Guid parentValidationId)
    {
      throw new NotImplementedException();
    }

    public virtual ExtensionVersionValidationStep UpdateValidationStep(
      IVssRequestContext requestContext,
      ExtensionVersionValidationStep step)
    {
      throw new NotImplementedException();
    }

    public virtual void DeleteValidationStepsByParentId(
      IVssRequestContext requestContext,
      Guid parentValidationId)
    {
      throw new NotImplementedException();
    }

    public virtual ExtensionVersionValidation QueryVersionValidation(
      Guid extensionId,
      string version,
      string targetPlatform = null)
    {
      throw new NotImplementedException();
    }

    public virtual ExtensionVersionValidation QueryVersionValidation(
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null)
    {
      throw new NotImplementedException();
    }

    public void EnableVsExtensionConsolidation(Guid extensionId)
    {
      this.PrepareStoredProcedure("Gallery.prc_EnableVsExtensionConsolidation");
      this.BindGuid(nameof (extensionId), extensionId);
      this.ExecuteNonQuery();
    }

    public virtual void CreateBackConsolidationMapping(
      Guid sourceExtensionId,
      string sourceExtensionName,
      Guid targetExtensionId,
      string targetExensionName)
    {
      throw new NotImplementedException();
    }

    public virtual IReadOnlyDictionary<string, BackConsolidationMappingEntry> GetBackConsolidationMapping(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }

    public virtual bool DoesExtensionNameExists(string extensionName, string installationTarget)
    {
      this.PrepareStoredProcedure("Gallery.prc_DoesExtensionNameExistForTarget");
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("target", installationTarget, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return (bool) this.ExecuteScalar();
    }

    public virtual void UpdateAssetIdForExtensionVersion(
      PublishedExtension extension,
      string assetType,
      int fileId)
    {
      throw new NotImplementedException();
    }

    public virtual int GetNumberOfExtensionsCreatedByPublisherForDuration(
      string publisherName,
      int durationInMinutes)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetNumberOfExtensionCreatedByPublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("lowerBoundInMinute", durationInMinutes);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }

    public virtual ExtensionValidationResult GetValidationResult(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string targetPlatform = null)
    {
      throw new NotImplementedException();
    }

    public virtual bool DoesExtensionDisplayNameExists(
      string extensionDisplayName,
      string installationTarget)
    {
      return false;
    }

    public virtual TypoSquattingData GetTyposquattingData(IVssRequestContext requestContext) => new TypoSquattingData();

    public virtual void UpdateCDNProperties(
      Guid extensionId,
      string version,
      string cdnDirectory = null,
      bool isCdnEnabled = false,
      string targetPlatform = null)
    {
      throw new NotImplementedException();
    }

    public virtual PublishedExtension ProcessValidationResult(
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      string message,
      bool success,
      int? fileId)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateisRepositorySignedProperty(
      Guid extensionId,
      string version,
      string targetPlatform = null,
      bool isRepositorySigned = false)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateIsPublisherSigned(
      Guid extensionId,
      string version,
      string targetPlatform = null,
      bool isPublisherSigned = false)
    {
      throw new NotImplementedException();
    }

    protected Dictionary<Guid, PublishedExtension> ProcessExtensionResult(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags)
    {
      List<PublishedExtension> items = resultCollection.GetCurrent<PublishedExtension>().Items;
      Dictionary<Guid, PublishedExtension> extensionMap = new Dictionary<Guid, PublishedExtension>();
      foreach (PublishedExtension publishedExtension in items)
        extensionMap[publishedExtension.ExtensionId] = publishedExtension;
      this.ProcessExtensionVersions(resultCollection, flags, extensionMap);
      this.ProcessExtensionFiles(resultCollection, flags, extensionMap);
      this.ProcessCategoriesAndTags(resultCollection, flags, extensionMap);
      this.ProcessExtensionShares(resultCollection, flags, extensionMap);
      this.ProcessInstallationTargets(resultCollection, flags, extensionMap);
      this.ProcessExtensionStatistics(resultCollection, flags, extensionMap);
      this.ProcessExtensionMetadata(resultCollection, flags, extensionMap);
      this.ProcessExtensionLcids(resultCollection, flags, extensionMap);
      return extensionMap;
    }

    protected virtual void ProcessExtensionLcids(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
    }

    protected virtual void ProcessExtensionVersions(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) && !flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersions) && !flags.HasFlag((Enum) ExtensionQueryFlags.IncludeLatestVersionOnly))
        return;
      resultCollection.AddBinder<ExtensionVersion>((ObjectBinder<ExtensionVersion>) this.GetExtensionVersionBinder());
      resultCollection.NextResult();
      foreach (ExtensionVersion extensionVersion in resultCollection.GetCurrent<ExtensionVersion>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(extensionVersion.ExtensionId, out publishedExtension))
        {
          if (publishedExtension.Versions == null)
            publishedExtension.Versions = new List<ExtensionVersion>();
          publishedExtension.Versions.Add(extensionVersion);
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessExtensionVersions), string.Format("No key found for Extension : {0} while processing extension versions", (object) extensionVersion.ExtensionId));
      }
    }

    protected virtual void ProcessExtensionFiles(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles))
        return;
      resultCollection.AddBinder<ExtensionFile>((ObjectBinder<ExtensionFile>) new ExtensionFileBinder());
      resultCollection.NextResult();
      foreach (ExtensionFile extensionFile in resultCollection.GetCurrent<ExtensionFile>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(extensionFile.ReferenceId, out publishedExtension))
        {
          foreach (ExtensionVersion version in publishedExtension.Versions)
          {
            if (version.Version == extensionFile.Version)
            {
              if (version.Files == null)
                version.Files = new List<ExtensionFile>();
              version.Files.Add(extensionFile);
            }
          }
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessExtensionFiles), string.Format("No key found for Extension : {0} while processing extension files", (object) extensionFile.ReferenceId));
      }
    }

    protected virtual void ProcessCategoriesAndTags(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeCategoryAndTags))
        return;
      resultCollection.AddBinder<GalleryTag>((ObjectBinder<GalleryTag>) new GalleryTagBinder());
      resultCollection.NextResult();
      foreach (GalleryTag galleryTag in resultCollection.GetCurrent<GalleryTag>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(galleryTag.ReferenceId, out publishedExtension))
        {
          if (publishedExtension.Categories == null)
            publishedExtension.Categories = new List<string>();
          publishedExtension.Categories.Add(galleryTag.TagName);
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", "ProcessCategories", string.Format("No key found for Extension : {0} while processing extension categories", (object) galleryTag.ReferenceId));
      }
      resultCollection.AddBinder<GalleryTag>((ObjectBinder<GalleryTag>) new GalleryTagBinder());
      resultCollection.NextResult();
      foreach (GalleryTag galleryTag in resultCollection.GetCurrent<GalleryTag>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(galleryTag.ReferenceId, out publishedExtension))
        {
          if (publishedExtension.Tags == null)
            publishedExtension.Tags = new List<string>();
          publishedExtension.Tags.Add(galleryTag.TagName);
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", "ProcessTags", string.Format("No key found for Extension : {0} while processing extension tags", (object) galleryTag.ReferenceId));
      }
    }

    protected virtual void ProcessInstallationTargets(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeInstallationTargets))
        return;
      resultCollection.AddBinder<GalleryInstallationTarget>((ObjectBinder<GalleryInstallationTarget>) new GalleryInstallationTargetBinder());
      resultCollection.NextResult();
      foreach (GalleryInstallationTarget installationTarget in resultCollection.GetCurrent<GalleryInstallationTarget>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(installationTarget.ReferenceId, out publishedExtension))
        {
          if (publishedExtension.InstallationTargets == null)
            publishedExtension.InstallationTargets = new List<InstallationTarget>();
          publishedExtension.InstallationTargets.Add(new InstallationTarget()
          {
            Target = installationTarget.Target,
            TargetVersion = installationTarget.TargetVersion,
            MaxInclusive = installationTarget.MaxInclusive,
            MinInclusive = installationTarget.MinInclusive,
            MaxVersion = installationTarget.MaxVersion,
            MinVersion = installationTarget.MinVersion
          });
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessInstallationTargets), string.Format("No key found for Extension : {0} while processing extension installation targets", (object) installationTarget.ReferenceId));
      }
    }

    public virtual ExtensionSearchResult SearchExtensions(
      ExtensionSearchParams searchParams,
      ExtensionQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_SearchExtensions");
      this.BindSearchFilterValueTable("searchFilters", (IEnumerable<SearchCriteria>) searchParams.CriteriaList);
      this.BindInt("pageNumber", searchParams.PageNumber);
      this.BindInt("pageSize", searchParams.PageSize);
      this.BindInt("sortByType", searchParams.SortBy);
      this.BindInt(nameof (flags), (int) flags);
      this.BindInt("sortOrderType", searchParams.SortOrder);
      this.BindInt("metadataFlags", (int) searchParams.MetadataFlags);
      this.BindString("product", searchParams.Product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      ExtensionSearchResult extensionSearchResult = new ExtensionSearchResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_SearchExtensions", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary1 = this.ProcessExtensionResult(resultCollection, flags);
        Dictionary<int, List<ExtensionFilterResultMetadata>> dictionary2 = this.ProcessResultMetadata(resultCollection);
        extensionSearchResult.Results = dictionary1.Values.ToList<PublishedExtension>();
        if (dictionary2.ContainsKey(0))
          extensionSearchResult.ResultMetadata = dictionary2[0];
        return extensionSearchResult;
      }
    }

    public virtual ExtensionQueryResult QueryExtensionsByQuery(
      List<QueryFilter> filters,
      List<QueryFilterValue> filterValues,
      ExtensionQueryFlags flags,
      string product)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryExtensionsByQuery");
      this.BindQueryFiltersTable("queryFilter", (IEnumerable<QueryFilter>) filters);
      this.BindQueryFilterValuesTable("queryFilterValue", (IEnumerable<QueryFilterValue>) filterValues);
      this.BindInt(nameof (flags), (int) flags);
      this.BindString(nameof (product), product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.QueryExtensionsByQuery", this.RequestContext))
      {
        resultCollection.AddBinder<QueryMatch>((ObjectBinder<QueryMatch>) new QueryMatchBinder());
        List<QueryMatch> items = resultCollection.GetCurrent<QueryMatch>().Items;
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        resultCollection.NextResult();
        Dictionary<Guid, PublishedExtension> dictionary1 = this.ProcessExtensionResult(resultCollection, flags);
        Dictionary<int, List<ExtensionFilterResultMetadata>> dictionary2 = this.ProcessResultMetadata(resultCollection);
        ExtensionQueryResult extensionQueryResult = new ExtensionQueryResult();
        extensionQueryResult.Results = new List<ExtensionFilterResult>();
        foreach (QueryFilter filter in filters)
          extensionQueryResult.Results.Add(new ExtensionFilterResult()
          {
            Extensions = new List<PublishedExtension>()
          });
        foreach (QueryMatch queryMatch in items)
          extensionQueryResult.Results[queryMatch.QueryIndex].Extensions.Add(dictionary1[queryMatch.ReferenceId]);
        foreach (KeyValuePair<int, List<ExtensionFilterResultMetadata>> keyValuePair in dictionary2)
          extensionQueryResult.Results[keyValuePair.Key].ResultMetadata = keyValuePair.Value;
        return extensionQueryResult;
      }
    }

    protected virtual Dictionary<int, List<ExtensionFilterResultMetadata>> ProcessResultMetadata(
      ResultCollection resultCollection)
    {
      Dictionary<int, List<ExtensionFilterResultMetadata>> dictionary = new Dictionary<int, List<ExtensionFilterResultMetadata>>();
      resultCollection.AddBinder<ExtensionResultMetadataRow>((ObjectBinder<ExtensionResultMetadataRow>) new ExtensionResultMetadataRowBinder());
      resultCollection.NextResult();
      foreach (ExtensionResultMetadataRow resultMetadataRow in resultCollection.GetCurrent<ExtensionResultMetadataRow>().Items)
      {
        List<ExtensionFilterResultMetadata> filterResultMetadataList = (List<ExtensionFilterResultMetadata>) null;
        if (!dictionary.TryGetValue(resultMetadataRow.QueryIndex, out filterResultMetadataList))
        {
          filterResultMetadataList = new List<ExtensionFilterResultMetadata>();
          dictionary[resultMetadataRow.QueryIndex] = filterResultMetadataList;
        }
        ExtensionFilterResultMetadata filterResultMetadata1 = (ExtensionFilterResultMetadata) null;
        foreach (ExtensionFilterResultMetadata filterResultMetadata2 in filterResultMetadataList)
        {
          if (filterResultMetadata2.MetadataType.Equals(resultMetadataRow.MetadataType))
          {
            filterResultMetadata1 = filterResultMetadata2;
            break;
          }
        }
        if (filterResultMetadata1 == null)
        {
          filterResultMetadata1 = new ExtensionFilterResultMetadata();
          filterResultMetadata1.MetadataType = resultMetadataRow.MetadataType;
          filterResultMetadata1.MetadataItems = new List<MetadataItem>();
          filterResultMetadataList.Add(filterResultMetadata1);
        }
        filterResultMetadata1.MetadataItems.Add(new MetadataItem()
        {
          Name = resultMetadataRow.MetadataName,
          Count = resultMetadataRow.Value
        });
      }
      return dictionary;
    }

    public virtual PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionVersionToPublish)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateExtension");
      this.BindString("publisherName", extensionVersionToPublish.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", extensionVersionToPublish.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("displayName", extensionVersionToPublish.DisplayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("ownerId", extensionVersionToPublish.OwnerId);
      this.BindInt("flags", (int) extensionVersionToPublish.Flags);
      this.BindString("shortDescription", extensionVersionToPublish.ShortDescription, 300, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("longDescription", extensionVersionToPublish.LongDescription, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("versionFlags", (int) extensionVersionToPublish.VersionFlags);
      this.BindGuid("privateIdentityId", extensionVersionToPublish.PrivateIdentityId);
      this.BindString("cdnDirectory", extensionVersionToPublish.CdnDirectory, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("isCdnEnabled", extensionVersionToPublish.IsCdnEnabled);
      this.BindGuid("extensionIdToCreate", extensionVersionToPublish.ExtensionId);
      this.BindDateTime("creationTime", extensionVersionToPublish.PublishedTime);
      this.BindExtensionFileTable2("assets", extensionVersionToPublish.Assets);
      this.BindKeyValuePairInt32StringTable("tags", (IEnumerable<KeyValuePair<int, string>>) extensionVersionToPublish.Tags);
      this.BindKeyValuePairStringTable("metadata", (IEnumerable<KeyValuePair<string, string>>) extensionVersionToPublish.MetadataValues);
      this.BindExtensionInstallationTargetTable("extensionInstallationTargets", extensionVersionToPublish.InstallationTargets);
      this.BindInt32Table("extensionLcids", (IEnumerable<int>) extensionVersionToPublish.ExtensionLcids);
      this.BindString("version", extensionVersionToPublish.Version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("versionDescription", extensionVersionToPublish.VersionDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreateExtension", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    public virtual PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionVersionToPublish)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtension");
      this.BindString("publisherName", extensionVersionToPublish.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", extensionVersionToPublish.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("displayName", extensionVersionToPublish.DisplayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("flags", (int) extensionVersionToPublish.Flags);
      this.BindString("shortDescription", extensionVersionToPublish.ShortDescription, 300, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("longDescription", extensionVersionToPublish.LongDescription, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("versionFlags", (int) extensionVersionToPublish.VersionFlags);
      this.BindGuid("privateIdentityId", extensionVersionToPublish.PrivateIdentityId);
      this.BindString("cdnDirectory", extensionVersionToPublish.CdnDirectory, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("isCdnEnabled", extensionVersionToPublish.IsCdnEnabled);
      this.BindDateTime("updateTime", extensionVersionToPublish.PublishedTime);
      this.BindExtensionFileTable2("assets", extensionVersionToPublish.Assets);
      this.BindKeyValuePairInt32StringTable("tags", (IEnumerable<KeyValuePair<int, string>>) extensionVersionToPublish.Tags);
      this.BindKeyValuePairStringTable("metadata", (IEnumerable<KeyValuePair<string, string>>) extensionVersionToPublish.MetadataValues);
      this.BindExtensionInstallationTargetTable("extensionInstallationTargets", extensionVersionToPublish.InstallationTargets);
      this.BindInt32Table("extensionLcids", (IEnumerable<int>) extensionVersionToPublish.ExtensionLcids);
      this.BindString("version", extensionVersionToPublish.Version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("versionDescription", extensionVersionToPublish.VersionDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateExtension", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    public virtual void ShareExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string accountName,
      Guid accountId,
      Guid collectionId,
      bool removeAccount)
    {
      this.PrepareStoredProcedure("Gallery.prc_ShareExtension");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (accountName), accountName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindGuid(nameof (accountId), accountId);
      this.BindGuid(nameof (collectionId), collectionId);
      this.BindBoolean(nameof (removeAccount), removeAccount);
      this.ExecuteNonQuery();
    }

    protected virtual void ProcessExtensionShares(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts))
        return;
      resultCollection.AddBinder<GalleryTag2>((ObjectBinder<GalleryTag2>) new GalleryTagBinder2());
      resultCollection.NextResult();
      foreach (GalleryTag2 galleryTag2 in resultCollection.GetCurrent<GalleryTag2>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(galleryTag2.ReferenceId, out publishedExtension))
        {
          if (publishedExtension.SharedWith == null)
            publishedExtension.SharedWith = new List<ExtensionShare>();
          publishedExtension.SharedWith.Add(new ExtensionShare()
          {
            Id = galleryTag2.TempTagName,
            Type = "account",
            Name = galleryTag2.Comment
          });
          publishedExtension.SharedWith.Add(new ExtensionShare()
          {
            Id = galleryTag2.TagName,
            Type = "account",
            Name = galleryTag2.Comment
          });
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessExtensionShares), string.Format("No key found for Extension : {0} while processing extension shares", (object) galleryTag2.ReferenceId));
      }
    }

    protected virtual void ProcessExtensionMetadata(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeMetadata))
        return;
      resultCollection.AddBinder<ExtensionMetadataRow>((ObjectBinder<ExtensionMetadataRow>) new ExtensionMetadataBinder());
      resultCollection.NextResult();
      foreach (ExtensionMetadataRow extensionMetadataRow in resultCollection.GetCurrent<ExtensionMetadataRow>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(extensionMetadataRow.ExtensionId, out publishedExtension))
        {
          if (publishedExtension.Metadata == null)
            publishedExtension.Metadata = new List<ExtensionMetadata>();
          publishedExtension.Metadata.Add(new ExtensionMetadata()
          {
            Key = extensionMetadataRow.KeyName,
            Value = extensionMetadataRow.Value
          });
          if (publishedExtension.DeploymentType == (ExtensionDeploymentTechnology) 0 && string.Equals(extensionMetadataRow.KeyName, "DeploymentTechnology", StringComparison.OrdinalIgnoreCase))
            publishedExtension.DeploymentType = this.GetDeploymentTechnology(extensionMetadataRow.Value);
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessExtensionMetadata), string.Format("No key found for Extension : {0} while processing extension metadata", (object) extensionMetadataRow.ExtensionId));
      }
    }

    public virtual List<ExtensionCategory> GetCategories(IVssRequestContext requestContext)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetCategories");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetCategories", requestContext))
      {
        resultCollection.AddBinder<ExtensionCategory>((ObjectBinder<ExtensionCategory>) new ExtensionCategoryBinder2());
        List<ExtensionCategory> items1 = resultCollection.GetCurrent<ExtensionCategory>().Items;
        resultCollection.AddBinder<ProductCategoryRow>((ObjectBinder<ProductCategoryRow>) new ProductCategoryBinder());
        resultCollection.NextResult();
        List<ProductCategoryRow> items2 = resultCollection.GetCurrent<ProductCategoryRow>().Items;
        resultCollection.AddBinder<CategoryTitleRow>((ObjectBinder<CategoryTitleRow>) new CategoryTitleBinder());
        resultCollection.NextResult();
        List<CategoryTitleRow> items3 = resultCollection.GetCurrent<CategoryTitleRow>().Items;
        Dictionary<int, ExtensionCategory> dictionary = new Dictionary<int, ExtensionCategory>();
        foreach (ExtensionCategory extensionCategory in items1)
          dictionary.Add(extensionCategory.CategoryId, extensionCategory);
        foreach (ExtensionCategory extensionCategory1 in items1)
        {
          ExtensionCategory extensionCategory2;
          if (extensionCategory1.ParentId != 0 && dictionary.TryGetValue(extensionCategory1.ParentId, out extensionCategory2))
          {
            extensionCategory1.Parent = extensionCategory2;
            extensionCategory1.ParentCategoryName = extensionCategory2.CategoryName;
          }
        }
        foreach (ProductCategoryRow productCategoryRow in items2)
        {
          ExtensionCategory extensionCategory;
          if (dictionary.TryGetValue(productCategoryRow.CategoryId, out extensionCategory))
          {
            if (extensionCategory.AssociatedProducts.IsNullOrEmpty<string>())
              extensionCategory.AssociatedProducts = new List<string>();
            extensionCategory.AssociatedProducts.Add(productCategoryRow.Product);
          }
        }
        foreach (CategoryTitleRow categoryTitleRow in items3)
        {
          ExtensionCategory extensionCategory;
          if (dictionary.TryGetValue(categoryTitleRow.CategoryId, out extensionCategory))
          {
            if (extensionCategory.LanguageTitles.IsNullOrEmpty<CategoryLanguageTitle>())
              extensionCategory.LanguageTitles = new List<CategoryLanguageTitle>();
            extensionCategory.LanguageTitles.Add(new CategoryLanguageTitle()
            {
              Lang = categoryTitleRow.Lang,
              Title = categoryTitleRow.Title,
              Lcid = categoryTitleRow.Lcid
            });
          }
        }
        return items1;
      }
    }

    public virtual IReadOnlyDictionary<string, string> GetConflictList(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }

    public virtual int AddCategory(ExtensionCategory category)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateCategory");
      this.BindString("CategoryInternalName", category.CategoryName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("ParentCategoryInternalName", category.ParentCategoryName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindStringTable("ProductNames", (IEnumerable<string>) category.AssociatedProducts, true);
      this.BindCategoryTitleLangTable("LangTitles", (IEnumerable<CategoryLanguageTitle>) category.LanguageTitles);
      if (category.MigratedId != 0)
        this.BindInt("MigratedId", category.MigratedId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreateCategory", this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ExtensionCategoryIdBinder());
        return resultCollection.GetCurrent<int>().Items[0];
      }
    }

    public void AddAssetForExtensionVersion(
      Guid extensionId,
      string version,
      string assetType,
      string contentType,
      int fileId,
      string shortDescription)
    {
      this.PrepareStoredProcedure("Gallery.prc_AddAssetForExtensionVersion");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (assetType), assetType, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (contentType), contentType, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (fileId), fileId);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    protected virtual ExtensionVersionBinder GetExtensionVersionBinder() => new ExtensionVersionBinder();

    public virtual void UpdateCDNProperties(
      Guid extensionId,
      string version,
      string cdnDirectory = null,
      bool isCdnEnabled = false)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateCDNProperties");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (cdnDirectory), cdnDirectory, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean(nameof (isCdnEnabled), isCdnEnabled);
      this.ExecuteNonQuery();
    }

    public virtual PublishedExtension ApplyExtensionIndexedTerm(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      TagType tagType,
      string tagValue,
      bool remove = false,
      bool updateExtension = true)
    {
      this.PrepareStoredProcedure("Gallery.prc_ApplyExtensionIndexedTerm");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (tagType), (int) tagType);
      this.BindString(nameof (tagValue), tagValue, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean(nameof (remove), remove);
      this.BindBoolean(nameof (updateExtension), updateExtension);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_ApplyExtensionIndexedTerm", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    protected virtual PublishedExtensionBinder GetPublishedExtensionBinder() => (PublishedExtensionBinder) new PublishedExtensionBinder2();

    public virtual void UpdateReleaseDate(Guid extensionId, DateTime releaseDateToUpdate)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionReleaseDate");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindDateTime("releaseDate", releaseDateToUpdate);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteExtension(string publisherName, string extensionName, string version)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteExtension");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual PublishedExtension ProcessValidationResult(
      Guid extensionId,
      string version,
      Guid validationId,
      string message,
      bool success,
      int? fileId = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_ProcessValidationResult");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (validationId), validationId);
      this.BindString(nameof (message), message, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean(nameof (success), success);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_ProcessValidationResult", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return this.ProcessExtensionResult(resultCollection, ExtensionQueryFlags.None).Values.FirstOrDefault<PublishedExtension>();
      }
    }

    public virtual PublishedExtension QueryExtension(
      string publisherName,
      string extensionName,
      string version,
      Guid validationId,
      ExtensionQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryExtension");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindNullableGuid(nameof (validationId), validationId);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryExtension", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary = this.ProcessExtensionResult(resultCollection, flags);
        return dictionary.Count != 0 ? dictionary.Values.First<PublishedExtension>() : throw new ExtensionDoesNotExistException(GalleryWebApiResources.ExtensionDoesNotExist((object) GalleryUtil.CreateExtensionIdForExceptionMessage(publisherName, extensionName)));
      }
    }

    public virtual PublishedExtension QueryExtensionById(
      Guid extensionId,
      string version,
      Guid validationId,
      ExtensionQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryExtensionById");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindNullableGuid(nameof (validationId), validationId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryExtensionById", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary = this.ProcessExtensionResult(resultCollection, flags);
        return dictionary.Count != 0 ? dictionary.Values.First<PublishedExtension>() : throw new ExtensionDoesNotExistException(GalleryWebApiResources.ExtensionDoesNotExist((object) extensionId.ToString()));
      }
    }

    public virtual ExtensionVersionValidation QueryVersionValidation(
      string publisherName,
      string extensionName,
      string version)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryVersionValidation");
      this.BindString(nameof (publisherName), publisherName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryVersionValidation", this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidation>((ObjectBinder<ExtensionVersionValidation>) new ExtensionVersionValidationBinder2());
        List<ExtensionVersionValidation> items = resultCollection.GetCurrent<ExtensionVersionValidation>().Items;
        return items.Count == 0 ? (ExtensionVersionValidation) null : items.First<ExtensionVersionValidation>();
      }
    }

    public virtual ExtensionVersionValidation QueryVersionValidation(
      Guid extensionId,
      string version)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryVersionValidation");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryVersionValidation", this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidation>((ObjectBinder<ExtensionVersionValidation>) new ExtensionVersionValidationBinder());
        List<ExtensionVersionValidation> items = resultCollection.GetCurrent<ExtensionVersionValidation>().Items;
        return items.Count == 0 ? (ExtensionVersionValidation) null : items.First<ExtensionVersionValidation>();
      }
    }

    protected virtual void ProcessExtensionStatistics(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeStatistics))
        return;
      resultCollection.AddBinder<ExtensionStatisticRow>((ObjectBinder<ExtensionStatisticRow>) new ExtensionStatisticBinder());
      resultCollection.NextResult();
      foreach (ExtensionStatisticRow extensionStatisticRow in resultCollection.GetCurrent<ExtensionStatisticRow>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(extensionStatisticRow.ExtensionId, out publishedExtension))
        {
          if (publishedExtension.Statistics == null)
            publishedExtension.Statistics = new List<ExtensionStatistic>();
          publishedExtension.Statistics.Add(new ExtensionStatistic()
          {
            StatisticName = extensionStatisticRow.Name,
            Value = extensionStatisticRow.Value
          });
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessExtensionStatistics), string.Format("No key found for Extension : {0} while processing extension statistics", (object) extensionStatisticRow.ExtensionId));
      }
    }

    public virtual PublishedExtension UpdateExtensionProperties(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string displayName,
      PublishedExtensionFlags flags,
      string shortDescription,
      string longDescription,
      Guid privateIdenitityId)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionProperties");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (displayName), displayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt(nameof (flags), (int) flags);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (longDescription), longDescription, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("privateIdentityId", privateIdenitityId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateExtensionProperties", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    private ExtensionDeploymentTechnology GetDeploymentTechnology(string value)
    {
      switch (value.ToLowerInvariant())
      {
        case "referral link":
          return ExtensionDeploymentTechnology.ReferralLink;
        case "vsix":
          return ExtensionDeploymentTechnology.Vsix;
        case "msi":
          return ExtensionDeploymentTechnology.Msi;
        case "exe":
          return ExtensionDeploymentTechnology.Exe;
        default:
          return (ExtensionDeploymentTechnology) 0;
      }
    }

    internal void SetExtensionFlags(Guid extensionId, PublishedExtensionFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_SetExtensionFlags");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindInt(nameof (flags), (int) flags);
      this.ExecuteNonQuery();
    }

    internal PublishedExtensionFlags GetExtensionFlags(Guid extensionId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetExtensionFlags");
      this.BindGuid(nameof (extensionId), extensionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetExtensionFlags", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtensionFlags>((ObjectBinder<PublishedExtensionFlags>) new PublishedExtensionComponent.GetPublishedExtensionFlags());
        List<PublishedExtensionFlags> items = resultCollection.GetCurrent<PublishedExtensionFlags>().Items;
        return items.Any<PublishedExtensionFlags>() ? items[0] : PublishedExtensionFlags.None;
      }
    }

    protected class GetPublishedExtensionFlags : ObjectBinder<PublishedExtensionFlags>
    {
      protected SqlColumnBinder flagsColumn = new SqlColumnBinder("Flags");

      protected override PublishedExtensionFlags Bind() => (PublishedExtensionFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
    }
  }
}
