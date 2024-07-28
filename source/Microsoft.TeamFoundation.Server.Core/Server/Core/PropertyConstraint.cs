// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.PropertyConstraint
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core
{
  public class PropertyConstraint
  {
    public PropertyConstraint()
    {
      this.PropertyExistence = PropertyExistence.Required;
      this.ValueExclusivity = PropertyValueExclusivity.None;
    }

    public string PropertyName { get; set; }

    public PropertyValueExclusivity ValueExclusivity { get; set; }

    public PropertyExistence PropertyExistence { get; set; }
  }
}
