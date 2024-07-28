// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestQueryTFIdentityField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestQueryTFIdentityField<TQueryItem> : QueryField<TQueryItem>
  {
    public TestQueryTFIdentityField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, Guid> getDataValueFunc)
      : base(testContext, referenceName, displayName, isQueryable, QueryFieldType.TFIdentity, (Func<TQueryItem, object>) (item => (object) getDataValueFunc(item)))
    {
      this.DefaultWidth = 150;
      this.CanSortBy = false;
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[2]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals
    };

    public override IEnumerable<string> GetSuggestedValues()
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.GetSuggestedValues");
        IEnumerable<string> first = (IEnumerable<string>) new string[1]
        {
          TestManagementServerResources.MeMacro
        };
        if (this.TestContext.Team != null)
        {
          IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> source = this.TestContext.TfsRequestContext.GetService<ITeamService>().ReadTeamMembers(this.TestContext.TfsRequestContext, this.TestContext.Team.Identity, MembershipQuery.Expanded);
          first = first.Concat<string>((IEnumerable<string>) source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (member => !member.IsContainer)).Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (member => member.DisplayName)).OrderBy<string, string>((Func<string, string>) (name => name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase));
        }
        return first;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.GetSuggestedValues");
      }
    }

    public override string GetConditionString(QueryOperator queryOperator, string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.GetConditionString");
        Guid rawValue = (Guid) this.ConvertDisplayValueToRawValue(displayValue);
        int num = rawValue == Guid.Empty ? 1 : 0;
        return string.Format("[{0}] {1} {2}", (object) this.ReferenceName, (object) queryOperator.RawValue, (object) this.GetQueryValueString((object) rawValue));
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.GetConditionString");
      }
    }

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.ConvertRawValueToDisplayValue");
        Guid guid = (Guid) value;
        if (!(guid != Guid.Empty))
          return string.Empty;
        Guid[] teamFoundationIds = new Guid[1]{ guid };
        return this.TestContext.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TestContext.TfsRequestContext, teamFoundationIds)[0].DisplayName ?? guid.ToString();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.ConvertRawValueToDisplayValue");
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.ConvertDisplayValueToRawValue");
        Guid result = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(displayValue) && !Guid.TryParse(displayValue, out result))
        {
          if (string.Equals(TestManagementServerResources.MeMacro, displayValue, StringComparison.CurrentCultureIgnoreCase))
            return (object) this.TestContext.TfsRequestContext.GetUserId();
          TeamFoundationIdentity userByDisplayName = this.TestContext.Identities.GetUserByDisplayName(displayValue);
          if (userByDisplayName != null)
            return (object) userByDisplayName.TeamFoundationId;
        }
        return (object) result;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestQueryTFIdentityField<TQueryItem>.ConvertDisplayValueToRawValue");
      }
    }

    protected override string GetQueryValueString(object rawValue) => string.Format("'{0}'", (object) (Guid) rawValue);
  }
}
