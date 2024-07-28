// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.CustomerIntelligenceHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public static class CustomerIntelligenceHelper
  {
    private const string ImageDetailsAreaName = "ImageDetailsService";
    private const string ImageDetailsLayer = "ImageDetails";
    private const string ImageNameKey = "ImageName";
    private const string PipelineIdKey = "PipelineId";
    private const string OccurrenceNameKey = " OccurrenceName";
    private const string OccurrenceNoteNameKey = " OccurrenceNoteName";
    private const string OccurrenceResourceNameKey = "OccurrenceResourceName";
    private const string OccurrenceKindKey = "OccurrenceKind";
    private const string NoteNameKey = "NoteName";
    private const string NoteKindKey = "NoteKind";
    private const string ExceptionMessageKey = "ExceptionMessage";
    private const string CreateNoteFeatureName = "CreateNoteEvent";
    private const string CreateNoteFailedFeatureName = "CreateNoteFailedEvent";
    private const string CreateOccurenceFeatureName = "CreateOccurenceEvent";
    private const string CreateOccurenceFailedFeatureName = "CreateOccurenceFailedEvent";
    private const string AddImageDetailsFeatureName = "AddImageDetailsEvent";
    private const string AddImageDetailsFailedFeatureName = "AddImageDetailsFailedEvent";

    public static void PublishImageDetailsAddedEvent(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails)
    {
      if (imageDetails == null)
        throw new ArgumentNullException(nameof (imageDetails));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (Action) (() =>
      {
        CustomerIntelligenceData imageDetailsCiData = CustomerIntelligenceHelper.GetImageDetailsCIData(imageDetails);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, requestContext.GetUserId(), requestContext.GetUserCuid(), "AddImageDetailsEvent", imageDetailsCiData);
      }), 100161004, "ImageDetails");
    }

    public static void PublishImageDetailsFailedEvent(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails,
      Exception ex)
    {
      if (imageDetails == null)
        throw new ArgumentNullException(nameof (imageDetails));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (Action) (() =>
      {
        CustomerIntelligenceData imageDetailsCiData = CustomerIntelligenceHelper.GetImageDetailsCIData(imageDetails);
        imageDetailsCiData.Add("ExceptionMessage", ex.Message);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, requestContext.GetUserId(), requestContext.GetUserCuid(), "AddImageDetailsFailedEvent", imageDetailsCiData);
      }), 100161005, "ImageDetails");
    }

    public static void PublishPipelineResourcesUsage(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      PipelineResources resources = null)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      if (!service.IsTracingEnabled(requestContext))
        return;
      if (resources == null)
      {
        CustomerIntelligenceData propertiesForPipeline = CustomerIntelligenceHelper.GetBasePropertiesForPipeline(projectId, definitionId);
        service.Publish(requestContext, "PipelineArtifacts", "PipelineResourcesUsage", propertiesForPipeline);
      }
      foreach (PipelineResource pipelineResource in (IEnumerable<PipelineResource>) (resources?.Pipelines ?? (ISet<PipelineResource>) new HashSet<PipelineResource>()))
      {
        CustomerIntelligenceData resourceProperties = CustomerIntelligenceHelper.GetResourceProperties(projectId, definitionId, (Resource) pipelineResource, (PipelineTrigger) pipelineResource.Trigger, "PipelineResource", "Pipeline");
        service.Publish(requestContext, "PipelineArtifacts", "PipelineResourcesUsage", resourceProperties);
      }
      foreach (BuildResource buildResource in (IEnumerable<BuildResource>) (resources?.Builds ?? (ISet<BuildResource>) new HashSet<BuildResource>()))
      {
        CustomerIntelligenceData resourceProperties = CustomerIntelligenceHelper.GetResourceProperties(projectId, definitionId, (Resource) buildResource, (PipelineTrigger) buildResource.Trigger, "BuildResource", buildResource.Type);
        service.Publish(requestContext, "PipelineArtifacts", "PipelineResourcesUsage", resourceProperties);
      }
      foreach (ContainerResource containerResource in (IEnumerable<ContainerResource>) (resources?.Containers ?? (ISet<ContainerResource>) new HashSet<ContainerResource>()))
      {
        string type = containerResource.Properties.Get<string>("type");
        CustomerIntelligenceData resourceProperties = CustomerIntelligenceHelper.GetResourceProperties(projectId, definitionId, (Resource) containerResource, (PipelineTrigger) containerResource.Trigger, "ContainerResource", type);
        service.Publish(requestContext, "PipelineArtifacts", "PipelineResourcesUsage", resourceProperties);
      }
      foreach (PackageResource packageResource in (IEnumerable<PackageResource>) (resources?.Packages ?? (ISet<PackageResource>) new HashSet<PackageResource>()))
      {
        CustomerIntelligenceData resourceProperties = CustomerIntelligenceHelper.GetResourceProperties(projectId, definitionId, (Resource) packageResource, (PipelineTrigger) packageResource.Trigger, "PackageResource", packageResource.Type);
        service.Publish(requestContext, "PipelineArtifacts", "PipelineResourcesUsage", resourceProperties);
      }
    }

    private static CustomerIntelligenceData GetResourceProperties(
      Guid projectId,
      int definitionId,
      Resource resource,
      PipelineTrigger trigger,
      string category,
      string type)
    {
      CustomerIntelligenceData propertiesForPipeline = CustomerIntelligenceHelper.GetBasePropertiesForPipeline(projectId, definitionId);
      propertiesForPipeline.Add("resourceAlias", resource.Alias);
      propertiesForPipeline.Add("triggerEnabled", trigger != null && trigger.Enabled);
      propertiesForPipeline.Add(nameof (category), category);
      propertiesForPipeline.Add("artifactType", type);
      return propertiesForPipeline;
    }

    private static CustomerIntelligenceData GetBasePropertiesForPipeline(
      Guid projectId,
      int definitionId)
    {
      CustomerIntelligenceData propertiesForPipeline = new CustomerIntelligenceData();
      propertiesForPipeline.Add(PipelinePropertyNames.ProjectId, (object) projectId);
      propertiesForPipeline.Add(PipelinePropertyNames.DefinitionId, (double) definitionId);
      return propertiesForPipeline;
    }

    public static void PublishNoteCreatedEvent(
      IVssRequestContext requestContext,
      Grafeas.V1.Note note)
    {
      if (note == null)
        throw new ArgumentNullException(nameof (note));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (Action) (() =>
      {
        CustomerIntelligenceData noteCiData = CustomerIntelligenceHelper.GetNoteCIData(note);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, requestContext.GetUserId(), requestContext.GetUserCuid(), "CreateNoteEvent", noteCiData);
      }), 100161000, "ImageDetails");
    }

    public static void PublishNoteCreatedFailedEvent(
      IVssRequestContext requestContext,
      Grafeas.V1.Note note,
      Exception ex)
    {
      if (note == null)
        throw new ArgumentNullException(nameof (note));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (Action) (() =>
      {
        CustomerIntelligenceData noteCiData = CustomerIntelligenceHelper.GetNoteCIData(note);
        noteCiData.Add("ExceptionMessage", ex.Message);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, requestContext.GetUserId(), requestContext.GetUserCuid(), "CreateNoteFailedEvent", noteCiData);
      }), 100161001, "ImageDetails");
    }

    public static void PublishOccurenceCreatedEvent(
      IVssRequestContext requestContext,
      Grafeas.V1.Occurrence occurrence)
    {
      if (occurrence == null)
        throw new ArgumentNullException(nameof (occurrence));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (Action) (() =>
      {
        CustomerIntelligenceData occurrenceCiData = CustomerIntelligenceHelper.GetOccurrenceCIData(occurrence);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, requestContext.GetUserId(), requestContext.GetUserCuid(), "CreateOccurenceEvent", occurrenceCiData);
      }), 100161002, "ImageDetails");
    }

    public static void PublishOccurenceCreatedFailedEvent(
      IVssRequestContext requestContext,
      Grafeas.V1.Occurrence occurrence,
      Exception ex)
    {
      if (occurrence == null)
        throw new ArgumentNullException(nameof (occurrence));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (Action) (() =>
      {
        CustomerIntelligenceData occurrenceCiData = CustomerIntelligenceHelper.GetOccurrenceCIData(occurrence);
        occurrenceCiData.Add("ExceptionMessage", ex.Message);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, requestContext.GetUserId(), requestContext.GetUserCuid(), "CreateOccurenceFailedEvent", occurrenceCiData);
      }), 100161003, "ImageDetails");
    }

    private static void SafeExecute(
      IVssRequestContext requestContext,
      Action publishAction,
      int tracepoint,
      string layer)
    {
      try
      {
        publishAction();
      }
      catch (Exception ex)
      {
        requestContext.Trace(tracepoint, TraceLevel.Error, "ImageDetailsService", layer, "Failed to publish customer intellligence data. Exception {0}", (object) ex);
      }
    }

    private static void PublishCustomerIntelligence(
      IVssRequestContext requestContext,
      string userDisplayName,
      Guid userIdentityId,
      Guid identityConsistentVSID,
      string feature,
      CustomerIntelligenceData intelligenceData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, requestContext.ServiceHost.InstanceId, userDisplayName, userIdentityId, identityConsistentVSID, DateTime.UtcNow, "ImageDetailsService", feature, intelligenceData);
    }

    private static CustomerIntelligenceData GetImageDetailsCIData(Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails)
    {
      CustomerIntelligenceData imageDetailsCiData = new CustomerIntelligenceData();
      imageDetailsCiData.Add("ImageName", imageDetails.ImageName);
      imageDetailsCiData.Add("PipelineId", imageDetails.PipelineId);
      return imageDetailsCiData;
    }

    private static CustomerIntelligenceData GetNoteCIData(Grafeas.V1.Note note)
    {
      CustomerIntelligenceData noteCiData = new CustomerIntelligenceData();
      noteCiData.Add("NoteName", note.Name);
      noteCiData.Add("NoteKind", (object) note.Kind);
      return noteCiData;
    }

    private static CustomerIntelligenceData GetOccurrenceCIData(Grafeas.V1.Occurrence occurrence)
    {
      CustomerIntelligenceData occurrenceCiData = new CustomerIntelligenceData();
      occurrenceCiData.Add(" OccurrenceName", occurrence.Name);
      occurrenceCiData.Add(" OccurrenceNoteName", occurrence.NoteName);
      occurrenceCiData.Add("OccurrenceResourceName", occurrence.ResourceUri);
      occurrenceCiData.Add("OccurrenceKind", (object) occurrence.Kind);
      return occurrenceCiData;
    }
  }
}
