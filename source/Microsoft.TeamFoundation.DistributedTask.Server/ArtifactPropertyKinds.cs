// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ArtifactPropertyKinds
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class ArtifactPropertyKinds
  {
    public static readonly Guid TaskAgent = new Guid("{43893EEF-C7CA-4530-8E52-98281C1733F7}");
    public static readonly Guid TaskAgentPool = new Guid("{F2417AA6-775E-4C34-BF8B-AC127FC300D2}");
    public static readonly Guid DeploymentTarget = new Guid("{7C74BCCB-AA43-42A2-91A5-5144779611C3}");

    public static IList<string> AsPropertyFilters(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (IList<string>) null;
      return (IList<string>) ((IEnumerable<string>) value.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).ToList<string>();
    }

    public static PropertiesCollection Convert(this IEnumerable<PropertyValue> value) => new PropertiesCollection((IDictionary<string, object>) value.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (x => x.PropertyName), (Func<PropertyValue, object>) (x => x.Value)));

    public static IEnumerable<PropertyValue> Convert(this PropertiesCollection value) => value.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (x => new PropertyValue(x.Key, x.Value)));

    public static ArtifactSpec CreateSpec(this Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgent value) => new ArtifactSpec(ArtifactPropertyKinds.TaskAgent, value.Id, 0);

    public static ArtifactSpec CreateSpec(this Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPool value) => new ArtifactSpec(ArtifactPropertyKinds.TaskAgentPool, value.Id, 0);

    public static ArtifactSpec CreateSpec(this DeploymentMachine value) => new ArtifactSpec(ArtifactPropertyKinds.DeploymentTarget, value.Id, 0);

    public static int ToInt32(byte[] bytes) => ((int) bytes[0] << 24) + ((int) bytes[1] << 16) + ((int) bytes[2] << 8) + (int) bytes[3];

    public static void MatchProperties<T>(
      TeamFoundationDataReader reader,
      IList<T> outputs,
      Func<T, int> getArtifactId,
      Action<T, PropertiesCollection> setProperties)
    {
      int index = 0;
      foreach (ArtifactPropertyValue current in reader.CurrentEnumerable<ArtifactPropertyValue>())
      {
        int int32 = ArtifactPropertyKinds.ToInt32(current.Spec.Id);
        int num;
        for (num = getArtifactId(outputs[index]); num != int32; num = getArtifactId(outputs[index]))
          ++index;
        if (int32 == num)
          setProperties(outputs[index++], current.PropertyValues.Convert());
      }
    }
  }
}
