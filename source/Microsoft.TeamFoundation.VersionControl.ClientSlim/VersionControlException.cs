// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.VersionControlException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.Runtime.Serialization;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class VersionControlException : TeamFoundationServerException
  {
    internal static Exception ConvertException(SoapException innerException) => VersionControlException.ConvertException(innerException, true);

    internal static Exception ConvertException(SoapException innerException, bool convertUnknown) => innerException.SubCode == null || innerException.SubCode.Code == (XmlQualifiedName) null || innerException.SubCode.Code.Name == null ? (Exception) new VersionControlException(innerException.Message, (Exception) innerException) : VersionControlException.ConvertException(innerException.SubCode.Code.Name, innerException.Message, (Exception) innerException, convertUnknown);

    internal static Exception ConvertException(
      string exceptionType,
      string exceptionMessage,
      Exception innerException,
      bool convertUnknown)
    {
      Exception exception;
      if (exceptionType != null)
      {
        switch (exceptionType.Length)
        {
          case 14:
            if (exceptionType == "DeltaException")
            {
              exception = (Exception) new DeltaException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 16:
            switch (exceptionType[0])
            {
              case 'G':
                if (exceptionType == "GenericException")
                {
                  exception = (Exception) new GenericException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'M':
                if (exceptionType == "MappingException")
                {
                  exception = (Exception) new MappingException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 17:
            if (exceptionType == "SecurityException")
            {
              exception = (Exception) new SecurityException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 18:
            if (exceptionType == "WorkspaceException")
            {
              exception = (Exception) new WorkspaceException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 19:
            switch (exceptionType[6])
            {
              case 'D':
                if (exceptionType == "FutureDateException")
                {
                  exception = (Exception) new FutureDateException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'E':
                if (exceptionType == "FolderEditException")
                {
                  exception = (Exception) new FolderEditException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "ServerItemException")
                {
                  exception = (Exception) new ServerItemException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "CannotLockException")
                {
                  exception = (Exception) new CannotLockException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "ItemLockedException")
                {
                  exception = (Exception) new ItemLockedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ItemExistsException")
                {
                  exception = (Exception) new ItemExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "IllegalUrlException")
                {
                  exception = (Exception) new IllegalUrlException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 20:
            switch (exceptionType[4])
            {
              case 'C':
                if (exceptionType == "ItemCloakedException")
                {
                  exception = (Exception) new ItemCloakedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "ItemDeletedException")
                {
                  exception = (Exception) new ItemDeletedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'h':
                if (exceptionType == "BadChecksumException")
                {
                  exception = (Exception) new BadChecksumException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "LabelExistsException")
                {
                  exception = (Exception) new LabelExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 21:
            switch (exceptionType[4])
            {
              case 'N':
                if (exceptionType == "ItemNotFoundException")
                {
                  exception = (Exception) new ItemNotFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "ItemRequiredException")
                {
                  exception = (Exception) new ItemRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "NoLockExistsException")
                {
                  exception = (Exception) new NoLockExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "PendingChildException")
                {
                  exception = (Exception) new PendingChildException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "CannotUnlockException")
                {
                  exception = (Exception) new CannotUnlockException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'y':
                if (exceptionType == "EmptyCheckInException")
                {
                  exception = (Exception) new EmptyCheckInException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 22:
            switch (exceptionType[7])
            {
              case 'F':
                switch (exceptionType)
                {
                  case "InvalidFileIdException":
                    exception = (Exception) new InvalidFileIdException(exceptionMessage, innerException);
                    goto label_429;
                  case "WorkingFolderException":
                    exception = (Exception) new WorkingFolderException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'M':
                if (exceptionType == "ItemNotMappedException")
                {
                  exception = (Exception) new ItemNotMappedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "IncorrectSizeException")
                {
                  exception = (Exception) new IncorrectSizeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "TargetCloakedException")
                {
                  exception = (Exception) new TargetCloakedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "FolderContentException")
                {
                  exception = (Exception) new FolderContentException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'q':
                if (exceptionType == "OwnerRequiredException")
                {
                  exception = (Exception) new OwnerRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 's':
                if (exceptionType == "TargetIsChildException")
                {
                  exception = (Exception) new TargetIsChildException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                if (exceptionType == "LabelNotFoundException")
                {
                  exception = (Exception) new LabelNotFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'z':
                if (exceptionType == "AuthorizationException")
                {
                  exception = (Exception) new AuthorizationException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 23:
            switch (exceptionType[7])
            {
              case 'C':
                if (exceptionType == "IllegalCommentException")
                {
                  exception = (Exception) new IllegalCommentException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "InvalidSqlDateException")
                {
                  exception = (Exception) new InvalidSqlDateException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'V':
                if (exceptionType == "IllegalVersionException")
                {
                  exception = (Exception) new IllegalVersionException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "ResourceAccessException")
                {
                  exception = (Exception) new ResourceAccessException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "FileIdNotFoundException")
                {
                  exception = (Exception) new FileIdNotFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                if (exceptionType == "LabelNotUniqueException")
                {
                  exception = (Exception) new LabelNotUniqueException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 24:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "ContentRequiredException")
                {
                  exception = (Exception) new ContentRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "DuplicateChangeException")
                {
                  exception = (Exception) new DuplicateChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'I':
                switch (exceptionType)
                {
                  case "IdentityDeletedException":
                    exception = (Exception) new IdentityDeletedException(exceptionMessage, innerException);
                    goto label_429;
                  case "IllegalIdentityException":
                    exception = (Exception) new IllegalIdentityException(exceptionMessage, innerException);
                    goto label_429;
                  case "InvalidIdentityException":
                    exception = (Exception) new InvalidIdentityException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'L':
                if (exceptionType == "LabelPendingAddException")
                {
                  exception = (Exception) new LabelPendingAddException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'M':
                if (exceptionType == "MergeEditDeleteException")
                {
                  exception = (Exception) new MergeEditDeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "RootInheritanceException")
                {
                  exception = (Exception) new RootInheritanceException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "ShelvesetExistsException")
                {
                  exception = (Exception) new ShelvesetExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'V':
                if (exceptionType == "VersionConflictException")
                {
                  exception = (Exception) new VersionConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'W':
                if (exceptionType == "WorkspaceExistsException")
                {
                  exception = (Exception) new WorkspaceExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 25:
            switch (exceptionType[7])
            {
              case 'E':
                switch (exceptionType)
                {
                  case "InvalidExtensionException":
                    exception = (Exception) new InvalidExtensionException(exceptionMessage, innerException);
                    goto label_429;
                  case "MaxRowsEvaluatedException":
                    exception = (Exception) new MaxRowsEvaluatedException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'F':
                if (exceptionType == "IllegalFieldNameException")
                {
                  exception = (Exception) new IllegalFieldNameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "IllegalLabelNameException")
                {
                  exception = (Exception) new IllegalLabelNameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "IllegalShelvesetException")
                {
                  exception = (Exception) new IllegalShelvesetException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'W':
                if (exceptionType == "IllegalWorkspaceException")
                {
                  exception = (Exception) new IllegalWorkspaceException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "IncompleteUploadException")
                {
                  exception = (Exception) new IncompleteUploadException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "LabelOwnerChangeException")
                {
                  exception = (Exception) new LabelOwnerChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "CannotMoveToRootException")
                {
                  exception = (Exception) new CannotMoveToRootException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 's':
                if (exceptionType == "LabelHasChildrenException")
                {
                  exception = (Exception) new LabelHasChildrenException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                switch (exceptionType)
                {
                  case "CouldNotAddProxyException":
                    exception = (Exception) new CouldNotAddProxyException(exceptionMessage, innerException);
                    goto label_429;
                  case "LocalPathTooLongException":
                    exception = (Exception) new LocalPathTooLongException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'y':
                if (exceptionType == "IdentityNotFoundException")
                {
                  exception = (Exception) new IdentityNotFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 26:
            switch (exceptionType[9])
            {
              case 'C':
                if (exceptionType == "WorkspaceCreationException")
                {
                  exception = (Exception) new WorkspaceCreationException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "WorkspaceDeletionException")
                {
                  exception = (Exception) new WorkspaceDeletionException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'F':
                if (exceptionType == "DuplicateFileTypeException")
                {
                  exception = (Exception) new DuplicateFileTypeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'N':
                switch (exceptionType)
                {
                  case "ChangesetNotFoundException":
                    exception = (Exception) new ChangesetNotFoundException(exceptionMessage, innerException);
                    goto label_429;
                  case "ShelvesetNotFoundException":
                    exception = (Exception) new ShelvesetNotFoundException(exceptionMessage, innerException);
                    goto label_429;
                  case "WorkspaceNotFoundException":
                    exception = (Exception) new WorkspaceNotFoundException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'R':
                if (exceptionType == "LocalItemRequiredException")
                {
                  exception = (Exception) new LocalItemRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'T':
                if (exceptionType == "NoChangesToShelveException")
                {
                  exception = (Exception) new NoChangesToShelveException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "NotWorkspaceOwnerException")
                {
                  exception = (Exception) new NotWorkspaceOwnerException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "ItemNotCheckedOutException")
                {
                  exception = (Exception) new ItemNotCheckedOutException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "IllegalPolicyNameException")
                {
                  exception = (Exception) new IllegalPolicyNameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "InvalidConflictIdException")
                {
                  exception = (Exception) new InvalidConflictIdException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "IllegalServerItemException")
                {
                  exception = (Exception) new IllegalServerItemException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 's':
                if (exceptionType == "InvalidResolutionException")
                {
                  exception = (Exception) new InvalidResolutionException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                if (exceptionType == "CannotDestroyRootException")
                {
                  exception = (Exception) new CannotDestroyRootException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 27:
            switch (exceptionType[9])
            {
              case 'E':
                if (exceptionType == "DuplicateExtensionException")
                {
                  exception = (Exception) new DuplicateExtensionException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "DuplicateItemFoundException")
                {
                  exception = (Exception) new DuplicateItemFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'O':
                if (exceptionType == "LocalItemOutOfDateException")
                {
                  exception = (Exception) new LocalItemOutOfDateException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "ExistingParentFileException")
                {
                  exception = (Exception) new ExistingParentFileException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'b':
                if (exceptionType == "IncompatibleChangeException")
                {
                  exception = (Exception) new IncompatibleChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "InvalidAcceptYoursException")
                {
                  exception = (Exception) new InvalidAcceptYoursException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'd':
                if (exceptionType == "NotAllowedOnFolderException")
                {
                  exception = (Exception) new NotAllowedOnFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                switch (exceptionType)
                {
                  case "InvalidCheckinDateException":
                    exception = (Exception) new InvalidCheckinDateException(exceptionMessage, innerException);
                    goto label_429;
                  case "MergeTargetCloakedException":
                    exception = (Exception) new MergeTargetCloakedException(exceptionMessage, innerException);
                    goto label_429;
                  case "MustUndeleteParentException":
                    exception = (Exception) new MustUndeleteParentException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'i':
                if (exceptionType == "LabelDuplicateItemException")
                {
                  exception = (Exception) new LabelDuplicateItemException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "WorkingFolderInUseException")
                {
                  exception = (Exception) new WorkingFolderInUseException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'm':
                if (exceptionType == "ServerItemRequiredException")
                {
                  exception = (Exception) new ServerItemRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'n':
                switch (exceptionType)
                {
                  case "CreateBranchObjectException":
                    exception = (Exception) new CreateBranchObjectException(exceptionMessage, innerException);
                    goto label_429;
                  case "ReservedAnnotationException":
                    exception = (Exception) new ReservedAnnotationException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'o':
                if (exceptionType == "WildcardNotAllowedException")
                {
                  exception = (Exception) new WildcardNotAllowedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "InvalidVersionSpecException")
                {
                  exception = (Exception) new InvalidVersionSpecException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                if (exceptionType == "ContentNotUploadedException")
                {
                  exception = (Exception) new ContentNotUploadedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 28:
            switch (exceptionType[0])
            {
              case 'A':
                if (exceptionType == "AutoMergeDisallowedException")
                {
                  exception = (Exception) new AutoMergeDisallowedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'C':
                if (exceptionType == "CouldNotDeleteProxyException")
                {
                  exception = (Exception) new CouldNotDeleteProxyException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "DuplicateServerItemException")
                {
                  exception = (Exception) new DuplicateServerItemException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'F':
                if (exceptionType == "FeatureNotSupportedException")
                {
                  exception = (Exception) new FeatureNotSupportedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "IllegalComputerNameException")
                {
                  exception = (Exception) new IllegalComputerNameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'M':
                if (exceptionType == "MergeConflictExistsException")
                {
                  exception = (Exception) new MergeConflictExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'N':
                if (exceptionType == "NoMergeRelationshipException")
                {
                  exception = (Exception) new NoMergeRelationshipException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "PendingParentDeleteException")
                {
                  exception = (Exception) new PendingParentDeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "RenameWorkingFolderException")
                {
                  exception = (Exception) new RenameWorkingFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'T':
                switch (exceptionType)
                {
                  case "TeamProjectNotEmptyException":
                    exception = (Exception) new TeamProjectNotEmptyException(exceptionMessage, innerException);
                    goto label_429;
                  case "TeamProjectNotFoundException":
                    exception = (Exception) new TeamProjectNotFoundException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
            }
            break;
          case 29:
            switch (exceptionType[6])
            {
              case 'A':
                if (exceptionType == "ChangeAlreadyPendingException")
                {
                  exception = (Exception) new ChangeAlreadyPendingException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "CannotSpecifyNewNameException")
                {
                  exception = (Exception) new CannotSpecifyNewNameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'V':
                if (exceptionType == "LatestVersionDeletedException")
                {
                  exception = (Exception) new LatestVersionDeletedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "MergeTargetNotMappedException")
                {
                  exception = (Exception) new MergeTargetNotMappedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'd':
                if (exceptionType == "InvalidCheckinTicketException")
                {
                  exception = (Exception) new InvalidCheckinTicketException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "LocalVersionNotFoundException")
                {
                  exception = (Exception) new LocalVersionNotFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'g':
                if (exceptionType == "PendingChangeChangedException")
                {
                  exception = (Exception) new PendingChangeChangedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "CheckoutLockRequiredException")
                {
                  exception = (Exception) new CheckoutLockRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 30:
            switch (exceptionType[3])
            {
              case 'M':
                if (exceptionType == "WebMethodNotSupportedException")
                {
                  exception = (Exception) new WebMethodNotSupportedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "CheckoutLocksDisabledException")
                {
                  exception = (Exception) new CheckoutLocksDisabledException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "IllegalShelvesetOwnerException")
                {
                  exception = (Exception) new IllegalShelvesetOwnerException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'k':
                if (exceptionType == "WorkingFolderCreationException")
                {
                  exception = (Exception) new WorkingFolderCreationException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                switch (exceptionType)
                {
                  case "RollbackInvalidOptionException":
                    exception = (Exception) new RollbackInvalidOptionException(exceptionMessage, innerException);
                    goto label_429;
                  case "ShelvedChangeNotFoundException":
                    exception = (Exception) new ShelvedChangeNotFoundException(exceptionMessage, innerException);
                    goto label_429;
                }
                break;
              case 'o':
                if (exceptionType == "RepositoryPathTooLongException")
                {
                  exception = (Exception) new RepositoryPathTooLongException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                if (exceptionType == "PartialRenameConflictException")
                {
                  exception = (Exception) new PartialRenameConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "SecurityConfigurationException")
                {
                  exception = (Exception) new SecurityConfigurationException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 31:
            switch (exceptionType[6])
            {
              case 'C':
                if (exceptionType == "CannotChangeRootFolderException")
                {
                  exception = (Exception) new CannotChangeRootFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'H':
                if (exceptionType == "TargetHasPendingChangeException")
                {
                  exception = (Exception) new TargetHasPendingChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'M':
                if (exceptionType == "CannotMergeUnderSourceException")
                {
                  exception = (Exception) new CannotMergeUnderSourceException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "DuplicateWorkingFolderException")
                {
                  exception = (Exception) new DuplicateWorkingFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "LocalWorkspaceRequiredException")
                {
                  exception = (Exception) new LocalWorkspaceRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 32:
            switch (exceptionType[6])
            {
              case 'C':
                if (exceptionType == "CannotCreateFilesInRootException")
                {
                  exception = (Exception) new CannotCreateFilesInRootException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "CannotResolveConflictAsAutoMerge")
                {
                  exception = (Exception) new CannotResolveConflictAsAutoMerge(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "IncompletePendingChangeException")
                {
                  exception = (Exception) new IncompletePendingChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "TeamProjectCannotDeleteException")
                {
                  exception = (Exception) new TeamProjectCannotDeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'v':
                if (exceptionType == "UnshelvingPartialRenameException")
                {
                  exception = (Exception) new UnshelvingPartialRenameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 33:
            switch (exceptionType[9])
            {
              case 'M':
                if (exceptionType == "CannotSetMappingOnRenameException")
                {
                  exception = (Exception) new CannotSetMappingOnRenameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "CannotCreateParentFolderException")
                {
                  exception = (Exception) new CannotCreateParentFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "CannotCheckinAllWithEditException")
                {
                  exception = (Exception) new CannotCheckinAllWithEditException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "InconsistentRequestTypesException")
                {
                  exception = (Exception) new InconsistentRequestTypesException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'g':
                if (exceptionType == "CannotMergeDestroyedFileException")
                {
                  exception = (Exception) new CannotMergeDestroyedFileException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "CannotResolveDueToRenameAndDelete")
                {
                  exception = (Exception) new CannotResolveDueToRenameAndDelete(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "BranchSourceNotCommittedException")
                {
                  exception = (Exception) new BranchSourceNotCommittedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 34:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotFindLatestChangesetException")
                {
                  exception = (Exception) new CannotFindLatestChangesetException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "DuplicateCheckinNoteFieldException")
                {
                  exception = (Exception) new DuplicateCheckinNoteFieldException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "ShelveMergeConflictExistsException")
                {
                  exception = (Exception) new ShelveMergeConflictExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 35:
            switch (exceptionType[0])
            {
              case 'A':
                if (exceptionType == "ArtifactIdentifierRequiredException")
                {
                  exception = (Exception) new ArtifactIdentifierRequiredException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'C':
                if (exceptionType == "CannotChangeWorkspaceOwnerException")
                {
                  exception = (Exception) new CannotChangeWorkspaceOwnerException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "InvalidBranchCheckinOptionException")
                {
                  exception = (Exception) new InvalidBranchCheckinOptionException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "RollbackInvalidVersionSpecException")
                {
                  exception = (Exception) new RollbackInvalidVersionSpecException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'U':
                if (exceptionType == "UndeleteAboveUnrelatedItemException")
                {
                  exception = (Exception) new UndeleteAboveUnrelatedItemException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 36:
            switch (exceptionType[10])
            {
              case 'H':
                if (exceptionType == "MergeDoNotHaveTargetLocallyException")
                {
                  exception = (Exception) new MergeDoNotHaveTargetLocallyException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "PendingLocalVersionMismatchException")
                {
                  exception = (Exception) new PendingLocalVersionMismatchException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "PendingDeleteConflictChangeException")
                {
                  exception = (Exception) new PendingDeleteConflictChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'j':
                if (exceptionType == "InvalidProjectPendingChangeException")
                {
                  exception = (Exception) new InvalidProjectPendingChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "GetLatestOnCheckoutDisabledException")
                {
                  exception = (Exception) new GetLatestOnCheckoutDisabledException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "DestroyedContentUnavailableException")
                {
                  exception = (Exception) new DestroyedContentUnavailableException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "PluginHasMultipleInterfacesException")
                {
                  exception = (Exception) new PluginHasMultipleInterfacesException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'v':
                if (exceptionType == "CannotShelvePartialUndeleteException")
                {
                  exception = (Exception) new CannotShelvePartialUndeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'w':
                if (exceptionType == "UndeleteNewNameNotSupportedException")
                {
                  exception = (Exception) new UndeleteNewNameNotSupportedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 37:
            switch (exceptionType[13])
            {
              case 'C':
                if (exceptionType == "PendingChangeContentNotFoundException")
                {
                  exception = (Exception) new PendingChangeContentNotFoundException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "CannotCheckinDependantRenameException")
                {
                  exception = (Exception) new CannotCheckinDependantRenameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "CannotCheckinPartialUndeleteException")
                {
                  exception = (Exception) new CannotCheckinPartialUndeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "CannotBranchDestroyedContentException")
                {
                  exception = (Exception) new CannotBranchDestroyedContentException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 38:
            switch (exceptionType[2])
            {
              case 'a':
                if (exceptionType == "ChangesetAuthorMustBeNonGroupException")
                {
                  exception = (Exception) new ChangesetAuthorMustBeNonGroupException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "IncompatibleCompressionFormatException")
                {
                  exception = (Exception) new IncompatibleCompressionFormatException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "ShelvingPartialRenameConflictException")
                {
                  exception = (Exception) new ShelvingPartialRenameConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "CannotDeleteTeamProjectFolderException")
                {
                  exception = (Exception) new CannotDeleteTeamProjectFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "RepositoryPathTooLongDetailedException")
                {
                  exception = (Exception) new RepositoryPathTooLongDetailedException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "WorkspaceRequiresComputerNameException")
                {
                  exception = (Exception) new WorkspaceRequiresComputerNameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 't':
                if (exceptionType == "NotPermittedForLocalWorkspaceException")
                {
                  exception = (Exception) new NotPermittedForLocalWorkspaceException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 39:
            switch (exceptionType[9])
            {
              case 'I':
                if (exceptionType == "DuplicateItemsInCheckInRequestException")
                {
                  exception = (Exception) new DuplicateItemsInCheckInRequestException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "CannotRenameDueToChildConflictException")
                {
                  exception = (Exception) new CannotRenameDueToChildConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "InvalidAcceptYoursRenameTheirsException")
                {
                  exception = (Exception) new InvalidAcceptYoursRenameTheirsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "MergeTargetFileSourceDirectoryException")
                {
                  exception = (Exception) new MergeTargetFileSourceDirectoryException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "CannotResolveToPartialUndeleteException")
                {
                  exception = (Exception) new CannotResolveToPartialUndeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "InvalidVersionSpecForOperationException")
                {
                  exception = (Exception) new InvalidVersionSpecForOperationException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 40:
            switch (exceptionType[6])
            {
              case 'M':
                if (exceptionType == "CannotMergeWithExistingConflictException")
                {
                  exception = (Exception) new CannotMergeWithExistingConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "CannotPendChangeOnDestroyedFileException")
                {
                  exception = (Exception) new CannotPendChangeOnDestroyedFileException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'U':
                if (exceptionType == "CannotUndeleteTeamProjectFolderException")
                {
                  exception = (Exception) new CannotUndeleteTeamProjectFolderException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'g':
                if (exceptionType == "MissingParentIsRenameOrUndeleteException")
                {
                  exception = (Exception) new MissingParentIsRenameOrUndeleteException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ReconcileBlockedByProjectRenameException")
                {
                  exception = (Exception) new ReconcileBlockedByProjectRenameException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 41:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotRenameBackToOriginalSourceException")
                {
                  exception = (Exception) new CannotRenameBackToOriginalSourceException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "PendingChangeMergeConflictExistsException")
                {
                  exception = (Exception) new PendingChangeMergeConflictExistsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 43:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotUndoRenameDueToChildConflictException")
                {
                  exception = (Exception) new CannotUndoRenameDueToChildConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "LongPathInWorkspaceRequires2012QU1Exception")
                {
                  exception = (Exception) new LongPathInWorkspaceRequires2012QU1Exception(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 44:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotUndoItemExistingLockConflictsException")
                {
                  exception = (Exception) new CannotUndoItemExistingLockConflictsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'T':
                if (exceptionType == "TrackMergesExceededMaxBranchesLimitException")
                {
                  exception = (Exception) new TrackMergesExceededMaxBranchesLimitException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 45:
            if (exceptionType == "CannotResolveBecauseShelvesetDeletedException")
            {
              exception = (Exception) new CannotResolveBecauseShelvesetDeletedException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 46:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotCheckinRenameDueToChildConflictException")
                {
                  exception = (Exception) new CannotCheckinRenameDueToChildConflictException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'M':
                if (exceptionType == "MergeTargetPathAlreadyHasPendingMergeException")
                {
                  exception = (Exception) new MergeTargetPathAlreadyHasPendingMergeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 47:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotTakeCheckoutLockInLocalWorkspaceException")
                {
                  exception = (Exception) new CannotTakeCheckoutLockInLocalWorkspaceException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "RepositoryPathTooLongDueToDeletedItemsException")
                {
                  exception = (Exception) new RepositoryPathTooLongDueToDeletedItemsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 48:
            if (exceptionType == "CannotResolveDeletedItemWithAcceptMergeException")
            {
              exception = (Exception) new CannotResolveDeletedItemWithAcceptMergeException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 49:
            switch (exceptionType[16])
            {
              case 'D':
                if (exceptionType == "CannotPendEditOnDeletedFileWithGetLatestException")
                {
                  exception = (Exception) new CannotPendEditOnDeletedFileWithGetLatestException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "CannotPendEditOnRenamedFileWithGetLatestException")
                {
                  exception = (Exception) new CannotPendEditOnRenamedFileWithGetLatestException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "CannotCheckinRenameAsPendingAddConflictsException")
                {
                  exception = (Exception) new CannotCheckinRenameAsPendingAddConflictsException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 51:
            switch (exceptionType[0])
            {
              case 'A':
                if (exceptionType == "AcceptYoursRenameTheirsRequiresDestinationException")
                {
                  exception = (Exception) new AcceptYoursRenameTheirsRequiresDestinationException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "DateVersionSpecBeforeBeginningOfRepositoryException")
                {
                  exception = (Exception) new DateVersionSpecBeforeBeginningOfRepositoryException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 52:
            switch (exceptionType[0])
            {
              case 'M':
                if (exceptionType == "MergeTargetPathHasIncompatiblePendingChangeException")
                {
                  exception = (Exception) new MergeTargetPathHasIncompatiblePendingChangeException(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
              case 'T':
                if (exceptionType == "TeamFoundationVersionControlNotEnabledForTeamProject")
                {
                  exception = (Exception) new TeamFoundationVersionControlNotEnabledForTeamProject(exceptionMessage, innerException);
                  goto label_429;
                }
                else
                  break;
            }
            break;
          case 53:
            if (exceptionType == "CannotMergeWithWorkspaceSpecAndPendingDeleteException")
            {
              exception = (Exception) new CannotMergeWithWorkspaceSpecAndPendingDeleteException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 54:
            if (exceptionType == "AcceptMergeVersionConflictRequiresDestinationException")
            {
              exception = (Exception) new AcceptMergeVersionConflictRequiresDestinationException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 55:
            if (exceptionType == "CannotResolveAcceptMergeWithNonexistentVersionException")
            {
              exception = (Exception) new CannotResolveAcceptMergeWithNonexistentVersionException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 56:
            if (exceptionType == "AcceptMergeNamespaceConflictRequiresDestinationException")
            {
              exception = (Exception) new AcceptMergeNamespaceConflictRequiresDestinationException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
          case 57:
            if (exceptionType == "MergeTargetPathHasNamespacePendingChangeConflictException")
            {
              exception = (Exception) new MergeTargetPathHasNamespacePendingChangeConflictException(exceptionMessage, innerException);
              goto label_429;
            }
            else
              break;
        }
      }
      exception = TeamFoundationServiceException.ConvertException(exceptionType, exceptionMessage, innerException, false);
      if (convertUnknown && exception == null)
        exception = (Exception) new VersionControlException(exceptionMessage, innerException);
label_429:
      return exception;
    }

    public VersionControlException()
    {
    }

    public VersionControlException(string message)
      : base(message)
    {
    }

    public VersionControlException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected VersionControlException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
