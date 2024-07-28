// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessMetadataFile
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;
using System.IO;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessMetadataFile
  {
    public Guid ProcessTypeId { get; set; }

    public string ResourceName { get; set; }

    public ProcessMetadataResourceType ResourceType { get; set; }

    public Stream ResourceStream { get; set; }
  }
}
