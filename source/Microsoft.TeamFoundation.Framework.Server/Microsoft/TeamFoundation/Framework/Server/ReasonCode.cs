// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReasonCode
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ReasonCode
  {
    public ReasonCode(int codeValue)
    {
      this.ThrottlingMode = (ThrottlingMode) (codeValue % 4);
      int num = codeValue / 256;
      this.ResourcesThrottled = (ResourceThrottled) (((num & 196608) >> 4 | num) & (int) ushort.MaxValue);
    }

    public List<KeyValuePair<SqlAzureResource, ResourceThrottlingType>> GetResourcesThrottled()
    {
      List<KeyValuePair<SqlAzureResource, ResourceThrottlingType>> resourcesThrottled = new List<KeyValuePair<SqlAzureResource, ResourceThrottlingType>>();
      foreach (SqlAzureResource sqlAzureResource in Enum.GetValues(typeof (SqlAzureResource)))
      {
        ResourceThrottlingType resourceThrottlingType = this[sqlAzureResource];
        if (resourceThrottlingType != ResourceThrottlingType.NotThrottled)
          resourcesThrottled.Add(new KeyValuePair<SqlAzureResource, ResourceThrottlingType>(sqlAzureResource, resourceThrottlingType));
      }
      return resourcesThrottled;
    }

    public override string ToString() => string.Format("Throttling Mode: {0}. Resource(s) Throttled:{1}", (object) this.ThrottlingMode, (object) this.GetString(this.GetResourcesThrottled()));

    private string GetString(
      List<KeyValuePair<SqlAzureResource, ResourceThrottlingType>> resourcesThrottled)
    {
      StringBuilder stringBuilder = new StringBuilder(1024);
      foreach (KeyValuePair<SqlAzureResource, ResourceThrottlingType> keyValuePair in resourcesThrottled)
        stringBuilder.AppendFormat(" [Resource: {0} Throttling Type: {1}]", (object) keyValuePair.Key, (object) keyValuePair.Value);
      return stringBuilder.ToString();
    }

    public ThrottlingMode ThrottlingMode { get; private set; }

    public ResourceThrottled ResourcesThrottled { get; private set; }

    public ResourceThrottlingType this[SqlAzureResource r] => (ResourceThrottlingType) ((int) this.ResourcesThrottled >> (int) (r & (SqlAzureResource) 31) & 3);
  }
}
