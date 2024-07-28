// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileIdentifier
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileIdentifier
  {
    private readonly Guid BadGuid = new Guid("deadbeef-dead-beef-dead-beefdeadbeef");
    public string m_string;

    public FileIdentifier(long fileId) => this.FileId = fileId;

    public FileIdentifier(long fileId, Guid dataspaceIdentifier)
    {
      this.FileId = fileId;
      this.DataspaceIdentifier = new Guid?(dataspaceIdentifier);
    }

    public FileIdentifier(long fileId, Guid dataspaceIdentifier, Microsoft.TeamFoundation.Framework.Server.OwnerId ownerId)
    {
      this.FileId = fileId;
      this.DataspaceIdentifier = new Guid?(dataspaceIdentifier);
      this.OwnerId = new Microsoft.TeamFoundation.Framework.Server.OwnerId?(ownerId);
    }

    public override string ToString()
    {
      if (this.m_string == null)
      {
        // ISSUE: variable of a boxed type
        __Boxed<Guid?> local1 = (ValueType) (this.DataspaceIdentifier.HasValue ? this.DataspaceIdentifier : new Guid?(this.BadGuid));
        Microsoft.TeamFoundation.Framework.Server.OwnerId? ownerId = this.OwnerId;
        int num;
        if (!ownerId.HasValue)
        {
          num = 0;
        }
        else
        {
          ownerId = this.OwnerId;
          num = (int) ownerId.Value;
        }
        // ISSUE: variable of a boxed type
        __Boxed<int> local2 = (ValueType) num;
        // ISSUE: variable of a boxed type
        __Boxed<long> fileId = (ValueType) this.FileId;
        this.m_string = string.Format("{0}:{1}:{2}", (object) local1, (object) local2, (object) fileId);
      }
      return this.m_string;
    }

    public long FileId { get; set; }

    public Microsoft.TeamFoundation.Framework.Server.OwnerId? OwnerId { get; private set; }

    public Guid? DataspaceIdentifier { get; private set; }
  }
}
