// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.HeaderItemContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public abstract class HeaderItemContext : WebSdkMetadata
  {
    public HeaderItemContext(int order)
    {
      this.Available = true;
      this.Order = order;
    }

    [DataMember]
    public bool Available { get; protected set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, HeaderAction> Actions { get; private set; }

    public int Order { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, object> Properties { get; private set; }

    protected virtual IDictionary<string, object> GetExtraProperties(
      IVssRequestContext requestContext)
    {
      return (IDictionary<string, object>) null;
    }

    protected virtual void AddAction(string name, HeaderAction action)
    {
      if (action == null)
        return;
      if (this.Actions == null)
        this.Actions = (IDictionary<string, HeaderAction>) new Dictionary<string, HeaderAction>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Actions.Add(name, action);
    }

    public virtual void AddServerContribution(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
    }

    public virtual void AddActions(IVssRequestContext requestContext)
    {
    }

    public virtual void AddExtraProperties(IVssRequestContext requestContext) => this.Properties = this.GetExtraProperties(requestContext);
  }
}
