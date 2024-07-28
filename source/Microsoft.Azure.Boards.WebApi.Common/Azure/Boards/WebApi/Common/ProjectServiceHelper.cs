// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ProjectServiceHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  internal static class ProjectServiceHelper
  {
    internal static Guid GetProjectTemplateTypeId(this ProjectInfo project)
    {
      if (project.Properties.Any<ProjectProperty>())
      {
        ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => p.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType, StringComparison.OrdinalIgnoreCase)));
        Guid result;
        if (projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result))
          return result;
      }
      return Guid.Empty;
    }
  }
}
