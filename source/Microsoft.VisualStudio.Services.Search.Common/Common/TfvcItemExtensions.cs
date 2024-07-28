// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcItemExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class TfvcItemExtensions
  {
    public static bool IsProjectWithName(this TfvcItem tfvcItem, string projectName)
    {
      if (tfvcItem == null)
        throw new ArgumentNullException(nameof (tfvcItem));
      return string.Equals(tfvcItem.Path.Replace("$/", string.Empty), projectName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
