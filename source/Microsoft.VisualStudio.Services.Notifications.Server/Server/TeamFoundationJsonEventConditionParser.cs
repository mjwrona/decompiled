// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationJsonEventConditionParser
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TeamFoundationJsonEventConditionParser : TeamFoundationEventConditionParser
  {
    public TeamFoundationJsonEventConditionParser(
      string input,
      IReadOnlyList<IDynamicEventPredicate> dynamicEventPredicates = null,
      StringFieldMode stringFieldMode = StringFieldMode.Legacy,
      IVssRequestContext requestContext = null,
      Subscription subscription = null)
      : base(input, dynamicEventPredicates, stringFieldMode, requestContext, subscription)
    {
      this.m_scanner = new TeamFoundationEventConditionScanner(input, (Func<char, bool>) (x => char.IsLetter(x) || x == '$' || x == '.' || x == '[' || x == ']' || x == '-'));
    }

    public override EventSerializerType SerializerType => EventSerializerType.Json;

    protected override void ValidateTokenName(Token fieldName) => new JObject().SelectTokens(fieldName.Spelling, false);
  }
}
