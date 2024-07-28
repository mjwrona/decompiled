// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.SqmHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class SqmHelper : TestHelperBase
  {
    public SqmHelper(TestManagerRequestContext context)
      : base(context)
    {
    }

    internal void UpdateSqmPoint(TcmProperty property, int incrementBy)
    {
      Sqm sqm = new Sqm();
      List<KeyValuePair<TcmProperty, int>> keyValuePairList = new List<KeyValuePair<TcmProperty, int>>();
      keyValuePairList.Add(new KeyValuePair<TcmProperty, int>(property, incrementBy));
      TfsTestManagementRequestContext testRequestContext = this.TestContext.TestRequestContext;
      List<KeyValuePair<TcmProperty, int>> propertyTable = keyValuePairList;
      sqm.UpdateProperties((TestManagementRequestContext) testRequestContext, propertyTable);
    }
  }
}
