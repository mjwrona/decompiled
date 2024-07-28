// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationXmlEventConditionParser
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TeamFoundationXmlEventConditionParser : TeamFoundationEventConditionParser
  {
    public TeamFoundationXmlEventConditionParser(
      string input,
      IReadOnlyList<IDynamicEventPredicate> dynamicEventPredicates = null,
      StringFieldMode stringFieldMode = StringFieldMode.Legacy,
      IVssRequestContext requestContext = null,
      Subscription subscription = null)
      : base(input, dynamicEventPredicates, stringFieldMode, requestContext, subscription)
    {
      this.m_scanner = new TeamFoundationEventConditionScanner(input, (Func<char, bool>) (x => char.IsLetter(x)));
    }

    public override EventSerializerType SerializerType => EventSerializerType.Xml;

    protected override void ValidateTokenName(Token fieldName)
    {
      if (fieldName.Spelling.Trim().IndexOf("customExpression:", StringComparison.OrdinalIgnoreCase) != -1)
      {
        NotificationXsltContext notificationXsltContext = new NotificationXsltContext(new NameTable());
        XPathExpression expr = XPathExpression.Compile(fieldName.QueriableValue, (IXmlNamespaceResolver) notificationXsltContext);
        expr.SetContext((XmlNamespaceManager) notificationXsltContext);
        new XmlDocument().CreateNavigator().Evaluate(expr);
      }
      else
        new XmlDocument().CreateNavigator().Evaluate(fieldName.QueriableValue, (IXmlNamespaceResolver) new TfsNamespaceResolver());
    }
  }
}
