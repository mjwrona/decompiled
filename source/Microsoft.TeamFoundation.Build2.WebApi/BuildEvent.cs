// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildEvent
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class BuildEvent
  {
    [DataMember(Name = "Data", EmitDefaultValue = false)]
    private List<string> m_data;

    public BuildEvent(string identifier)
      : this(identifier, (string) null)
    {
    }

    public BuildEvent(string identifier, string data)
      : this(identifier, (IList<string>) new string[1]
      {
        data
      })
    {
      this.Identifier = identifier;
      if (data == null)
        return;
      this.Data.Add(data);
    }

    public BuildEvent(string identifier, IList<string> data)
    {
      this.Identifier = identifier;
      if (data == null || data.Count <= 0)
        return;
      this.Data.AddRange<string, IList<string>>((IEnumerable<string>) data);
    }

    [DataMember]
    public string Identifier { get; private set; }

    public IList<string> Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = new List<string>();
        return (IList<string>) this.m_data;
      }
    }
  }
}
