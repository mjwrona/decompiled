// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class ServiceEndpointHelper
  {
    public static IList<string> AsPropertyFilters(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (IList<string>) null;
      return (IList<string>) ((IEnumerable<string>) value.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).ToList<string>();
    }

    public static void PatchEndpointSecrets(
      IDictionary<string, string> newValues,
      IDictionary<string, string> oldValues)
    {
      if (newValues == null || oldValues == null)
        return;
      foreach (KeyValuePair<string, string> oldValue in (IEnumerable<KeyValuePair<string, string>>) oldValues)
      {
        if (newValues.ContainsKey(oldValue.Key) && newValues[oldValue.Key] == null)
          newValues[oldValue.Key] = oldValues[oldValue.Key];
      }
    }

    public static List<Guid> GetEndpointIdsAsGuid(string endpointIds) => ServiceEndpointHelper.GetResourceIdsAsGuid(endpointIds, nameof (endpointIds));

    public static List<Guid> GetProjectIdsAsGuid(string projectIds) => ServiceEndpointHelper.GetResourceIdsAsGuid(projectIds, nameof (projectIds));

    private static List<Guid> GetResourceIdsAsGuid(string endpointIds, string resourceName)
    {
      List<Guid> resourceIdsAsGuid = (List<Guid>) null;
      IList<string> stringList1 = ServiceEndpointHelper.AsPropertyFilters(endpointIds);
      if (stringList1 != null)
      {
        resourceIdsAsGuid = new List<Guid>();
        List<string> stringList2 = new List<string>();
        foreach (string input in (IEnumerable<string>) stringList1)
        {
          Guid result;
          if (Guid.TryParse(input, out result))
            resourceIdsAsGuid.Add(result);
          else
            stringList2.Add(input);
        }
        if (stringList2.Any<string>())
          throw new ArgumentException(string.Format("{0}, " + resourceName + ": {1}", (object) TFCommonResources.EntityModel_BadGuidFormat(), (object) string.Join(", ", (IEnumerable<string>) stringList2)));
      }
      return resourceIdsAsGuid;
    }
  }
}
