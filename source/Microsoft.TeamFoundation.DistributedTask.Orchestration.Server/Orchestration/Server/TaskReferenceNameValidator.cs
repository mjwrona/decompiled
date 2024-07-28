// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskReferenceNameValidator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TaskReferenceNameValidator
  {
    private static readonly Regex s_validRefName = new Regex("^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    public static void Validate(IVssRequestContext requestContext, List<string> refNames)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TaskOutputVariables"))
        return;
      ArgumentUtility.CheckForNull<List<string>>(refNames, nameof (refNames), "DistributedTask");
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < refNames.Count; ++index)
      {
        string refName = refNames[index];
        ArgumentUtility.CheckStringForNullOrEmpty(refName, "RefName");
        if (!TaskReferenceNameValidator.s_validRefName.IsMatch(refName))
          throw new ArgumentException("Reference name should only contains 'a-z', 'A-Z', '0-9' and '_'.", "RefName").Expected("DistributedTask");
        if (stringSet.Contains(refName))
          throw new ArgumentException("Duplicate task reference name '" + refName + "' within same phase.", "RefName").Expected("DistributedTask");
        stringSet.Add(refName);
      }
    }
  }
}
