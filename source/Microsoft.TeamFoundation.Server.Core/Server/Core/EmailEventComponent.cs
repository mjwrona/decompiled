// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.EmailEventComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class EmailEventComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<EmailEventComponent>(1, true),
      (IComponentCreator) new ComponentCreator<EmailEventComponent2>(2),
      (IComponentCreator) new ComponentCreator<EmailEventComponent3>(3),
      (IComponentCreator) new ComponentCreator<EmailEventComponent4>(4),
      (IComponentCreator) new ComponentCreator<EmailEventComponent5>(5),
      (IComponentCreator) new ComponentCreator<EmailEventComponent6>(6),
      (IComponentCreator) new ComponentCreator<EmailEventComponent7>(7),
      (IComponentCreator) new ComponentCreator<EmailEventComponent8>(8)
    }, "EmailEvent");
    private static readonly Dictionary<int, SqlExceptionFactory> s_exceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) EmailEventComponent.s_exceptionFactories;

    internal virtual void AddPendingEmailConfirmation(Guid tfId, string preferredEmailAddress) => throw new NotSupportedException();

    internal virtual ResultCollection QueryEmailConfirmations() => throw new NotSupportedException();

    internal virtual void EmailConfirmationSent(
      IEnumerable<PreferredEmailConfirmationEntry> sentConfirmations)
    {
      throw new NotSupportedException();
    }
  }
}
