// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.QueryFilters
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class QueryFilters
  {
    internal static IEnumerable<GraphSubject> ApplySubjectKindFilter(
      IEnumerable<GraphSubject> graphSubjects,
      string subjectKinds)
    {
      if (string.IsNullOrWhiteSpace(subjectKinds) || graphSubjects == null)
        return graphSubjects;
      HashSet<string> subjectKindsSet = ((IEnumerable<string>) subjectKinds.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return graphSubjects.Where<GraphSubject>((Func<GraphSubject, bool>) (x => subjectKindsSet.Contains(x.SubjectKind)));
    }

    internal static IEnumerable<GraphSubject> ApplySubjectTypeFilter(
      IEnumerable<GraphSubject> graphSubjects,
      string subjectTypes)
    {
      if (string.IsNullOrWhiteSpace(subjectTypes) || graphSubjects == null)
        return graphSubjects;
      HashSet<string> subjectTypesSet = ((IEnumerable<string>) subjectTypes.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return graphSubjects.Where<GraphSubject>((Func<GraphSubject, bool>) (x => subjectTypesSet.Contains(x.Descriptor.SubjectType)));
    }

    internal static Func<GraphSubject, bool> ApplySubjectKindFilter(ISet<string> subjectKinds) => !subjectKinds.Any<string>() ? (Func<GraphSubject, bool>) (x => true) : (Func<GraphSubject, bool>) (x => subjectKinds.Contains(x.SubjectKind));

    internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> ApplySubjectTypeFilter(
      ISet<string> subjectTypes)
    {
      return !subjectTypes.Any<string>() ? (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => true) : (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => subjectTypes.Contains(x.SubjectDescriptor.SubjectType));
    }

    internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> ApplyMetaTypeFilter(
      ISet<IdentityMetaType> metaTypesSet,
      bool allowEmpty)
    {
      return !metaTypesSet.Any<IdentityMetaType>() ? (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => allowEmpty) : (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => metaTypesSet.Contains(x.MetaType));
    }

    internal static ISet<string> ParseFilter(string filter)
    {
      if (string.IsNullOrWhiteSpace(filter))
        return (ISet<string>) new HashSet<string>();
      return (ISet<string>) ((IEnumerable<string>) filter.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
