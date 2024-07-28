// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authorization.IRequestActor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Authorization
{
  internal interface IRequestActor
  {
    IdentityDescriptor Descriptor { get; }

    Guid Identifier { get; }

    IReadOnlyDictionary<SubjectType, EvaluationPrincipal> Principals { get; }

    bool TryAppendPrincipal(SubjectType principalType, EvaluationPrincipal principal);

    bool TryReplacePrincipal(SubjectType subjectType, EvaluationPrincipal principal);

    bool TryGetPrincipal(SubjectType subjectType, out EvaluationPrincipal principal);
  }
}
