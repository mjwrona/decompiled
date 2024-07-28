// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.MapCase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public class MapCase : MapValues, IEquatable<MapCase>
  {
    [XmlAttribute("value")]
    public string Value { get; set; }

    public bool Equals(MapCase other) => other != null && StringComparer.OrdinalIgnoreCase.Equals(this.Value, other.Value) && this.Equals((MapValues) other);

    internal override void Validate(IRuleValidationContext validationHelper)
    {
      if (string.IsNullOrEmpty(this.Value))
        throw new InvalidRuleException("MapCase must have a case value.");
      base.Validate(validationHelper);
    }
  }
}
