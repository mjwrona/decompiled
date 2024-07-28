// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent32
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent32 : PublishedExtensionComponent31
  {
    public override PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionVersionToPublish)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateExtension");
      this.BindString("publisherName", extensionVersionToPublish.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", extensionVersionToPublish.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("displayName", extensionVersionToPublish.DisplayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("targetPlatform", extensionVersionToPublish.TargetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
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
      this.BindExtensionFileTable3("assets", extensionVersionToPublish.Assets);
      this.BindKeyValuePairInt32StringTable("tags", (IEnumerable<KeyValuePair<int, string>>) extensionVersionToPublish.Tags);
      this.BindKeyValuePairStringTable("metadata", (IEnumerable<KeyValuePair<string, string>>) extensionVersionToPublish.MetadataValues);
      this.BindExtensionInstallationTargetTable2("extensionInstallationTargets", extensionVersionToPublish.InstallationTargets);
      this.BindInt32Table("extensionLcids", (IEnumerable<int>) extensionVersionToPublish.ExtensionLcids);
      this.BindString("version", extensionVersionToPublish.Version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("versionDescription", extensionVersionToPublish.VersionDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreateExtension", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    public override PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionVersionToPublish)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtension");
      this.BindString("publisherName", extensionVersionToPublish.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", extensionVersionToPublish.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("displayName", extensionVersionToPublish.DisplayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("targetPlatform", extensionVersionToPublish.TargetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("flags", (int) extensionVersionToPublish.Flags);
      this.BindString("shortDescription", extensionVersionToPublish.ShortDescription, 300, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("longDescription", extensionVersionToPublish.LongDescription, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("versionFlags", (int) extensionVersionToPublish.VersionFlags);
      this.BindGuid("privateIdentityId", extensionVersionToPublish.PrivateIdentityId);
      this.BindGuid("ownerId", extensionVersionToPublish.OwnerId);
      this.BindString("cdnDirectory", extensionVersionToPublish.CdnDirectory, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("isCdnEnabled", extensionVersionToPublish.IsCdnEnabled);
      this.BindDateTime("updateTime", extensionVersionToPublish.PublishedTime);
      this.BindExtensionFileTable3("assets", extensionVersionToPublish.Assets);
      this.BindKeyValuePairInt32StringTable("tags", (IEnumerable<KeyValuePair<int, string>>) extensionVersionToPublish.Tags);
      this.BindKeyValuePairStringTable("metadata", (IEnumerable<KeyValuePair<string, string>>) extensionVersionToPublish.MetadataValues);
      this.BindExtensionInstallationTargetTable2("extensionInstallationTargets", extensionVersionToPublish.InstallationTargets);
      this.BindInt32Table("extensionLcids", (IEnumerable<int>) extensionVersionToPublish.ExtensionLcids);
      this.BindString("version", extensionVersionToPublish.Version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("versionDescription", extensionVersionToPublish.VersionDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateExtension", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    public override void AddAssetsForExtensionVersion(
      PublishedExtension extension,
      Guid validationId,
      IEnumerable<ExtensionFile> assets)
    {
      if (extension == null || extension.Versions == null || extension.Versions.Count != 1)
      {
        int num;
        if (extension == null)
        {
          num = 1;
        }
        else
        {
          Guid extensionId = extension.ExtensionId;
          num = 0;
        }
        throw new ExtensionDoesNotExistException(num != 0 ? "" : extension.ExtensionId.ToString());
      }
      this.PrepareStoredProcedure("Gallery.prc_AddAssetsForExtensionVersion");
      this.BindGuid("extensionId", extension.ExtensionId);
      this.BindString("version", extension.Versions[0].Version, 43, false, SqlDbType.VarChar);
      this.BindNullableGuid(nameof (validationId), validationId);
      this.BindString("targetPlatform", extension.Versions[0].TargetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindExtensionFileTable3(nameof (assets), assets);
      this.ExecuteNonQuery();
    }

    public override PublishedExtension ProcessValidationResult(
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      string message,
      bool success,
      int? fileId)
    {
      this.PrepareStoredProcedure("Gallery.prc_ProcessValidationResult");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindGuid(nameof (validationId), validationId);
      this.BindString(nameof (message), message, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean(nameof (success), success);
      this.BindNullableInt(nameof (fileId), fileId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_ProcessValidationResult", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary = this.ProcessExtensionResult(resultCollection, ExtensionQueryFlags.None);
        return dictionary != null && !dictionary.Values.IsNullOrEmpty<PublishedExtension>() ? dictionary.Values.FirstOrDefault<PublishedExtension>() : (PublishedExtension) null;
      }
    }

    public override void UpdateCDNProperties(
      Guid extensionId,
      string version,
      string cdnDirectory = null,
      bool isCdnEnabled = false,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateCDNProperties");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (cdnDirectory), cdnDirectory, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean(nameof (isCdnEnabled), isCdnEnabled);
      this.ExecuteNonQuery();
    }

    public override ExtensionVersionValidation QueryVersionValidation(
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryVersionValidation");
      this.BindString(nameof (publisherName), publisherName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryVersionValidation", this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidation>((ObjectBinder<ExtensionVersionValidation>) new PublishedExtensionComponent32.ExtensionVersionValidationBinder3());
        List<ExtensionVersionValidation> items = resultCollection.GetCurrent<ExtensionVersionValidation>().Items;
        return items.IsNullOrEmpty<ExtensionVersionValidation>() ? (ExtensionVersionValidation) null : items.First<ExtensionVersionValidation>();
      }
    }

    public override ExtensionVersionValidation QueryVersionValidation(
      Guid extensionId,
      string version,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryVersionValidation");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryVersionValidation", this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidation>((ObjectBinder<ExtensionVersionValidation>) new PublishedExtensionComponent32.ExtensionVersionValidationBinder3());
        List<ExtensionVersionValidation> items = resultCollection.GetCurrent<ExtensionVersionValidation>().Items;
        return items.IsNullOrEmpty<ExtensionVersionValidation>() ? (ExtensionVersionValidation) null : items.First<ExtensionVersionValidation>();
      }
    }

    public override ExtensionValidationResult GetValidationResult(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string targetPlatform = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetValidationResult");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetValidationResult", requestContext))
      {
        resultCollection.AddBinder<ExtensionValidationResult>((ObjectBinder<ExtensionValidationResult>) new ExtensionValidationResultBinder());
        List<ExtensionValidationResult> items = resultCollection.GetCurrent<ExtensionValidationResult>().Items;
        return items.IsNullOrEmpty<ExtensionValidationResult>() ? (ExtensionValidationResult) null : items.First<ExtensionValidationResult>();
      }
    }

    public virtual void UpdateWebExtensionTagForVSCodeExtensions(
      IVssRequestContext requestContext,
      List<Guid> webExtensionGuids,
      List<Guid> nonWebExtensionGuids)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateVSCodeExtensionsWithHiddenWebTags");
      this.BindGuidTable(nameof (webExtensionGuids), (IEnumerable<Guid>) webExtensionGuids);
      this.BindGuidTable(nameof (nonWebExtensionGuids), (IEnumerable<Guid>) nonWebExtensionGuids);
      this.ExecuteNonQuery();
    }

    protected override void ProcessExtensionVersions(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) && !flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersions) && !flags.HasFlag((Enum) ExtensionQueryFlags.IncludeLatestVersionOnly))
        return;
      resultCollection.AddBinder<ExtensionVersion>((ObjectBinder<ExtensionVersion>) new PublishedExtensionComponent32.ExtensionVersionBinder2());
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

    protected override void ProcessExtensionFiles(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles))
        return;
      resultCollection.AddBinder<ExtensionFile>((ObjectBinder<ExtensionFile>) new PublishedExtensionComponent32.ExtensionFileBinder2());
      resultCollection.NextResult();
      foreach (ExtensionFile extensionFile in resultCollection.GetCurrent<ExtensionFile>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(extensionFile.ReferenceId, out publishedExtension))
        {
          foreach (ExtensionVersion version in publishedExtension.Versions)
          {
            if (version.Version == extensionFile.Version && version.TargetPlatform == extensionFile.TargetPlatform)
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

    internal class ExtensionVersionBinder2 : ExtensionVersionBinder
    {
      protected SqlColumnBinder targetPlatformColumn = new SqlColumnBinder("TargetPlatform");

      protected override ExtensionVersion Bind()
      {
        ExtensionVersion extensionVersion = new ExtensionVersion()
        {
          ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader),
          Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
          LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
          VersionDescription = this.versionDescription.GetString((IDataReader) this.Reader, true),
          Flags = ExtensionVersionFlags.Validated
        };
        extensionVersion.Flags = (ExtensionVersionFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
        extensionVersion.ValidationResultMessage = this.validationResultMessage.GetString((IDataReader) this.Reader, true);
        extensionVersion.TargetPlatform = this.targetPlatformColumn.GetString((IDataReader) this.Reader, true);
        extensionVersion.CdnDirectory = this.cdnDirectoryColumn.GetString((IDataReader) this.Reader, true);
        extensionVersion.IsCdnEnabled = this.isCdnEnabledColumn.GetBoolean((IDataReader) this.Reader);
        return extensionVersion;
      }
    }

    internal class ExtensionFileBinder2 : ExtensionFileBinder
    {
      protected SqlColumnBinder targetPlatformColumn = new SqlColumnBinder("TargetPlatform");

      protected override ExtensionFile Bind()
      {
        AssetInfo assetInfo = new AssetInfo(this.assetTypeColumn.GetString((IDataReader) this.Reader, false));
        return new ExtensionFile()
        {
          ReferenceId = this.referenceIdColumn.GetGuid((IDataReader) this.Reader),
          Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
          TargetPlatform = this.targetPlatformColumn.GetString((IDataReader) this.Reader, true),
          AssetType = assetInfo.AssetType,
          Language = assetInfo.Language,
          ContentType = this.contentTypeColumn.GetString((IDataReader) this.Reader, false),
          FileId = this.fileIdColumn.GetInt32((IDataReader) this.Reader),
          ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true)
        };
      }
    }

    internal class ExtensionVersionValidationBinder3 : ExtensionVersionValidationBinder2
    {
      protected SqlColumnBinder targetPlatformColumn = new SqlColumnBinder("TargetPlatform");

      protected override ExtensionVersionValidation Bind() => new ExtensionVersionValidation()
      {
        PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, true),
        ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, true),
        ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader, true),
        Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
        TargetPlatform = this.targetPlatformColumn.GetString((IDataReader) this.Reader, true),
        ValidationId = this.validationIdColumn.GetGuid((IDataReader) this.Reader),
        LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
        VersionDescription = this.versionDescriptionColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
