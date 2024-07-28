// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ProcessedInstalledExtension
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ProcessedInstalledExtension : InstalledExtension
  {
    private Dictionary<string, InstalledExtensionFile> m_processedFiles;

    internal ProcessedInstalledExtension(InstalledExtension sourceExtension)
      : base(sourceExtension)
    {
      this.m_processedFiles = new Dictionary<string, InstalledExtensionFile>();
      this.ProcessExtensionFiles();
    }

    internal ProcessedInstalledExtension(ProcessedInstalledExtension sourceExtension)
      : base((InstalledExtension) sourceExtension)
    {
      this.m_processedFiles = sourceExtension.ProcessedFiles;
    }

    internal Dictionary<string, InstalledExtensionFile> ProcessedFiles => this.m_processedFiles;

    private void ProcessExtensionFiles()
    {
      try
      {
        if (this.Files == null || string.IsNullOrEmpty(this.BaseUri) || string.IsNullOrEmpty(this.FallbackBaseUri))
          return;
        foreach (ExtensionFile file in this.Files)
        {
          InstalledExtensionFile installedExtensionFile = new InstalledExtensionFile();
          installedExtensionFile.AssetType = file.AssetType;
          installedExtensionFile.Source = file.Source;
          if (!string.IsNullOrEmpty(file.Source) && file.Source.StartsWith(this.BaseUri, StringComparison.OrdinalIgnoreCase))
            installedExtensionFile.FallbackSource = this.FallbackBaseUri + file.Source.Substring(this.BaseUri.Length);
          this.m_processedFiles.Add(file.AssetType, installedExtensionFile);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(100136269, TraceLevel.Error, "ExtensionManagement", nameof (ProcessedInstalledExtension), ex);
      }
    }
  }
}
