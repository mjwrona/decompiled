// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.RoleBasedExpression
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class RoleBasedExpression
  {
    public HashSet<string> Inclusions { get; set; }

    public HashSet<string> Exclusions { get; set; }

    public string Condition { get; set; }

    public string Serialize() => JsonConvert.SerializeObject((object) this, NotificationsSerialization.JsonSerializerSettings).Replace("\\u0007", '\a'.ToString()).Replace("\\u000b", '\v'.ToString());

    public void Validate()
    {
      if (this.Inclusions == null || this.Inclusions.Count == 0)
        throw new InvalidRoleBasedExpressionException(CoreRes.DefaultSubscriptionMustHaveInclusions());
    }

    public static RoleBasedExpression Deserialize(string expression)
    {
      RoleBasedExpression roleBasedExpression = JsonConvert.DeserializeObject<RoleBasedExpression>(expression, NotificationsSerialization.JsonSerializerSettings);
      if (roleBasedExpression == null)
        throw new InvalidRoleBasedExpressionException(CoreRes.InvalidRoleBasedExpression((object) expression));
      roleBasedExpression.Validate();
      return roleBasedExpression;
    }
  }
}
