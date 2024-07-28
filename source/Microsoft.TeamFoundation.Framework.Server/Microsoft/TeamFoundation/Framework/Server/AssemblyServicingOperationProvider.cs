// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AssemblyServicingOperationProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AssemblyServicingOperationProvider : ServicingOperationProviderBase
  {
    private readonly Assembly m_resourceAssembly;
    private readonly string m_resourceAssemblyPath;
    private readonly string m_resourcePrefix;

    public AssemblyServicingOperationProvider(
      string resourceAssemblyPath,
      string resourcePrefix,
      IServicingStepGroupProvider stepGroupProvider,
      ServicingOperationProviderBase fallbackProvider,
      ServicingOperationTarget target,
      bool hostedDeployment,
      ITFLogger logger)
      : base(stepGroupProvider, fallbackProvider, target, hostedDeployment, logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceAssemblyPath, nameof (resourceAssemblyPath));
      this.m_resourceAssemblyPath = resourceAssemblyPath;
      this.m_resourcePrefix = resourcePrefix ?? string.Empty;
      this.m_resourceAssembly = Assembly.ReflectionOnlyLoadFrom(resourceAssemblyPath);
    }

    protected override string[] GetServicingOperationResourceNames()
    {
      int length = this.m_resourcePrefix.Length;
      return ((IEnumerable<string>) this.m_resourceAssembly.GetManifestResourceNames()).Where<string>((Func<string, bool>) (resourceName => resourceName.StartsWith(this.m_resourcePrefix, StringComparison.OrdinalIgnoreCase))).ToArray<string>();
    }

    protected override Stream OpenServicingOperationSteam(string resourceName) => this.m_resourceAssembly.GetManifestResourceStream(resourceName);
  }
}
