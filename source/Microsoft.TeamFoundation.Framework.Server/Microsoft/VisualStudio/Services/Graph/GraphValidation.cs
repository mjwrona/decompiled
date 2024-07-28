// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphValidation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphValidation
  {
    internal static void CheckDescriptor(
      SubjectDescriptor descriptor,
      string parameterName,
      bool allowWellKnownGroups = true)
    {
      if (descriptor == new SubjectDescriptor())
        throw new ArgumentException(GraphResources.EmptySubjectDescriptorNotAllowed((object) parameterName));
      if (descriptor.IsUnknownSubjectType())
        throw new InvalidSubjectTypeException(descriptor.SubjectType);
      if (!allowWellKnownGroups && descriptor.IsVstsGroupType() && descriptor.Identifier != null && descriptor.Identifier.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(GraphResources.WellKnownSidNotAllowed((object) parameterName));
    }
  }
}
