// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.IntegrationInterface
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class IntegrationInterface
  {
    internal IntegrationInterface()
    {
    }

    internal Artifact[] BisGetArtifacts(
      VersionControlRequestContext versionControlRequestContext,
      string[] ArtifactUriList)
    {
      if (ArtifactUriList == null || ArtifactUriList.Length == 0)
        return Array.Empty<Artifact>();
      List<Artifact> artifactList = new List<Artifact>();
      foreach (string artifactUri in ArtifactUriList)
      {
        Artifact artifact = (Artifact) null;
        try
        {
          artifact = this.ArtifactUriToArtifact(versionControlRequestContext, artifactUri);
        }
        catch (TeamFoundationServerException ex)
        {
          versionControlRequestContext.RequestContext.Trace(700030, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, Resources.Format("ArtifactUnaccessible", (object) artifactUri));
          versionControlRequestContext.RequestContext.TraceException(700030, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
        }
        artifactList.Add(artifact);
      }
      if (versionControlRequestContext.RequestContext.IsTracing(700031, TraceLevel.Verbose, TraceArea.Bis, TraceLayer.BusinessLogic))
      {
        versionControlRequestContext.RequestContext.Trace(700031, TraceLevel.Verbose, TraceArea.Bis, TraceLayer.BusinessLogic, "GetArtifacts returns {0} elements", (object) artifactList.Count);
        foreach (Artifact artifact in artifactList)
        {
          if (artifact != null)
            versionControlRequestContext.RequestContext.Trace(700031, TraceLevel.Verbose, TraceArea.Bis, TraceLayer.BusinessLogic, "Artifact: {0}, [id = {1}]", (object) artifact.ArtifactTitle, (object) artifact.Uri);
          else
            versionControlRequestContext.RequestContext.Trace(700031, TraceLevel.Verbose, TraceArea.Bis, TraceLayer.BusinessLogic, "Artifact: null");
        }
      }
      return artifactList.ToArray();
    }

    internal void FilterShelveset(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] userIdentities,
      string shelvesetName,
      string shelvesetOwner,
      int alertChangeLimit,
      out List<PathRestriction> restrictions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(shelvesetName, nameof (shelvesetName));
      ArgumentUtility.CheckStringForNullOrEmpty(shelvesetOwner, nameof (shelvesetOwner));
      restrictions = new List<PathRestriction>();
      for (int index = 0; index < userIdentities.Length; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = userIdentities[index];
        PathRestriction pathRestriction = (PathRestriction) null;
        try
        {
          pathRestriction = this.GetPathRestrictionForShelveset(versionControlRequestContext, userIdentities[index], shelvesetName, shelvesetOwner, alertChangeLimit, userIdentity);
        }
        catch (ShelvesetNotFoundException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700035, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
        }
        catch (ResourceAccessException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700036, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
          pathRestriction = new PathRestriction(userIdentity, (List<ClientArtifact>) null, false);
        }
        catch (Exception ex)
        {
          pathRestriction = new PathRestriction(userIdentity, (List<ClientArtifact>) null, false);
          versionControlRequestContext.RequestContext.TraceException(700037, TraceArea.Bis, TraceLayer.BusinessLogic, ex);
          TeamFoundationEventLog.Default.LogException(Resources.Format("InvalidIdentityException", (object) userIdentity.DisplayName), ex, TeamFoundationEventId.InvalidIdentity, EventLogEntryType.Warning);
        }
        finally
        {
          if (pathRestriction != null)
            restrictions.Add(pathRestriction);
        }
      }
    }

    internal void FilterChangeset(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] userIdentities,
      int changeset,
      int alertChangeLimit,
      out List<PathRestriction> restrictions)
    {
      ArgumentUtility.CheckForOutOfRange(changeset, nameof (changeset), 1);
      restrictions = new List<PathRestriction>();
      for (int index = 0; index < userIdentities.Length; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = userIdentities[index];
        PathRestriction pathRestriction = (PathRestriction) null;
        try
        {
          pathRestriction = this.GetPathRestrictionForChangeset(versionControlRequestContext, userIdentities[index], changeset, alertChangeLimit, userIdentity);
        }
        catch (ResourceAccessException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700038, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
          pathRestriction = new PathRestriction(userIdentity, (List<ClientArtifact>) null, false);
        }
        catch (Exception ex)
        {
          pathRestriction = new PathRestriction(userIdentity, (List<ClientArtifact>) null, false);
          versionControlRequestContext.RequestContext.TraceException(700039, TraceArea.Bis, TraceLayer.BusinessLogic, ex);
          TeamFoundationEventLog.Default.LogException(Resources.Format("InvalidIdentityException", (object) userIdentity), ex, TeamFoundationEventId.InvalidIdentity, EventLogEntryType.Warning);
        }
        finally
        {
          if (pathRestriction != null)
            restrictions.Add(pathRestriction);
        }
      }
    }

    private PathRestriction GetPathRestrictionForShelveset(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity principal,
      string shelvesetName,
      string shelvesetOwner,
      int alertChangeLimit,
      Microsoft.VisualStudio.Services.Identity.Identity user)
    {
      List<ClientArtifact> items = new List<ClientArtifact>();
      bool allChangesIncluded = true;
      using (IVssRequestContext userContext = versionControlRequestContext.RequestContext.CreateUserContext(principal.Descriptor))
      {
        VersionControlRequestContext versionControlRequestContext1 = new VersionControlRequestContext(userContext, versionControlRequestContext.VersionControlService);
        ITswaServerHyperlinkService tswaHyperlinkService = versionControlRequestContext.Elevate().RequestContext.GetService<ITswaServerHyperlinkService>();
        TeamFoundationLinkingService linkingService = (TeamFoundationLinkingService) null;
        string shelvesetUrl;
        try
        {
          shelvesetUrl = tswaHyperlinkService.GetShelvesetDetailsUrl(versionControlRequestContext.RequestContext, shelvesetName, shelvesetOwner).AbsoluteUri;
        }
        catch (InvalidOperationException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700040, TraceLevel.Info, TraceArea.Linking, TraceLayer.BusinessLogic, (Exception) ex);
          tswaHyperlinkService = (ITswaServerHyperlinkService) null;
          if (linkingService == null)
            linkingService = versionControlRequestContext.Elevate().RequestContext.GetService<TeamFoundationLinkingService>();
          VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ShelvesetUri(shelvesetName, shelvesetOwner, UriType.Extended);
          shelvesetUrl = linkingService.GetArtifactUrlExternal(versionControlRequestContext.Elevate().RequestContext, controlIntegrationUri.ArtifactId);
        }
        using (CommandQueryPendingSets queryPendingSets = new CommandQueryPendingSets(versionControlRequestContext1))
        {
          queryPendingSets.Execute((Workspace) null, shelvesetOwner, new ItemSpec[1]
          {
            new ItemSpec()
            {
              ItemPathPair = ItemPathPair.FromServerItem("$/"),
              RecursionType = RecursionType.Full
            }
          }, shelvesetName, PendingSetType.Shelveset, false, alertChangeLimit + 1, (string) null, false, false, (string[]) null, false);
          if (!queryPendingSets.PendingSets.MoveNext())
            return new PathRestriction(user, (List<ClientArtifact>) null, false);
          while (queryPendingSets.PendingSets.Current.PendingChanges.MoveNext())
          {
            if (items.Count < alertChangeLimit || alertChangeLimit == 0)
            {
              PendingChange current = queryPendingSets.PendingSets.Current.PendingChanges.Current;
              string shelvedItemUrl;
              try
              {
                shelvedItemUrl = Shelveset.GetShelvedItemUrl(versionControlRequestContext.Elevate(), shelvesetName, shelvesetOwner, current, shelvesetUrl, tswaHyperlinkService, linkingService);
              }
              catch (InvalidOperationException ex)
              {
                versionControlRequestContext.RequestContext.TraceException(700041, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
                tswaHyperlinkService = (ITswaServerHyperlinkService) null;
                if (linkingService == null)
                  linkingService = versionControlRequestContext.Elevate().RequestContext.GetService<TeamFoundationLinkingService>();
                shelvedItemUrl = Shelveset.GetShelvedItemUrl(versionControlRequestContext.Elevate(), shelvesetName, shelvesetOwner, current, shelvesetUrl, tswaHyperlinkService, linkingService);
              }
              items.Add(new ClientArtifact(shelvedItemUrl, "ShelvedItem")
              {
                ServerItem = current.ServerItem,
                ChangeType = Changeset.ChangeTypeToString(current.ChangeType)
              });
            }
            else
              allChangesIncluded = false;
          }
          return new PathRestriction(user, items, allChangesIncluded);
        }
      }
    }

    private PathRestriction GetPathRestrictionForChangeset(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity principal,
      int changeset,
      int alertChangeLimit,
      Microsoft.VisualStudio.Services.Identity.Identity user)
    {
      PathRestriction restrictionForChangeset = (PathRestriction) null;
      List<ClientArtifact> items = new List<ClientArtifact>();
      bool allChangesIncluded = true;
      using (IVssRequestContext userContext = versionControlRequestContext.RequestContext.CreateUserContext(principal.Descriptor))
      {
        VersionControlRequestContext versionControlRequestContext1 = new VersionControlRequestContext(userContext, versionControlRequestContext.VersionControlService);
        ITswaServerHyperlinkService tswaHyperlinkService = versionControlRequestContext.Elevate().RequestContext.GetService<ITswaServerHyperlinkService>();
        TeamFoundationLinkingService linkingService = (TeamFoundationLinkingService) null;
        using (CommandQueryChangeset commandQueryChangeset = new CommandQueryChangeset(versionControlRequestContext1))
        {
          commandQueryChangeset.Execute(changeset, true, false, alertChangeLimit + 1);
          while (commandQueryChangeset.Changeset.Changes.MoveNext())
          {
            if (items.Count < alertChangeLimit || alertChangeLimit == 0)
            {
              Change current = commandQueryChangeset.Changeset.Changes.Current;
              string versionedItemUrl;
              try
              {
                versionedItemUrl = Changeset.GetVersionedItemUrl(versionControlRequestContext.Elevate(), changeset, current, tswaHyperlinkService, linkingService);
              }
              catch (InvalidOperationException ex)
              {
                versionControlRequestContext.RequestContext.TraceException(700042, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
                tswaHyperlinkService = (ITswaServerHyperlinkService) null;
                if (linkingService == null)
                  linkingService = versionControlRequestContext.Elevate().RequestContext.GetService<TeamFoundationLinkingService>();
                versionedItemUrl = Changeset.GetVersionedItemUrl(versionControlRequestContext.Elevate(), changeset, current, tswaHyperlinkService, linkingService);
              }
              items.Add(new ClientArtifact(versionedItemUrl, "VersionedItem")
              {
                ServerItem = current.Item.ServerItem,
                ItemVersion = current.Item.ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
                ChangeType = Changeset.ChangeTypeToString(current.ChangeType)
              });
            }
            else
              allChangesIncluded = false;
          }
          restrictionForChangeset = new PathRestriction(user, items, allChangesIncluded);
        }
        return restrictionForChangeset;
      }
    }

    private void GetLabelArtifactInformation(
      VersionControlRequestContext versionControlRequestContext,
      VersionControlIntegrationUri genericUri,
      ref Artifact artifact)
    {
      artifact.ArtifactTitle = genericUri.ArtifactTitle;
      int labelId = int.Parse(genericUri.ArtifactName, (IFormatProvider) CultureInfo.InvariantCulture);
      VersionControlLabel labelByLabelId = VersionControlLabel.FindLabelByLabelId(versionControlRequestContext, labelId);
      if (IdentityDescriptorComparer.Instance.Equals(TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, labelByLabelId.ownerId).Descriptor, versionControlRequestContext.RequestContext.UserContext))
        return;
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.LabelOther, labelByLabelId.ScopePair);
    }

    private void GetChangesetArtifactInformation(
      VersionControlRequestContext versionControlRequestContext,
      VersionControlIntegrationUri genericUri,
      ref Artifact artifact)
    {
      int changeset = int.Parse(genericUri.ArtifactName, (IFormatProvider) CultureInfo.InvariantCulture);
      artifact.ArtifactTitle = new ChangesetUri(changeset, UriType.Normal).ArtifactTitle;
    }

    private void GetShelvesetArtifactInformation(
      VersionControlRequestContext versionControlRequestContext,
      VersionControlIntegrationUri genericUri,
      ref Artifact artifact)
    {
      string shelvesetName;
      string shelvesetOwner;
      ShelvesetUri.Decode(HttpUtility.UrlDecode(genericUri.ArtifactName), out shelvesetName, out shelvesetOwner);
      artifact.ArtifactTitle = new ShelvesetUri(shelvesetName, shelvesetOwner, UriType.Normal).ArtifactTitle;
    }

    private void GetLatestItemVersionArtifactInformation(
      VersionControlRequestContext versionControlRequestContext,
      VersionControlIntegrationUri genericUri,
      ref Artifact artifact,
      List<ExtendedAttribute> extAttrList)
    {
      Item obj = Item.QueryItem(versionControlRequestContext, int.Parse(genericUri.ArtifactName, (IFormatProvider) CultureInfo.InvariantCulture), new ChangesetVersionSpec(int.MaxValue), false);
      if (obj.DeletionId == 0)
      {
        extAttrList.Add(new ExtendedAttribute()
        {
          Name = "ServerPath",
          Value = obj.ServerItem
        });
        artifact.ArtifactTitle = Resources.Format("BisLatestItemVersionArtifactFormat", (object) obj.ServerItem);
      }
      else
        artifact.ArtifactTitle = Resources.Format("BisLatestItemVersionDeletedArtifactFormat", (object) obj.ServerItem);
    }

    private void GetVersionedItemArtifactInformation(
      VersionControlRequestContext versionControlRequestContext,
      VersionControlIntegrationUri genericUri,
      ref Artifact artifact)
    {
      string serverItem;
      int changeset;
      int deletionId;
      VersionedItemUri.Decode(HttpUtility.UrlDecode(genericUri.ArtifactName), out serverItem, out changeset, out deletionId);
      if (changeset == 0)
        changeset = versionControlRequestContext.VersionControlService.GetLatestChangeset(versionControlRequestContext);
      artifact.ArtifactTitle = new VersionedItemUri(serverItem, changeset, deletionId, UriType.Normal).ArtifactTitle;
    }

    private void GetShelvedItemArtifactInformation(
      VersionControlRequestContext versionControlRequestContext,
      VersionControlIntegrationUri genericUri,
      ref Artifact artifact)
    {
      string serverItem;
      string shelvesetName;
      string shelvesetOwner;
      ShelvedItemUri.Decode(HttpUtility.UrlDecode(genericUri.ArtifactName), out serverItem, out shelvesetName, out shelvesetOwner);
      artifact.ArtifactTitle = new ShelvedItemUri(serverItem, shelvesetName, shelvesetOwner, UriType.Normal).ArtifactTitle;
    }

    private Artifact ArtifactUriToArtifact(
      VersionControlRequestContext versionControlRequestContext,
      string artifactUri)
    {
      VersionControlIntegrationUri genericUri = new VersionControlIntegrationUri(artifactUri);
      Artifact artifact = new Artifact();
      artifact.Uri = artifactUri;
      List<ExtendedAttribute> extAttrList = new List<ExtendedAttribute>();
      if (genericUri.ArtifactType == ArtifactType.Label)
        this.GetLabelArtifactInformation(versionControlRequestContext, genericUri, ref artifact);
      else if (genericUri.ArtifactType == ArtifactType.VersionedItem)
        this.GetVersionedItemArtifactInformation(versionControlRequestContext, genericUri, ref artifact);
      else if (genericUri.ArtifactType == ArtifactType.LatestItemVersion)
        this.GetLatestItemVersionArtifactInformation(versionControlRequestContext, genericUri, ref artifact, extAttrList);
      else if (genericUri.ArtifactType == ArtifactType.Changeset)
        this.GetChangesetArtifactInformation(versionControlRequestContext, genericUri, ref artifact);
      else if (genericUri.ArtifactType == ArtifactType.Shelveset)
        this.GetShelvesetArtifactInformation(versionControlRequestContext, genericUri, ref artifact);
      else if (genericUri.ArtifactType == ArtifactType.ShelvedItem)
        this.GetShelvedItemArtifactInformation(versionControlRequestContext, genericUri, ref artifact);
      extAttrList.Add(new ExtendedAttribute()
      {
        Name = "RepositoryUrl",
        Value = versionControlRequestContext.RequestContext.VirtualPath().TrimEnd('/')
      });
      artifact.ExtendedAttributes = extAttrList.ToArray();
      return artifact;
    }
  }
}
