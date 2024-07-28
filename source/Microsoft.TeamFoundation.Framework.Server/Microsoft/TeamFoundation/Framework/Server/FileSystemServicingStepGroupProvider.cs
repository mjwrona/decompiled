// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileSystemServicingStepGroupProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileSystemServicingStepGroupProvider : ServicingStepGroupProviderBase
  {
    private readonly string m_directory;

    public FileSystemServicingStepGroupProvider(
      string directory,
      IServicingStepGroupProvider fallbackProvider,
      ITFLogger logger)
      : base(fallbackProvider, logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(directory, nameof (directory));
      this.m_directory = directory;
      this.ThrowOnDuplicates = false;
    }

    protected override string[] GetServicingStepGroupResourceNames() => !Directory.Exists(this.m_directory) ? Array.Empty<string>() : Directory.GetFiles(this.m_directory, "*.xml", SearchOption.AllDirectories);

    protected override Stream OpenServicingStepGroupSteam(string resourceName) => (Stream) File.OpenRead(resourceName);
  }
}
