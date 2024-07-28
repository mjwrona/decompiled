// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DockerProcess
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class DockerProcess : BuildProcess
  {
    private static readonly RegistryQuery RegistryQuery = (RegistryQuery) "/Service/Build/Settings/DockerProcess/...";
    private static readonly Guid DefaultCommandLineTaskId = Guid.Parse("d9bafed4-0b18-4f58-968d-86655b4d2ce9");

    public DockerProcess()
      : base(3)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public DockerProcessTarget Target { get; set; }

    internal DesignerProcess GenerateDesignerProcess(
      IVssRequestContext requestContext,
      BuildData build)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      DesignerProcess designerProcess = new DesignerProcess();
      designerProcess.Version = 3;
      designerProcess.Target = (DesignerProcessTarget) this.Target;
      Phase phase = new Phase();
      designerProcess.Phases.Add(phase);
      RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in DockerProcess.RegistryQuery);
      Guid registryValue1 = this.GetRegistryValue<Guid>(registryEntries, "/Service/Build/Settings/DockerProcess/CommandLineTaskId", DockerProcess.DefaultCommandLineTaskId);
      string registryValue2 = this.GetRegistryValue<string>(registryEntries, "/Service/Build/Settings/DockerProcess/CommandLineVersionSpec", "2.*");
      string registryValue3 = this.GetRegistryValue<string>(registryEntries, "/Service/Build/Settings/DockerProcess/CommandLineInput", "script");
      string registryValue4 = this.GetRegistryValue<string>(registryEntries, "/Service/Build/Settings/DockerProcess/BuildScript", "docker build .");
      BuildDefinitionStep buildDefinitionStep = new BuildDefinitionStep()
      {
        DisplayName = BuildServerResources.DockerProcess_BuildStep(),
        Enabled = true,
        ContinueOnError = false,
        AlwaysRun = false,
        TimeoutInMinutes = 0,
        RetryCountOnTaskFailure = 0,
        TaskDefinition = new TaskDefinitionReference()
        {
          DefinitionType = "task",
          Id = registryValue1,
          VersionSpec = registryValue2
        }
      };
      buildDefinitionStep.Inputs.Add(registryValue3, registryValue4);
      phase.Steps.Add(buildDefinitionStep);
      return designerProcess;
    }

    private T GetRegistryValue<T>(
      RegistryEntryCollection registryEntries,
      string key,
      T defaultValue)
    {
      RegistryEntry entry;
      return registryEntries.TryGetValue(key, out entry) && !string.IsNullOrEmpty(entry.Value) ? entry.GetValue<T>(defaultValue) : defaultValue;
    }

    internal static class RegistryKeys
    {
      private const string Root = "/Service/Build/Settings/DockerProcess/";
      public const string CommandLineTaskId = "/Service/Build/Settings/DockerProcess/CommandLineTaskId";
      public const string CommandLineVersionSpec = "/Service/Build/Settings/DockerProcess/CommandLineVersionSpec";
      public const string CommandLineInputName = "/Service/Build/Settings/DockerProcess/CommandLineInput";
      public const string BuildScript = "/Service/Build/Settings/DockerProcess/BuildScript";
    }
  }
}
