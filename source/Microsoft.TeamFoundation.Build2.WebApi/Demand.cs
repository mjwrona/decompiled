// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Demand
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [JsonConverter(typeof (DemandJsonConverter))]
  public abstract class Demand : BaseSecuredObject
  {
    private static readonly Regex s_demandRegex = new Regex("^(?<name>[^ ]+)([ ]+\\-(?<opcode>[^ ]+)[ ]+(?<value>.*))?$", RegexOptions.Compiled);

    protected Demand(string name, string value)
      : this(name, value, (ISecuredObject) null)
    {
    }

    protected Demand(string name, string value, ISecuredObject securedObject)
      : base(securedObject)
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

    public static bool TryParse(string input, out Demand demand)
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
