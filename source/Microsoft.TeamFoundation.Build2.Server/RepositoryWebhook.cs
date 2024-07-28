// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RepositoryWebhook
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class RepositoryWebhook
  {
    [DataMember(Name = "Types", EmitDefaultValue = false)]
    private List<DefinitionTriggerType> m_serializedTypes;
    private List<DefinitionTriggerType> m_types;

    public List<DefinitionTriggerType> Types
    {
      get
      {
        if (this.m_types == null)
          this.m_types = new List<DefinitionTriggerType>();
        return this.m_types;
      }
      set => this.m_types = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Url { get; set; }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<DefinitionTriggerType>(ref this.m_serializedTypes, ref this.m_types, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<DefinitionTriggerType>(ref this.m_types, ref this.m_serializedTypes);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedTypes = (List<DefinitionTriggerType>) null;
  }
}
