// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationControl
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class TeamFoundationControl : UserControl
  {
    private TeamFoundationApplication m_teamFoundationApplication;
    private IVssRequestContext m_requestContext;

    protected TeamFoundationControl()
    {
      this.m_teamFoundationApplication = HttpContext.Current.ApplicationInstance as TeamFoundationApplication;
      this.m_requestContext = this.m_teamFoundationApplication.VssRequestContext;
    }

    protected IVssRequestContext RequestContext => this.m_requestContext;
  }
}
