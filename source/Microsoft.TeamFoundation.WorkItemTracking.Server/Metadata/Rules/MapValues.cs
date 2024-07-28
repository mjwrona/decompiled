// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.MapValues
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public class MapValues : IEquatable<MapValues>
  {
    [XmlElement("value", typeof (string))]
    public string[] Values { get; set; }

    [XmlElement("default", typeof (string))]
    public string Default { get; set; }

    public bool Equals(MapValues other)
    {
      if (other != null && StringComparer.OrdinalIgnoreCase.Equals(this.Default, other.Default))
      {
        if (this.Values != null && ((IEnumerable<string>) this.Values).Any<string>())
          return other.Values != null && ((IEnumerable<string>) other.Values).Any<string>() && ((IEnumerable<string>) this.Values).OrderBy<string, string>((Func<string, string>) (v => v), (IComparer<string>) StringComparer.OrdinalIgnoreCase).SequenceEqual<string>((IEnumerable<string>) ((IEnumerable<string>) other.Values).OrderBy<string, string>((Func<string, string>) (v => v), (IComparer<string>) StringComparer.OrdinalIgnoreCase));
        if (other.Values != null)
        {
          ((IEnumerable<string>) other.Values).Any<string>();
          return false;
        }
      }
      return false;
    }

    internal virtual void Validate(IRuleValidationContext validationHelper)
    {
      if (!string.IsNullOrEmpty(this.Default) && (this.Values == null || !((IEnumerable<string>) this.Values).Contains<string>(this.Default, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)))
        throw new InvalidRuleException("When default value is present, the Values list must contain the default value.");
    }
  }
}
