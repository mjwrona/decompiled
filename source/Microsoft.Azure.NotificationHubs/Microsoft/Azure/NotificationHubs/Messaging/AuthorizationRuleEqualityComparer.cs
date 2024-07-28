// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRuleEqualityComparer
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal sealed class AuthorizationRuleEqualityComparer : EqualityComparer<AuthorizationRule>
  {
    public override bool Equals(AuthorizationRule x, AuthorizationRule y) => x != null && y != null && x.Equals((object) y);

    public override int GetHashCode(AuthorizationRule obj) => obj != null ? obj.GetHashCode() : throw new ArgumentNullException(nameof (obj));
  }
}
