// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessInvalidOverrideException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [Serializable]
  public class ProcessInvalidOverrideException : ProcessServiceException
  {
    public ProcessInvalidOverrideException(
      ProcessDescriptor oldDescriptor,
      ProcessScope newScope,
      Guid newTypeId,
      ProcessVersion newVersion)
      : base(Resources.ProcessTemplateInvalidOverride((object) oldDescriptor.Scope, (object) oldDescriptor.TypeId, (object) oldDescriptor.Version.Major, (object) oldDescriptor.Version.Minor, (object) newScope, (object) newTypeId, (object) newVersion.Major, (object) newVersion.Minor), 402357)
    {
      this.OldScope = oldDescriptor.Scope;
      this.OldTypeId = oldDescriptor.TypeId;
      this.OldVersion = oldDescriptor.Version;
      this.NewScope = newScope;
      this.NewTypeId = newTypeId;
      this.NewVersion = newVersion;
    }

    public ProcessScope OldScope { get; set; }

    public Guid OldTypeId { get; set; }

    public ProcessVersion OldVersion { get; private set; }

    public ProcessScope NewScope { get; set; }

    public Guid NewTypeId { get; set; }

    public ProcessVersion NewVersion { get; private set; }
  }
}
