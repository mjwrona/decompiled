// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TokenTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TokenTemplate
  {
    private const string c_area = "Security";
    private const string c_layer = "TokenTemplate";

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public JToken Value { get; set; }

    public IEnumerable<TokenAndContext> GetTokens(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      ISecurityTemplateTokenGeneratorExtension generatorExtension = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecurityTemplateService>().GetTokenGeneratorExtension(requestContext.ServiceHost.HostType, this.Type);
      if (generatorExtension == null)
      {
        requestContext.Trace(56290, TraceLevel.Error, "Security", nameof (TokenTemplate), string.Format("Unable to load token generator extension of type {0} for host type {1}", (object) this.Type, (object) requestContext.ServiceHost.HostType));
      }
      else
      {
        foreach (KeyValuePair<string, object> token1 in generatorExtension.GetTokens(requestContext, this.Value))
        {
          string token2 = token1.Key;
          if (token2 == null)
          {
            if (description.NamespaceStructure == SecurityNamespaceStructure.Flat)
              throw new FormatException();
            if (description.SeparatorValue == char.MinValue)
              token2 = string.Empty;
          }
          else
            token2 = SecurityServiceHelpers.CanonicalizeToken(description, token2);
          yield return new TokenAndContext(token2, token1.Value);
        }
      }
    }
  }
}
