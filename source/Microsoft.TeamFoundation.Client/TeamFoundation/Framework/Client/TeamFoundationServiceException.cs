// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationServiceException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Serializable]
  public class TeamFoundationServiceException : TeamFoundationServerException
  {
    public TeamFoundationServiceException()
    {
    }

    public TeamFoundationServiceException(string message)
      : base(message)
    {
    }

    public TeamFoundationServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TeamFoundationServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Exception ConvertException(SoapException innerException) => TeamFoundationServiceException.ConvertException(innerException, true);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Exception ConvertException(SoapException innerException, bool convertUnknown) => innerException.SubCode == null || innerException.SubCode.Code == (XmlQualifiedName) null || innerException.SubCode.Code.Name == null ? (Exception) new TeamFoundationServiceException(innerException.Message, (Exception) innerException) : TeamFoundationServiceException.ConvertException(innerException.SubCode.Code.Name, innerException.Message, (Exception) innerException, convertUnknown);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Exception ConvertException(
      string exceptionType,
      string exceptionMessage,
      Exception innerException,
      bool convertUnknown)
    {
      Exception exception = (Exception) null;
      if (exceptionType != null)
      {
        switch (exceptionType.Length)
        {
          case 14:
            switch (exceptionType[1])
            {
              case 'a':
                if (exceptionType == "ParseException")
                {
                  exception = (Exception) new ParseException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "DeltaException")
                {
                  exception = (Exception) new DeltaException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "ProxyException")
                {
                  exception = (Exception) new ProxyException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'v':
                if (exceptionType == "EventException")
                {
                  exception = (Exception) new EventException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 17:
            switch (exceptionType[0])
            {
              case 'J':
                if (exceptionType == "JobAgentException")
                {
                  exception = (Exception) new JobAgentException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "SqlAzureException")
                {
                  exception = (Exception) new SqlAzureException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 18:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CollationException")
                {
                  exception = (Exception) new CollationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "DeleteACEException")
                {
                  exception = (Exception) new DeleteACEException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "StrongBoxException")
                {
                  exception = (Exception) new StrongBoxException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 19:
            if (exceptionType == "DeleteSelfException")
            {
              exception = (Exception) new DeleteSelfException(exceptionMessage, innerException);
              goto label_569;
            }
            else
              break;
          case 20:
            switch (exceptionType[0])
            {
              case 'A':
                if (exceptionType == "AccessCheckException")
                {
                  exception = (Exception) new AccessCheckException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'B':
                if (exceptionType == "BadChecksumException")
                {
                  exception = (Exception) new BadChecksumException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'G':
                if (exceptionType == "GroupRenameException")
                {
                  exception = (Exception) new GroupRenameException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "LockTimeoutException")
                {
                  exception = (Exception) new LockTimeoutException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "RedirectionException")
                {
                  exception = (Exception) new RedirectionException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'U':
                if (exceptionType == "UpdateGroupException")
                {
                  exception = (Exception) new UpdateGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 21:
            switch (exceptionType[4])
            {
              case 'C':
                if (exceptionType == "HostCreationException")
                {
                  exception = (Exception) new HostCreationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "HostShutdownException")
                {
                  exception = (Exception) new HostShutdownException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "TooManyItemsException")
                {
                  exception = (Exception) new TooManyItemsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'b':
                if (exceptionType == "DatabaseFullException")
                {
                  exception = (Exception) new DatabaseFullException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ServiceLevelException")
                {
                  exception = (Exception) new ServiceLevelException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "ClientCancelException")
                {
                  exception = (Exception) new ClientCancelException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                if (exceptionType == "RegistryPathException")
                {
                  exception = (Exception) new RegistryPathException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "IdentitySyncException")
                {
                  exception = (Exception) new IdentitySyncException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 22:
            switch (exceptionType[7])
            {
              case 'A':
                if (exceptionType == "InvalidAccessException")
                {
                  exception = (Exception) new InvalidAccessException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "InvalidDomainException")
                {
                  exception = (Exception) new InvalidDomainException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'F':
                if (exceptionType == "RequestFilterException")
                {
                  exception = (Exception) new RequestFilterException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'b':
                if (exceptionType == "ReaderAbortedException")
                {
                  exception = (Exception) new ReaderAbortedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "IncorrectSizeException")
                {
                  exception = (Exception) new IncorrectSizeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "GroupCreationException")
                {
                  exception = (Exception) new GroupCreationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "DeltaTooLargeException")
                {
                  exception = (Exception) new DeltaTooLargeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "JobDidntPauseException")
                {
                  exception = (Exception) new JobDidntPauseException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 23:
            switch (exceptionType[3])
            {
              case 'a':
                if (exceptionType == "DatabaseSchemaException")
                {
                  exception = (Exception) new DatabaseSchemaException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "FileIdNotFoundException")
                {
                  exception = (Exception) new FileIdNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "DuplicateJobIdException")
                {
                  exception = (Exception) new DuplicateJobIdException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                if (exceptionType == "MissingFeatureException")
                {
                  exception = (Exception) new MissingFeatureException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "HostManagementException")
                {
                  exception = (Exception) new HostManagementException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 24:
            switch (exceptionType[7])
            {
              case 'C':
                if (exceptionType == "RequestCanceledException")
                {
                  exception = (Exception) new RequestCanceledException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "RequestDisabledException")
                {
                  exception = (Exception) new RequestDisabledException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "IllegalIdentityException")
                {
                  exception = (Exception) new IllegalIdentityException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'N':
                if (exceptionType == "ProjectNotFoundException")
                {
                  exception = (Exception) new ProjectNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "InvalidRegistryException")
                {
                  exception = (Exception) new InvalidRegistryException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "DatabaseRuntimeException")
                {
                  exception = (Exception) new DatabaseRuntimeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "AzureProcessingException")
                {
                  exception = (Exception) new AzureProcessingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "QueryExpressionException")
                {
                  exception = (Exception) new QueryExpressionException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "RegisterProjectException")
                {
                  exception = (Exception) new RegisterProjectException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'y':
                if (exceptionType == "PropertyServiceException")
                {
                  exception = (Exception) new PropertyServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 25:
            switch (exceptionType[11])
            {
              case 'E':
                switch (exceptionType)
                {
                  case "HostDoesNotExistException":
                    exception = (Exception) new HostDoesNotExistException(exceptionMessage, innerException);
                    goto label_569;
                  case "NodeDoesNotExistException":
                    exception = (Exception) new NodeDoesNotExistException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'F':
                switch (exceptionType)
                {
                  case "DataTierNotFoundException":
                    exception = (Exception) new DataTierNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                  case "DatabaseNotFoundException":
                    exception = (Exception) new DatabaseNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                  case "IdentityNotFoundException":
                    exception = (Exception) new IdentityNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                  case "LocationNotFoundException":
                    exception = (Exception) new LocationNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'G':
                if (exceptionType == "AddMemberNoGroupException")
                {
                  exception = (Exception) new AddMemberNoGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                switch (exceptionType)
                {
                  case "DuplicateFeatureException":
                    exception = (Exception) new DuplicateFeatureException(exceptionMessage, innerException);
                    goto label_569;
                  case "InvalidCollationException":
                    exception = (Exception) new InvalidCollationException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'c':
                switch (exceptionType)
                {
                  case "AttachCollectionException":
                    exception = (Exception) new AttachCollectionException(exceptionMessage, innerException);
                    goto label_569;
                  case "ComponentFactoryException":
                    exception = (Exception) new ComponentFactoryException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'd':
                if (exceptionType == "DBResultDeadlockException")
                {
                  exception = (Exception) new DBResultDeadlockException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "AzureServiceBusyException")
                {
                  exception = (Exception) new AzureServiceBusyException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'h':
                if (exceptionType == "HostStatusChangeException")
                {
                  exception = (Exception) new HostStatusChangeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "DatabasePoolFullException")
                {
                  exception = (Exception) new DatabasePoolFullException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "JobsStillRunningException")
                {
                  exception = (Exception) new JobsStillRunningException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "IncompleteUploadException")
                {
                  exception = (Exception) new IncompleteUploadException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "DatabaseInstanceException")
                {
                  exception = (Exception) new DatabaseInstanceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 26:
            char ch = exceptionType[10];
            if (ch <= 'i')
            {
              if (ch <= 'P')
              {
                if (ch != 'B')
                {
                  if (ch == 'P' && exceptionType == "UnregisterProjectException")
                  {
                    exception = (Exception) new UnregisterProjectException(exceptionMessage, innerException);
                    goto label_569;
                  }
                  else
                    break;
                }
                else if (exceptionType == "AzureLoginBadUserException")
                {
                  exception = (Exception) new AzureLoginBadUserException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              }
              else if (ch != 'R')
              {
                switch ((int) ch - 99)
                {
                  case 0:
                    if (exceptionType == "ReplaceDescriptorException")
                    {
                      exception = (Exception) new ReplaceDescriptorException(exceptionMessage, innerException);
                      goto label_569;
                    }
                    else
                      break;
                  case 2:
                    switch (exceptionType)
                    {
                      case "InvalidParentHostException":
                        exception = (Exception) new InvalidParentHostException(exceptionMessage, innerException);
                        goto label_569;
                      case "JobCannotBePausedException":
                        exception = (Exception) new JobCannotBePausedException(exceptionMessage, innerException);
                        goto label_569;
                    }
                    break;
                  case 4:
                    if (exceptionType == "CannotChangeTreesException")
                    {
                      exception = (Exception) new CannotChangeTreesException(exceptionMessage, innerException);
                      goto label_569;
                    }
                    else
                      break;
                  case 6:
                    if (exceptionType == "DuplicateFileNameException")
                    {
                      exception = (Exception) new DuplicateFileNameException(exceptionMessage, innerException);
                      goto label_569;
                    }
                    else
                      break;
                }
              }
              else if (exceptionType == "AzureQuotaReachedException")
              {
                exception = (Exception) new AzureQuotaReachedException(exceptionMessage, innerException);
                goto label_569;
              }
              else
                break;
            }
            else if (ch <= 't')
            {
              switch ((int) ch - 108)
              {
                case 0:
                  if (exceptionType == "IncompatibleTokenException")
                  {
                    exception = (Exception) new IncompatibleTokenException(exceptionMessage, innerException);
                    goto label_569;
                  }
                  else
                    break;
                case 1:
                case 2:
                  break;
                case 3:
                  switch (exceptionType)
                  {
                    case "DataspaceNotFoundException":
                      exception = (Exception) new DataspaceNotFoundException(exceptionMessage, innerException);
                      goto label_569;
                    case "UnknownRepositoryException":
                      exception = (Exception) new UnknownRepositoryException(exceptionMessage, innerException);
                      goto label_569;
                    case "CreateACENoActionException":
                      exception = (Exception) new CreateACENoActionException(exceptionMessage, innerException);
                      goto label_569;
                    case "CreateACENoObjectException":
                      exception = (Exception) new CreateACENoObjectException(exceptionMessage, innerException);
                      goto label_569;
                  }
                  break;
                case 4:
                  if (exceptionType == "RemoveGroupMemberException")
                  {
                    exception = (Exception) new RemoveGroupMemberException(exceptionMessage, innerException);
                    goto label_569;
                  }
                  else
                    break;
                default:
                  if (ch == 't' && exceptionType == "NotASecurityGroupException")
                  {
                    exception = (Exception) new NotASecurityGroupException(exceptionMessage, innerException);
                    goto label_569;
                  }
                  else
                    break;
              }
            }
            else if (ch != 'v')
            {
              if (ch == 'y')
              {
                switch (exceptionType)
                {
                  case "HostAlreadyExistsException":
                    exception = (Exception) new HostAlreadyExistsException(exceptionMessage, innerException);
                    goto label_569;
                  case "NodeAlreadyExistsException":
                    exception = (Exception) new NodeAlreadyExistsException(exceptionMessage, innerException);
                    goto label_569;
                }
              }
              else
                break;
            }
            else if (exceptionType == "AddMemberEveryoneException")
            {
              exception = (Exception) new AddMemberEveryoneException(exceptionMessage, innerException);
              goto label_569;
            }
            else
              break;
            break;
          case 27:
            switch (exceptionType[13])
            {
              case 'G':
                if (exceptionType == "RemoveSpecialGroupException")
                {
                  exception = (Exception) new RemoveSpecialGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "HostMustBeTopLevelException")
                {
                  exception = (Exception) new HostMustBeTopLevelException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "InvalidAccessPointException")
                {
                  exception = (Exception) new InvalidAccessPointException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "GroupScopeCreationException")
                {
                  exception = (Exception) new GroupScopeCreationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "DatabaseConnectionException")
                {
                  exception = (Exception) new DatabaseConnectionException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'd':
                if (exceptionType == "JobCannotBeUpdatedException")
                {
                  exception = (Exception) new JobCannotBeUpdatedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "InvalidFeatureNameException")
                {
                  exception = (Exception) new InvalidFeatureNameException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'm':
                if (exceptionType == "UnexpectedItemKindException")
                {
                  exception = (Exception) new UnexpectedItemKindException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "TeamFoundationLockException")
                {
                  exception = (Exception) new TeamFoundationLockException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                switch (exceptionType)
                {
                  case "InvalidTokenFormatExpcetion":
                    exception = (Exception) new InvalidTokenFormatExpcetion(exceptionMessage, innerException);
                    goto label_569;
                  case "JobCannotBeStoppedException":
                    exception = (Exception) new JobCannotBeStoppedException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'p':
                switch (exceptionType)
                {
                  case "CollectionPropertyException":
                    exception = (Exception) new CollectionPropertyException(exceptionMessage, innerException);
                    goto label_569;
                  case "VirtualPathMappingException":
                    exception = (Exception) new VirtualPathMappingException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 's':
                switch (exceptionType)
                {
                  case "IdentityExpressionException":
                    exception = (Exception) new IdentityExpressionException(exceptionMessage, innerException);
                    goto label_569;
                  case "JobCannotBeResumedException":
                    exception = (Exception) new JobCannotBeResumedException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 't':
                if (exceptionType == "UnexpectedHostTypeException")
                {
                  exception = (Exception) new UnexpectedHostTypeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'y':
                if (exceptionType == "InvalidDisplayNameException")
                {
                  exception = (Exception) new InvalidDisplayNameException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 28:
            switch (exceptionType[4])
            {
              case 'A':
                if (exceptionType == "FileAlreadyUploadedException")
                {
                  exception = (Exception) new FileAlreadyUploadedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'C':
                if (exceptionType == "HostCannotBeDeletedException")
                {
                  exception = (Exception) new HostCannotBeDeletedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "HostProcessNotFoundException")
                {
                  exception = (Exception) new HostProcessNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'U':
                if (exceptionType == "NodeUriDoesNotExistException")
                {
                  exception = (Exception) new NodeUriDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                switch (exceptionType)
                {
                  case "DBExecutingDeadlockException":
                    exception = (Exception) new DBExecutingDeadlockException(exceptionMessage, innerException);
                    goto label_569;
                  case "ProjectDoesNotExistException":
                    exception = (Exception) new ProjectDoesNotExistException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'f':
                if (exceptionType == "ModifyEveryoneGroupException")
                {
                  exception = (Exception) new ModifyEveryoneGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ServicingValidationException")
                {
                  exception = (Exception) new ServicingValidationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "InvalidLobParameterException")
                {
                  exception = (Exception) new InvalidLobParameterException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "NotApplicationGroupException")
                {
                  exception = (Exception) new NotApplicationGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                switch (exceptionType)
                {
                  case "RegistryPathPatternException":
                    exception = (Exception) new RegistryPathPatternException(exceptionMessage, innerException);
                    goto label_569;
                  case "ClassIdDoesNotExistException":
                    exception = (Exception) new ClassIdDoesNotExistException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 't':
                if (exceptionType == "UnauthorizedRequestException")
                {
                  exception = (Exception) new UnauthorizedRequestException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'v':
                if (exceptionType == "RemoveAccessMappingException")
                {
                  exception = (Exception) new RemoveAccessMappingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'y':
                if (exceptionType == "ProxyClientRedirectException")
                {
                  exception = (Exception) new ProxyClientRedirectException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 29:
            switch (exceptionType[7])
            {
              case 'A':
                if (exceptionType == "ProjectAlreadyExistsException")
                {
                  exception = (Exception) new ProjectAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "MaximumDepthExceededException")
                {
                  exception = (Exception) new MaximumDepthExceededException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'J':
                if (exceptionType == "CleanupJobInProgressException")
                {
                  exception = (Exception) new CleanupJobInProgressException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'N':
                if (exceptionType == "ServiceNotRegisteredException")
                {
                  exception = (Exception) new ServiceNotRegisteredException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "VirtualPathsConflictException")
                {
                  exception = (Exception) new VirtualPathsConflictException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'Q':
                if (exceptionType == "MessageQueueNotFoundException")
                {
                  exception = (Exception) new MessageQueueNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "InvalidSecurityTokenException")
                {
                  exception = (Exception) new InvalidSecurityTokenException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "DatabasePoolNotFoundException")
                {
                  exception = (Exception) new DatabasePoolNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'h':
                if (exceptionType == "NodePathDoesNotExistException")
                {
                  exception = (Exception) new NodePathDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "CannotModifyRootNodeException")
                {
                  exception = (Exception) new CannotModifyRootNodeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "RegisterObjectExistsException")
                {
                  exception = (Exception) new RegisterObjectExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                if (exceptionType == "SyncSubsystemServiceException")
                {
                  exception = (Exception) new SyncSubsystemServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "DuplicateJobScheduleException")
                {
                  exception = (Exception) new DuplicateJobScheduleException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 30:
            switch (exceptionType[9])
            {
              case 'D':
                if (exceptionType == "StrongBoxDrawerExistsException")
                {
                  exception = (Exception) new StrongBoxDrawerExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'G':
                if (exceptionType == "AddMemberGroupMissingException")
                {
                  exception = (Exception) new AddMemberGroupMissingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "StrongBoxItemNotFoundException")
                {
                  exception = (Exception) new StrongBoxItemNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'b':
                if (exceptionType == "RegisterObjectNoClassException")
                {
                  exception = (Exception) new RegisterObjectNoClassException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "InvalidEscapeSequenceException")
                {
                  exception = (Exception) new InvalidEscapeSequenceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "ActiveDirectoryAccessException")
                {
                  exception = (Exception) new ActiveDirectoryAccessException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'g':
                if (exceptionType == "UnknownMigrationOwnerException")
                {
                  exception = (Exception) new UnknownMigrationOwnerException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'h':
                if (exceptionType == "DateTimeShiftDetectedException")
                {
                  exception = (Exception) new DateTimeShiftDetectedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ReclassifiedToSubTreeException")
                {
                  exception = (Exception) new ReclassifiedToSubTreeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "IdentityAlreadyExistsException")
                {
                  exception = (Exception) new IdentityAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                switch (exceptionType)
                {
                  case "DatabaseConfigurationException":
                    exception = (Exception) new DatabaseConfigurationException(exceptionMessage, innerException);
                    goto label_569;
                  case "CircularNodeReferenceException":
                    exception = (Exception) new CircularNodeReferenceException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'r':
                switch (exceptionType)
                {
                  case "InvalidServiceVersionException":
                    exception = (Exception) new InvalidServiceVersionException(exceptionMessage, innerException);
                    goto label_569;
                  case "StepPerformerNotFoundException":
                    exception = (Exception) new StepPerformerNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 't':
                if (exceptionType == "JobDefinitionNotFoundException")
                {
                  exception = (Exception) new JobDefinitionNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 31:
            switch (exceptionType[8])
            {
              case 'D':
                if (exceptionType == "IdentityDomainMismatchException")
                {
                  exception = (Exception) new IdentityDomainMismatchException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "BadClassIdActionIdPairException")
                {
                  exception = (Exception) new BadClassIdActionIdPairException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'K':
                if (exceptionType == "ArtifactKindRestrictedException")
                {
                  exception = (Exception) new ArtifactKindRestrictedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "ResourceStreamNotFoundException")
                {
                  exception = (Exception) new ResourceStreamNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                switch (exceptionType)
                {
                  case "InvalidCatalogNodeMoveException":
                    exception = (Exception) new InvalidCatalogNodeMoveException(exceptionMessage, innerException);
                    goto label_569;
                  case "InvalidCatalogNodePathException":
                    exception = (Exception) new InvalidCatalogNodePathException(exceptionMessage, innerException);
                    goto label_569;
                  case "InvalidCatalogSaveNodeException":
                    exception = (Exception) new InvalidCatalogSaveNodeException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'd':
                if (exceptionType == "ParentNodeDoesNotExistException")
                {
                  exception = (Exception) new ParentNodeDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "GroupNameNotRecognizedException")
                {
                  exception = (Exception) new GroupNameNotRecognizedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                switch (exceptionType)
                {
                  case "ApplicationTierTimeoutException":
                    exception = (Exception) new ApplicationTierTimeoutException(exceptionMessage, innerException);
                    goto label_569;
                  case "RequestDisabledDetailsException":
                    exception = (Exception) new RequestDisabledDetailsException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'm':
                if (exceptionType == "MoveArgumentOutOfRangeException")
                {
                  exception = (Exception) new MoveArgumentOutOfRangeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "RemoveNonexistentGroupException")
                {
                  exception = (Exception) new RemoveNonexistentGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "CollectionDoesNotExistException")
                {
                  exception = (Exception) new CollectionDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "GroupScopeDoesNotExistException")
                {
                  exception = (Exception) new GroupScopeDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "AzureDeprecatedFeatureException")
                {
                  exception = (Exception) new AzureDeprecatedFeatureException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                if (exceptionType == "AzureSessionTerminatedException")
                {
                  exception = (Exception) new AzureSessionTerminatedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "BadParentObjectClassIdException")
                {
                  exception = (Exception) new BadParentObjectClassIdException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'v':
                if (exceptionType == "AzureServerUnavailableException")
                {
                  exception = (Exception) new AzureServerUnavailableException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 32:
            switch (exceptionType[6])
            {
              case 'B':
                if (exceptionType == "StrongBoxDrawerNotFoundException")
                {
                  exception = (Exception) new StrongBoxDrawerNotFoundException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "RemoveLastLicensedAdminException")
                {
                  exception = (Exception) new RemoveLastLicensedAdminException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                switch (exceptionType)
                {
                  case "EnumerationNoneArgumentException":
                    exception = (Exception) new EnumerationNoneArgumentException(exceptionMessage, innerException);
                    goto label_569;
                  case "ProxyCacheMissBecameHitException":
                    exception = (Exception) new ProxyCacheMissBecameHitException(exceptionMessage, innerException);
                    goto label_569;
                  case "InternalStoredProcedureException":
                    exception = (Exception) new InternalStoredProcedureException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'c':
                if (exceptionType == "SyncAccessDeniedServiceException")
                {
                  exception = (Exception) new SyncAccessDeniedServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'd':
                if (exceptionType == "InvalidReclassificationException")
                {
                  exception = (Exception) new InvalidReclassificationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                switch (exceptionType)
                {
                  case "JobIntervalNotSupportedException":
                    exception = (Exception) new JobIntervalNotSupportedException(exceptionMessage, innerException);
                    goto label_569;
                  case "RegisterObjectBadParentException":
                    exception = (Exception) new RegisterObjectBadParentException(exceptionMessage, innerException);
                    goto label_569;
                  case "RegisterObjectNoProjectException":
                    exception = (Exception) new RegisterObjectNoProjectException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'g':
                if (exceptionType == "CatalogNodeDoesNotExistException")
                {
                  exception = (Exception) new CatalogNodeDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ServicingStepGroupInUseException")
                {
                  exception = (Exception) new ServicingStepGroupInUseException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'l':
                switch (exceptionType)
                {
                  case "AzureClientIPRestrictedException":
                    exception = (Exception) new AzureClientIPRestrictedException(exceptionMessage, innerException);
                    goto label_569;
                  case "MultipleIdentitiesFoundException":
                    exception = (Exception) new MultipleIdentitiesFoundException(exceptionMessage, innerException);
                    goto label_569;
                  case "ProxyAlreadyAddedToSiteException":
                    exception = (Exception) new ProxyAlreadyAddedToSiteException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'n':
                if (exceptionType == "JobAgentTeardownTimeoutException")
                {
                  exception = (Exception) new JobAgentTeardownTimeoutException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "CollectionMisconfiguredException")
                {
                  exception = (Exception) new CollectionMisconfiguredException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "TeamFoundationServicingException")
                {
                  exception = (Exception) new TeamFoundationServicingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 33:
            switch (exceptionType[9])
            {
              case 'I':
                if (exceptionType == "AddMemberIdentityMissingException")
                {
                  exception = (Exception) new AddMemberIdentityMissingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "DuplicateLocationMappingException")
                {
                  exception = (Exception) new DuplicateLocationMappingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "FindGroupSidDoesNotExistException")
                {
                  exception = (Exception) new FindGroupSidDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                switch (exceptionType)
                {
                  case "TeamFoundationJobServiceException":
                    exception = (Exception) new TeamFoundationJobServiceException(exceptionMessage, innerException);
                    goto label_569;
                  case "TeamFoundationValidationException":
                    exception = (Exception) new TeamFoundationValidationException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'b':
                if (exceptionType == "RemoveMemberGroupMissingException")
                {
                  exception = (Exception) new RemoveMemberGroupMissingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "InvalidSecurityNamespaceException")
                {
                  exception = (Exception) new InvalidSecurityNamespaceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'd':
                if (exceptionType == "UnexpectedDatabaseResultException")
                {
                  exception = (Exception) new UnexpectedDatabaseResultException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'f':
                if (exceptionType == "RemoveSelfFromAdminGroupException")
                {
                  exception = (Exception) new RemoveSelfFromAdminGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                switch (exceptionType)
                {
                  case "ActionDeniedBySubscriberException":
                    exception = (Exception) new ActionDeniedBySubscriberException(exceptionMessage, innerException);
                    goto label_569;
                  case "DownloadTicketValidationException":
                    exception = (Exception) new DownloadTicketValidationException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'm':
                if (exceptionType == "ProjectNameNotRecognizedException")
                {
                  exception = (Exception) new ProjectNameNotRecognizedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "HostInstanceDoesNotExistException")
                {
                  exception = (Exception) new HostInstanceDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "DatabaseOperationTimeoutException")
                {
                  exception = (Exception) new DatabaseOperationTimeoutException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                switch (exceptionType)
                {
                  case "IdentityPropertyRequiredException":
                    exception = (Exception) new IdentityPropertyRequiredException(exceptionMessage, innerException);
                    goto label_569;
                  case "InvalidServiceDefinitionException":
                    exception = (Exception) new InvalidServiceDefinitionException(exceptionMessage, innerException);
                    goto label_569;
                  case "InvalidServicingStepTypeException":
                    exception = (Exception) new InvalidServicingStepTypeException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 't':
                switch (exceptionType)
                {
                  case "AcquireDatabasePartitionException":
                    exception = (Exception) new AcquireDatabasePartitionException(exceptionMessage, innerException);
                    goto label_569;
                  case "InvalidCatalogDeleteNodeException":
                    exception = (Exception) new InvalidCatalogDeleteNodeException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
            }
            break;
          case 34:
            switch (exceptionType[4])
            {
              case 'B':
                if (exceptionType == "SyncBadBaselineRevServiceException")
                {
                  exception = (Exception) new SyncBadBaselineRevServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'G':
                if (exceptionType == "FindGroupNameDoesNotExistException")
                {
                  exception = (Exception) new FindGroupNameDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "MessageQueueAlreadyExistsException")
                {
                  exception = (Exception) new MessageQueueAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'b':
                switch (exceptionType)
                {
                  case "DatabaseOperationCanceledException":
                    exception = (Exception) new DatabaseOperationCanceledException(exceptionMessage, innerException);
                    goto label_569;
                  case "DatabasePartitionNotFoundException":
                    exception = (Exception) new DatabasePartitionNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                  case "DatabasePoolAlreadyExistsException":
                    exception = (Exception) new DatabasePoolAlreadyExistsException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'e':
                switch (exceptionType)
                {
                  case "AddMemberCyclicMembershipException":
                    exception = (Exception) new AddMemberCyclicMembershipException(exceptionMessage, innerException);
                    goto label_569;
                  case "AzureDatabaseQuotaReachedException":
                    exception = (Exception) new AzureDatabaseQuotaReachedException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'f':
                if (exceptionType == "ArtifactKindAlreadyExistsException")
                {
                  exception = (Exception) new ArtifactKindAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'l':
                if (exceptionType == "InvalidRequestContextHostException")
                {
                  exception = (Exception) new InvalidRequestContextHostException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "ParentNodeUriDoesNotExistException")
                {
                  exception = (Exception) new ParentNodeUriDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "CssProjectUriDoesNotExistException")
                {
                  exception = (Exception) new CssProjectUriDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "CircularObjectInheritanceException")
                {
                  exception = (Exception) new CircularObjectInheritanceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'y':
                if (exceptionType == "AnalysisServiceConnectionException")
                {
                  exception = (Exception) new AnalysisServiceConnectionException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 35:
            switch (exceptionType[8])
            {
              case 'A':
                if (exceptionType == "SecurityActionDoesNotExistException")
                {
                  exception = (Exception) new SecurityActionDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'D':
                if (exceptionType == "IdentityDomainDoesNotExistException")
                {
                  exception = (Exception) new IdentityDomainDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'F':
                if (exceptionType == "DownloadFromMidTierAbortedException")
                {
                  exception = (Exception) new DownloadFromMidTierAbortedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'K':
                if (exceptionType == "ArtifactKindsMustBeUniformException")
                {
                  exception = (Exception) new ArtifactKindsMustBeUniformException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'N':
                if (exceptionType == "IdentityNotServiceIdentityException")
                {
                  exception = (Exception) new IdentityNotServiceIdentityException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'O':
                if (exceptionType == "SecurityObjectDoesNotExistException")
                {
                  exception = (Exception) new SecurityObjectDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'a':
                if (exceptionType == "InvalidCatalogSaveResourceException")
                {
                  exception = (Exception) new InvalidCatalogSaveResourceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'd':
                switch (exceptionType)
                {
                  case "TeamFoundationDatabaseCopyException":
                    exception = (Exception) new TeamFoundationDatabaseCopyException(exceptionMessage, innerException);
                    goto label_569;
                  case "TeamFoundationMessageQueueException":
                    exception = (Exception) new TeamFoundationMessageQueueException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'e':
                switch (exceptionType)
                {
                  case "InvalidServiceIdentityNameException":
                    exception = (Exception) new InvalidServiceIdentityNameException(exceptionMessage, innerException);
                    goto label_569;
                  case "ServiceVersionNotSupportedException":
                    exception = (Exception) new ServiceVersionNotSupportedException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'g':
                switch (exceptionType)
                {
                  case "ServicingOperationNotFoundException":
                    exception = (Exception) new ServicingOperationNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                  case "ServicingStepGroupNotFoundException":
                    exception = (Exception) new ServicingStepGroupNotFoundException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'm':
                if (exceptionType == "RemoveMemberServiceAccountException")
                {
                  exception = (Exception) new RemoveMemberServiceAccountException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "RemoveGroupMemberNotMemberException")
                {
                  exception = (Exception) new RemoveGroupMemberNotMemberException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "AccessMappingNotRegisteredException")
                {
                  exception = (Exception) new AccessMappingNotRegisteredException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "AzureOperationNotSupportedException")
                {
                  exception = (Exception) new AzureOperationNotSupportedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                if (exceptionType == "RemoveLastAdminGroupMemberException")
                {
                  exception = (Exception) new RemoveLastAdminGroupMemberException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 36:
            switch (exceptionType[11])
            {
              case 'N':
                if (exceptionType == "ProjectDoesNotExistWithNameException")
                {
                  exception = (Exception) new ProjectDoesNotExistWithNameException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'b':
                if (exceptionType == "AddGroupMemberIllegalMemberException")
                {
                  exception = (Exception) new AddGroupMemberIllegalMemberException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'd':
                if (exceptionType == "ReclassifiedToDifferentTreeException")
                {
                  exception = (Exception) new ReclassifiedToDifferentTreeException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                switch (exceptionType)
                {
                  case "TeamFoundationAccessControlException":
                    exception = (Exception) new TeamFoundationAccessControlException(exceptionMessage, innerException);
                    goto label_569;
                  case "TeamFoundationDatabaseSplitException":
                    exception = (Exception) new TeamFoundationDatabaseSplitException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
              case 'l':
                if (exceptionType == "DatabasePoolCannotBeDeletedException")
                {
                  exception = (Exception) new DatabasePoolCannotBeDeletedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'n':
                if (exceptionType == "DestroyedContentUnavailableException")
                {
                  exception = (Exception) new DestroyedContentUnavailableException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "LocationMappingDoesNotExistException")
                {
                  exception = (Exception) new LocationMappingDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "RemoveMemberIdentityMissingException")
                {
                  exception = (Exception) new RemoveMemberIdentityMissingException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "CannotAddDateToNonIterationException")
                {
                  exception = (Exception) new CannotAddDateToNonIterationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "CatalogResourceDoesNotExistException")
                {
                  exception = (Exception) new CatalogResourceDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'v':
                if (exceptionType == "IdentityProviderUnavailableException")
                {
                  exception = (Exception) new IdentityProviderUnavailableException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 37:
            switch (exceptionType[14])
            {
              case 'C':
                if (exceptionType == "TeamFoundationCatalogServiceException")
                {
                  exception = (Exception) new TeamFoundationCatalogServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "TeamFoundationProfileServiceException")
                {
                  exception = (Exception) new TeamFoundationProfileServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'e':
                if (exceptionType == "FailedToAcquireServicingLockException")
                {
                  exception = (Exception) new FailedToAcquireServicingLockException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "AddProjectGroupToGlobalGroupException")
                {
                  exception = (Exception) new AddProjectGroupToGlobalGroupException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "ServicingStepGroupDependencyException")
                {
                  exception = (Exception) new ServicingStepGroupDependencyException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 38:
            switch (exceptionType[14])
            {
              case 'I':
                if (exceptionType == "TeamFoundationIdentityServiceException")
                {
                  exception = (Exception) new TeamFoundationIdentityServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'L':
                if (exceptionType == "TeamFoundationLocationServiceException")
                {
                  exception = (Exception) new TeamFoundationLocationServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "RegisterObjectProjectMismatchException")
                {
                  exception = (Exception) new RegisterObjectProjectMismatchException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "TeamFoundationSecurityServiceException")
                {
                  exception = (Exception) new TeamFoundationSecurityServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'f':
                if (exceptionType == "CannotUpdateDefaultCollectionException")
                {
                  exception = (Exception) new CannotUpdateDefaultCollectionException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'i':
                if (exceptionType == "ServiceDefinitionDoesNotExistException")
                {
                  exception = (Exception) new ServiceDefinitionDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'm':
                if (exceptionType == "IncompatibleCompressionFormatException")
                {
                  exception = (Exception) new IncompatibleCompressionFormatException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'r':
                if (exceptionType == "DatabaseCategoryNotRegisteredException")
                {
                  exception = (Exception) new DatabaseCategoryNotRegisteredException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'u':
                switch (exceptionType)
                {
                  case "AuthorizationSubsystemServiceException":
                    exception = (Exception) new AuthorizationSubsystemServiceException(exceptionMessage, innerException);
                    goto label_569;
                  case "GroupSecuritySubsystemServiceException":
                    exception = (Exception) new GroupSecuritySubsystemServiceException(exceptionMessage, innerException);
                    goto label_569;
                }
                break;
            }
            break;
          case 39:
            switch (exceptionType[3])
            {
              case 'G':
                if (exceptionType == "AddGroupMemberOverLicenseLimitException")
                {
                  exception = (Exception) new AddGroupMemberOverLicenseLimitException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'M':
                if (exceptionType == "AddMemberIdentityAlreadyMemberException")
                {
                  exception = (Exception) new AddMemberIdentityAlreadyMemberException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'P':
                if (exceptionType == "AddProjectGroupProjectMismatchException")
                {
                  exception = (Exception) new AddProjectGroupProjectMismatchException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'o':
                if (exceptionType == "AccountPreferencesAlreadyExistException")
                {
                  exception = (Exception) new AccountPreferencesAlreadyExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'p':
                if (exceptionType == "PropertyDefinitionDoesNotExistException")
                {
                  exception = (Exception) new PropertyDefinitionDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 't':
                if (exceptionType == "MultiplePartitionsNotSupportedException")
                {
                  exception = (Exception) new MultiplePartitionsNotSupportedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "SecurityNamespaceAlreadyExistsException")
                {
                  exception = (Exception) new SecurityNamespaceAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 40:
            switch (exceptionType[9])
            {
              case 'O':
                if (exceptionType == "ServicingOperationAlreadyExistsException")
                {
                  exception = (Exception) new ServicingOperationAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "ServicingStepGroupAlreadyExistsException")
                {
                  exception = (Exception) new ServicingStepGroupAlreadyExistsException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'c':
                if (exceptionType == "IdentityAccountNameAlreadyInUseException")
                {
                  exception = (Exception) new IdentityAccountNameAlreadyInUseException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 's':
                if (exceptionType == "CatalogResourceTypeDoesNotExistException")
                {
                  exception = (Exception) new CatalogResourceTypeDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'u':
                if (exceptionType == "CommonStructureSubsystemServiceException")
                {
                  exception = (Exception) new CommonStructureSubsystemServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 41:
            switch (exceptionType[0])
            {
              case 'D':
                if (exceptionType == "DatabasePartitionCannotBeDeletedException")
                {
                  exception = (Exception) new DatabasePartitionCannotBeDeletedException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "ReclassificationNodeDoesNotExistException")
                {
                  exception = (Exception) new ReclassificationNodeDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'S':
                if (exceptionType == "SyncSupersededBaselineRevServiceException")
                {
                  exception = (Exception) new SyncSupersededBaselineRevServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'T':
                if (exceptionType == "TeamFoundationInvalidCertificateException")
                {
                  exception = (Exception) new TeamFoundationInvalidCertificateException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 42:
            switch (exceptionType[0])
            {
              case 'C':
                if (exceptionType == "CannotAccessIdentitiesAfterDetachException")
                {
                  exception = (Exception) new CannotAccessIdentitiesAfterDetachException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'T':
                if (exceptionType == "TeamFoundationProcessStillRunningException")
                {
                  exception = (Exception) new TeamFoundationProcessStillRunningException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 44:
            switch (exceptionType[0])
            {
              case 'I':
                if (exceptionType == "InvalidSecurityNamespaceDescriptionException")
                {
                  exception = (Exception) new InvalidSecurityNamespaceDescriptionException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'R':
                if (exceptionType == "ReclassificationNodeUriDoesNotExistException")
                {
                  exception = (Exception) new ReclassificationNodeUriDoesNotExistException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 45:
            switch (exceptionType[0])
            {
              case 'A':
                if (exceptionType == "AddGroupMemberIllegalWindowsIdentityException")
                {
                  exception = (Exception) new AddGroupMemberIllegalWindowsIdentityException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'I':
                if (exceptionType == "InvalidBindPendingIdentityDescriptorException")
                {
                  exception = (Exception) new InvalidBindPendingIdentityDescriptorException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 46:
            if (exceptionType == "AddGroupMemberIllegalInternetIdentityException")
            {
              exception = (Exception) new AddGroupMemberIllegalInternetIdentityException(exceptionMessage, innerException);
              goto label_569;
            }
            else
              break;
          case 48:
            switch (exceptionType[0])
            {
              case 'S':
                if (exceptionType == "StreamingCollectionUnsupportedOperationException")
                {
                  exception = (Exception) new StreamingCollectionUnsupportedOperationException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
              case 'T':
                if (exceptionType == "TeamFoundationResourceManagementServiceException")
                {
                  exception = (Exception) new TeamFoundationResourceManagementServiceException(exceptionMessage, innerException);
                  goto label_569;
                }
                else
                  break;
            }
            break;
          case 52:
            if (exceptionType == "IllegalDeleteSelfReferenceServiceDefinitionException")
            {
              exception = (Exception) new IllegalDeleteSelfReferenceServiceDefinitionException(exceptionMessage, innerException);
              goto label_569;
            }
            else
              break;
        }
      }
      if (convertUnknown && exception == null)
        exception = (Exception) new TeamFoundationServiceException(exceptionMessage, innerException);
label_569:
      return exception;
    }
  }
}
