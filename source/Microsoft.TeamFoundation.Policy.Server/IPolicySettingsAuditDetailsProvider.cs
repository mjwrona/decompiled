// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.IPolicySettingsAuditDetailsProvider
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public interface IPolicySettingsAuditDetailsProvider
  {
    Dictionary<string, JToken> SummarizePolicySettings(
      IVssRequestContext requestContext,
      object settings);

    Dictionary<string, JToken> SummarizePolicyScope(
      IVssRequestContext requestContext,
      object settings);

    string AuditDisplayName { get; }
  }
}
