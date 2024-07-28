// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DemandMinimumVersion
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class DemandMinimumVersion : Demand
  {
    public DemandMinimumVersion(string name, string value)
      : base(name, value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
    }

    public DemandMinimumVersion(string name, string value, DemandSource source)
      : base(name, value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      this.Source = source;
    }

    [DataMember(EmitDefaultValue = false)]
    public DemandSource Source { get; private set; }

    public override Demand Clone() => (Demand) new DemandMinimumVersion(this.Name, this.Value, this.Source);

    protected override string GetExpression() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -gtVersion {1}", (object) this.Name, (object) this.Value);

    public override bool IsSatisfied(IDictionary<string, string> capabilities)
    {
      string semanticVersion2;
      return capabilities.TryGetValue(this.Name, out semanticVersion2) && DemandMinimumVersion.CompareVersion(this.Value, semanticVersion2) <= 0;
    }

    public static int CompareVersion(string semanticVersion1, string semanticVersion2)
    {
      Version version1 = DemandMinimumVersion.ParseVersion(semanticVersion1);
      Version version2 = DemandMinimumVersion.ParseVersion(semanticVersion2);
      if (version1 == (Version) null && version2 == (Version) null)
        return 0;
      if (version1 == (Version) null)
        return -1;
      return version2 == (Version) null ? 1 : version1.CompareTo(version2);
    }

    public static DemandMinimumVersion MaxAndRemove(ISet<Demand> demands)
    {
      DemandMinimumVersion demandMinimumVersion1 = (DemandMinimumVersion) null;
      foreach (DemandMinimumVersion demandMinimumVersion2 in demands.Where<Demand>((Func<Demand, bool>) (x => x.Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase))).OfType<DemandMinimumVersion>().ToList<DemandMinimumVersion>())
      {
        if (demandMinimumVersion1 == null || DemandMinimumVersion.CompareVersion(demandMinimumVersion2.Value, demandMinimumVersion1.Value) > 0)
          demandMinimumVersion1 = demandMinimumVersion2;
        demands.Remove((Demand) demandMinimumVersion2);
      }
      return demandMinimumVersion1;
    }

    public static DemandMinimumVersion Max(IEnumerable<Demand> demands)
    {
      DemandMinimumVersion demandMinimumVersion1 = (DemandMinimumVersion) null;
      foreach (DemandMinimumVersion demandMinimumVersion2 in demands.Where<Demand>((Func<Demand, bool>) (x => x.Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase))).OfType<DemandMinimumVersion>())
      {
        if (demandMinimumVersion1 == null || DemandMinimumVersion.CompareVersion(demandMinimumVersion2.Value, demandMinimumVersion1.Value) > 0)
          demandMinimumVersion1 = demandMinimumVersion2;
      }
      return demandMinimumVersion1;
    }

    public static Version ParseVersion(string versionString)
    {
      Version result = (Version) null;
      if (!string.IsNullOrEmpty(versionString))
      {
        int length = versionString.IndexOf('-');
        if (length > 0)
          versionString = versionString.Substring(0, length);
        if (!Version.TryParse(versionString, out result))
          result = (Version) null;
      }
      return result;
    }
  }
}
