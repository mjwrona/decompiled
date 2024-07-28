// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Demand
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [JsonConverter(typeof (DemandJsonConverter))]
  public abstract class Demand
  {
    private static readonly Regex s_demandRegex = new Regex("^(?<name>[^ ]+)([ ]+\\-(?<opcode>[^ ]+)[ ]+(?<value>.*))?$", RegexOptions.Compiled);

    protected Demand(string name, string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; private set; }

    public string Value { get; private set; }

    public override sealed bool Equals(object obj) => obj is Demand demand && demand.ToString().Equals(this.ToString(), StringComparison.OrdinalIgnoreCase);

    public override sealed int GetHashCode() => this.ToString().ToUpperInvariant().GetHashCode();

    public override sealed string ToString() => this.GetExpression();

    public abstract Demand Clone();

    protected abstract string GetExpression();

    internal static bool TryParse(string input, out Demand demand)
    {
      demand = (Demand) null;
      Match match = Demand.s_demandRegex.Match(input);
      if (!match.Success)
        return false;
      string name = match.Groups["name"].Value;
      string str1 = match.Groups["opcode"].Value;
      string str2 = match.Groups["value"].Value;
      if (string.IsNullOrEmpty(str1))
        demand = (Demand) new DemandExists(name);
      else if (str1 == "equals")
        demand = (Demand) new DemandEquals(name, str2);
      return demand != null;
    }
  }
}
