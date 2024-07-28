// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.CachingFileContentsProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class CachingFileContentsProvider : InstrumentedFileContentsProvider
  {
    private readonly Dictionary<string, string> m_fileContentCache = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public CachingFileContentsProvider(IFileContentsProvider actualContentsProvider)
      : base(actualContentsProvider)
    {
    }

    public override string GetFileContents(IVssRequestContext requestContext, string filePath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filePath, nameof (filePath));
      string fileContents1;
      if (this.m_fileContentCache.TryGetValue(filePath, out fileContents1))
      {
        this.StatisticsBuilder?.IncrementFileCacheHits();
        return fileContents1;
      }
      this.StatisticsBuilder?.IncrementFileCacheMisses();
      string fileContents2 = base.GetFileContents(requestContext, filePath);
      this.m_fileContentCache[filePath] = fileContents2;
      return fileContents2;
    }
  }
}
