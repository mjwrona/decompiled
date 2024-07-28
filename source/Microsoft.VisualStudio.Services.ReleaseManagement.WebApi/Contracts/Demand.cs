// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Demand
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (DemandEquals))]
  [KnownType(typeof (DemandExists))]
  [KnownType(typeof (DemandMinimumVersion))]
  [JsonConverter(typeof (DemandJsonConverter))]
  public abstract class Demand : ReleaseManagementSecuredObject
  {
    private static readonly Regex DemandRegex = new Regex("^(?<name>[^ ]+)([ ]+\\-(?<opcode>[^ ]+)[ ]+(?<value>.*))?$", RegexOptions.Compiled);

    public string Name { get; private set; }

    public string Value { get; private set; }

    protected Demand(string name, string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Name = name;
      this.Value = value;
    }

    public static bool TryParse(string input, out Demand demand)
    {
      demand = (Demand) null;
      Match match = Demand.DemandRegex.Match(input);
      if (!match.Success)
        return false;
      string name = match.Groups["name"].Value;
      string str1 = match.Groups["opcode"].Value;
      string str2 = match.Groups["value"].Value;
      if (string.IsNullOrEmpty(str1))
      {
        demand = (Demand) new DemandExists(name);
      }
      else
      {
        switch (str1)
        {
          case "equals":
            demand = (Demand) new DemandEquals(name, str2);
            break;
          case "gtVersion":
            demand = (Demand) new DemandMinimumVersion(name, str2);
            break;
        }
      }
      return demand != null;
    }

    public override sealed string ToString() => this.GetExpression();

    public override sealed bool Equals(object obj) => obj is Demand demand && demand.ToString().Equals(this.ToString(), StringComparison.OrdinalIgnoreCase);

    public override sealed int GetHashCode() => this.ToString().ToUpperInvariant().GetHashCode();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Signature needs to be in sync with DistributedTask Module")]
    protected abstract string GetExpression();
  }
}
