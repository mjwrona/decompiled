// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ArtifactPropertyKinds
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ArtifactPropertyKinds
  {
    public static readonly Guid Build = new Guid("{64533A40-F497-4313-BF79-5A83B0D9898F}");
    public static readonly Guid Definition = BuildPropertyKinds.BuildDefinition;
    public static readonly string[] AllProperties = new string[1]
    {
      "*"
    };

    public static IEnumerable<string> AsPropertyFilters(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (IEnumerable<string>) null;
      return ((IEnumerable<string>) value.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)));
    }

    public static int ToInt32(byte[] bytes) => ((int) bytes[0] << 24) + ((int) bytes[1] << 16) + ((int) bytes[2] << 8) + (int) bytes[3];

    public static IEnumerable<PropertyValue> Convert(this PropertiesCollection value) => value.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (x => new PropertyValue(x.Key, x.Value)));

    public static PropertiesCollection Convert(this IEnumerable<PropertyValue> value) => new PropertiesCollection((IDictionary<string, object>) value.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (x => x.PropertyName), (Func<PropertyValue, object>) (x => x.Value)));

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
