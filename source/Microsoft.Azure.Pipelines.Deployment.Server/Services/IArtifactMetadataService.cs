// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.IArtifactMetadataService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  [DefaultServiceImplementation(typeof (ArtifactMetadataService))]
  public interface IArtifactMetadataService : IVssFrameworkService
  {
    Grafeas.V1.Note CreateNote(IVssRequestContext requestContext, Grafeas.V1.Note note);

    Grafeas.V1.Occurrence CreateOccurrence(
      IVssRequestContext requestContext,
      Grafeas.V1.Occurrence occurrence);

    IList<string> CreateOccurrenceTag(
      IVssRequestContext requestContext,
      string occurrenceName,
      string tag);

    void DeleteOccurrenceTag(IVssRequestContext requestContext, string occurrenceName, string tag);

    Grafeas.V1.Note GetNote(IVssRequestContext requestContext, string name);

    IList<Grafeas.V1.OccurrenceReference> GetNoteOccurrences(
      IVssRequestContext requestContext,
      string noteName);

    Grafeas.V1.Occurrence GetOccurrence(IVssRequestContext requestContext, string name);

    IList<Grafeas.V1.OccurrenceReference> GetOccurrences(
      IVssRequestContext requestContext,
      string resourceId);

    IEnumerable<string> GetResourceIdsByKind(
      IVssRequestContext requestContext,
      ISet<string> resourceIds,
      NoteKind kind);

    IEnumerable<string> GetResourceIdsByTag(IVssRequestContext requestContext, string tag);

    IEnumerable<Grafeas.V1.Occurrence> GetOccurrences(
      IVssRequestContext requestContext,
      IEnumerable<string> resourceUris,
      IEnumerable<NoteKind> kinds);
  }
}
