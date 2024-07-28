// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent35
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent35 : PublishedExtensionComponent34
  {
    public override PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionVersionToPublish)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateExtension");
      this.BindString("publisherName", extensionVersionToPublish.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", extensionVersionToPublish.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("displayName", extensionVersionToPublish.DisplayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("targetPlatform", extensionVersionToPublish.TargetPlatform, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
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
      this.BindExtensionInstallationTargetTable3("extensionInstallationTargets", extensionVersionToPublish.InstallationTargets);
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
      this.BindString("targetPlatform", extensionVersionToPublish.TargetPlatform, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
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
      this.BindExtensionInstallationTargetTable3("extensionInstallationTargets", extensionVersionToPublish.InstallationTargets);
      this.BindInt32Table("extensionLcids", (IEnumerable<int>) extensionVersionToPublish.ExtensionLcids);
      this.BindString("version", extensionVersionToPublish.Version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("versionDescription", extensionVersionToPublish.VersionDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateExtension", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        return resultCollection.GetCurrent<PublishedExtension>().Items[0];
      }
    }

    public override void UpdateExtensionInstallationTargets(
      string publisherName,
      string extensionName,
      bool isExtensionPublic,
      IEnumerable<InstallationTarget> installationTargets)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionInstallationTarget");
      this.BindExtensionInstallationTargetTable3("extensionInstallationTargets", installationTargets);
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("isPublicExtension", isExtensionPublic);
      this.ExecuteNonQuery();
    }

    protected override void ProcessInstallationTargets(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeInstallationTargets))
        return;
      resultCollection.AddBinder<GalleryInstallationTarget3>((ObjectBinder<GalleryInstallationTarget3>) new GalleryInstallationTargetBinder3());
      resultCollection.NextResult();
      foreach (GalleryInstallationTarget3 installationTarget3 in resultCollection.GetCurrent<GalleryInstallationTarget3>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(installationTarget3.ReferenceId, out publishedExtension))
        {
          if (publishedExtension.InstallationTargets == null)
            publishedExtension.InstallationTargets = new List<InstallationTarget>();
          publishedExtension.InstallationTargets.Add(new InstallationTarget()
          {
            Target = installationTarget3.Target,
            TargetVersion = installationTarget3.TargetVersion,
            ProductArchitecture = installationTarget3.ProductArchitecture,
            MaxInclusive = installationTarget3.MaxInclusive,
            MinInclusive = installationTarget3.MinInclusive,
            MaxVersion = installationTarget3.MaxVersion,
            MinVersion = installationTarget3.MinVersion,
            ExtensionVersion = installationTarget3.ExtensionVersion,
            TargetPlatform = installationTarget3.TargetPlatform
          });
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessInstallationTargets), string.Format("No key found for Extension : {0} while processing extension installation targets", (object) installationTarget3.ReferenceId));
      }
    }

    public override IReadOnlyDictionary<string, string> GetConflictList(
      IVssRequestContext requestContext)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetConflictList");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetConflictList", requestContext))
      {
        resultCollection.AddBinder<ConflictListRow>((ObjectBinder<ConflictListRow>) new ConflictListBinder());
        List<ConflictListRow> items = resultCollection.GetCurrent<ConflictListRow>().Items;
        IDictionary<string, string> conflictList = (IDictionary<string, string>) new Dictionary<string, string>();
        foreach (ConflictListRow conflictListRow in items)
          conflictList.Add(conflictListRow.OldName, conflictListRow.NewName);
        return (IReadOnlyDictionary<string, string>) conflictList;
      }
    }
  }
}
