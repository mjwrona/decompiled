// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskDefinitionData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class MetaTaskDefinitionData
  {
    private TaskGroup _definition;

    internal Guid Id { get; set; }

    internal string Name { get; set; }

    internal string MetaTaskDocument { get; set; }

    internal int Revision { get; set; }

    internal Guid CreatedBy { get; set; }

    internal DateTime CreatedOn { get; set; }

    internal Guid ModifiedBy { get; set; }

    internal DateTime ModifiedOn { get; set; }

    internal TeamFoundationHostType HostType { get; set; }

    public int MajorVersion { get; set; }

    public Guid? ParentDefinitionId { get; set; }

    public bool Preview { get; set; }

    public bool Disabled { get; set; }

    public bool Deleted { get; set; }

    internal TaskGroup GetDefinition() => this.EnsureAndGetDefinition();

    private TaskGroup EnsureAndGetDefinition()
    {
      TaskGroup definition;
      if (this._definition == null)
      {
        this._definition = JsonConvert.DeserializeObject<TaskGroup>(this.MetaTaskDocument);
        this._definition.Revision = this.Revision;
        this._definition.CreatedBy = new IdentityRef()
        {
          Id = this.CreatedBy.ToString()
        };
        this._definition.CreatedOn = this.CreatedOn;
        this._definition.ModifiedBy = new IdentityRef()
        {
          Id = this.ModifiedBy.ToString()
        };
        this._definition.ModifiedOn = this.ModifiedOn;
        if (this._definition.Version == (TaskVersion) null)
          this._definition.Version = new TaskVersion(string.Format("{0}.0.0", (object) this.MajorVersion));
        else
          this._definition.Version.Major = this.MajorVersion;
        this._definition.Preview = this.Preview;
        this._definition.Disabled = this.Disabled;
        this._definition.Deleted = this.Deleted;
        this._definition.ParentDefinitionId = this.ParentDefinitionId;
        TaskVersion version = this._definition.Version;
        Guid? parentDefinitionId = this._definition.ParentDefinitionId;
        int num;
        if (parentDefinitionId.HasValue)
        {
          parentDefinitionId = this._definition.ParentDefinitionId;
          num = !parentDefinitionId.Equals((object) Guid.Empty) ? 1 : 0;
        }
        else
          num = 0;
        version.IsTest = num != 0;
        this._definition.Id = this.Id;
        definition = this._definition;
      }
      else
        definition = this._definition.Clone();
      return definition;
    }
  }
}
