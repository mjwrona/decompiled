// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IDataAccessLayer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public interface IDataAccessLayer
  {
    void GetMetadata(
      IVssIdentity user,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string dbStamp,
      out int mode);

    XmlElement BulkUpdate(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      out bool bulkUpdateSuccess);

    XmlElement BulkUpdate(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      out bool bulkUpdateSuccess,
      bool bypassRules);

    XmlElement Update(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      bool bypassRules);

    XmlElement Update(XmlElement package, bool overwrite = false, bool provisionRules = true);

    XmlElement UpdateImpl(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      bool bulkUpdate,
      out bool bulkUpdateSuccess,
      IVssIdentity user,
      bool overwrite = false,
      bool bypassRules = false,
      bool validationOnly = false,
      bool authorizeAdminChanges = true);

    bool HandleStaleViewsException(XmlElement package, IVssIdentity user, Exception e);

    void TryRefreshReadViews(IVssIdentity user);

    void TryRefreshWriteViews(IVssIdentity user, XmlElement failedUpdateBatch);

    ExtendedAccessControlListData GetQueryAccessControlList(Guid queryItemId, bool getMetadata);

    Microsoft.VisualStudio.Services.Identity.Identity GetQueryItemOwner(
      IVssRequestContext requestContext,
      Guid queryItemId);

    void DeleteQueryItem(Guid queryId);

    void UpdateQueryItem(Guid id, Guid parentId, string name, string queryText);

    void UpdateQueryItemOwner(
      IVssRequestContext requestContext,
      Guid queryItemId,
      IdentityDescriptor descriptor);

    void CreateQueryItem(Guid id, Guid parentId, string name, string queryText);

    Artifact[] GetArtifacts(IVssIdentity user, string[] artifactUriList);

    Artifact[] GetReferencingArtifacts(IVssIdentity user, string[] artifactUriList);

    Artifact[] GetReferencingArtifacts(
      IVssIdentity user,
      string[] artifactUriList,
      LinkFilter[] linkFilterList);

    string[] GetReferencingWorkitemUris(IVssIdentity user, string artifactUri);

    TeamFoundationDataReader GetWorkItemIds(
      IVssRequestContext context,
      long rowVersion,
      bool destroyed);

    TeamFoundationDataReader GetWorkItemLinkChanges(IVssRequestContext context, long rowVersion);

    WorkItemLinkChange[] GetWorkItemLinkChanges(
      IVssRequestContext context,
      long rowVersion,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types,
      IEnumerable<string> linkTypes,
      ref DateTime? createdDateWatermark,
      ref DateTime? removedDateWatermark,
      out long maxReadRowVersion,
      out int totalRowCount);

    long GetLinkTimeStampForDateTime(DateTime dateTime);

    Tuple<DateTime, DateTime> GetLinkDateTimeForTimeStamp(long timestamp);

    void SyncBisGroupsAndUsers(string projectUri);

    bool ProjectDelete(string projectUri, bool witOnly = false);

    void StampDb(IVssIdentity user);

    PayloadTable GetSequenceIds();

    IEnumerable<string> GetMissingIdentities(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups);

    void AddNewBuild(string buildName, string project, string buildDefinitionName);

    void DestroyAttachments(IEnumerable<int> workItemIds);

    void GetFields(
      out IEnumerable<FieldDefinitionRecord> fieldDefinitions,
      out IEnumerable<FieldUsageRecord> fieldUsages);

    IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IEnumerable<ConstantSetReference> setReferences);

    Dictionary<int, string> GetConstants(IEnumerable<int> constantIds);

    IEnumerable<string> GetConstantSet(
      int setId,
      bool direct,
      bool includeGroups,
      bool includeTop);

    IEnumerable<string> GetGlobalAndProjectGroups(int projectId, bool includeGlobal);

    MetadataDBStamps GetMetadataTableTimestamps(
      IEnumerable<MetadataTable> tableNames,
      int projectId);

    IEnumerable<long> GetQueryItemsTimestamps(int projectId);

    void AddWorkItemTypeActions(string projectName, IEnumerable<WorkItemTypeAction> actions);

    void SetDisplayForm(string projectName, string workItemType, XmlNode displayForm);
  }
}
