// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsUserContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class TfsUserContext : UserContext
  {
    private IVssRequestContext m_requestContext;
    private bool? m_limitedAccess;

    public TfsUserContext(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
      : base(requestContext, identity)
    {
      this.m_requestContext = requestContext;
    }

    public override bool LimitedAccess
    {
      get
      {
        if (!this.m_limitedAccess.HasValue)
          this.m_limitedAccess = new bool?(!this.m_requestContext.FeatureContext().AreStandardFeaturesAvailable);
        return this.m_limitedAccess.Value;
      }
    }
  }
}
