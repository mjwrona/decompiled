// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.AttestationDetailsService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal sealed class AttestationDetailsService : BaseMetadataDetailsService
  {
    public override void AddMetadataDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      BaseMetadataDetails details)
    {
      this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);
      if (!ArtifactMetadataHelper.IsPublishPipelineMetadataEnabled(requestContext, projectId))
        return;
      AttestationDetails attestationDetails = (AttestationDetails) details;
      IArtifactMetadataService service = requestContext.GetService<IArtifactMetadataService>();
      Grafeas.V1.Note attestationNote = attestationDetails.ToAttestationNote(projectId);
      try
      {
        service.CreateNote(requestContext, attestationNote);
      }
      catch (NoteExistsException ex)
      {
      }
      try
      {
        Grafeas.V1.Occurrence attestationOccurrence = attestationDetails.ToAttestationOccurrence(projectId);
        service.CreateOccurrence(requestContext, attestationOccurrence);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(100161003, "Deployment", "Service", ex);
        throw;
      }
    }
  }
}
