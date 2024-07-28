// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.MetadataDetailsExtensions
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public static class MetadataDetailsExtensions
  {
    private static readonly Regex ImageNameRegex = new Regex("(?<=https:\\/\\/)(.*)(?=@sha256)", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1.0));

    public static List<Grafeas.V1.Note> ToBuildAndImageNotes(
      this Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails>(imageDetails, nameof (imageDetails));
      List<Grafeas.V1.Note> buildAndImageNotes = new List<Grafeas.V1.Note>();
      DateTime utcNow = DateTime.UtcNow;
      Grafeas.V1.BuildNote buildNote1 = new Grafeas.V1.BuildNote();
      buildNote1.Name = imageDetails.PipelineId;
      buildNote1.ScopeId = projectId;
      buildNote1.Kind = NoteKind.Build;
      buildNote1.CreateTime = new DateTime?(utcNow);
      buildNote1.UpdateTime = new DateTime?(utcNow);
      Grafeas.V1.BuildNote buildNote2 = buildNote1;
      Grafeas.V1.ImageNote imageNote1 = new Grafeas.V1.ImageNote();
      imageNote1.Name = imageDetails.BaseImageName;
      imageNote1.ScopeId = projectId;
      imageNote1.Kind = NoteKind.Image;
      imageNote1.ResourceUrl = imageDetails.BaseImageUri;
      imageNote1.CreateTime = new DateTime?(utcNow);
      imageNote1.UpdateTime = new DateTime?(utcNow);
      Grafeas.V1.ImageNote imageNote2 = imageNote1;
      buildAndImageNotes.Add((Grafeas.V1.Note) buildNote2);
      buildAndImageNotes.Add((Grafeas.V1.Note) imageNote2);
      return buildAndImageNotes;
    }

    public static List<Grafeas.V1.Occurrence> ToBuildAndImageOccurrences(
      this Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails>(imageDetails, nameof (imageDetails));
      ArgumentUtility.CheckForNull<List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer>>(imageDetails.LayerInfo, "LayerInfo");
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) imageDetails.LayerInfo, "LayerInfo");
      List<Grafeas.V1.Occurrence> imageOccurrences = new List<Grafeas.V1.Occurrence>();
      DateTime utcNow = DateTime.UtcNow;
      Grafeas.V1.GitSourceContext gitSourceContext = new Grafeas.V1.GitSourceContext()
      {
        Url = imageDetails.ContextUrl,
        RevisionId = imageDetails.RevisionId
      };
      Grafeas.V1.SourceContext sourceContext = new Grafeas.V1.SourceContext()
      {
        Git = gitSourceContext
      };
      Grafeas.V1.SourceProvenance sourceProvenance = new Grafeas.V1.SourceProvenance()
      {
        ArtifactStorageSourceUri = imageDetails.ArtifactStorageSourceUri,
        Context = sourceContext
      };
      Grafeas.V1.BuildArtifact buildArtifact = new Grafeas.V1.BuildArtifact()
      {
        Checksum = imageDetails.Hash != null ? Convert.ToBase64String(imageDetails.Hash) : "",
        Id = imageDetails.ImageUri,
        Names = MetadataDetailsExtensions.GetImageNamesWithTags(imageDetails.ImageUri, imageDetails.Tags)
      };
      Grafeas.V1.BuildProvenance buildProvenance = new Grafeas.V1.BuildProvenance()
      {
        Id = imageDetails.PipelineId.ToString(),
        BuilderVersion = imageDetails.PipelineVersion,
        ProjectId = projectId.ToString(),
        Creator = imageDetails.Creator,
        LogsUri = imageDetails.LogsUri,
        BuildOptions = imageDetails.BuildOptions,
        BuildArtifacts = new List<Grafeas.V1.BuildArtifact>()
        {
          buildArtifact
        },
        SourceProvenance = sourceProvenance,
        CreateTime = utcNow,
        StartTime = utcNow
      };
      Grafeas.V1.BuildOccurrence buildOccurrence1 = new Grafeas.V1.BuildOccurrence();
      buildOccurrence1.ScopeId = projectId;
      buildOccurrence1.Name = Guid.NewGuid().ToString();
      buildOccurrence1.Kind = NoteKind.Build;
      buildOccurrence1.NoteName = imageDetails.PipelineId;
      buildOccurrence1.ResourceUri = imageDetails.ImageUri;
      buildOccurrence1.CreateTime = new DateTime?(utcNow);
      buildOccurrence1.UpdateTime = new DateTime?(utcNow);
      buildOccurrence1.Provenance = buildProvenance;
      Grafeas.V1.BuildOccurrence buildOccurrence2 = buildOccurrence1;
      List<Grafeas.V1.ImageLayer> imageLayerList = new List<Grafeas.V1.ImageLayer>();
      foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer imageLayer1 in imageDetails.LayerInfo)
      {
        Grafeas.V1.ImageLayer imageLayer2 = new Grafeas.V1.ImageLayer()
        {
          Arguments = imageLayer1.Arguments,
          Directive = string.IsNullOrWhiteSpace(imageLayer1.Directive) ? "UNSPECIFIED" : imageLayer1.Directive,
          Size = imageLayer1.Size,
          CreatedOn = imageLayer1.CreatedOn
        };
        imageLayerList.Add(imageLayer2);
      }
      imageDetails.Tags = MetadataDetailsExtensions.AddRunIdTag(imageDetails.Tags, projectId, imageDetails.RunId);
      Grafeas.V1.ImageFingerprint imageFingerprint1 = new Grafeas.V1.ImageFingerprint();
      if (imageDetails.ImageFingerprint != null)
      {
        Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageFingerprint imageFingerprint2 = imageDetails.ImageFingerprint;
        imageFingerprint1.V1Name = imageFingerprint2.V1Name;
        imageFingerprint1.V2Blob = imageFingerprint2.V2Blobs;
        imageFingerprint1.V2Name = imageFingerprint2.V2Name;
      }
      Grafeas.V1.ImageOccurrence imageOccurrence1 = new Grafeas.V1.ImageOccurrence();
      imageOccurrence1.ScopeId = projectId;
      imageOccurrence1.Name = Guid.NewGuid().ToString();
      imageOccurrence1.Kind = NoteKind.Image;
      imageOccurrence1.NoteName = imageDetails.BaseImageName;
      imageOccurrence1.ResourceUri = imageDetails.ImageUri;
      imageOccurrence1.CreateTime = new DateTime?(utcNow);
      imageOccurrence1.UpdateTime = new DateTime?(utcNow);
      imageOccurrence1.ImageType = imageDetails.ImageType;
      imageOccurrence1.MediaType = imageDetails.MediaType;
      imageOccurrence1.BaseResourceUrl = imageDetails.BaseImageUri;
      imageOccurrence1.Distance = imageDetails.Distance;
      imageOccurrence1.LayerInfo = imageLayerList;
      imageOccurrence1.Tags = imageDetails.Tags;
      imageOccurrence1.ImageSize = imageDetails.ImageSize;
      imageOccurrence1.JobName = imageDetails.JobName;
      imageOccurrence1.PipelineId = imageDetails.PipelineId;
      imageOccurrence1.RunId = imageDetails.RunId;
      imageOccurrence1.PipelineVersion = imageDetails.PipelineVersion;
      imageOccurrence1.PipelineName = imageDetails.PipelineName;
      imageOccurrence1.Fingerprint = imageFingerprint1;
      imageOccurrence1.RepositoryId = imageDetails.RepositoryId;
      imageOccurrence1.RepositoryName = imageDetails.RepositoryName;
      imageOccurrence1.RepositoryTypeName = imageDetails.RepositoryTypeName;
      imageOccurrence1.Branch = imageDetails.Branch;
      Grafeas.V1.ImageOccurrence imageOccurrence2 = imageOccurrence1;
      imageOccurrences.Add((Grafeas.V1.Occurrence) buildOccurrence2);
      imageOccurrences.Add((Grafeas.V1.Occurrence) imageOccurrence2);
      return imageOccurrences;
    }

    public static Grafeas.V1.Note ToAttestationNote(
      this AttestationDetails attestationDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<AttestationDetails>(attestationDetails, nameof (attestationDetails));
      DateTime utcNow = DateTime.UtcNow;
      Grafeas.V1.Hint hint = new Grafeas.V1.Hint()
      {
        HumanReadableName = attestationDetails.HumanReadableName
      };
      Grafeas.V1.AttestationNote attestationNote = new Grafeas.V1.AttestationNote();
      attestationNote.Name = attestationDetails.Name;
      attestationNote.ScopeId = projectId;
      attestationNote.Kind = NoteKind.Attestation;
      attestationNote.CreateTime = new DateTime?(utcNow);
      attestationNote.UpdateTime = new DateTime?(utcNow);
      attestationNote.Hint = hint;
      attestationNote.ShortDescription = attestationDetails.Description;
      attestationNote.LongDescription = attestationDetails.Description;
      attestationNote.RelatedUrl = attestationDetails.RelatedUrl.ToRelatedUrl();
      return (Grafeas.V1.Note) attestationNote;
    }

    public static Grafeas.V1.Occurrence ToAttestationOccurrence(
      this AttestationDetails attestationDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<AttestationDetails>(attestationDetails, nameof (attestationDetails));
      ArgumentUtility.CheckForNull<string>(attestationDetails.SerializedPayload, "SerializedPayload");
      DateTime utcNow = DateTime.UtcNow;
      List<Grafeas.V1.Signature> signatureList = new List<Grafeas.V1.Signature>();
      if (attestationDetails.Signatures != null)
      {
        foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Signature signature1 in attestationDetails.Signatures)
        {
          Grafeas.V1.Signature signature2 = new Grafeas.V1.Signature()
          {
            SignatureContent = signature1.SignatureContent,
            PublicKeyId = signature1.PublicKeyId
          };
          signatureList.Add(signature2);
        }
      }
      Grafeas.V1.AttestationOccurrence attestationOccurrence = new Grafeas.V1.AttestationOccurrence();
      attestationOccurrence.ScopeId = projectId;
      attestationOccurrence.Name = Guid.NewGuid().ToString();
      attestationOccurrence.Kind = NoteKind.Attestation;
      attestationOccurrence.NoteName = attestationDetails.Name;
      attestationOccurrence.ResourceUri = attestationDetails.ResourceUri[0];
      attestationOccurrence.CreateTime = new DateTime?(utcNow);
      attestationOccurrence.UpdateTime = new DateTime?(utcNow);
      attestationOccurrence.SerializedPayload = (object) JObject.Parse(attestationDetails.SerializedPayload);
      attestationOccurrence.Signatures = signatureList;
      return (Grafeas.V1.Occurrence) attestationOccurrence;
    }

    public static List<Grafeas.V1.Note> ToDeploymentNotes(
      this DeploymentDetails deploymentDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<DeploymentDetails>(deploymentDetails, nameof (deploymentDetails));
      ArgumentUtility.CheckForNull<List<string>>(deploymentDetails.ResourceUri, "ResourceUri");
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) deploymentDetails.ResourceUri, "ResourceUri");
      DateTime utcNow = DateTime.UtcNow;
      List<Grafeas.V1.Note> deploymentNotes = new List<Grafeas.V1.Note>();
      foreach (string str in deploymentDetails.ResourceUri)
      {
        Grafeas.V1.DeploymentNote deploymentNote1 = new Grafeas.V1.DeploymentNote();
        deploymentNote1.Name = deploymentDetails.Name;
        deploymentNote1.ScopeId = projectId;
        deploymentNote1.Kind = NoteKind.Deployment;
        deploymentNote1.CreateTime = new DateTime?(utcNow);
        deploymentNote1.UpdateTime = new DateTime?(utcNow);
        deploymentNote1.ResourceUri = new List<string>()
        {
          str
        };
        deploymentNote1.ShortDescription = deploymentDetails.Description;
        deploymentNote1.LongDescription = deploymentDetails.Description;
        deploymentNote1.RelatedUrl = deploymentDetails.RelatedUrl.ToRelatedUrl();
        Grafeas.V1.DeploymentNote deploymentNote2 = deploymentNote1;
        deploymentNotes.Add((Grafeas.V1.Note) deploymentNote2);
      }
      return deploymentNotes;
    }

    public static List<Grafeas.V1.Occurrence> ToDeploymentOccurrences(
      this DeploymentDetails deploymentDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<DeploymentDetails>(deploymentDetails, nameof (deploymentDetails));
      ArgumentUtility.CheckForNull<List<string>>(deploymentDetails.ResourceUri, "ResourceUri");
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) deploymentDetails.ResourceUri, "ResourceUri");
      DateTime utcNow = DateTime.UtcNow;
      List<Grafeas.V1.Occurrence> deploymentOccurrences = new List<Grafeas.V1.Occurrence>();
      foreach (string str in deploymentDetails.ResourceUri)
      {
        Grafeas.V1.DeploymentOccurrence deploymentOccurrence1 = new Grafeas.V1.DeploymentOccurrence();
        deploymentOccurrence1.ScopeId = projectId;
        deploymentOccurrence1.Name = Guid.NewGuid().ToString();
        deploymentOccurrence1.Kind = NoteKind.Deployment;
        deploymentOccurrence1.NoteName = deploymentDetails.Name;
        deploymentOccurrence1.ResourceUri = str;
        deploymentOccurrence1.CreateTime = new DateTime?(utcNow);
        deploymentOccurrence1.UpdateTime = new DateTime?(utcNow);
        deploymentOccurrence1.UserEmail = deploymentDetails.UserEmail;
        deploymentOccurrence1.DeployTime = utcNow;
        deploymentOccurrence1.Config = deploymentDetails.Config;
        deploymentOccurrence1.Address = deploymentDetails.Address;
        deploymentOccurrence1.ResourceUrl = deploymentDetails.ResourceUri;
        deploymentOccurrence1.Platform = (Grafeas.V1.Platform) deploymentDetails.Platform;
        Grafeas.V1.DeploymentOccurrence deploymentOccurrence2 = deploymentOccurrence1;
        deploymentOccurrences.Add((Grafeas.V1.Occurrence) deploymentOccurrence2);
      }
      return deploymentOccurrences;
    }

    public static Grafeas.V1.Note ToVulnerabilityNote(
      this VulnerabilityDetails vulnerabilityDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<VulnerabilityDetails>(vulnerabilityDetails, nameof (vulnerabilityDetails));
      DateTime utcNow = DateTime.UtcNow;
      List<Grafeas.V1.VulnerabilityDetail> vulnerabilityDetailList = new List<Grafeas.V1.VulnerabilityDetail>();
      if (vulnerabilityDetails.Details != null)
      {
        foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.VulnerabilityDetail detail in vulnerabilityDetails.Details)
        {
          Grafeas.V1.PackageVersion packageVersion1 = detail.AffectedVersionStart.ToPackageVersion();
          Grafeas.V1.PackageVersion packageVersion2 = detail.AffectedVersionEnd.ToPackageVersion();
          Grafeas.V1.PackageVersion packageVersion3 = detail.FixedVersion.ToPackageVersion();
          Grafeas.V1.VulnerabilityDetail vulnerabilityDetail = new Grafeas.V1.VulnerabilityDetail()
          {
            SeverityName = detail.SeverityName,
            Description = detail.Description,
            PackageType = detail.PackageType,
            AffectedCpeUri = detail.AffectedCpeUri,
            AffectedPackage = detail.AffectedPackage,
            AffectedVersionStart = packageVersion1,
            AffectedVersionEnd = packageVersion2,
            FixedCpeUri = detail.FixedCpeUri,
            FixedVersion = packageVersion3,
            IsObsolete = detail.IsObsolete,
            SourceUpdateTime = detail.SourceUpdateTime
          };
          vulnerabilityDetailList.Add(vulnerabilityDetail);
        }
      }
      List<Grafeas.V1.WindowsDetail> windowsDetailList = new List<Grafeas.V1.WindowsDetail>();
      if (vulnerabilityDetails.WindowsDetails != null)
      {
        foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.WindowsDetail windowsDetail1 in vulnerabilityDetails.WindowsDetails)
        {
          List<Grafeas.V1.KnowledgeBase> knowledgeBaseList = new List<Grafeas.V1.KnowledgeBase>();
          if (windowsDetail1.FixingKbs != null)
          {
            foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.KnowledgeBase fixingKb in windowsDetail1.FixingKbs)
            {
              Grafeas.V1.KnowledgeBase knowledgeBase = new Grafeas.V1.KnowledgeBase()
              {
                Name = fixingKb.Name,
                Url = fixingKb.Url
              };
              knowledgeBaseList.Add(knowledgeBase);
            }
          }
          Grafeas.V1.WindowsDetail windowsDetail2 = new Grafeas.V1.WindowsDetail()
          {
            CpeUri = windowsDetail1.CpeUri,
            Name = windowsDetail1.Name,
            Description = windowsDetail1.Description,
            FixingKbs = knowledgeBaseList,
            SourceUpdateTime = windowsDetail1.SourceUpdateTime
          };
          windowsDetailList.Add(windowsDetail2);
        }
      }
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.CVSSv3 cvssV3 = vulnerabilityDetails.CvssV3;
      Grafeas.V1.CVSSv3 cvsSv3 = new Grafeas.V1.CVSSv3();
      if (cvssV3 != null)
        cvsSv3 = new Grafeas.V1.CVSSv3()
        {
          BaseScore = cvssV3.BaseScore,
          ExploitabilityScore = cvssV3.ExploitabilityScore,
          ImpactScore = cvssV3.ImpactScore,
          AttackVector = (Grafeas.V1.AttackVector) cvssV3.AttackVector,
          AttackComplexity = (Grafeas.V1.AttackComplexity) cvssV3.AttackComplexity,
          PrivilegesRequired = (Grafeas.V1.PrivilegesRequired) cvssV3.PrivilegesRequired,
          UserInteraction = (Grafeas.V1.UserInteraction) cvssV3.UserInteraction,
          Scope = (Grafeas.V1.Scope) cvssV3.Scope,
          ConfidentialityImpact = (Grafeas.V1.Impact) cvssV3.ConfidentialityImpact,
          IntegrityImpact = (Grafeas.V1.Impact) cvssV3.IntegrityImpact,
          AvailabilityImpact = (Grafeas.V1.Impact) cvssV3.AvailabilityImpact
        };
      Grafeas.V1.VulnerabilityNote vulnerabilityNote = new Grafeas.V1.VulnerabilityNote();
      vulnerabilityNote.Name = vulnerabilityDetails.Name;
      vulnerabilityNote.ScopeId = projectId;
      vulnerabilityNote.Kind = NoteKind.Vulnerability;
      vulnerabilityNote.CreateTime = new DateTime?(utcNow);
      vulnerabilityNote.UpdateTime = new DateTime?(utcNow);
      vulnerabilityNote.ShortDescription = vulnerabilityDetails.Description;
      vulnerabilityNote.LongDescription = vulnerabilityDetails.Description;
      vulnerabilityNote.RelatedUrl = vulnerabilityDetails.RelatedUrl.ToRelatedUrl();
      vulnerabilityNote.CvssScore = vulnerabilityDetails.CvssScore;
      vulnerabilityNote.Severity = (Grafeas.V1.Severity) vulnerabilityDetails.Severity;
      vulnerabilityNote.CvssV3 = cvsSv3;
      vulnerabilityNote.Details = vulnerabilityDetailList;
      vulnerabilityNote.WindowsDetails = windowsDetailList;
      return (Grafeas.V1.Note) vulnerabilityNote;
    }

    public static Grafeas.V1.Occurrence ToVulnerabilityOccurrence(
      this VulnerabilityDetails vulnerabilityDetails,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<VulnerabilityDetails>(vulnerabilityDetails, nameof (vulnerabilityDetails));
      DateTime utcNow = DateTime.UtcNow;
      List<Grafeas.V1.PackageIssue> packageIssueList = new List<Grafeas.V1.PackageIssue>();
      List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.PackageIssue> packageIssue1 = vulnerabilityDetails.PackageIssue;
      if (packageIssue1 != null)
      {
        foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.PackageIssue packageIssue2 in packageIssue1)
        {
          Grafeas.V1.PackageVersion packageVersion1 = packageIssue2.AffectedVersion.ToPackageVersion();
          Grafeas.V1.PackageVersion packageVersion2 = packageIssue2.FixedVersion.ToPackageVersion();
          Grafeas.V1.PackageIssue packageIssue3 = new Grafeas.V1.PackageIssue()
          {
            AffectedCpeUri = packageIssue2.AffectedCpeUri,
            AffectedPackage = packageIssue2.AffectedPackage,
            AffectedVersion = packageVersion1,
            FixedCpeUri = packageIssue2.FixedCpeUri,
            FixedPackage = packageIssue2.FixedPackage,
            FixedVersion = packageVersion2,
            FixAvailable = packageIssue2.FixAvailable
          };
          packageIssueList.Add(packageIssue3);
        }
      }
      Grafeas.V1.VulnerabilityOccurrence vulnerabilityOccurrence = new Grafeas.V1.VulnerabilityOccurrence();
      vulnerabilityOccurrence.ScopeId = projectId;
      vulnerabilityOccurrence.Name = Guid.NewGuid().ToString();
      vulnerabilityOccurrence.Kind = NoteKind.Vulnerability;
      vulnerabilityOccurrence.NoteName = vulnerabilityDetails.Name;
      vulnerabilityOccurrence.ResourceUri = vulnerabilityDetails.ResourceUri[0];
      vulnerabilityOccurrence.CreateTime = new DateTime?(utcNow);
      vulnerabilityOccurrence.UpdateTime = new DateTime?(utcNow);
      vulnerabilityOccurrence.Type = vulnerabilityDetails.Type;
      vulnerabilityOccurrence.Severity = (Grafeas.V1.Severity) vulnerabilityDetails.Severity;
      vulnerabilityOccurrence.CvssScore = vulnerabilityDetails.CvssScore;
      vulnerabilityOccurrence.PackageIssue = packageIssueList;
      vulnerabilityOccurrence.ShortDescription = vulnerabilityDetails.Description;
      vulnerabilityOccurrence.LongDescription = vulnerabilityDetails.Description;
      vulnerabilityOccurrence.RelatedUrls = vulnerabilityDetails.RelatedUrl.ToRelatedUrl();
      vulnerabilityOccurrence.EffectiveSeverity = (Grafeas.V1.Severity) vulnerabilityDetails.EffectiveSeverity;
      vulnerabilityOccurrence.FixAvailable = vulnerabilityDetails.FixAvailable;
      return (Grafeas.V1.Occurrence) vulnerabilityOccurrence;
    }

    public static string GetPipelineRunIdTag(Guid projectId, int runId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PIPELINERUNID:{0}:{1}", (object) projectId, (object) runId);

    private static List<Grafeas.V1.RelatedUrl> ToRelatedUrl(this List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.RelatedUrl> relatedUrl)
    {
      List<Grafeas.V1.RelatedUrl> relatedUrl1 = new List<Grafeas.V1.RelatedUrl>();
      if (relatedUrl != null)
      {
        foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.RelatedUrl relatedUrl2 in relatedUrl)
        {
          if (relatedUrl2 != null)
          {
            Grafeas.V1.RelatedUrl relatedUrl3 = new Grafeas.V1.RelatedUrl()
            {
              Url = relatedUrl2.Url,
              Label = relatedUrl2.Label
            };
            relatedUrl1.Add(relatedUrl3);
          }
        }
      }
      return relatedUrl1;
    }

    private static Grafeas.V1.PackageVersion ToPackageVersion(this Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.PackageVersion versionDetail)
    {
      Grafeas.V1.PackageVersion packageVersion = new Grafeas.V1.PackageVersion();
      if (versionDetail != null)
        packageVersion = new Grafeas.V1.PackageVersion()
        {
          Epoch = versionDetail.Epoch,
          Name = versionDetail.Name,
          Revision = versionDetail.Revision,
          Kind = (Grafeas.V1.VersionKind) versionDetail.Kind,
          FullName = versionDetail.FullName
        };
      return packageVersion;
    }

    private static List<string> AddRunIdTag(List<string> tags, Guid projectId, int runId)
    {
      string pipelineRunIdTag = MetadataDetailsExtensions.GetPipelineRunIdTag(projectId, runId);
      if (tags == null)
        return new List<string>() { pipelineRunIdTag };
      tags.Add(pipelineRunIdTag);
      return tags;
    }

    private static List<string> GetImageNamesWithTags(string imageId, List<string> tags)
    {
      List<string> imageNamesWithTags = new List<string>();
      Match match = MetadataDetailsExtensions.ImageNameRegex.Match(imageId);
      if (match != null && match.Success)
      {
        if (tags == null)
        {
          imageNamesWithTags.Add(match.Value);
        }
        else
        {
          foreach (string tag in tags)
            imageNamesWithTags.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) match, (object) tag));
        }
      }
      return imageNamesWithTags;
    }
  }
}
