// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models
{
  [Serializable]
  public class CloudError : ISerializable
  {
    public string Code { get; set; }

    public string Message { get; set; }

    public string Target { get; set; }

    public List<CloudError> Details { get; } = new List<CloudError>();

    public CloudError()
    {
    }

    protected CloudError(SerializationInfo info, StreamingContext context)
    {
      this.Code = info.GetString(nameof (Code));
      this.Message = info.GetString(nameof (Message));
      this.Target = info.GetString(nameof (Target));
      this.Details = (List<CloudError>) info.GetValue(nameof (Details), typeof (List<CloudError>));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Code", (object) this.Code);
      info.AddValue("Message", (object) this.Message);
      info.AddValue("Target", (object) this.Target);
      info.AddValue("Details", (object) this.Details);
    }

    public static CloudError CreateFrom(Microsoft.Rest.Azure.CloudError azureError)
    {
      if (azureError == null)
        return (CloudError) null;
      CloudError from = new CloudError();
      from.Code = azureError.Code;
      from.Message = azureError.Message;
      from.Target = azureError.Target;
      from.Details.AddRange(azureError.Details.Select<Microsoft.Rest.Azure.CloudError, CloudError>((Func<Microsoft.Rest.Azure.CloudError, CloudError>) (detail => CloudError.CreateFrom(detail))));
      return from;
    }
  }
}
