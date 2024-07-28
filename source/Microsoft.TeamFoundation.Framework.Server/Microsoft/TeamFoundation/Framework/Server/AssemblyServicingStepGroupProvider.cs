// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AssemblyServicingStepGroupProvider
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
  public class AssemblyServicingStepGroupProvider : ServicingStepGroupProviderBase
  {
    private readonly Assembly m_resourceAssembly;
    private readonly string m_resourceAssemblyPath;
    private readonly string m_resourcePrefix;

    public AssemblyServicingStepGroupProvider(
      string resourceAssemblyPath,
      string resourcePrefix,
      IServicingStepGroupProvider fallbackProvider,
      ITFLogger logger)
      : base(fallbackProvider, logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceAssemblyPath, nameof (resourceAssemblyPath));
      this.m_resourceAssemblyPath = resourceAssemblyPath;
      this.m_resourcePrefix = resourcePrefix ?? string.Empty;
      this.m_resourceAssembly = Assembly.ReflectionOnlyLoadFrom(resourceAssemblyPath);
    }

    protected override string[] GetServicingStepGroupResourceNames()
    {
      int length = this.m_resourcePrefix.Length;
      return ((IEnumerable<string>) this.m_resourceAssembly.GetManifestResourceNames()).Where<string>((Func<string, bool>) (resourceName => resourceName.StartsWith(this.m_resourcePrefix, StringComparison.OrdinalIgnoreCase))).ToArray<string>();
    }

    protected override Stream OpenServicingStepGroupSteam(string resourceName) => this.m_resourceAssembly.GetManifestResourceStream(resourceName);
  }
}
