// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Configuration
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class Configuration
  {
    public Configuration(IReadOnlyList<ConfigurationFile> files)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) files, nameof (files));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) files, nameof (files));
      this.Files = files;
    }

    public IReadOnlyList<ConfigurationFile> Files { get; }
  }
}
