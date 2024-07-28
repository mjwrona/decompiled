// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Audit.ProjectAuditData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core.Audit
{
  public static class ProjectAuditData
  {
    public static Dictionary<string, object> Create(
      string projectName,
      string processTemplate,
      ProjectVisibility projectVisibility)
    {
      return new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectName
        },
        {
          "ProcessTemplate",
          (object) processTemplate
        },
        {
          "ProjectVisibility",
          (object) projectVisibility.ToString()
        }
      };
    }

    public static Dictionary<string, object> Rename(string projectName, object previousProjectName) => new Dictionary<string, object>()
    {
      {
        "PreviousProjectName",
        previousProjectName
      },
      {
        "ProjectName",
        (object) projectName
      }
    };

    public static Dictionary<string, object> Visibility(
      string projectName,
      object previousVisibility,
      object Visibility)
    {
      return new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectName
        },
        {
          "PreviousProjectVisibility",
          previousVisibility
        },
        {
          "ProjectVisibility",
          Visibility
        }
      };
    }

    [Obsolete("A breaking change needed to be made to the string resources for project delete messages so this ID is deprecated in favor of Delete2")]
    public static Dictionary<string, object> Delete(string projectName, bool isSoftDelete = false) => new Dictionary<string, object>()
    {
      {
        "ProjectName",
        (object) projectName
      },
      {
        "ProjectDeleteType",
        isSoftDelete ? (object) "soft" : (object) "hard"
      }
    };

    public static Dictionary<string, object> SoftDelete(string projectName) => new Dictionary<string, object>()
    {
      {
        "PreviousProjectName",
        (object) projectName
      }
    };

    public static Dictionary<string, object> HardDelete(string projectName) => new Dictionary<string, object>()
    {
      {
        "PreviousProjectName",
        (object) projectName
      }
    };

    public static Dictionary<string, object> Restore(string projectName) => new Dictionary<string, object>()
    {
      {
        "ProjectName",
        (object) projectName
      }
    };
  }
}
