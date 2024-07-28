// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ArtifactHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class ArtifactHelper
  {
    public static ArtifactSpec CreateEmptyArtifactSpec(
      Guid artifactKind,
      int index,
      Guid projectId = default (Guid))
    {
      int id = index;
      return new ArtifactSpec(artifactKind, id, 0, projectId);
    }

    public static T GetValue<T>(
      this IDictionary<string, object> values,
      string name,
      T defaultValue = null)
    {
      object obj1;
      return values.TryGetValue(name, out obj1) && obj1 is T obj2 ? obj2 : defaultValue;
    }

    public static ArtifactSpec CreateArtifactSpec(string uri, Guid artifactKind = default (Guid), Guid projectId = default (Guid))
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(uri);
      if (artifactKind == Guid.Empty)
      {
        if (VssStringComparer.ArtifactType.Equals("Agent", artifactId.ArtifactType))
          artifactKind = BuildPropertyKinds.BuildAgent;
        else if (VssStringComparer.ArtifactType.Equals("Controller", artifactId.ArtifactType))
          artifactKind = BuildPropertyKinds.BuildController;
        else if (VssStringComparer.ArtifactType.Equals("Definition", artifactId.ArtifactType))
          artifactKind = BuildPropertyKinds.BuildDefinition;
        else if (VssStringComparer.ArtifactType.Equals("ServiceHost", artifactId.ArtifactType))
          artifactKind = BuildPropertyKinds.BuildServiceHost;
      }
      int result;
      return artifactKind != Guid.Empty && int.TryParse(artifactId.ToolSpecificId, out result) ? new ArtifactSpec(artifactKind, result, 0, projectId) : (ArtifactSpec) null;
    }

    public static string EncodeUri(string type, ArtifactSpec spec)
    {
      int int32 = ArtifactHelper.ToInt32(spec.Id);
      return LinkingUtilities.EncodeUri(new ArtifactId()
      {
        ToolSpecificId = int32.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        ArtifactType = type,
        Tool = "Build"
      });
    }

    public static int GetArtifactId(string uri) => int.Parse(LinkingUtilities.DecodeUri(uri).ToolSpecificId);

    public static int ToInt32(byte[] bytes) => ((int) bytes[0] << 24) + ((int) bytes[1] << 16) + ((int) bytes[2] << 8) + (int) bytes[3];

    public static void MatchProperties<T>(
      TeamFoundationDataReader reader,
      IList<T> outputs,
      Func<T, int> getArtifactId,
      Action<T, List<PropertyValue>> setProperties)
    {
      int index = 0;
      foreach (ArtifactPropertyValue current in reader.CurrentEnumerable<ArtifactPropertyValue>())
      {
        int int32 = ArtifactHelper.ToInt32(current.Spec.Id);
        int num;
        for (num = getArtifactId(outputs[index]); num != int32; num = getArtifactId(outputs[index]))
          ++index;
        if (int32 == num)
          setProperties(outputs[index++], current.PropertyValues.ToList<PropertyValue>());
      }
    }
  }
}
