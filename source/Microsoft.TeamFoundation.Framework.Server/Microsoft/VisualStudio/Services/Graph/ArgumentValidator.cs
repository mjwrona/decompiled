// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.ArgumentValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class ArgumentValidator
  {
    public static void CheckSubjectType(SubjectDescriptor subjectDescriptor, string subjectType)
    {
      if (!StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, subjectType))
        throw new GraphBadRequestException("The subject type '" + subjectDescriptor.SubjectType + "' is not valid for this endpoint.");
    }

    public static void CheckSubjectKind(string subjectKind, List<string> validTypes)
    {
      if (subjectKind == null)
        return;
      List<string> list = ((IEnumerable<string>) subjectKind.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (x => !validTypes.Contains<string>(x, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))).ToList<string>();
      if (list.Any<string>())
        throw new GraphBadRequestException(FrameworkResources.InvalidSubjectKinds((object) string.Join(",", list.Select<string, string>((Func<string, string>) (p => p.ToString())))));
    }

    public static void CheckDescriptorIsGroupSubjectKind(SubjectDescriptor subjectDescriptor)
    {
      GraphValidation.CheckDescriptor(subjectDescriptor, nameof (subjectDescriptor));
      if (!subjectDescriptor.IsGroupType())
        throw new GraphBadRequestException(GraphResources.InvalidGraphSubjectDescriptor((object) subjectDescriptor));
    }

    public static void CheckDescriptorIsMemberSubjectKind(SubjectDescriptor subjectDescriptor)
    {
      GraphValidation.CheckDescriptor(subjectDescriptor, nameof (subjectDescriptor));
      if (!subjectDescriptor.IsGroupType() && !subjectDescriptor.IsUserType() && !subjectDescriptor.IsImportedIdentityType())
        throw new GraphBadRequestException(GraphResources.InvalidGraphSubjectDescriptor((object) subjectDescriptor));
    }

    public static void CheckDescriptorIsScopeSubjectKind(SubjectDescriptor subjectDescriptor)
    {
      GraphValidation.CheckDescriptor(subjectDescriptor, nameof (subjectDescriptor));
      if (!subjectDescriptor.IsGroupScopeType())
        throw new GraphBadRequestException(GraphResources.InvalidGraphSubjectDescriptor((object) subjectDescriptor));
    }
  }
}
