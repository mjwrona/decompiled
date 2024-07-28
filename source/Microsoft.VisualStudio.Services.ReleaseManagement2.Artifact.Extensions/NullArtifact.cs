// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.NullArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public class NullArtifact : ArtifactTypeBase
  {
    private const string NullArtifactName = "nullArtifact";
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "this field is actually immutable as none of the properties can be set")]
    [StaticSafe]
    public static readonly ArtifactSource Instance = NullArtifact.CreateNullArtifact();

    public override string DisplayName => "nullArtifact";

    public override bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      throw new NotImplementedException();
    }

    public override string UniqueSourceIdentifier => "{{NullArtifactInput}}";

    public override string Name => "nullArtifact";

    public override string EndpointTypeId => "nullArtifact";

    public override Guid ArtifactDownloadTaskId { get; }

    public override IDictionary<string, Guid> ArtifactDownloadTaskIds { get; }

    public override IDictionary<Guid, IDictionary<string, string>> TaskInputMappings { get; }

    public override IDictionary<string, string> TaskInputDefaultValues { get; }

    public override string Type { get; }

    public override IDictionary<string, string> TaskInputMapping { get; }

    public override IDictionary<string, string> GetFormatMaskTokensFromReleaseArtifactInstance(
      IDictionary<string, InputValue> sourceInputs)
    {
      return (IDictionary<string, string>) new Dictionary<string, string>(base.GetFormatMaskTokensFromReleaseArtifactInstance(sourceInputs))
      {
        ["Artifact.ArtifactType"] = string.Empty
      };
    }

    protected override IDictionary<string, ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      return (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>();
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      return (Uri) null;
    }

    public override Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      return (Uri) null;
    }

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = "NullArtifactInput",
        Description = "NullArtifactInput",
        InputMode = InputMode.None,
        Id = "NullArtifactInput",
        IsConfidential = false,
        Validation = new InputValidation(),
        HasDynamicValueInformation = false
      }
    };

    public override bool IsCommitsTraceabilitySupported => false;

    public override bool IsWorkitemsTraceabilitySupported => false;

    public override InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      throw new NotSupportedException();
    }

    public override AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact)
    {
      throw new NotSupportedException();
    }

    public override IList<InputValue> GetAvailableVersions(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      throw new NotSupportedException();
    }

    public override InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      throw new NotSupportedException();
    }

    public override IList<Change> GetChanges(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext)
    {
      throw new NotSupportedException();
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "By design")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "3#", Justification = "By design")]
    public override IList<Change> GetChangesBetweenArtifactSource(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineArtifactSource currentReleaseArtifactSource,
      PipelineArtifactSource lastReleaseArtifactSource,
      int top)
    {
      throw new NotSupportedException();
    }

    public override IList<WorkItemRef> GetWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext,
      GetConfig getConfig)
    {
      throw new NotSupportedException();
    }

    private static ArtifactSource CreateNullArtifact() => new ArtifactSource()
    {
      ArtifactTypeId = "nullArtifact"
    };
  }
}
