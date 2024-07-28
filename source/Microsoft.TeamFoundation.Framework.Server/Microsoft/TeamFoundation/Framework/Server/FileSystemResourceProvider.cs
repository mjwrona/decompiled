// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileSystemResourceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileSystemResourceProvider : IServicingResourceProvider
  {
    private readonly IServicingResourceProvider m_fallbackResourceProvider;
    private readonly string m_directoryPath;

    public FileSystemResourceProvider(
      string directoryPath,
      IServicingResourceProvider fallbackResourceProvider)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(directoryPath, nameof (directoryPath));
      this.m_directoryPath = directoryPath;
      this.m_fallbackResourceProvider = fallbackResourceProvider;
    }

    public Stream GetServicingResource(string resourceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      string path = Path.Combine(this.m_directoryPath, resourceName);
      Stream servicingResource = (Stream) null;
      try
      {
        if (File.Exists(path))
          servicingResource = (Stream) File.OpenRead(path);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Error("An error occurred while reading the following file: '{0}'", (object) path);
        TeamFoundationTrace.TraceException(ex);
      }
      if (servicingResource == null && this.m_fallbackResourceProvider != null)
        servicingResource = this.m_fallbackResourceProvider.GetServicingResource(resourceName);
      return servicingResource;
    }
  }
}
