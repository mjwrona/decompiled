// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.ArtifactMetadataService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal sealed class ArtifactMetadataService : IArtifactMetadataService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Grafeas.V1.Note CreateNote(IVssRequestContext requestContext, Grafeas.V1.Note note)
    {
      Guid scopeId = note.ScopeId;
      this.CheckProjectPermission(requestContext, scopeId, TeamProjectPermissions.GenericRead);
      int file = requestContext.GetService<ITeamFoundationFileService>().SaveToFile<Grafeas.V1.Note>(requestContext, note);
      try
      {
        using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
          component.CreateNote(scopeId, note.Name, note.Kind, requestContext.GetUserId(), new int?(file));
        CustomerIntelligenceHelper.PublishNoteCreatedEvent(requestContext, note);
      }
      catch (Exception ex)
      {
        if (!(ex is NoteExistsException))
        {
          CustomerIntelligenceHelper.PublishNoteCreatedFailedEvent(requestContext, note, ex);
          throw;
        }
      }
      return note;
    }

    public Grafeas.V1.Occurrence CreateOccurrence(
      IVssRequestContext requestContext,
      Grafeas.V1.Occurrence occurrence)
    {
      Guid scopeId = occurrence.ScopeId;
      this.CheckProjectPermission(requestContext, scopeId, TeamProjectPermissions.GenericRead);
      List<string> tags = new List<string>();
      if (occurrence is Grafeas.V1.ImageOccurrence imageOccurrence)
        tags = imageOccurrence.Tags;
      int file = requestContext.GetService<ITeamFoundationFileService>().SaveToFile<Grafeas.V1.Occurrence>(requestContext, occurrence);
      Guid guid = Guid.NewGuid();
      occurrence.Name = guid.ToString();
      try
      {
        using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
          component.CreateOccurrence(scopeId, occurrence.NoteName, occurrence.Name, occurrence.Kind, occurrence.ResourceUri, requestContext.GetUserId(), new int?(file), (IList<string>) tags);
        CustomerIntelligenceHelper.PublishOccurenceCreatedEvent(requestContext, occurrence);
      }
      catch (Exception ex)
      {
        CustomerIntelligenceHelper.PublishOccurenceCreatedFailedEvent(requestContext, occurrence, ex);
        throw;
      }
      return occurrence;
    }

    public IList<string> CreateOccurrenceTag(
      IVssRequestContext requestContext,
      string occurrenceName,
      string tag)
    {
      OccurrenceData occurrence;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        occurrence = component.GetOccurrence(occurrenceName);
      if (occurrence == null)
        throw new OccurrenceNotFoundException(DeploymentResources.OccurrenceNotFound((object) occurrenceName));
      this.CheckProjectPermission(requestContext, occurrence.ScopeId, TeamProjectPermissions.GenericRead);
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        component.CreateOccurrenceTag(occurrence.Name, tag);
      return (IList<string>) new List<string>() { tag };
    }

    public void DeleteOccurrenceTag(
      IVssRequestContext requestContext,
      string occurrenceName,
      string tag)
    {
      OccurrenceData occurrence;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        occurrence = component.GetOccurrence(occurrenceName);
      if (occurrence == null)
        throw new OccurrenceNotFoundException(DeploymentResources.OccurrenceNotFound((object) occurrenceName));
      this.CheckProjectPermission(requestContext, occurrence.ScopeId, TeamProjectPermissions.GenericRead);
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        component.DeleteOccurrenceTag(occurrence.Name, tag);
    }

    public Grafeas.V1.Note GetNote(IVssRequestContext requestContext, string name)
    {
      NoteData note;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        note = component.GetNote(name);
      if (note == null)
        throw new NoteNotFoundException(DeploymentResources.NoteNotFound((object) name));
      this.CheckProjectPermission(requestContext, note.ScopeId, TeamProjectPermissions.GenericRead);
      return requestContext.GetService<ITeamFoundationFileService>().RetrieveFromFile<Grafeas.V1.Note>(requestContext, note.FileId.Value);
    }

    public IList<Grafeas.V1.OccurrenceReference> GetNoteOccurrences(
      IVssRequestContext requestContext,
      string noteName)
    {
      List<Grafeas.V1.OccurrenceReference> noteOccurrences1 = new List<Grafeas.V1.OccurrenceReference>();
      List<OccurrenceData> noteOccurrences2;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        noteOccurrences2 = component.GetNoteOccurrences(noteName);
      if (noteOccurrences2.Any<OccurrenceData>())
      {
        this.CheckProjectPermission(requestContext, noteOccurrences2.First<OccurrenceData>().ScopeId, TeamProjectPermissions.GenericRead);
        foreach (OccurrenceData occurrenceData in noteOccurrences2)
          noteOccurrences1.Add(new Grafeas.V1.OccurrenceReference()
          {
            Name = occurrenceData.Name,
            Kind = occurrenceData.Kind,
            NoteName = occurrenceData.NoteName,
            ScopeId = occurrenceData.ScopeId
          });
      }
      return (IList<Grafeas.V1.OccurrenceReference>) noteOccurrences1;
    }

    public Grafeas.V1.Occurrence GetOccurrence(IVssRequestContext requestContext, string name)
    {
      OccurrenceData occurrence;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        occurrence = component.GetOccurrence(name);
      if (occurrence == null)
        throw new OccurrenceNotFoundException(DeploymentResources.OccurrenceNotFound((object) name));
      this.CheckProjectPermission(requestContext, occurrence.ScopeId, TeamProjectPermissions.GenericRead);
      return requestContext.GetService<ITeamFoundationFileService>().RetrieveFromFile<Grafeas.V1.Occurrence>(requestContext, occurrence.FileId.Value);
    }

    public IList<Grafeas.V1.OccurrenceReference> GetOccurrences(
      IVssRequestContext requestContext,
      string resourceId)
    {
      List<Grafeas.V1.OccurrenceReference> occurrences1 = new List<Grafeas.V1.OccurrenceReference>();
      List<OccurrenceData> occurrences2;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        occurrences2 = component.GetOccurrences(resourceId);
      if (occurrences2.Any<OccurrenceData>())
      {
        this.CheckProjectPermission(requestContext, occurrences2.First<OccurrenceData>().ScopeId, TeamProjectPermissions.GenericRead);
        foreach (OccurrenceData occurrenceData in occurrences2)
          occurrences1.Add(new Grafeas.V1.OccurrenceReference()
          {
            Name = occurrenceData.Name,
            Kind = occurrenceData.Kind,
            NoteName = occurrenceData.NoteName,
            ScopeId = occurrenceData.ScopeId
          });
      }
      return (IList<Grafeas.V1.OccurrenceReference>) occurrences1;
    }

    public IEnumerable<string> GetResourceIdsByKind(
      IVssRequestContext requestContext,
      ISet<string> resourceIds,
      NoteKind kind)
    {
      List<string> stringList = new List<string>();
      List<OccurrenceData> occurrencesByResources;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        occurrencesByResources = component.GetOccurrencesByResources((IEnumerable<string>) resourceIds, kind);
      return occurrencesByResources.Select<OccurrenceData, string>((Func<OccurrenceData, string>) (o => o.ResourceId));
    }

    public IEnumerable<Grafeas.V1.Occurrence> GetOccurrences(
      IVssRequestContext requestContext,
      IEnumerable<string> resourceUris,
      IEnumerable<NoteKind> kinds)
    {
      List<string> stringList = new List<string>();
      List<OccurrenceData> occurrences;
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        occurrences = component.GetOccurrences(resourceUris, kinds);
      ITeamFoundationFileService fileService = requestContext.GetService<ITeamFoundationFileService>();
      return occurrences.Select<OccurrenceData, Grafeas.V1.Occurrence>((Func<OccurrenceData, Grafeas.V1.Occurrence>) (occurrence => fileService.RetrieveFromFile<Grafeas.V1.Occurrence>(requestContext, occurrence.FileId.Value)));
    }

    public IEnumerable<string> GetResourceIdsByTag(IVssRequestContext requestContext, string tag)
    {
      using (ArtifactMetaDataSqlResourceComponent component = requestContext.CreateComponent<ArtifactMetaDataSqlResourceComponent>())
        return component.GetResourceIdsByTag(tag);
    }

    private void CheckProjectPermission(
      IVssRequestContext requestContext,
      Guid scopeId,
      int permission)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      string projectUri = ProjectInfo.GetProjectUri(scopeId);
      requestContext.GetService<IProjectService>().CheckProjectPermission(requestContext, projectUri, permission);
    }
  }
}
