// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ServicingOperationProvider
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class ServicingOperationProvider : IServicingOperationProvider
  {
    private const string c_servicingDataDefaultNamespace = "ServicingData";
    private readonly ServicingOperationProviderBase m_operationProvider;
    private readonly ServicingStepGroupProviderBase m_stepGroupProvider;

    public ServicingOperationProvider(
      string servicingFilesPath,
      ServicingOperationTarget target,
      bool hostedDeployment,
      params string[] servicingDataAssemblyFiles)
      : this(servicingFilesPath, target, hostedDeployment, ((IEnumerable<string>) servicingDataAssemblyFiles).ToList<string>())
    {
    }

    public ServicingOperationProvider(
      string servicingFilesPath,
      ServicingOperationTarget target,
      bool hostedDeployment,
      List<string> servicingDataAssemblyFiles)
    {
      if (servicingDataAssemblyFiles != null)
        servicingDataAssemblyFiles = servicingDataAssemblyFiles.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).Select<string, string>((Func<string, string>) (x => Path.Combine(servicingFilesPath, x))).ToList<string>();
      if (servicingDataAssemblyFiles == null || servicingDataAssemblyFiles.Count == 0)
        throw new ArgumentException(ConfigurationResources.ServicingAssemblyRequired());
      this.HostedDeployment = hostedDeployment;
      this.m_stepGroupProvider = this.ChainStepGroupProvider((ServicingStepGroupProviderBase) null, servicingFilesPath, servicingDataAssemblyFiles, target);
      this.m_operationProvider = this.ChainOperationProvider((ServicingOperationProviderBase) null, servicingFilesPath, servicingDataAssemblyFiles, target);
      this.Target = target;
    }

    public bool HostedDeployment { get; private set; }

    private ServicingOperationProviderBase ChainOperationProvider(
      ServicingOperationProviderBase operationProvider,
      string servicingFilesPath,
      List<string> servicingDataAssemblyFiles,
      ServicingOperationTarget target)
    {
      string resourcePrefix = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.Operations.", (object) "ServicingData", (object) ServicingResourcesHelper.GetOperationTargetDirectory(target));
      foreach (string dataAssemblyFile in servicingDataAssemblyFiles)
        operationProvider = (ServicingOperationProviderBase) new AssemblyServicingOperationProvider(dataAssemblyFile, resourcePrefix, (IServicingStepGroupProvider) this.m_stepGroupProvider, operationProvider, target, this.HostedDeployment, (ITFLogger) AdminTraceLogger.Default);
      operationProvider = (ServicingOperationProviderBase) new FileSystemServicingOperationProvider(ServicingResourcesHelper.GetServicingOperationsPath(servicingFilesPath, target), (IServicingStepGroupProvider) this.m_stepGroupProvider, operationProvider, target, this.HostedDeployment, (ITFLogger) AdminTraceLogger.Default);
      return operationProvider;
    }

    private ServicingStepGroupProviderBase ChainStepGroupProvider(
      ServicingStepGroupProviderBase stepGroupProvider,
      string servicingFilesPath,
      List<string> servicingDataAssemblyFiles,
      ServicingOperationTarget target)
    {
      string resourcePrefix = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.Groups.", (object) "ServicingData", (object) ServicingResourcesHelper.GetOperationTargetDirectory(target));
      foreach (string dataAssemblyFile in servicingDataAssemblyFiles)
        stepGroupProvider = (ServicingStepGroupProviderBase) new AssemblyServicingStepGroupProvider(dataAssemblyFile, resourcePrefix, (IServicingStepGroupProvider) stepGroupProvider, (ITFLogger) AdminTraceLogger.Default);
      stepGroupProvider = (ServicingStepGroupProviderBase) new FileSystemServicingStepGroupProvider(ServicingResourcesHelper.GetServicingGroupsPath(servicingFilesPath, target), (IServicingStepGroupProvider) stepGroupProvider, (ITFLogger) AdminTraceLogger.Default);
      return stepGroupProvider;
    }

    public ServicingOperation GetServicingOperation(string servicingOperation) => this.m_operationProvider.GetServicingOperation(servicingOperation) ?? throw new ConfigurationException(ConfigurationResources.ServicingOperationNotValidDuringAppServicing((object) servicingOperation, (object) this.Target));

    public ServicingOperation[] GetServicingOperations() => this.m_operationProvider.GetServicingOperations();

    public string[] GetServicingOperationNames()
    {
      ServicingOperation[] servicingOperations = this.GetServicingOperations();
      string[] servicingOperationNames = new string[servicingOperations.Length];
      for (int index = 0; index < servicingOperationNames.Length; ++index)
        servicingOperationNames[index] = servicingOperations[index].Name;
      return servicingOperationNames;
    }

    public ServicingStepGroup[] GetServicingGroups() => this.m_stepGroupProvider.GetServicingStepGroups();

    public ServicingOperationTarget Target { get; private set; }
  }
}
