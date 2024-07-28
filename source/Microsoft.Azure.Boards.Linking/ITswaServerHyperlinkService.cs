// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Linking.ITswaServerHyperlinkService
// Assembly: Microsoft.Azure.Boards.Linking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2FA874A3-91E6-4EEC-B5F5-3126D83824FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Linking.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Boards.Linking
{
  [DefaultServiceImplementation(typeof (TswaServerHyperlinkService))]
  public interface ITswaServerHyperlinkService : IVssFrameworkService
  {
    Uri GetHomeUrl();

    Uri GetViewBuildDetailsUrl(Uri buildUri, Guid projectId);

    Uri GetChangesetDetailsUrl(IVssRequestContext requestContext, int changesetId);

    Uri GetDifferenceSourceControlItemsUrl(
      IVssRequestContext requestContext,
      string originalItemServerPath,
      int originalItemChangeset,
      string modifiedItemServerPath,
      int modifiedItemChangeset);

    Uri GetDifferenceSourceControlItemsUrl(
      IVssRequestContext requestContext,
      string originalItemServerPath,
      string originalItemVersionSpec,
      string modifiedItemServerPath,
      string modifiedItemVersionSpec);

    Uri GetSourceExplorerUrl(IVssRequestContext requestContext, string serverItemPath);

    Uri GetViewSourceControlItemUrl(
      IVssRequestContext requestContext,
      string serverItemPath,
      int changesetId);

    Uri GetShelvesetDetailsUrl(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner);

    Uri GetDifferenceSourceControlShelvedItemUrl(
      IVssRequestContext requestContext,
      string originalItemServerPath,
      int originalItemChangeset,
      string shelvedItemServerPath,
      string shelvesetName,
      string shelvesetOwner);

    Uri GetViewSourceControlShelvedItemUrl(
      IVssRequestContext requestContext,
      string serverItemPath,
      string shelvesetName,
      string shelvesetOwner);

    Uri GetWorkItemEditorUrl(IVssRequestContext requestContext, int workItemId);

    Uri GetWorkItemEditorUrl(IVssRequestContext requestContext, Uri projectUri, int workItemId);

    Uri GetWorkItemEditorUrl(IVssRequestContext requestContext, Guid projectGuid, int workItemId);

    Uri GetWorkItemEditorUrl(
      IVssRequestContext requestContext,
      int workItemId,
      Guid remoteHostId,
      string remoteHostUrl,
      Guid? remoteProjectId = null);

    Uri GetWorkItemQueryResultsUrl(IVssRequestContext requestContext, Uri projectUri, Guid queryId);
  }
}
