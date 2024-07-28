// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Model.Grafeas
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.Model
{
  public class Grafeas
  {
    public class V1Beta
    {
      [DataContract]
      public class ArtifactResource
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Uri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.ProvenanceHash ContentHash { get; set; }
      }

      [DataContract]
      public class BuildDetails : Grafeas.V1Beta.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.BuildProvenance Provenance { get; set; }
      }

      [DataContract]
      public class BuildNote : Grafeas.V1Beta.Note
      {
        public BuildNote()
          : base(NoteKind.Build)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public string BuilderVersion { get; set; }
      }

      [DataContract]
      public class BuildProvenance
      {
        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BuilderVersion { get; set; }

        [DataMember]
        public Grafeas.V1Beta.SourceProvenance SourceProvenance { get; set; }
      }

      [DataContract]
      public class GitSourceContext
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RevisionId { get; set; }
      }

      [DataContract]
      public enum HashType
      {
        HASHTYPEUNSPECIFIED,
        SHA256,
      }

      [DataContract]
      public class ImageBasisNote : Grafeas.V1Beta.Note
      {
        public ImageBasisNote()
          : base(NoteKind.Image)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public string ResourceUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.ImageFingerprint Fingerprint { get; set; }
      }

      [DataContract]
      public class ImageDerived
      {
        [DataMember(EmitDefaultValue = false)]
        public string ImageType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MediaType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Tags { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ImageSize { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string JobName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PipelineVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PipelineName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PipelineId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int RunId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.ImageFingerprint Fingerprint { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Distance { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BaseResourceUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1Beta.ImageLayer> LayerInfo { get; set; }
      }

      [DataContract]
      public class ImageDetails : Grafeas.V1Beta.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.ImageDerived DerivedImage { get; set; }
      }

      [DataContract]
      public class ImageFingerprint
      {
        [DataMember(Name = "v1_name", EmitDefaultValue = false)]
        public string V1Name { get; set; }

        [DataMember(Name = "v2_blob", EmitDefaultValue = false)]
        public List<string> V2Blob { get; set; }

        [DataMember(Name = "v2_name", EmitDefaultValue = false)]
        public string V2Name { get; set; }
      }

      [DataContract]
      public class ImageLayer
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.LayerDirective? Directive { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Arguments { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Size { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime CreatedOn { get; set; }
      }

      [DataContract]
      public enum LayerDirective
      {
        DIRECTIVEUNSPECIFIED = 1,
        MAINTAINER = 2,
        RUN = 3,
        CMD = 4,
        LABEL = 5,
        EXPOSE = 6,
        ENV = 7,
        ADD = 8,
        COPY = 9,
        ENTRYPOINT = 10, // 0x0000000A
        VOLUME = 11, // 0x0000000B
        USER = 12, // 0x0000000C
        WORKDIR = 13, // 0x0000000D
        ARG = 14, // 0x0000000E
        ONBUILD = 15, // 0x0000000F
        STOPSIGNAL = 16, // 0x00000010
        HEALTHCHECK = 17, // 0x00000011
        SHELL = 18, // 0x00000012
      }

      [DataContract]
      [JsonConverter(typeof (Grafeas.V1Beta.NoteJsonConverter))]
      public abstract class Note
      {
        protected Note(NoteKind kind) => this.Kind = kind;

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid ScopeId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ShortDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LongDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public NoteKind Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? CreateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? UpdateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1Beta.RelatedUrl> RelatedUrl { get; set; }
      }

      public sealed class NoteJsonConverter : VssSecureJsonConverter
      {
        public override bool CanConvert(Type objectType) => typeof (Grafeas.V1Beta.Note).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
          if (reader == null)
            throw new ArgumentNullException(nameof (reader));
          if (serializer == null)
            throw new ArgumentNullException(nameof (serializer));
          if (reader.TokenType != JsonToken.StartObject)
            return (object) null;
          if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
            return existingValue;
          JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Kind");
          if (closestMatchProperty == null)
            return existingValue;
          JObject jobject = JObject.Load(reader);
          JToken jtoken;
          if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
            return existingValue;
          NoteKind result;
          if (jtoken.Type == JTokenType.Integer)
            result = (NoteKind) (int) jtoken;
          else if (jtoken.Type != JTokenType.String || !Enum.TryParse<NoteKind>((string) jtoken, true, out result))
            return existingValue;
          object target = (object) null;
          switch (result)
          {
            case NoteKind.Build:
              target = (object) new Grafeas.V1Beta.BuildNote();
              break;
            case NoteKind.Image:
              target = (object) new Grafeas.V1Beta.ImageBasisNote();
              break;
          }
          if (target != null)
          {
            using (JsonReader reader1 = jobject.CreateReader())
              serializer.Populate(reader1, target);
          }
          return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
      }

      [DataContract]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public enum NoteKind
      {
        Build = 3,
        Image = 4,
      }

      [DataContract]
      [JsonConverter(typeof (Grafeas.V1Beta.OccurrenceJsonConverter))]
      [ClientInternalUseOnly(true)]
      public abstract class Occurrence : Grafeas.V1Beta.OccurrenceReference
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.ArtifactResource Resource { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? CreateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? UpdateTime { get; set; }
      }

      [ClientInternalUseOnly(true)]
      public sealed class OccurrenceJsonConverter : VssSecureJsonConverter
      {
        public override bool CanConvert(Type objectType) => typeof (Grafeas.V1Beta.Occurrence).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
          if (reader == null)
            throw new ArgumentNullException(nameof (reader));
          if (serializer == null)
            throw new ArgumentNullException(nameof (serializer));
          if (reader.TokenType != JsonToken.StartObject)
            return (object) null;
          if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
            return existingValue;
          JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Kind");
          if (closestMatchProperty == null)
            return existingValue;
          JObject jobject = JObject.Load(reader);
          JToken jtoken;
          if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
            return existingValue;
          NoteKind result;
          if (jtoken.Type == JTokenType.Integer)
            result = (NoteKind) (int) jtoken;
          else if (jtoken.Type != JTokenType.String || !Enum.TryParse<NoteKind>((string) jtoken, true, out result))
            return existingValue;
          object target = (object) null;
          switch (result)
          {
            case NoteKind.Build:
              target = (object) new Grafeas.V1Beta.BuildDetails();
              break;
            case NoteKind.Image:
              target = (object) new Grafeas.V1Beta.ImageDetails();
              break;
          }
          if (target != null)
          {
            using (JsonReader reader1 = jobject.CreateReader())
              serializer.Populate(reader1, target);
          }
          return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class OccurrenceReference
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid ScopeId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NoteName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public NoteKind Kind { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class ProvenanceHash
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.HashType? Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public byte[] Value { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class RelatedUrl
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Label { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class SourceContext
      {
        public Grafeas.V1Beta.GitSourceContext Git { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class SourceProvenance
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1Beta.SourceContext Context { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1Beta.SourceContext> AdditionalContexts { get; set; }
      }
    }

    public class V1
    {
      [DataContract]
      public class BuildOccurrence : Grafeas.V1.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.BuildProvenance Provenance { get; set; }
      }

      [DataContract]
      public class BuildNote : Grafeas.V1.Note
      {
        public BuildNote()
          : base(NoteKind.Build)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public string BuilderVersion { get; set; }
      }

      [DataContract]
      public class BuildProvenance
      {
        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ProjectId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BuilderVersion { get; set; }

        [DataMember]
        public Grafeas.V1.SourceProvenance SourceProvenance { get; set; }

        [DataMember]
        public List<Grafeas.V1.BuildArtifact> BuildArtifacts { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime CreateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? EndTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Creator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LogsUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IDictionary<string, string> BuildOptions { get; set; }
      }

      [DataContract]
      public class BuildArtifact
      {
        [DataMember(EmitDefaultValue = false)]
        public string Checksum { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Names { get; set; }
      }

      [DataContract]
      public class GitSourceContext
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RevisionId { get; set; }
      }

      [DataContract]
      public class ImageNote : Grafeas.V1.Note
      {
        public ImageNote()
          : base(NoteKind.Image)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public string ResourceUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.ImageFingerprint Fingerprint { get; set; }
      }

      [DataContract]
      public class ImageOccurrence : Grafeas.V1.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public string ImageType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MediaType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Tags { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ImageSize { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string JobName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PipelineVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PipelineName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PipelineId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int RunId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.ImageFingerprint Fingerprint { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Distance { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BaseResourceUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.ImageLayer> LayerInfo { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RepositoryTypeName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RepositoryId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RepositoryName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Branch { get; set; }
      }

      [DataContract]
      public class ImageFingerprint
      {
        [DataMember(Name = "v1Name", EmitDefaultValue = false)]
        public string V1Name { get; set; }

        [DataMember(Name = "v2Blob", EmitDefaultValue = false)]
        public List<string> V2Blob { get; set; }

        [DataMember(Name = "v2Name", EmitDefaultValue = false)]
        public string V2Name { get; set; }
      }

      [DataContract]
      public class ImageLayer
      {
        [DataMember(EmitDefaultValue = false)]
        public string Directive { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Arguments { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Size { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime CreatedOn { get; set; }
      }

      [DataContract]
      [JsonConverter(typeof (Grafeas.V1.NoteJsonConverter))]
      public abstract class Note
      {
        protected Note(NoteKind kind) => this.Kind = kind;

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid ScopeId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ShortDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LongDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public NoteKind Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? CreateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? UpdateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.RelatedUrl> RelatedUrl { get; set; }
      }

      public sealed class NoteJsonConverter : VssSecureJsonConverter
      {
        public override bool CanConvert(Type objectType) => typeof (Grafeas.V1.Note).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
          if (reader == null)
            throw new ArgumentNullException(nameof (reader));
          if (serializer == null)
            throw new ArgumentNullException(nameof (serializer));
          if (reader.TokenType != JsonToken.StartObject)
            return (object) null;
          if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
            return existingValue;
          JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Kind");
          if (closestMatchProperty == null)
            return existingValue;
          JObject jobject = JObject.Load(reader);
          JToken jtoken;
          if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
            return existingValue;
          NoteKind result;
          if (jtoken.Type == JTokenType.Integer)
            result = (NoteKind) (int) jtoken;
          else if (jtoken.Type != JTokenType.String || !Enum.TryParse<NoteKind>((string) jtoken, true, out result))
            return existingValue;
          object target = (object) null;
          switch (result)
          {
            case NoteKind.Build:
              target = (object) new Grafeas.V1.BuildNote();
              break;
            case NoteKind.Image:
              target = (object) new Grafeas.V1.ImageNote();
              break;
            case NoteKind.Deployment:
              target = (object) new Grafeas.V1.DeploymentNote();
              break;
            case NoteKind.Vulnerability:
              target = (object) new Grafeas.V1.VulnerabilityNote();
              break;
            case NoteKind.Attestation:
              target = (object) new Grafeas.V1.AttestationNote();
              break;
          }
          if (target != null)
          {
            using (JsonReader reader1 = jobject.CreateReader())
              serializer.Populate(reader1, target);
          }
          return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
      }

      [DataContract]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public enum NoteKind
      {
        Vulnerability = 1,
        Build = 2,
        Image = 3,
        Deployment = 5,
        Attestation = 7,
      }

      [DataContract]
      [JsonConverter(typeof (Grafeas.V1.OccurrenceJsonConverter))]
      [ClientInternalUseOnly(true)]
      public abstract class Occurrence : Grafeas.V1.OccurrenceReference
      {
        [DataMember(EmitDefaultValue = false)]
        public string ResourceUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? CreateTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? UpdateTime { get; set; }
      }

      [ClientInternalUseOnly(true)]
      public sealed class OccurrenceJsonConverter : VssSecureJsonConverter
      {
        public override bool CanConvert(Type objectType) => typeof (Grafeas.V1.Occurrence).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
          if (reader == null)
            throw new ArgumentNullException(nameof (reader));
          if (serializer == null)
            throw new ArgumentNullException(nameof (serializer));
          if (reader.TokenType != JsonToken.StartObject)
            return (object) null;
          if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
            return existingValue;
          JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Kind");
          if (closestMatchProperty == null)
            return existingValue;
          JObject jobject = JObject.Load(reader);
          JToken jtoken;
          if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
            return existingValue;
          NoteKind result;
          if (jtoken.Type == JTokenType.Integer)
            result = (NoteKind) (int) jtoken;
          else if (jtoken.Type != JTokenType.String || !Enum.TryParse<NoteKind>((string) jtoken, true, out result))
            return existingValue;
          object target = (object) null;
          switch (result)
          {
            case NoteKind.Build:
              target = (object) new Grafeas.V1.BuildOccurrence();
              break;
            case NoteKind.Image:
              target = (object) new Grafeas.V1.ImageOccurrence();
              break;
            case NoteKind.Deployment:
              target = (object) new Grafeas.V1.DeploymentOccurrence();
              break;
            case NoteKind.Vulnerability:
              target = (object) new Grafeas.V1.VulnerabilityOccurrence();
              break;
            case NoteKind.Attestation:
              target = (object) new Grafeas.V1.AttestationOccurrence();
              break;
          }
          if (target != null)
          {
            using (JsonReader reader1 = jobject.CreateReader())
              serializer.Populate(reader1, target);
          }
          return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class OccurrenceReference
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid ScopeId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NoteName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public NoteKind Kind { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class ProvenanceHash
      {
        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public byte[] Value { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class RelatedUrl
      {
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Label { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class SourceContext
      {
        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.GitSourceContext Git { get; set; }
      }

      [DataContract]
      [ClientInternalUseOnly(true)]
      public class SourceProvenance
      {
        [DataMember(EmitDefaultValue = false)]
        public string ArtifactStorageSourceUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.SourceContext Context { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.SourceContext> AdditionalContexts { get; set; }
      }

      [DataContract]
      public class VulnerabilityNote : Grafeas.V1.Note
      {
        public VulnerabilityNote()
          : base(NoteKind.Vulnerability)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public float CvssScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Severity Severity { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.VulnerabilityDetail> Details { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.WindowsDetail> WindowsDetails { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.CVSSv3 CvssV3 { get; set; }
      }

      [DataContract]
      public class VulnerabilityOccurrence : Grafeas.V1.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Severity Severity { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float CvssScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.PackageIssue> PackageIssue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ShortDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LongDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.RelatedUrl> RelatedUrls { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Severity EffectiveSeverity { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool FixAvailable { get; set; }
      }

      [DataContract]
      public class PackageIssue
      {
        [DataMember(EmitDefaultValue = false)]
        public string AffectedCpeUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string AffectedPackage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.PackageVersion AffectedVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FixedCpeUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FixedPackage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.PackageVersion FixedVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool FixAvailable { get; set; }
      }

      [DataContract]
      public class PackageVersion
      {
        [DataMember(EmitDefaultValue = false)]
        public int Epoch { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Revision { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.VersionKind Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FullName { get; set; }
      }

      [DataContract]
      public enum VersionKind
      {
        Unspecified,
        Normal,
        Minimum,
        Maximum,
      }

      [DataContract]
      public enum Severity
      {
        Unspecified = 0,
        Minimal = 1,
        Critical = 2,
        High = 2,
        Low = 2,
        Medium = 3,
      }

      [DataContract]
      public class VulnerabilityDetail
      {
        [DataMember(EmitDefaultValue = false)]
        public string SeverityName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PackageType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string AffectedCpeUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string AffectedPackage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.PackageVersion AffectedVersionStart { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.PackageVersion AffectedVersionEnd { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FixedCpeUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FixedPackage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.PackageVersion FixedVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsObsolete { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime SourceUpdateTime { get; set; }
      }

      [DataContract]
      public class KnowledgeBase
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }
      }

      public class WindowsDetail
      {
        [DataMember(EmitDefaultValue = false)]
        public string CpeUri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.KnowledgeBase> FixingKbs { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime SourceUpdateTime { get; set; }
      }

      public class CVSSv3
      {
        [DataMember(EmitDefaultValue = false)]
        public float BaseScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float ExploitabilityScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float ImpactScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.AttackVector AttackVector { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.AttackComplexity AttackComplexity { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.PrivilegesRequired PrivilegesRequired { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.UserInteraction UserInteraction { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Scope Scope { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Impact ConfidentialityImpact { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Impact IntegrityImpact { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Impact AvailabilityImpact { get; set; }
      }

      [DataContract]
      public enum AttackVector
      {
        Unspecified,
        Network,
        Adjacent,
        Local,
        Physical,
      }

      [DataContract]
      public enum AttackComplexity
      {
        Unspecified,
        Low,
        High,
      }

      [DataContract]
      public enum PrivilegesRequired
      {
        Unspecified,
        None,
        Low,
        High,
      }

      [DataContract]
      public enum UserInteraction
      {
        Unspecified,
        None,
        Required,
      }

      [DataContract]
      public enum Scope
      {
        Unspecified,
        Unchanged,
        Changed,
      }

      [DataContract]
      public enum Impact
      {
        Unspecified,
        High,
        Low,
        None,
      }

      [DataContract]
      public class DeploymentNote : Grafeas.V1.Note
      {
        public DeploymentNote()
          : base(NoteKind.Deployment)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public List<string> ResourceUri { get; set; }
      }

      [DataContract]
      public class DeploymentOccurrence : Grafeas.V1.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public string UserEmail { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime DeployTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime UnDeployTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Config { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Address { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> ResourceUrl { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Platform Platform { get; set; }
      }

      [DataContract]
      public enum Platform
      {
        Unspecified,
        Gke,
        Flex,
        Custom,
      }

      [DataContract]
      public class AttestationNote : Grafeas.V1.Note
      {
        public AttestationNote()
          : base(NoteKind.Attestation)
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public Grafeas.V1.Hint Hint { get; set; }
      }

      [DataContract]
      public class Hint
      {
        [DataMember(EmitDefaultValue = false)]
        public string HumanReadableName { get; set; }
      }

      [DataContract]
      public class Signature
      {
        [DataMember(EmitDefaultValue = false)]
        public string SignatureContent { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PublicKeyId { get; set; }
      }

      [DataContract]
      public class AttestationOccurrence : Grafeas.V1.Occurrence
      {
        [DataMember(EmitDefaultValue = false)]
        public object SerializedPayload { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<Grafeas.V1.Signature> Signatures { get; set; }
      }
    }
  }
}
