// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.OrderByQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class OrderByQueryValidator
  {
    private readonly DefaultQuerySettings _defaultQuerySettings;

    public OrderByQueryValidator(DefaultQuerySettings defaultQuerySettings) => this._defaultQuerySettings = defaultQuerySettings;

    public virtual void Validate(
      OrderByQueryOption orderByOption,
      ODataValidationSettings validationSettings)
    {
      if (orderByOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (orderByOption));
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      int num = 0;
      for (OrderByClause orderByClause = orderByOption.OrderByClause; orderByClause != null; orderByClause = orderByClause.ThenBy)
      {
        ++num;
        if (num > validationSettings.MaxOrderByNodeCount)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.OrderByNodeCountExceeded, (object) validationSettings.MaxOrderByNodeCount));
      }
      OrderByModelLimitationsValidator limitationsValidator = new OrderByModelLimitationsValidator(orderByOption.Context, this._defaultQuerySettings.EnableOrderBy);
      bool explicitPropertiesDefined = validationSettings.AllowedOrderByProperties.Count > 0;
      foreach (OrderByNode orderByNode in (IEnumerable<OrderByNode>) orderByOption.OrderByNodes)
      {
        if (orderByNode is OrderByPropertyNode orderByPropertyNode)
        {
          string name = orderByPropertyNode.Property.Name;
          bool flag = !limitationsValidator.TryValidate(orderByPropertyNode.OrderByClause, explicitPropertiesDefined);
          if (name != null & flag & explicitPropertiesDefined)
          {
            if (!OrderByQueryValidator.IsAllowed(validationSettings, name))
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedOrderByProperty, (object) name, (object) "AllowedOrderByProperties"));
          }
          else if (name != null && !OrderByQueryValidator.IsAllowed(validationSettings, name))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedOrderByProperty, (object) name, (object) "AllowedOrderByProperties"));
        }
        else
        {
          string propertyName = "$it";
          if (!OrderByQueryValidator.IsAllowed(validationSettings, propertyName))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedOrderByProperty, (object) propertyName, (object) "AllowedOrderByProperties"));
        }
      }
    }

    internal static OrderByQueryValidator GetOrderByQueryValidator(ODataQueryContext context)
    {
      if (context == null)
        return new OrderByQueryValidator(new DefaultQuerySettings());
      return context.RequestContainer != null ? ServiceProviderServiceExtensions.GetRequiredService<OrderByQueryValidator>(context.RequestContainer) : new OrderByQueryValidator(context.DefaultQuerySettings);
    }

    private static bool IsAllowed(ODataValidationSettings validationSettings, string propertyName) => validationSettings.AllowedOrderByProperties.Count == 0 || validationSettings.AllowedOrderByProperties.Contains(propertyName);
  }
}
