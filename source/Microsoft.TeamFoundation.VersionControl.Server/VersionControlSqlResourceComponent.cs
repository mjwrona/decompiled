// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionControlSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    protected SqlExceptionHandler m_resultExHandler;
    private VersionControlRequestContext m_versionControlRequestContext;
    private static IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();

    static VersionControlSqlResourceComponent()
    {
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500015, new SqlExceptionFactory(typeof (IdentityNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500151, new SqlExceptionFactory(typeof (CannotShelvePartialUndeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500054, new SqlExceptionFactory(typeof (ChangesetNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500155, new SqlExceptionFactory(typeof (CouldNotDeleteProxyException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500204, new SqlExceptionFactory(typeof (CouldNotAddProxyException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500014, new SqlExceptionFactory(typeof (ItemNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500077, new SqlExceptionFactory(typeof (ItemNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500125, new SqlExceptionFactory(typeof (MissingParentIsRenameOrUndeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500070, new SqlExceptionFactory(typeof (CannotMergeUnderSourceException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500133, new SqlExceptionFactory(typeof (CannotMergeWithWorkspaceSpecAndPendingDeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500122, new SqlExceptionFactory(typeof (DateVersionSpecBeforeBeginningOfRepositoryException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500089, new SqlExceptionFactory(typeof (MergeTargetFileSourceDirectoryException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500021, new SqlExceptionFactory(typeof (ItemExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500040, new SqlExceptionFactory(typeof (LocalItemOutOfDateException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500019, new SqlExceptionFactory(typeof (WorkspaceDeletionException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500020, new SqlExceptionFactory(typeof (WorkspaceExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500016, new SqlExceptionFactory(typeof (WorkspaceNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500012, new SqlExceptionFactory(typeof (DuplicateWorkingFolderException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500141, new SqlExceptionFactory(typeof (CannotPendChangeOnDestroyedFileException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500149, new SqlExceptionFactory(typeof (CannotPendEditOnDeletedFileWithGetLatestException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500150, new SqlExceptionFactory(typeof (CannotPendEditOnRenamedFileWithGetLatestException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500143, new SqlExceptionFactory(typeof (CannotBranchDestroyedContentException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500144, new SqlExceptionFactory(typeof (CannotMergeDestroyedFileException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500041, new SqlExceptionFactory(typeof (ItemNotCheckedOutException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500037, new SqlExceptionFactory(typeof (ItemNotCheckedOutException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500030, new SqlExceptionFactory(typeof (LocalVersionNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500048, new SqlExceptionFactory(typeof (NotAllowedOnFolderException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500036, new SqlExceptionFactory(typeof (FolderContentException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500067, new SqlExceptionFactory(typeof (ContentRequiredException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500060, new SqlExceptionFactory(typeof (ItemRequiredException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500039, new SqlExceptionFactory(typeof (ChangeAlreadyPendingException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500038, new SqlExceptionFactory(typeof (IncompatibleChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500035, new SqlExceptionFactory(typeof (CannotLockException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500033, new SqlExceptionFactory(typeof (ItemLockedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500031, new SqlExceptionFactory(typeof (ItemLockedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500137, new SqlExceptionFactory(typeof (ItemNotMappedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500152, new SqlExceptionFactory(typeof (ItemNotMappedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500147, new SqlExceptionFactory(typeof (ItemCloakedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500026, new SqlExceptionFactory(typeof (PendingParentDeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500032, new SqlExceptionFactory(typeof (PendingChildException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500063, new SqlExceptionFactory(typeof (RenameWorkingFolderException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500011, new SqlExceptionFactory(typeof (WorkspaceRequiresComputerNameException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500023, new SqlExceptionFactory(typeof (MustUndeleteParentException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500024, new SqlExceptionFactory(typeof (NoLockExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500062, new SqlExceptionFactory(typeof (TargetCloakedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500064, new SqlExceptionFactory(typeof (TargetHasPendingChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500088, new SqlExceptionFactory(typeof (BranchSourceNotCommittedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500052, new SqlExceptionFactory(typeof (CannotChangeRootFolderException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500053, new SqlExceptionFactory(typeof (WorkingFolderInUseException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500066, new SqlExceptionFactory(typeof (TargetIsChildException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500074, new SqlExceptionFactory(typeof (CannotRenameBackToOriginalSourceException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500102, new SqlExceptionFactory(typeof (LabelNotUniqueException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500050, new SqlExceptionFactory(typeof (LabelNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500081, new SqlExceptionFactory(typeof (LabelNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500079, new SqlExceptionFactory(typeof (LabelExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500080, new SqlExceptionFactory(typeof (NoMergeRelationshipException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500115, new SqlExceptionFactory(typeof (MergeTargetNotMappedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500129, new SqlExceptionFactory(typeof (MergeDoNotHaveTargetLocallyException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500116, new SqlExceptionFactory(typeof (MergeTargetCloakedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500084, new SqlExceptionFactory(typeof (PartialRenameConflictException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500123, new SqlExceptionFactory(typeof (ShelvingPartialRenameConflictException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500142, new SqlExceptionFactory(typeof (UnshelvingPartialRenameException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500124, new SqlExceptionFactory(typeof (UndeleteAboveUnrelatedItemException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500085, new SqlExceptionFactory(typeof (CannotSetMappingOnRenameException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500047, new SqlExceptionFactory(typeof (CannotCreateParentFolderException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500153, new SqlExceptionFactory(typeof (LabelDuplicateItemException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500090, new SqlExceptionFactory(typeof (ShelvesetNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500092, new SqlExceptionFactory(typeof (ShelvesetExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500095, new SqlExceptionFactory(typeof (IncompletePendingChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500082, new SqlExceptionFactory(typeof (MergeTargetPathAlreadyHasPendingMergeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500071, new SqlExceptionFactory(typeof (MergeTargetPathHasIncompatiblePendingChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500072, new SqlExceptionFactory(typeof (MergeTargetPathHasNamespacePendingChangeConflictException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500096, new SqlExceptionFactory(typeof (InvalidConflictIdException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500100, new SqlExceptionFactory(typeof (CannotResolveToPartialUndeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500132, new SqlExceptionFactory(typeof (CannotResolveDeletedItemWithAcceptMergeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500078, new SqlExceptionFactory(typeof (CannotUnlockException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500119, new SqlExceptionFactory(typeof (PendingChangeMergeConflictExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500086, new SqlExceptionFactory(typeof (RepositoryPathTooLongException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500120, new SqlExceptionFactory(typeof (ShelveMergeConflictExistsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500113, new SqlExceptionFactory(typeof (InvalidAcceptYoursException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500110, new SqlExceptionFactory(typeof (InvalidAcceptYoursRenameTheirsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500111, new SqlExceptionFactory(typeof (AcceptMergeNamespaceConflictRequiresDestinationException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500112, new SqlExceptionFactory(typeof (AcceptYoursRenameTheirsRequiresDestinationException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500055, new SqlExceptionFactory(typeof (DuplicateChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500094, new SqlExceptionFactory(typeof (PendingChangeContentNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500029, new SqlExceptionFactory(typeof (IncompleteUploadException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500121, new SqlExceptionFactory(typeof (FileIdNotFoundException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500013, new SqlExceptionFactory(typeof (DuplicateCheckinNoteFieldException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500126, new SqlExceptionFactory(typeof (CannotUndoItemExistingLockConflictsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500127, new SqlExceptionFactory(typeof (CannotUndoItemExistingLockConflictsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500128, new SqlExceptionFactory(typeof (CheckoutLockRequiredException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500131, new SqlExceptionFactory(typeof (AcceptMergeVersionConflictRequiresDestinationException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500130, new SqlExceptionFactory(typeof (CannotCheckinPartialUndeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500056, new SqlExceptionFactory(typeof (ExistingParentFileException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500138, new SqlExceptionFactory(typeof (MergeEditDeleteException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500097, new SqlExceptionFactory(typeof (CannotSpecifyNewNameException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500145, new SqlExceptionFactory(typeof (MaxRowsEvaluatedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500146, new SqlExceptionFactory(typeof (GenericException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500148, new SqlExceptionFactory(typeof (CannotFindLatestChangesetException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500166, new SqlExceptionFactory(typeof (RollbackInvalidVersionSpecException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500163, new SqlExceptionFactory(typeof (RollbackInvalidVersionSpecException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500087, new SqlExceptionFactory(typeof (LocalPathTooLongException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500178, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500170, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500171, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500172, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500173, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500174, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500175, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500232, new SqlExceptionFactory(typeof (CreateBranchObjectException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500156, new SqlExceptionFactory(typeof (PendingChangeChangedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500159, new SqlExceptionFactory(typeof (CannotChangeWorkspaceOwnerException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500158, new SqlExceptionFactory(typeof (DuplicateItemsInCheckInRequestException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500183, new SqlExceptionFactory(typeof (EmptyCheckInException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500184, new SqlExceptionFactory(typeof (UndeleteNewNameNotSupportedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500201, new SqlExceptionFactory(typeof (TrackMergesExceededMaxBranchesLimitException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500202, new SqlExceptionFactory(typeof (PendingDeleteConflictChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500203, new SqlExceptionFactory(typeof (WebMethodNotSupportedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500205, new SqlExceptionFactory(typeof (InvalidResolutionException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500191, new SqlExceptionFactory(typeof (CannotResolveAcceptMergeWithNonexistentVersionException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500208, new SqlExceptionFactory(typeof (AutoMergeDisallowedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500210, new SqlExceptionFactory(typeof (InvalidCheckinDateException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500211, new SqlExceptionFactory(typeof (InvalidCheckinDateException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500212, new SqlExceptionFactory(typeof (TeamProjectNotEmptyException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500213, new SqlExceptionFactory(typeof (InvalidCheckinDateException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500214, new SqlExceptionFactory(typeof (NotPermittedForLocalWorkspaceException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500215, new SqlExceptionFactory(typeof (CannotResolveBecauseShelvesetDeletedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500217, new SqlExceptionFactory(typeof (CannotCreateFilesInRootException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500219, new SqlExceptionFactory(typeof (CannotResolveDueToRenameAndDelete)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500218, new SqlExceptionFactory(typeof (InvalidProjectPendingChangeException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500220, new SqlExceptionFactory(typeof (CannotRenameDueToChildConflictException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500221, new SqlExceptionFactory(typeof (CannotUndoRenameDueToChildConflictException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500223, new SqlExceptionFactory(typeof (LocalWorkspaceRequiredException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500224, new SqlExceptionFactory(typeof (CannotResolveConflictAsAutoMerge)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500225, new SqlExceptionFactory(typeof (ContentNotUploadedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500226, new SqlExceptionFactory(typeof (RepositoryPathTooLongDueToDeletedItemsException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500227, new SqlExceptionFactory(typeof (RepositoryPathTooLongDetailedException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500004, new SqlExceptionFactory(typeof (GenericDatabaseUpdateErrorException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500228, new SqlExceptionFactory(typeof (IdenticalPathsDueToCollationException)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500229, new SqlExceptionFactory(typeof (InvalidOperationDueToDefunctLocalWorkspace)));
      VersionControlSqlResourceComponent.s_sqlExceptionFactories.Add(500231, new SqlExceptionFactory(typeof (InvalidLastServerItemForPaging)));
    }

    internal static bool IsDateNull(DateTime date) => date.Year < 1700;

    public VersionControlSqlResourceComponent()
    {
      this.ContainerErrorCode = 50000;
      this.m_resultExHandler = new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException);
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
      this.DataspaceRlsEnabled = false;
    }

    public int GetDataspaceIdFromPathPair(ItemPathPair itemPathPair) => this.GetDataspaceIdFromPath(itemPathPair.ProjectGuidPath ?? itemPathPair.ProjectNamePath);

    public int GetDataspaceIdFromPath(string serverItemPath)
    {
      int dataspaceId;
      this.ConvertToPathWithProjectGuid(serverItemPath, out dataspaceId, out string _);
      return dataspaceId;
    }

    internal string ConvertPathPairToPathWithProjectGuid(ItemPathPair itemPathPair) => this.ConvertToPathWithProjectGuid(itemPathPair.ProjectGuidPath ?? itemPathPair.ProjectNamePath);

    public string ConvertToPathWithProjectGuid(string serverItemPath) => this.ConvertToPathWithProjectGuid(serverItemPath, out int _, out string _);

    internal string ConvertPathPairToPathWithProjectGuid(
      ItemPathPair itemPathPair,
      out int dataspaceId)
    {
      return this.ConvertToPathWithProjectGuid(itemPathPair.ProjectGuidPath ?? itemPathPair.ProjectNamePath, out dataspaceId);
    }

    public string ConvertToPathWithProjectGuid(string serverItemPath, out int dataspaceId) => this.ConvertToPathWithProjectGuid(serverItemPath, out dataspaceId, out string _);

    internal string ConvertPathPairToPathWithProjectGuid(
      ItemPathPair itemPathPair,
      out int dataspaceId,
      out string projectName)
    {
      return this.ConvertToPathWithProjectGuid(itemPathPair.ProjectGuidPath ?? itemPathPair.ProjectNamePath, out dataspaceId, out projectName);
    }

    public string ConvertToPathWithProjectGuid(
      string serverItemPath,
      out int dataspaceId,
      out string projectName)
    {
      Guid projectId;
      string pathWithProjectId = ProjectUtility.ConvertToPathWithProjectId(this.RequestContext, serverItemPath, out projectId, out projectName);
      dataspaceId = this.GetDataspaceIdDebug(projectId, pathWithProjectId);
      return pathWithProjectId;
    }

    public override int GetDataspaceId(Guid dataspaceIdentifier)
    {
      int num = dataspaceIdentifier != Guid.Empty ? 1 : 0;
      return base.GetDataspaceId(dataspaceIdentifier);
    }

    public static Guid GetDataspaceIdentifierDebug(
      VersionControlRequestContext versionControlRequestContext,
      Guid projectId,
      string itemPath)
    {
      if (versionControlRequestContext == null)
        return projectId;
      Guid guid = versionControlRequestContext.VersionControlService.DebugDataspace(versionControlRequestContext);
      if (projectId == Guid.Empty || string.IsNullOrEmpty(itemPath) || guid == Guid.Empty)
        return projectId;
      int num = 0;
      if (VersionControlPath.GetFolderDepth(itemPath) >= 3)
        num = TFStringComparer.VersionControlPath.GetHashCode(itemPath.Split('/')[3]);
      return num % 2 != 0 ? guid : projectId;
    }

    public int GetDataspaceIdDebug(Guid projectId, string itemPath) => base.GetDataspaceId(VersionControlSqlResourceComponent.GetDataspaceIdentifierDebug(this.VersionControlRequestContext, projectId, itemPath));

    public string BestEffortConvertToPathWithProjectName(string serverItemProjectIdPath)
    {
      string convertedPath = serverItemProjectIdPath;
      ProjectUtility.TryConvertToPathWithProjectName(this.RequestContext, serverItemProjectIdPath, out convertedPath, out Guid _, out string _);
      return convertedPath;
    }

    public string BestEffortConvertToPathWithProjectGuid(string serverItemPath) => this.BestEffortConvertToPathWithProjectGuid(ItemPathPair.FromServerItem(serverItemPath), out int _);

    public string BestEffortConvertToPathWithProjectGuid(string serverItemPath, out int dataspaceId) => this.BestEffortConvertToPathWithProjectGuid(ItemPathPair.FromServerItem(serverItemPath), out dataspaceId);

    public string BestEffortConvertPathPairToPathWithProjectGuid(ItemPathPair itemPathPair) => this.BestEffortConvertToPathWithProjectGuid(itemPathPair, out int _);

    public string BestEffortConvertToPathWithProjectGuid(
      ItemPathPair itemPathPair,
      out int dataspaceId)
    {
      return this.BestEffortConvertToPathWithProjectGuid(itemPathPair, out dataspaceId, out string _);
    }

    public string BestEffortConvertToPathWithProjectGuid(
      ItemPathPair itemPathPair,
      out int dataspaceId,
      out string projectName)
    {
      string pathWithProjectGuid = itemPathPair.ProjectGuidPath ?? itemPathPair.ProjectNamePath;
      try
      {
        Guid projectId;
        pathWithProjectGuid = ProjectUtility.ConvertToPathWithProjectId(this.RequestContext, pathWithProjectGuid, out projectId, out projectName);
        try
        {
          dataspaceId = this.GetDataspaceIdDebug(projectId, pathWithProjectGuid);
        }
        catch (DataspaceNotFoundException ex)
        {
          dataspaceId = 0;
        }
      }
      catch (TeamProjectNotFoundException ex)
      {
        dataspaceId = 0;
        projectName = string.Empty;
      }
      return pathWithProjectGuid;
    }

    public string ConvertToPathWithProjectName(string serverItemPath) => ProjectUtility.ConvertToPathWithProjectName(this.RequestContext, serverItemPath);

    public SqlParameter BindLocalItem(string parameterName, string parameterValue, bool allowNull) => this.BindString(parameterName, DBPath.LocalToDatabasePath(parameterValue), -1, allowNull, SqlDbType.NVarChar);

    public SqlParameter BindServerItem(string parameterName, string parameterValue, bool allowNull) => this.BindString(parameterName, DBPath.ServerToDatabasePath(parameterValue), -1, allowNull, SqlDbType.NVarChar);

    public SqlParameter BindPreDataspaceServerItemPathPair(
      string parameterName,
      ItemPathPair parameterValuePair,
      bool allowNull)
    {
      return this.BindServerItem(parameterName, parameterValuePair.ProjectNamePath, allowNull);
    }

    public SqlParameter BindServerItemPathPair(
      string parameterName,
      ItemPathPair parameterValuePair,
      bool allowNull)
    {
      string pathWithProjectGuid = this.ConvertToPathWithProjectGuid(parameterValuePair.ProjectGuidPath ?? parameterValuePair.ProjectNamePath);
      return this.BindString(parameterName, DBPath.ServerToDatabasePath(pathWithProjectGuid), -1, allowNull, SqlDbType.NVarChar);
    }

    public SqlParameter BindServiceDataspaceId(string parameterName) => this.BindInt(parameterName, this.GetDataspaceId(Guid.Empty));

    public SqlParameter BindDataspaceId(
      string parameterName,
      ItemPathPair serverItemPathPair,
      out string pathWithProjectGuid)
    {
      int dataspaceId;
      pathWithProjectGuid = this.ConvertPathPairToPathWithProjectGuid(serverItemPathPair, out dataspaceId);
      return this.BindInt(parameterName, dataspaceId);
    }

    public SqlParameter BindDataspaceId(
      string parameterName,
      string serverItemPath,
      out string pathWithProjectGuid)
    {
      int dataspaceId;
      pathWithProjectGuid = this.ConvertToPathWithProjectGuid(serverItemPath, out dataspaceId);
      return this.BindInt(parameterName, dataspaceId);
    }

    public void BindDataspaceIdAndServerItemPathPair(
      string dataspaceIdParameterName,
      string serverItemParameterName,
      ItemPathPair serverItemPathPair,
      bool allowNull)
    {
      string pathWithProjectGuid;
      this.BindDataspaceId(dataspaceIdParameterName, serverItemPathPair, out pathWithProjectGuid);
      this.BindServerItem(serverItemParameterName, pathWithProjectGuid, allowNull);
    }

    public void BindDataspaceIdAndServerItem(
      string dataspaceIdParameterName,
      string serverItemParameterName,
      string serverItemPath,
      bool allowNull)
    {
      string pathWithProjectGuid;
      this.BindDataspaceId(dataspaceIdParameterName, serverItemPath, out pathWithProjectGuid);
      this.BindServerItem(serverItemParameterName, pathWithProjectGuid, allowNull);
    }

    public SqlParameter BindLockLevel(string parameterName, LockLevel value) => value != LockLevel.Unchanged ? this.BindByte(parameterName, (byte) value) : this.BindNullValue(parameterName, SqlDbType.TinyInt);

    public SqlParameter PrepareAndBindVersionSpec(
      string parameterName,
      VersionSpec value,
      bool allowNull)
    {
      return this.BindVersionSpec(parameterName, this.PrepareVersionSpec(value), allowNull);
    }

    public VersionSpec PrepareVersionSpec(VersionSpec versionSpec)
    {
      if (!(versionSpec is LabelVersionSpec))
        return versionSpec;
      LabelVersionSpec labelVersionSpec = (LabelVersionSpec) versionSpec;
      labelVersionSpec.Scope = this.ConvertToPathWithProjectGuid(labelVersionSpec.Scope);
      return (VersionSpec) labelVersionSpec;
    }

    public SqlParameter BindVersionSpec(string parameterName, VersionSpec value, bool allowNull)
    {
      if (!allowNull && value == null)
        throw new ArgumentNullException(parameterName);
      return this.BindString(parameterName, value?.ToDBString(this.VersionControlRequestContext.RequestContext), -1, allowNull, SqlDbType.NVarChar);
    }

    public SqlParameter BindPathLength(string parameterName, PathLength parameterValue) => this.BindInt(parameterName, (int) (parameterValue + 1));

    public SqlParameter BindPathWithGuidLength(string parameterName, PathLength parameterValue) => this.BindInt(parameterName, (int) (parameterValue + 35 + 1));

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => VersionControlSqlResourceComponent.s_sqlExceptionFactories;

    protected int MaxItemsPerRequest => this.m_versionControlRequestContext != null ? this.m_versionControlRequestContext.VersionControlService.GetMaxItemsPerRequest(this.m_versionControlRequestContext) : int.MaxValue;

    public static ChangeType GetChangeType(int pendingCommand)
    {
      if ((pendingCommand & 2112) == 2112)
        pendingCommand = pendingCommand & -2113 | 8;
      return (ChangeType) pendingCommand;
    }

    internal VersionControlRequestContext VersionControlRequestContext
    {
      get => this.m_versionControlRequestContext;
      set => this.m_versionControlRequestContext = value;
    }
  }
}
