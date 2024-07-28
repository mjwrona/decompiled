// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseManagementArtifactPropertyKinds
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ReleaseManagementArtifactPropertyKinds
  {
    public static readonly Guid Release = new Guid("{9DA2EDC3-D038-46A2-A380-E745EE66CB5B}");
    public static readonly Guid ReleaseDefinition = new Guid("{0BF6E1B7-D5A5-432F-8656-C470DE22635A}");
    public static readonly Guid DefinitionEnvironment = new Guid("{49C45CDA-A5F7-4DBD-8036-57FAC252A93D}");

    public static IEnumerable<string> AsPropertyFilters(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (IEnumerable<string>) null;
      return ((IEnumerable<string>) value.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)));
    }

    public static int ToInt32(byte[] value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return ((int) value[0] << 24) + ((int) value[1] << 16) + ((int) value[2] << 8) + (int) value[3];
    }

    public static IEnumerable<PropertyValue> Convert(this PropertiesCollection value) => value.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (x => new PropertyValue(x.Key, x.Value)));

    public static PropertiesCollection Convert(this IEnumerable<PropertyValue> value) => new PropertiesCollection((IDictionary<string, object>) value.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (x => x.PropertyName), (Func<PropertyValue, object>) (x => x.Value)));

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "As per design")]
    public static void MatchProperties<T>(
      TeamFoundationDataReader reader,
      IList<T> outputs,
      Func<T, int> getArtifactId,
      Action<T, IList<PropertyValue>> setProperties)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (outputs == null)
        throw new ArgumentNullException(nameof (outputs));
      if (getArtifactId == null)
        throw new ArgumentNullException(nameof (getArtifactId));
      if (setProperties == null)
        throw new ArgumentNullException(nameof (setProperties));
      int index = 0;
      foreach (ArtifactPropertyValue current in reader.CurrentEnumerable<ArtifactPropertyValue>())
      {
        int int32 = ReleaseManagementArtifactPropertyKinds.ToInt32(current.Spec.Id);
        int num;
        for (num = getArtifactId(outputs[index]); num != int32; num = getArtifactId(outputs[index]))
          ++index;
        if (int32 == num)
          setProperties(outputs[index++], (IList<PropertyValue>) current.PropertyValues.ToList<PropertyValue>());
      }
    }

    public static void CopyProperties(PropertiesCollection destination, PropertiesCollection source)
    {
      if (destination == null)
        destination = new PropertiesCollection();
      if (source == null || !source.Any<KeyValuePair<string, object>>())
        return;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) source)
        destination.TryAdd<string, object>(keyValuePair.Key, keyValuePair.Value);
    }

    public static void CopyProperties(IList<PropertyValue> destination, PropertiesCollection source)
    {
      if (destination == null)
        destination = (IList<PropertyValue>) new List<PropertyValue>();
      if (source == null || !source.Any<KeyValuePair<string, object>>())
        return;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) source)
        destination.Add(new PropertyValue(keyValuePair.Key, keyValuePair.Value));
    }

    public static void CopyProperties(PropertiesCollection destination, IList<PropertyValue> source)
    {
      if (destination == null)
        destination = new PropertiesCollection();
      if (source == null || !source.Any<PropertyValue>())
        return;
      foreach (PropertyValue propertyValue in (IEnumerable<PropertyValue>) source)
        destination.TryAdd<string, object>(propertyValue.PropertyName, propertyValue.Value);
    }

    public static void CopyProperties(IList<PropertyValue> destination, IList<PropertyValue> source)
    {
      if (destination == null)
        destination = (IList<PropertyValue>) new List<PropertyValue>();
      else
        destination.Clear();
      if (source == null || !source.Any<PropertyValue>())
        return;
      foreach (PropertyValue propertyValue in (IEnumerable<PropertyValue>) source)
        destination.Add(propertyValue);
    }
  }
}
