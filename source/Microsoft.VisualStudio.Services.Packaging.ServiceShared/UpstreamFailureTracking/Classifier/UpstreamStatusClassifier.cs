// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier.UpstreamStatusClassifier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier
{
  public class UpstreamStatusClassifier : IUpstreamStatusClassifier
  {
    public UpstreamFailureException Classify(
      Exception inputException,
      UpstreamSource upstream,
      FeedCore feed)
    {
      if (inputException is UpstreamFailureException failureException)
        return failureException;
      Exception innerException = inputException;
      UpstreamStatusCategory upstreamStatusCategory;
      if (!(inputException is FeedViewNotFoundException))
      {
        if (!(inputException is FeedIdNotFoundException))
        {
          if (!(inputException is ProjectDoesNotExistException))
          {
            if (!(inputException is ServiceHostDoesNotExistException))
            {
              upstreamStatusCategory = inputException is OperationCanceledException ? UpstreamStatusCategory.Aborted : (inputException is PublicUpstreamFailureException ? UpstreamStatusCategory.PublicUpstreamFailure : (inputException is FeedNeedsPermissionsException ? UpstreamStatusCategory.TargetViewInsufficientVisibility : (inputException is UpstreamOrganizationNotAccessibleException ? UpstreamStatusCategory.TargetOrganizationInaccessible : (inputException is NoTokenFoundFromServiceEndpointException ? UpstreamStatusCategory.TargetOrganizationServiceConnectionFailure : UpstreamStatusCategory.UnknownFailure))));
            }
            else
            {
              upstreamStatusCategory = UpstreamStatusCategory.TargetOrganizationInaccessible;
              innerException = (Exception) new UpstreamOrganizationNotAccessibleException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InternalUpstreamSourceNotFound((object) upstream.Name));
            }
          }
          else
          {
            upstreamStatusCategory = UpstreamStatusCategory.TargetProjectDeleted;
            innerException = (Exception) new UpstreamProjectDoesNotExistException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamProjectDoesNotExist((object) upstream.Name, (object) upstream.InternalUpstreamProjectId), inputException);
          }
        }
        else
        {
          upstreamStatusCategory = UpstreamStatusCategory.TargetFeedDeleted;
          innerException = (Exception) new UpstreamSourceNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InternalUpstreamSourceDeleted((object) upstream.Name, (object) feed.Name), inputException);
        }
      }
      else
      {
        upstreamStatusCategory = UpstreamStatusCategory.TargetViewDeleted;
        innerException = (Exception) new UpstreamSourceNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InternalUpstreamSourceDeleted((object) upstream.Name, (object) feed.Name), inputException);
      }
      if (upstream.UpstreamSourceType == UpstreamSourceType.Public && (object) WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstream.Location) == null)
      {
        switch (inputException)
        {
          case InvalidUpstreamSourceDuplicateVersionsException _:
            upstreamStatusCategory = UpstreamStatusCategory.CustomPublicUpstreamFailure_DuplicatePackageVersions;
            break;
          default:
            upstreamStatusCategory = UpstreamStatusCategory.CustomPublicUpstreamFailure;
            break;
        }
      }
      if (inputException is VssUnauthorizedException && upstream.ServiceEndpointId.HasValue)
        upstreamStatusCategory = UpstreamStatusCategory.TargetOrganizationServiceConnectionFailure;
      return new UpstreamFailureException(innerException.Message, innerException, upstreamStatusCategory);
    }
  }
}
