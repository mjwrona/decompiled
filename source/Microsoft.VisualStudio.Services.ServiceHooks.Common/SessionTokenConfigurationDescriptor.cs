// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.SessionTokenConfigurationDescriptor
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class SessionTokenConfigurationDescriptor
  {
    public Guid ClientId { get; set; }

    public string Scope { get; set; }

    public int TimeoutMinutes { get; set; }

    public bool Required { get; set; }

    public SessionTokenType TokenType { get; set; }

    public Func<IVssRequestContext, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, string> TokenNameBuilder { get; set; }
  }
}
