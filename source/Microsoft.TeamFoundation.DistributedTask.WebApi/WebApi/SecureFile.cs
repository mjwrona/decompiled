// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.SecureFile
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class SecureFile
  {
    [DataMember(EmitDefaultValue = false, Name = "Properties")]
    private IDictionary<string, string> m_properties;

    public SecureFile()
    {
    }

    private SecureFile(SecureFile secureFile, bool shallow = false)
    {
      this.Id = secureFile.Id;
      this.Name = secureFile.Name;
      this.Ticket = secureFile.Ticket;
      if (shallow)
        return;
      this.Properties = secureFile.Properties;
      this.CreatedBy = secureFile.CreatedBy;
      this.CreatedOn = secureFile.CreatedOn;
      this.ModifiedBy = secureFile.ModifiedBy;
      this.ModifiedOn = secureFile.ModifiedOn;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    public IDictionary<string, string> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_properties;
      }
      set => this.m_properties = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Ticket { get; set; }

    public SecureFile Clone() => new SecureFile(this);

    public SecureFile CloneShallow() => new SecureFile(this, true);
  }
}
