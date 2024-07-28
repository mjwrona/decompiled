// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.ITokenRevocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  [DefaultServiceImplementation(typeof (FrameworkTokenRevocationService))]
  public interface ITokenRevocationService : IVssFrameworkService
  {
    bool IsValid(
      IVssRequestContext requestContext,
      IAuthCredential authCredential,
      Guid userId,
      out Guid failingRuleId,
      bool ignoreOrgHostCheck = false);

    TokenRevocationRule[] GetAllRules(IVssRequestContext requestContext);

    TokenRevocationRule[] GetHostTranslationRules(IVssRequestContext requestContext);

    IList<Guid> CreateRules(
      IVssRequestContext requestContext,
      IEnumerable<TokenRevocationRule> rules);

    void DeleteRule(IVssRequestContext requestContext, Guid ruleId);
  }
}
