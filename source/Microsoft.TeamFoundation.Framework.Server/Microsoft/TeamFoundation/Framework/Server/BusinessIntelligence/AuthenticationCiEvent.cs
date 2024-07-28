// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.AuthenticationCiEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public class AuthenticationCiEvent : CiEvent
  {
    public const string SourceParam = "Source";
    public const string FlowParam = "Flow";

    public AuthenticationCiEvent.Sources Source
    {
      get
      {
        int source = 0;
        this.Properties.TryGetValue<int>(nameof (Source), out source);
        return (AuthenticationCiEvent.Sources) source;
      }
      set => this.Properties[nameof (Source)] = (object) (int) value;
    }

    public AuthenticationCiEvent.Flows Flow
    {
      get
      {
        int flow = 0;
        this.Properties.TryGetValue<int>(nameof (Flow), out flow);
        return (AuthenticationCiEvent.Flows) flow;
      }
      set => this.Properties[nameof (Flow)] = (object) (int) value;
    }

    public AuthenticationCiEvent(string feature)
      : this(feature, AuthenticationCiEvent.Sources.Unknown, AuthenticationCiEvent.Flows.Unknown)
    {
    }

    public AuthenticationCiEvent(
      string feature,
      AuthenticationCiEvent.Sources source,
      AuthenticationCiEvent.Flows flow)
      : base(CustomerIntelligenceArea.Authentication, feature)
    {
      this.Source = source;
      this.Flow = flow;
    }

    public enum Sources
    {
      Unknown,
      Web,
      Ide,
    }

    public enum Flows
    {
      Unknown,
      Aad,
      Msa,
    }
  }
}
