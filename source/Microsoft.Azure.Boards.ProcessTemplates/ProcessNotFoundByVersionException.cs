// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessNotFoundByVersionException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [Serializable]
  public class ProcessNotFoundByVersionException : ProcessNotFoundException
  {
    public ProcessNotFoundByVersionException(Guid typeId, int majorVersion, int minorVersion)
      : base(Resources.ProcessTemplateNotFoundByVersion((object) typeId, (object) majorVersion, (object) minorVersion), 402361)
    {
      this.TypeId = typeId;
      this.MajorVersion = majorVersion;
      this.MinorVersion = minorVersion;
    }

    public Guid TypeId { get; private set; }

    public int MajorVersion { get; private set; }

    public int MinorVersion { get; private set; }
  }
}
