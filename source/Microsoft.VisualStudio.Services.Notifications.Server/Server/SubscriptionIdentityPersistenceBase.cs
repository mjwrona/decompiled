// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionIdentityPersistenceBase
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class SubscriptionIdentityPersistenceBase : ISubscriptionPersistence
  {
    protected ProcessTreeContext m_context;
    protected const char c_processedFlagCharacter = '\a';
    protected const char c_processedTfIdFlagCharacter = '\v';
    protected const string c_null = "null";

    public abstract bool IsSupportedEventType(string evenType);

    protected abstract bool IsIdentityField(string fieldName);

    public void AfterReadSubscription(IVssRequestContext requestContext, Subscription subscription)
    {
      if (!this.IsSupportedEventType(subscription.EventTypeName))
        return;
      this.m_context = new ProcessTreeContext(requestContext, subscription, true);
      this.TranformSubscription();
    }

    public void BeforeWriteSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      if (!this.IsSupportedEventType(subscription.EventTypeName))
        return;
      this.m_context = new ProcessTreeContext(requestContext, subscription, false);
      this.TranformSubscription();
    }

    private void TranformSubscription()
    {
      Condition condition = new TeamFoundationXmlEventConditionParser(this.m_context.Subscription.ConditionString).Parse();
      this.ProcessTree(condition);
      this.m_context.Subscription.ConditionString = condition.ToString();
    }

    protected virtual void TranslateDisplayNameToId(StringFieldCondition condition)
    {
      string str = condition.Target.Spelling.Trim('\a', '\v');
      if (str.Equals("null", StringComparison.OrdinalIgnoreCase))
        return;
      Guid id = this.TranslateDisplayNameToId(str.Trim());
      if (!(id != Guid.Empty))
        return;
      condition.Target = (Token) new XPathToken(id.ToString());
    }

    protected Guid TranslateDisplayNameToId(string displayName)
    {
      Guid id = Guid.Empty;
      if (!string.IsNullOrEmpty(displayName) && displayName != "@@MyDisplayName@@" && !this.m_context.DisplayNameMap.TryGetValue(displayName, out id))
      {
        IdentityService service = this.m_context.IVssRequestContext.GetService<IdentityService>();
        try
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(this.m_context.IVssRequestContext, IdentitySearchFilter.DisplayName, displayName, QueryMembership.None, (IEnumerable<string>) null);
          if (identityList.Count > 0)
          {
            if (identityList[0] != null)
              id = identityList[0].Id;
          }
        }
        catch (MultipleIdentitiesFoundException ex)
        {
          TeamFoundationTrace.Info(ex.ToString());
        }
      }
      return id;
    }

    protected virtual void TranslateIdToDisplayName(StringFieldCondition condition)
    {
      string tfIdString = condition.Target.Spelling.Trim('\a', '\v');
      if (tfIdString.Equals("null", StringComparison.OrdinalIgnoreCase))
        return;
      string displayName = this.TranslateIdToDisplayName(tfIdString);
      if (string.IsNullOrEmpty(displayName))
        return;
      string spelling = displayName;
      condition.Target = (Token) new XPathToken(spelling);
    }

    protected string TranslateIdToDisplayName(string tfIdString)
    {
      string displayName = (string) null;
      Guid result;
      if (Guid.TryParse(tfIdString, out result) && !this.m_context.TeamFoundationIdMap.TryGetValue(result, out displayName))
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_context.IVssRequestContext.GetService<IdentityService>().ReadIdentities(this.m_context.IVssRequestContext, (IList<Guid>) new Guid[1]
        {
          result
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (identityList[0] != null)
          displayName = identityList[0].DisplayName;
        this.m_context.TeamFoundationIdMap[result] = displayName;
      }
      return displayName;
    }

    protected virtual void ProcessTree(Condition condition)
    {
      AndCondition andCondition = condition as AndCondition;
      OrCondition orCondition = condition as OrCondition;
      NotCondition notCondition = condition as NotCondition;
      StringFieldCondition condition1 = condition as StringFieldCondition;
      if (andCondition != null)
      {
        this.ProcessTree(andCondition.condition1);
        this.ProcessTree(andCondition.condition2);
      }
      else if (orCondition != null)
      {
        this.ProcessTree(orCondition.Condition1);
        this.ProcessTree(orCondition.Condition2);
      }
      else if (notCondition != null)
      {
        this.ProcessTree(notCondition.condition);
      }
      else
      {
        if (condition1 == null || !this.TryGetIdentityFieldNameFromPath(condition1, out string _))
          return;
        if (this.m_context.IsAfterReadSubscription)
          this.TranslateIdToDisplayName(condition1);
        else
          this.TranslateDisplayNameToId(condition1);
      }
    }

    protected virtual bool TryGetIdentityFieldNameFromPath(
      StringFieldCondition condition,
      out string fieldName)
    {
      fieldName = condition.FieldName.Spelling;
      PathSubscriptionExpression path = new XPathSubscriptionExpression().ParsePath(fieldName);
      if (path != null && !string.IsNullOrEmpty(path.Path))
      {
        ISubscriptionAdapter defaultAdapter = this.m_context.Subscription.GetDefaultAdapter(this.m_context.IVssRequestContext, false);
        PathSubscriptionAdapter subscriptionAdapter = (PathSubscriptionAdapter) null;
        if (defaultAdapter is PathSubscriptionAdapter)
          subscriptionAdapter = defaultAdapter as PathSubscriptionAdapter;
        else if (defaultAdapter is RoleBasedSubscriptionAdapter)
          subscriptionAdapter = (defaultAdapter as RoleBasedSubscriptionAdapter).InnerPathAdapter;
        if (subscriptionAdapter == null)
          return false;
        string fromPathExpression = subscriptionAdapter.GetFieldNameFromPathExpression(this.m_context.IVssRequestContext, path);
        if (fromPathExpression != null)
          fieldName = fromPathExpression;
      }
      return this.IsIdentityField(fieldName);
    }
  }
}
