// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateDescriptorEntry
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessTemplateDescriptorEntry
  {
    public Guid Id { get; set; }

    public Guid TypeId { get; set; }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public Guid Inherits { get; set; }

    public string Description { get; set; }

    public int MinorVersion { get; set; }

    public int MajorVersion { get; set; }

    public int FileId { get; set; }

    public byte[] HashValue { get; set; }

    public int IntegerId { get; set; }

    public ProcessScope Scope { get; set; }

    public string ServiceLevel { get; set; }

    public DateTime RevisedDate { get; set; }

    public bool IsDeleted { get; set; }

    public ProcessStatus ProcessStatus { get; set; }
  }
}
