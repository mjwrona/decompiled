// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ProcessDescriptorModel
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [DataContract]
  public class ProcessDescriptorModel : IComparable<ProcessDescriptorModel>
  {
    public Guid TemplateTypeId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public string Version { get; set; }

    public string Description { get; set; }

    public bool IsDefault { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsSystemTemplate { get; set; }

    public bool IsInherited { get; set; }

    public bool EditPermission { get; set; }

    public bool DeletePermission { get; set; }

    public bool CreatePermission { get; set; }

    public ProcessStatus Status { get; set; }

    public int SubscribedProjectCount { get; set; }

    public Guid Inherits { get; set; }

    public int DerivedProcessCount { get; set; }

    public int CompareTo(ProcessDescriptorModel other) => this.IsSystemTemplate ? (!other.IsSystemTemplate ? -1 : this.Name.CompareTo(other.Name)) : (!other.IsSystemTemplate ? this.Name.CompareTo(other.Name) : 1);
  }
}
