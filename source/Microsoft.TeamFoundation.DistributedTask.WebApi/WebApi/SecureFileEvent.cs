// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.SecureFileEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class SecureFileEvent
  {
    public SecureFileEvent(string eventType, IEnumerable<SecureFile> secureFiles, Guid projectId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      ArgumentUtility.CheckForNull<IEnumerable<SecureFile>>(secureFiles, nameof (secureFiles));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      this.EventType = eventType;
      this.SecureFiles = secureFiles;
      this.ProjectId = projectId;
    }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public IEnumerable<SecureFile> SecureFiles { get; set; }

    [DataMember]
    public Guid ProjectId { get; set; }
  }
}
