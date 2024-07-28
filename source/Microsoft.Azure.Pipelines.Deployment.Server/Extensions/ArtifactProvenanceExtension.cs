// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Extensions.ArtifactProvenanceExtension
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Pipelines.Deployment.Extensions
{
  public static class ArtifactProvenanceExtension
  {
    public static IEnumerable<ArtifactProvenance> ToArtifactProvenances(
      this IEnumerable<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence> occurrences,
      IEnumerable<string> resourceUris)
    {
      List<ArtifactProvenance> artifactProvenances = new List<ArtifactProvenance>();
      if (occurrences == null || !occurrences.Any<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>() || resourceUris == null || !resourceUris.Any<string>())
        return (IEnumerable<ArtifactProvenance>) artifactProvenances;
      Dictionary<string, IEnumerable<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>> dictionary = new Dictionary<string, IEnumerable<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>>();
      foreach (string resourceUri1 in resourceUris)
      {
        string resourceUri = resourceUri1;
        dictionary[resourceUri] = occurrences.Where<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, bool>) (o => o.ResourceUri.Contains(resourceUri)));
      }
      foreach (KeyValuePair<string, IEnumerable<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>> keyValuePair in dictionary)
      {
        if (keyValuePair.Value.Any<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>())
          artifactProvenances.Add(ArtifactProvenanceExtension.ToArtifactProvenance(keyValuePair.Key, keyValuePair.Value));
      }
      return (IEnumerable<ArtifactProvenance>) artifactProvenances;
    }

    private static ArtifactProvenance ToArtifactProvenance(
      string image,
      IEnumerable<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence> occurrences)
    {
      return new ArtifactProvenance()
      {
        ResourceUri = image,
        Build = occurrences.FirstOrDefault<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, bool>) (o => o.Kind == Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Build)).ToWebApiBuild(),
        Image = occurrences.FirstOrDefault<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, bool>) (o => o.Kind == Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Image)).ToWebApiImage(),
        Deployment = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>) occurrences.Where<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, bool>) (o => o.Kind == Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Deployment)).Select<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>) (d => d.ToWebApiDeployment())).ToList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>(),
        Attestation = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>) occurrences.Where<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, bool>) (o => o.Kind == Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Attestation)).Select<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>) (a => a.ToWebApiAttestation())).ToList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>(),
        Vulnerability = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>) occurrences.Where<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, bool>) (o => o.Kind == Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Vulnerability)).Select<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>) (a => a.ToWebApiVulnerability())).ToList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence>()
      };
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence ToWebApiBuild(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence)
    {
      if (occurrence == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence webApiOccurrence = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence();
      occurrence.ToWebApiOccurrence(webApiOccurrence);
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.BuildOccurrence buildOccurrence = (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.BuildOccurrence) occurrence;
      webApiOccurrence.Build = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildOccurrence()
      {
        Provenance = buildOccurrence.Provenance.ToBuildProvenance()
      };
      return webApiOccurrence;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildProvenance ToBuildProvenance(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.BuildProvenance buildProvenance)
    {
      if (buildProvenance == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildProvenance) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildProvenance buildProvenance1 = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildProvenance()
      {
        BuilderVersion = buildProvenance.BuilderVersion,
        Id = buildProvenance.Id,
        ProjectId = buildProvenance.ProjectId,
        Creator = buildProvenance.Creator,
        LogsUri = buildProvenance.LogsUri,
        BuildOptions = buildProvenance.BuildOptions,
        BuildArtifacts = ArtifactProvenanceExtension.ToBuildArtifacts(buildProvenance.BuildArtifacts),
        CreateTime = buildProvenance.CreateTime,
        StartTime = buildProvenance.StartTime,
        SourceProvenance = new Source()
      };
      buildProvenance1.SourceProvenance.Context = ArtifactProvenanceExtension.ToSourceContext(buildProvenance.SourceProvenance?.Context);
      if (buildProvenance.SourceProvenance?.AdditionalContexts != null)
      {
        buildProvenance1.SourceProvenance.AdditionalContexts = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext>) new List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext>();
        foreach (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.SourceContext additionalContext in buildProvenance.SourceProvenance?.AdditionalContexts)
          buildProvenance1.SourceProvenance.AdditionalContexts.Add(ArtifactProvenanceExtension.ToSourceContext(additionalContext));
      }
      return buildProvenance1;
    }

    private static List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildArtifact> ToBuildArtifacts(
      List<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.BuildArtifact> artifacts)
    {
      if (artifacts == null)
        return (List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildArtifact>) null;
      List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildArtifact> buildArtifacts = new List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildArtifact>();
      foreach (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.BuildArtifact artifact in artifacts)
      {
        Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildArtifact buildArtifact = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildArtifact()
        {
          Checksum = artifact.Checksum,
          Id = artifact.Id,
          Names = artifact.Names
        };
        buildArtifacts.Add(buildArtifact);
      }
      return buildArtifacts;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext ToSourceContext(
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.SourceContext context)
    {
      if (context == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext sourceContext = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext()
      {
        Git = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.GitSourceContext()
      };
      sourceContext.Git.RevisionId = context?.Git?.RevisionId;
      sourceContext.Git.Url = context?.Git?.Url;
      return sourceContext;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind ToWebApiNoteKind(
      Microsoft.Azure.Pipelines.Deployment.Model.NoteKind kind)
    {
      switch (kind)
      {
        case Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Build:
          return Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind.Build;
        case Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Image:
          return Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind.Image;
        case Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Deployment:
          return Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind.Deployment;
        case Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Vulnerability:
          return Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind.Vulnerability;
        case Microsoft.Azure.Pipelines.Deployment.Model.NoteKind.Attestation:
          return Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind.Attestation;
        default:
          return Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind.Unspecified;
      }
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence ToWebApiImage(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence)
    {
      if (occurrence == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence webApiOccurrence = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence();
      occurrence.ToWebApiOccurrence(webApiOccurrence);
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageOccurrence imageOccurrence1 = (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageOccurrence) occurrence;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.ImageOccurrence imageOccurrence2 = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.ImageOccurrence();
      imageOccurrence2.BaseResourceUrl = imageOccurrence1.BaseResourceUrl;
      imageOccurrence2.Distance = imageOccurrence1.Distance.GetValueOrDefault();
      imageOccurrence2.Fingerprint = imageOccurrence1.Fingerprint.ToWebApiFingerprint();
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.ImageOccurrence imageOccurrence3 = imageOccurrence2;
      List<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageLayer> layerInfo = imageOccurrence1.LayerInfo;
      List<Layer> layerList = (layerInfo != null ? layerInfo.Select<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageLayer, Layer>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageLayer, Layer>) (l => l.ToWebApiLayerInfo())).ToList<Layer>() : (List<Layer>) null) ?? (List<Layer>) null;
      imageOccurrence3.LayerInfo = layerList;
      webApiOccurrence.Image = imageOccurrence2;
      return webApiOccurrence;
    }

    private static Fingerprint ToWebApiFingerprint(this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageFingerprint imageFingerprint)
    {
      if (imageFingerprint == null)
        return (Fingerprint) null;
      return new Fingerprint()
      {
        V1Name = imageFingerprint.V1Name,
        V2Blob = imageFingerprint.V2Blob,
        V2Name = imageFingerprint.V2Name
      };
    }

    private static Layer ToWebApiLayerInfo(this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.ImageLayer imageLayer)
    {
      if (imageLayer == null)
        return (Layer) null;
      return new Layer()
      {
        Arguments = imageLayer.Arguments,
        Directive = imageLayer.Directive
      };
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence ToWebApiDeployment(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence)
    {
      if (occurrence == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence webApiOccurrence = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence();
      occurrence.ToWebApiOccurrence(webApiOccurrence);
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.DeploymentOccurrence deploymentOccurrence = (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.DeploymentOccurrence) occurrence;
      webApiOccurrence.Deployment = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.DeploymentOccurrence()
      {
        Address = deploymentOccurrence.Address,
        Config = deploymentOccurrence.Config,
        DeployTime = deploymentOccurrence.DeployTime,
        Platform = (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Platform) deploymentOccurrence.Platform,
        UndeployTime = deploymentOccurrence.UnDeployTime,
        UserEmail = deploymentOccurrence.UserEmail,
        ResourceUri = (IList<string>) deploymentOccurrence.ResourceUrl
      };
      return webApiOccurrence;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence ToWebApiAttestation(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence)
    {
      if (occurrence == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence webApiOccurrence = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence();
      occurrence.ToWebApiOccurrence(webApiOccurrence);
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.AttestationOccurrence attestationOccurrence1 = (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.AttestationOccurrence) occurrence;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.AttestationOccurrence attestationOccurrence2 = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.AttestationOccurrence();
      attestationOccurrence2.SerializedPayload = attestationOccurrence1.SerializedPayload;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.AttestationOccurrence attestationOccurrence3 = attestationOccurrence2;
      List<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Signature> signatures = attestationOccurrence1.Signatures;
      List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature> signatureList = (signatures != null ? signatures.Select<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Signature, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Signature, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature>) (s => s.ToWebApiSignature())).ToList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature>() : (List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature>) null) ?? (List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature>) null;
      attestationOccurrence3.Signatures = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature>) signatureList;
      webApiOccurrence.Attestation = attestationOccurrence2;
      return webApiOccurrence;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature ToWebApiSignature(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Signature signature)
    {
      if (signature == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature) null;
      return new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Signature()
      {
        PublicKeyId = signature.PublicKeyId,
        SignatureContent = Encoding.UTF8.GetBytes(signature.SignatureContent)
      };
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence ToWebApiVulnerability(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence)
    {
      if (occurrence == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence webApiOccurrence = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence();
      occurrence.ToWebApiOccurrence(webApiOccurrence);
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.VulnerabilityOccurrence vulnerabilityOccurrence1 = (Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.VulnerabilityOccurrence) occurrence;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.VulnerabilityOccurrence vulnerabilityOccurrence2 = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.VulnerabilityOccurrence();
      vulnerabilityOccurrence2.Type = vulnerabilityOccurrence1.Type;
      vulnerabilityOccurrence2.ShortDescription = vulnerabilityOccurrence1.ShortDescription;
      vulnerabilityOccurrence2.Severity = (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Severity) vulnerabilityOccurrence1.Severity;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.VulnerabilityOccurrence vulnerabilityOccurrence3 = vulnerabilityOccurrence2;
      List<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.RelatedUrl> relatedUrls = vulnerabilityOccurrence1.RelatedUrls;
      List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl> relatedUrlList = (relatedUrls != null ? relatedUrls.Select<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.RelatedUrl, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl>((Func<Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.RelatedUrl, Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl>) (r => r.ToWebApiRelatedUrl())).ToList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl>() : (List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl>) null) ?? (List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl>) null;
      vulnerabilityOccurrence3.RelatedUrls = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl>) relatedUrlList;
      vulnerabilityOccurrence2.PackageIssue = (IList<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.PackageIssue>) null;
      vulnerabilityOccurrence2.LongDescription = vulnerabilityOccurrence1.LongDescription;
      vulnerabilityOccurrence2.FixAvailable = vulnerabilityOccurrence1.FixAvailable;
      vulnerabilityOccurrence2.EffectiveSeverity = (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Severity) vulnerabilityOccurrence1.EffectiveSeverity;
      webApiOccurrence.Vulnerability = vulnerabilityOccurrence2;
      return webApiOccurrence;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl ToWebApiRelatedUrl(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.RelatedUrl relatedUrl)
    {
      if (relatedUrl == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl) null;
      return new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RelatedUrl()
      {
        Label = relatedUrl.Label,
        Url = relatedUrl.Url
      };
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.PackageIssue ToWebApiPackageIssue(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.PackageIssue packageIssue)
    {
      if (packageIssue == null)
        return (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.PackageIssue) null;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.PackageIssue webApiPackageIssue = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.PackageIssue();
      webApiPackageIssue.AffectedCpeUri = packageIssue.AffectedCpeUri;
      webApiPackageIssue.AffectedPackage = packageIssue.AffectedPackage;
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.PackageVersion affectedVersion = packageIssue.AffectedVersion;
      webApiPackageIssue.AffectedVersion = affectedVersion != null ? affectedVersion.ToWebApiPackageVersion() : (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Version) null;
      webApiPackageIssue.FixAvailable = packageIssue.FixAvailable;
      webApiPackageIssue.FixedCpeUri = packageIssue.FixedCpeUri;
      Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.PackageVersion fixedVersion = packageIssue.FixedVersion;
      webApiPackageIssue.FixedVersion = fixedVersion != null ? fixedVersion.ToWebApiPackageVersion() : (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Version) null;
      webApiPackageIssue.FixedPackage = packageIssue.FixedPackage;
      return webApiPackageIssue;
    }

    private static Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Version ToWebApiPackageVersion(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.PackageVersion packageIssue)
    {
      return new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Version()
      {
        Epoch = packageIssue.Epoch,
        FullName = packageIssue.FullName,
        Kind = (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.VersionKind) packageIssue.Kind,
        Name = packageIssue.Name,
        Revision = packageIssue.Revision
      };
    }

    private static void ToWebApiOccurrence(
      this Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence,
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence webApiOccurrence)
    {
      webApiOccurrence.Kind = ArtifactProvenanceExtension.ToWebApiNoteKind(occurrence.Kind);
      webApiOccurrence.Name = occurrence.Name;
      webApiOccurrence.NoteName = ArtifactProvenanceExtension.GetWebApiNoteName(occurrence);
      webApiOccurrence.ResourceUri = occurrence.ResourceUri;
      DateTime? nullable;
      if (occurrence.CreateTime.HasValue && occurrence.CreateTime.HasValue)
      {
        Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence occurrence1 = webApiOccurrence;
        nullable = occurrence.CreateTime;
        DateTime dateTime = nullable.Value;
        occurrence1.CreateTime = dateTime;
      }
      nullable = occurrence.UpdateTime;
      if (!nullable.HasValue)
        return;
      nullable = occurrence.UpdateTime;
      if (!nullable.HasValue)
        return;
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence occurrence2 = webApiOccurrence;
      nullable = occurrence.UpdateTime;
      DateTime dateTime1 = nullable.Value;
      occurrence2.UpdateTime = dateTime1;
    }

    private static string GetWebApiNoteName(Microsoft.Azure.Pipelines.Deployment.Model.Grafeas.V1.Occurrence occurrence)
    {
      if (string.IsNullOrWhiteSpace(occurrence.NoteName))
        return string.Empty;
      Guid scopeId = occurrence.ScopeId;
      return occurrence.ScopeId == Guid.Empty ? occurrence.NoteName : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "projects/{0}/notes/{1}", (object) occurrence.ScopeId.ToString(), (object) occurrence.NoteName);
    }
  }
}
