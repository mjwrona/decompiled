// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionValidation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  internal static class NameResolutionValidation
  {
    internal static void ValidateEntries(IEnumerable<NameResolutionEntry> entries)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) entries, nameof (entries));
      foreach (NameResolutionEntry entry in entries)
        NameResolutionValidation.ValidateEntry(entry);
    }

    internal static void ValidateEntry(NameResolutionEntry entry)
    {
      ArgumentUtility.CheckForNull<NameResolutionEntry>(entry, nameof (entry));
      NameResolutionValidation.ValidateNamespace(entry.Namespace);
      NameResolutionValidation.ValidateName(entry.Name);
    }

    internal static void ValidateNamespace(string @namespace)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(@namespace, nameof (@namespace));
      ArgumentUtility.CheckForOutOfRange(@namespace.Length, nameof (@namespace), 0, 256);
    }

    internal static void ValidateName(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckForOutOfRange(name.Length, nameof (name), 0, 256);
    }
  }
}
