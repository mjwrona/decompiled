// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Demand
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [JsonConverter(typeof (DemandJsonConverter))]
  public abstract class Demand
  {
    private static readonly Regex s_demandRegex = new Regex("^(?<name>\\S+)(\\s+\\-(?<opcode>\\S+)\\s+(?<value>.*))?$", RegexOptions.Compiled);

    protected Demand(string name, string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Name = name;
      this.Value = value;
    }

    [DataMember]
    public string Name { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public string Value { get; private set; }

    public override sealed bool Equals(object obj) => obj is Demand demand && demand.ToString().Equals(this.ToString(), StringComparison.OrdinalIgnoreCase);

    public override sealed int GetHashCode() => this.ToString().ToUpperInvariant().GetHashCode();

    public override sealed string ToString() => this.GetExpression();

    public abstract Demand Clone();

    protected abstract string GetExpression();

    public abstract bool IsSatisfied(IDictionary<string, string> capabilities);

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

    public void Update(string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      this.Value = value;
    }
  }
}
