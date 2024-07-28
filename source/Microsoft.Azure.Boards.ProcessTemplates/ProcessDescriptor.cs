// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessDescriptor
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public abstract class ProcessDescriptor
  {
    private static readonly DateTime FutureDateTimeValue = new DateTime(3155063616000000000L, DateTimeKind.Utc);

    public Guid TypeId { get; protected set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int IntegerId { get; protected set; }

    public virtual ProcessScope Scope { get; protected set; }

    public string Name { get; protected set; }

    public string Description { get; protected set; }

    public ProcessVersion Version { get; protected set; }

    public byte[] HashValue { get; protected set; }

    public int FileId { get; protected set; }

    public bool IsDeleted { get; protected set; }

    public string Plugins { get; protected set; }

    public DateTime RevisedDate { get; protected set; }

    public ProcessStatus ProcessStatus { get; protected set; }

    public string ReferenceName { get; protected set; }

    public virtual Guid Inherits { get; protected set; }

    public bool IsCustom => !this.IsSystem && !this.IsDerived;

    public bool IsDerived => this.Inherits != Guid.Empty;

    public bool IsSystem => this.Scope == ProcessScope.Deployment;

    public bool IsLatest => this.RevisedDate == ProcessDescriptor.FutureDateTimeValue || this.RevisedDate == DateTime.MaxValue;

    public Guid RowId { get; protected set; }

    public bool IsOverriddenBy(ProcessDescriptor other)
    {
      ArgumentUtility.CheckForNull<ProcessDescriptor>(other, nameof (other));
      return this.Scope == other.Scope && this.TypeId == other.TypeId;
    }
  }
}
