// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessDescriptorImpl
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public sealed class ProcessDescriptorImpl : ProcessDescriptor
  {
    public ProcessDescriptorImpl(ProcessTemplateDescriptorEntry entry)
    {
      this.RowId = entry.Id;
      this.TypeId = entry.TypeId;
      this.Name = entry.Name;
      this.Description = entry.Description;
      this.RevisedDate = entry.RevisedDate;
      this.HashValue = entry.HashValue;
      this.FileId = entry.FileId;
      this.Scope = entry.Scope;
      this.Version = ProcessVersion.Create(entry.MajorVersion, entry.MinorVersion);
      this.IntegerId = entry.IntegerId;
      this.IsDeleted = entry.IsDeleted;
      this.ProcessStatus = entry.ProcessStatus;
      this.Inherits = entry.Inherits;
      this.ReferenceName = entry.ReferenceName;
    }

    public ProcessDescriptorImpl(
      ProcessDescriptor descriptorToCopy,
      ProcessStatus status,
      string name = null)
    {
      this.RowId = descriptorToCopy.RowId;
      this.TypeId = descriptorToCopy.TypeId;
      this.Name = name ?? descriptorToCopy.Name;
      this.Description = descriptorToCopy.Description;
      this.RevisedDate = descriptorToCopy.RevisedDate;
      this.HashValue = descriptorToCopy.HashValue;
      this.FileId = descriptorToCopy.FileId;
      this.Scope = descriptorToCopy.Scope;
      this.Version = descriptorToCopy.Version;
      this.IntegerId = descriptorToCopy.IntegerId;
      this.IsDeleted = descriptorToCopy.IsDeleted;
      this.ProcessStatus = status;
      this.Inherits = descriptorToCopy.Inherits;
      this.ReferenceName = descriptorToCopy.ReferenceName;
    }
  }
}
