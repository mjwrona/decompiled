// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.ImageDetailsExtensionsV1Beta
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal static class ImageDetailsExtensionsV1Beta
  {
    public static List<Grafeas.V1Beta.Note> ToV1BetaNotes(
      this Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails>(imageDetails, nameof (imageDetails));
      List<Grafeas.V1Beta.Note> v1BetaNotes = new List<Grafeas.V1Beta.Note>();
      DateTime utcNow = DateTime.UtcNow;
      Grafeas.V1Beta.BuildNote buildNote1 = new Grafeas.V1Beta.BuildNote();
      buildNote1.Name = imageDetails.PipelineId;
      buildNote1.ScopeId = projectId;
      buildNote1.Kind = NoteKind.Build;
      buildNote1.CreateTime = new DateTime?(utcNow);
      buildNote1.UpdateTime = new DateTime?(utcNow);
      Grafeas.V1Beta.BuildNote buildNote2 = buildNote1;
      Grafeas.V1Beta.ImageBasisNote imageBasisNote1 = new Grafeas.V1Beta.ImageBasisNote();
      imageBasisNote1.Name = imageDetails.BaseImageName;
      imageBasisNote1.ScopeId = projectId;
      imageBasisNote1.Kind = NoteKind.Image;
      imageBasisNote1.ResourceUrl = imageDetails.BaseImageUri;
      imageBasisNote1.CreateTime = new DateTime?(utcNow);
      imageBasisNote1.UpdateTime = new DateTime?(utcNow);
      Grafeas.V1Beta.ImageBasisNote imageBasisNote2 = imageBasisNote1;
      v1BetaNotes.Add((Grafeas.V1Beta.Note) buildNote2);
      v1BetaNotes.Add((Grafeas.V1Beta.Note) imageBasisNote2);
      return v1BetaNotes;
    }

    public static List<Grafeas.V1Beta.Occurrence> ToV1BetaOccurrences(
      this Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails>(imageDetails, nameof (imageDetails));
      ArgumentUtility.CheckForNull<List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer>>(imageDetails.LayerInfo, "LayerInfo");
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) imageDetails.LayerInfo, "LayerInfo");
      List<Grafeas.V1Beta.Occurrence> v1BetaOccurrences = new List<Grafeas.V1Beta.Occurrence>();
      Grafeas.V1Beta.ArtifactResource artifactResource = new Grafeas.V1Beta.ArtifactResource()
      {
        Name = imageDetails.ImageName,
        Uri = imageDetails.ImageUri,
        ContentHash = new Grafeas.V1Beta.ProvenanceHash()
        {
          Type = new Grafeas.V1Beta.HashType?(Grafeas.V1Beta.HashType.SHA256),
          Value = imageDetails.Hash
        }
      };
      DateTime utcNow = DateTime.UtcNow;
      Grafeas.V1Beta.BuildProvenance buildProvenance = new Grafeas.V1Beta.BuildProvenance()
      {
        Id = imageDetails.PipelineId.ToString(),
        BuilderVersion = imageDetails.PipelineVersion
      };
      Grafeas.V1Beta.BuildDetails buildDetails1 = new Grafeas.V1Beta.BuildDetails();
      buildDetails1.ScopeId = projectId;
      buildDetails1.Name = Guid.NewGuid().ToString();
      buildDetails1.Kind = NoteKind.Build;
      buildDetails1.NoteName = imageDetails.PipelineId;
      buildDetails1.Resource = artifactResource;
      buildDetails1.CreateTime = new DateTime?(utcNow);
      buildDetails1.UpdateTime = new DateTime?(utcNow);
      buildDetails1.Provenance = buildProvenance;
      Grafeas.V1Beta.BuildDetails buildDetails2 = buildDetails1;
      List<Grafeas.V1Beta.ImageLayer> imageLayerList = new List<Grafeas.V1Beta.ImageLayer>();
      foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer imageLayer1 in imageDetails.LayerInfo)
      {
        Grafeas.V1Beta.LayerDirective result;
        if (!Enum.TryParse<Grafeas.V1Beta.LayerDirective>(imageLayer1.Directive, out result))
          result = Grafeas.V1Beta.LayerDirective.DIRECTIVEUNSPECIFIED;
        Grafeas.V1Beta.ImageLayer imageLayer2 = new Grafeas.V1Beta.ImageLayer()
        {
          Arguments = imageLayer1.Arguments,
          Directive = new Grafeas.V1Beta.LayerDirective?(result),
          Size = imageLayer1.Size,
          CreatedOn = imageLayer1.CreatedOn
        };
        imageLayerList.Add(imageLayer2);
      }
      Grafeas.V1Beta.ImageDerived imageDerived = new Grafeas.V1Beta.ImageDerived()
      {
        ImageType = imageDetails.ImageType,
        MediaType = imageDetails.MediaType,
        BaseResourceUrl = imageDetails.BaseImageUri,
        Distance = imageDetails.Distance,
        LayerInfo = imageLayerList,
        Tags = imageDetails.Tags,
        ImageSize = imageDetails.ImageSize,
        JobName = imageDetails.JobName,
        PipelineId = imageDetails.PipelineId,
        RunId = imageDetails.RunId,
        PipelineVersion = imageDetails.PipelineVersion,
        PipelineName = imageDetails.PipelineName
      };
      Grafeas.V1Beta.ImageDetails imageDetails1 = new Grafeas.V1Beta.ImageDetails();
      imageDetails1.ScopeId = projectId;
      imageDetails1.Name = Guid.NewGuid().ToString();
      imageDetails1.Kind = NoteKind.Image;
      imageDetails1.NoteName = imageDetails.BaseImageName;
      imageDetails1.Resource = artifactResource;
      imageDetails1.CreateTime = new DateTime?(utcNow);
      imageDetails1.UpdateTime = new DateTime?(utcNow);
      imageDetails1.DerivedImage = imageDerived;
      Grafeas.V1Beta.ImageDetails imageDetails2 = imageDetails1;
      v1BetaOccurrences.Add((Grafeas.V1Beta.Occurrence) buildDetails2);
      v1BetaOccurrences.Add((Grafeas.V1Beta.Occurrence) imageDetails2);
      return v1BetaOccurrences;
    }
  }
}
