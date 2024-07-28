// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionReportsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionReportsService : IExtensionReportsService, IVssFrameworkService
  {
    private const string s_area = "Gallery";
    private const string s_layer = "ExtensionReportsService";
    private const int MAX_QNA_ITEMS = 1000;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public byte[] GetExtensionReports(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int? lastNDays,
      int? count,
      DateTime? afterDate)
    {
      if (lastNDays.HasValue)
      {
        lastNDays = lastNDays.Value > 360 || lastNDays.Value <= 0 ? new int?(360) : lastNDays;
        afterDate = new DateTime?(GalleryServerUtil.GetAfterDateForLastNDays(lastNDays));
      }
      return new GalleryExcelProvider().GetFileContent(this.GetExtensionReportsTables(requestContext, publisherName, extensionName, count, afterDate));
    }

    private DataSet GetExtensionReportsTables(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int? count,
      DateTime? afterDate)
    {
      IExtensionDailyStatsService service = requestContext.GetService<IExtensionDailyStatsService>();
      DataSet dataSet = (DataSet) null;
      DataSet extensionReportsTables = (DataSet) null;
      try
      {
        dataSet = new DataSet("ExtensionReports");
        dataSet.Locale = CultureInfo.InvariantCulture;
        PublishedExtension extension = DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName, true);
        ArgumentUtility.CheckForNull<PublishedExtension>(extension, "extension");
        string str = "";
        if (extension.InstallationTargets != null)
          str = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
        ArgumentUtility.CheckStringForNullOrEmpty(str, "product");
        bool isVSSExtension = GalleryUtil.IsVSSExtensionInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets);
        ExtensionDailyStats extensionDailyStats = service.GetExtensionDailyStats(requestContext, publisherName, extensionName, new int?(), new ExtensionStatsAggregateType?(ExtensionStatsAggregateType.Daily), afterDate);
        ExtensionEvents extensionEvents = service.GetExtensionEvents(requestContext, publisherName, extensionName, count, afterDate, includeProperty: GalleryServiceConstants.LastContactDetails);
        dataSet.Tables.Add(this.GetExtensionDailyStatDataTable(extensionDailyStats, isVSSExtension, str));
        if (isVSSExtension)
        {
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableAcquisitionTabForPaid") && extensionEvents.Events.ContainsKey("acquisition"))
            dataSet.Tables.Add(this.GetExtensionAcquisitionsEventsTable(extensionEvents));
          dataSet.Tables.Add(this.GetExtensionUninstallEventsTable(extensionEvents));
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableSalesTransactionsTab") && extensionEvents.Events.ContainsKey("sales"))
            dataSet.Tables.Add(this.GetExtensionSalesTransactionsEventsTable(extensionEvents));
        }
        dataSet.Tables.Add(this.GetExtensionRnREventsTable(extensionEvents));
        if (this.CanShowQnATab(requestContext, extension))
        {
          QuestionsResult questionsList = requestContext.GetService<IQnAService>().GetQuestionsList(requestContext, publisherName, extensionName, 1000, afterDate: afterDate);
          dataSet.Tables.Add(this.GetExtensionQnAEventsTable(questionsList, publisherName));
        }
        extensionReportsTables = dataSet;
        dataSet = (DataSet) null;
      }
      finally
      {
        dataSet?.Dispose();
      }
      return extensionReportsTables;
    }

    private DataTable GetExtensionQnAEventsTable(
      QuestionsResult questionsResult,
      string publisherName)
    {
      DataTable dataTable = (DataTable) null;
      DataTable extensionQnAeventsTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.QnAEventsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("qnaDetails");
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (questionsResult != null && questionsResult.Questions != null)
        {
          foreach (Question question in questionsResult.Questions)
          {
            List<Response> responses = question.Responses;
            string str = responses.Count > 0 ? GalleryResources.YesText() : GalleryResources.NoText();
            dataTable.Rows.Add((object) question.CreatedDate, (object) question.User.DisplayName, (object) question.Text, (object) str);
            foreach (Response response in responses)
              dataTable.Rows.Add((object) response.CreatedDate, response.Status.HasFlag((Enum) QnAItemStatus.PublisherCreated) ? (object) publisherName : (object) response.User?.DisplayName, (object) response.Text, (object) string.Empty);
          }
        }
        extensionQnAeventsTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return extensionQnAeventsTable;
    }

    private DataTable GetExtensionRnREventsTable(ExtensionEvents extensionEvents)
    {
      DataTable dataTable = (DataTable) null;
      DataTable extensionRnReventsTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.RatingAndReviewEventsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("ratingReviewDetails");
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (extensionEvents != null && extensionEvents.Events != null)
        {
          IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents.Events["review"];
          if (extensionEvents1 != null)
          {
            foreach (ExtensionEvent extensionEvent in extensionEvents1)
            {
              DateTime? nullable1 = extensionEvent.Properties["ReviewDate"].Value<DateTime?>();
              DateTime? nullable2 = extensionEvent.Properties["ReplyDate"].Value<DateTime?>();
              DateTime dateTime1 = nullable1.HasValue ? nullable1.Value : new DateTime();
              DateTime dateTime2 = nullable2.HasValue ? nullable2.Value : new DateTime();
              dataTable.Rows.Add((object) dateTime1, (object) extensionEvent.Properties["UserDisplayName"], (object) extensionEvent.Properties["Rating"], (object) extensionEvent.Properties["ReviewText"], (object) dateTime2, (object) this.GetReplyText(extensionEvent.Properties["ReplyText"], extensionEvent.Properties["IsAdminReply"]));
            }
          }
        }
        extensionRnReventsTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return extensionRnReventsTable;
    }

    private string GetReplyText(JToken replyText, JToken isAdminReply)
    {
      string str = "";
      bool result = false;
      if (((isAdminReply == null ? 0 : (bool.TryParse(isAdminReply.ToString(), out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
        str = GalleryResources.MarketPlaceText();
      return str + replyText.ToString();
    }

    private DataTable GetExtensionUninstallEventsTable(ExtensionEvents extensionEvents)
    {
      DataTable dataTable = (DataTable) null;
      DataTable uninstallEventsTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.UninstallEventsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("uninstallDetails");
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (extensionEvents != null && extensionEvents.Events != null)
        {
          IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents.Events["uninstall"];
          if (extensionEvents1 != null)
          {
            foreach (ExtensionEvent extensionEvent in extensionEvents1)
            {
              DateTime dateTime = new DateTime();
              if (extensionEvent.Properties["lastContact"] != null)
              {
                DateTime? nullable = extensionEvent.Properties["lastContact"].Value<DateTime?>();
                dateTime = nullable.HasValue ? nullable.Value : new DateTime();
              }
              dataTable.Rows.Add((object) extensionEvent.StatisticDate, (object) extensionEvent.Properties["hostName"], (object) extensionEvent.Properties["reasonCode"], (object) extensionEvent.Properties["reasonText"], (object) dateTime);
            }
          }
        }
        uninstallEventsTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return uninstallEventsTable;
    }

    private DataTable GetExtensionInstallEventsTable(ExtensionEvents extensionEvents)
    {
      DataTable dataTable = (DataTable) null;
      DataTable installEventsTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.InstallEventsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("installDetails");
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (extensionEvents != null && extensionEvents.Events != null)
        {
          IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents.Events["install"];
          if (extensionEvents1 != null)
          {
            foreach (ExtensionEvent extensionEvent in extensionEvents1)
              dataTable.Rows.Add((object) extensionEvent.StatisticDate, (object) extensionEvent.Properties["hostId"], (object) extensionEvent.Properties["hostName"]);
          }
        }
        installEventsTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return installEventsTable;
    }

    private DataTable GetExtensionDailyStatDataTable(
      ExtensionDailyStats extensionDailyStats,
      bool isVSSExtension,
      string product)
    {
      DataTable dataTable = (DataTable) null;
      DataTable dailyStatDataTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.ExtensionDailyStatsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        dataTable.ExtendedProperties.Add((object) "useShortDateFormat", (object) true);
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("dailyStats", isVSSExtension, product);
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (extensionDailyStats != null && extensionDailyStats.DailyStats != null)
        {
          if (isVSSExtension)
          {
            foreach (ExtensionDailyStat dailyStat in extensionDailyStats.DailyStats)
            {
              EventCounts counts = dailyStat.Counts;
              dataTable.Rows.Add((object) dailyStat.StatisticDate, (object) counts.WebPageViews, (object) counts.InstallCount, (object) counts.WebDownloadCount, (object) counts.UninstallCount, (object) counts.ConnectedInstallCount, (object) Math.Round((double) counts.AverageRating, 2));
            }
          }
          else if (product.Equals("vs", StringComparison.OrdinalIgnoreCase) || product.Equals("vsformac", StringComparison.OrdinalIgnoreCase))
          {
            foreach (ExtensionDailyStat dailyStat in extensionDailyStats.DailyStats)
            {
              EventCounts counts = dailyStat.Counts;
              dataTable.Rows.Add((object) dailyStat.StatisticDate, (object) counts.WebPageViews, (object) counts.InstallCount, (object) counts.WebDownloadCount, (object) Math.Round((double) counts.AverageRating, 2));
            }
          }
          else
          {
            foreach (ExtensionDailyStat dailyStat in extensionDailyStats.DailyStats)
            {
              EventCounts counts = dailyStat.Counts;
              dataTable.Rows.Add((object) dailyStat.StatisticDate, (object) counts.WebPageViews, (object) counts.InstallCount, (object) Math.Round((double) counts.AverageRating, 2));
            }
          }
        }
        dailyStatDataTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return dailyStatDataTable;
    }

    private DataTable GetExtensionSalesTransactionsEventsTable(ExtensionEvents extensionEvents)
    {
      DataTable dataTable = (DataTable) null;
      DataTable transactionsEventsTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.SalesTransactionsEventsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("salesTransactionsDetails");
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (extensionEvents != null && extensionEvents.Events != null)
        {
          IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents.Events["sales"];
          if (extensionEvents1 != null)
          {
            foreach (ExtensionEvent extensionEvent in extensionEvents1)
            {
              int currentQuantity = extensionEvent.Properties["currentQuantity"].Value<int>();
              int previousQuantity = extensionEvent.Properties["previousQuantity"].Value<int>();
              DateTime dateTime1 = new DateTime();
              DateTime dateTime2 = new DateTime();
              if (extensionEvent.Properties["lastContact"] != null)
              {
                DateTime? nullable = extensionEvent.Properties["lastContact"].Value<DateTime?>();
                dateTime2 = nullable.HasValue ? nullable.Value : new DateTime();
              }
              if (extensionEvent.Properties["trialEndDate"] != null)
              {
                DateTime? nullable = extensionEvent.Properties["trialEndDate"].Value<DateTime?>();
                dateTime1 = nullable.HasValue ? nullable.Value : new DateTime();
              }
              dataTable.Rows.Add((object) extensionEvent.StatisticDate, (object) extensionEvent.Properties["collectionName"], (object) this.GetEnvironmentType(extensionEvent.Properties["environment"].ToString()), (object) this.GetPurchaseStateDisplayText(extensionEvent.Properties["eventName"].ToString()), (object) this.GetChangedQuantity(currentQuantity, previousQuantity), (object) currentQuantity, (object) dateTime1, (object) dateTime2);
            }
          }
        }
        transactionsEventsTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return transactionsEventsTable;
    }

    private DataTable GetExtensionAcquisitionsEventsTable(ExtensionEvents extensionEvents)
    {
      DataTable dataTable = (DataTable) null;
      DataTable acquisitionsEventsTable = (DataTable) null;
      try
      {
        dataTable = new DataTable(GalleryResources.AcquisitionsEventsTabText());
        dataTable.Locale = CultureInfo.InvariantCulture;
        IDictionary<string, SheetColumnData> columnNamesAndType = this.GetColumnNamesAndType("acquisitionsDetails");
        foreach (string key in (IEnumerable<string>) columnNamesAndType.Keys)
        {
          DataColumn column = new DataColumn(key, columnNamesAndType[key].ColumnType);
          column.ExtendedProperties.Add((object) "columnWidth", (object) columnNamesAndType[key].ColumnWidth);
          dataTable.Columns.Add(column);
        }
        if (extensionEvents != null && extensionEvents.Events != null)
        {
          IEnumerable<ExtensionEvent> extensionEvents1 = extensionEvents.Events["acquisition"];
          if (extensionEvents1 != null)
          {
            foreach (ExtensionEvent extensionEvent in extensionEvents1)
            {
              int num = extensionEvent.Properties["currentQuantity"].Value<int>();
              DateTime dateTime1 = new DateTime();
              DateTime dateTime2 = new DateTime();
              if (extensionEvent.Properties["trialEndDate"] != null)
              {
                DateTime? nullable = extensionEvent.Properties["trialEndDate"].Value<DateTime?>();
                dateTime1 = nullable.HasValue ? nullable.Value : new DateTime();
              }
              if (extensionEvent.Properties["lastContact"] != null)
              {
                DateTime? nullable = extensionEvent.Properties["lastContact"].Value<DateTime?>();
                dateTime2 = nullable.HasValue ? nullable.Value : new DateTime();
              }
              dataTable.Rows.Add((object) extensionEvent.StatisticDate, (object) extensionEvent.Properties["collectionName"], (object) this.GetEnvironmentType(extensionEvent.Properties["environment"].ToString()), (object) this.GetPurchaseStateDisplayText(extensionEvent.Properties["eventName"].ToString()), (object) num, (object) dateTime1, (object) dateTime2);
            }
          }
        }
        acquisitionsEventsTable = dataTable;
        dataTable = (DataTable) null;
      }
      finally
      {
        dataTable?.Dispose();
      }
      return acquisitionsEventsTable;
    }

    private string GetEnvironmentType(string environmentType) => !environmentType.Equals("Hosted", StringComparison.OrdinalIgnoreCase) ? GalleryResources.OnPremisesEnvironmentText() : GalleryResources.HostedEnvironmentText();

    private int GetChangedQuantity(int currentQuantity, int previousQuantity) => Math.Abs(currentQuantity - previousQuantity);

    private string GetPurchaseStateDisplayText(string state) => PurchaseEventCodeMapConstant.purchaseEventsToDisplayStringMap[state] == null ? state : PurchaseEventCodeMapConstant.purchaseEventsToDisplayStringMap[state];

    private bool CanShowQnATab(IVssRequestContext requestContext, PublishedExtension extension)
    {
      if (extension == null)
        return false;
      bool flag = false;
      QnAMode qnAmode = QnAMode.None;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ShowQnA") && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableQnATab") && !GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets))
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQnABypass"))
        {
          if (extension.InstallationTargets != null && (GalleryUtil.IsVSSExtensionInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || GalleryUtil.IsVSTSOrTFSIntegrationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets)) && !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQnABypassForVSTS"))
          {
            flag = true;
            qnAmode = QnAMode.MarketplaceQnA;
          }
          else
          {
            qnAmode = new QnAUtils("Gallery", nameof (ExtensionReportsService)).GetQnAMode(GalleryUtil.GetExtensionProperties(extension));
            if (qnAmode != QnAMode.None)
            {
              if (GalleryUtil.IsVSInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets))
                qnAmode = QnAMode.MarketplaceQnA;
              flag = true;
            }
          }
        }
        else if (!GalleryUtil.IsVSCodeInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets))
        {
          flag = true;
          qnAmode = QnAMode.MarketplaceQnA;
        }
      }
      return flag && qnAmode == QnAMode.MarketplaceQnA;
    }

    private IDictionary<string, SheetColumnData> GetColumnNamesAndType(
      string tableName,
      bool isVSSExtension = true,
      string product = "vsts")
    {
      IDictionary<string, SheetColumnData> columnNamesAndType;
      if (tableName != null)
      {
        switch (tableName.Length)
        {
          case 10:
            switch (tableName[0])
            {
              case 'd':
                if (tableName == "dailyStats")
                {
                  columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>();
                  columnNamesAndType.Add(GalleryResources.StatisticDateText(), new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 13.0
                  });
                  columnNamesAndType.Add(GalleryResources.WebPageViewsText(), new SheetColumnData()
                  {
                    ColumnType = typeof (long),
                    ColumnWidth = 12.0
                  });
                  if (product.Equals("vscode", StringComparison.OrdinalIgnoreCase))
                    columnNamesAndType.Add(GalleryResources.VSCodeInstallCountText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 19.0
                    });
                  else if (product.Equals("vs", StringComparison.OrdinalIgnoreCase) || product.Equals("vsformac", StringComparison.OrdinalIgnoreCase))
                  {
                    columnNamesAndType.Add(GalleryResources.DownloadFromIDEText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 19.0
                    });
                    columnNamesAndType.Add(GalleryResources.WebDownloadText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 27.0
                    });
                  }
                  else
                    columnNamesAndType.Add(GalleryResources.VSTSInstallCountText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 17.0
                    });
                  if (isVSSExtension)
                  {
                    columnNamesAndType.Add(GalleryResources.DownloadCountText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 16.0
                    });
                    columnNamesAndType.Add(GalleryResources.UninstallCountText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 15.0
                    });
                    columnNamesAndType.Add(GalleryResources.ConnectedInstallCountText(), new SheetColumnData()
                    {
                      ColumnType = typeof (long),
                      ColumnWidth = 26.0
                    });
                  }
                  columnNamesAndType.Add(GalleryResources.AverageRatingText(), new SheetColumnData()
                  {
                    ColumnType = typeof (float),
                    ColumnWidth = 15.0
                  });
                  goto label_26;
                }
                else
                  break;
              case 'q':
                if (tableName == "qnaDetails")
                {
                  columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>()
                  {
                    {
                      GalleryResources.StatisticDateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (DateTime),
                        ColumnWidth = 20.0
                      }
                    },
                    {
                      GalleryResources.NameText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 37.0
                      }
                    },
                    {
                      GalleryResources.QnAText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 70.0
                      }
                    },
                    {
                      GalleryResources.RespondedText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 22.0
                      }
                    }
                  };
                  goto label_26;
                }
                else
                  break;
            }
            break;
          case 14:
            if (tableName == "installDetails")
            {
              columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>()
              {
                {
                  GalleryResources.StatisticDateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 20.0
                  }
                },
                {
                  GalleryResources.AccountIdText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 37.0
                  }
                },
                {
                  GalleryResources.AccountNameText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 30.0
                  }
                }
              };
              goto label_26;
            }
            else
              break;
          case 16:
            if (tableName == "uninstallDetails")
            {
              columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>()
              {
                {
                  GalleryResources.StatisticDateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 20.0
                  }
                },
                {
                  GalleryResources.AccountNameText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 30.0
                  }
                },
                {
                  GalleryResources.ReasonCodeText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 35.0
                  }
                },
                {
                  GalleryResources.ReasonText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 60.0
                  }
                },
                {
                  GalleryResources.LastContactedDateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 30.0
                  }
                }
              };
              goto label_26;
            }
            else
              break;
          case 19:
            switch (tableName[0])
            {
              case 'a':
                if (tableName == "acquisitionsDetails")
                {
                  columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>()
                  {
                    {
                      GalleryResources.StatisticDateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (DateTime),
                        ColumnWidth = 20.0
                      }
                    },
                    {
                      GalleryResources.AccountNameText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 40.0
                      }
                    },
                    {
                      GalleryResources.TypeText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 15.0
                      }
                    },
                    {
                      GalleryResources.StateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 15.0
                      }
                    },
                    {
                      GalleryResources.CurrentQuantityText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (int),
                        ColumnWidth = 20.0
                      }
                    },
                    {
                      GalleryResources.TrialEndDateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (DateTime),
                        ColumnWidth = 20.0
                      }
                    },
                    {
                      GalleryResources.LastContactedDateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (DateTime),
                        ColumnWidth = 30.0
                      }
                    }
                  };
                  goto label_26;
                }
                else
                  break;
              case 'r':
                if (tableName == "ratingReviewDetails")
                {
                  columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>()
                  {
                    {
                      GalleryResources.ReviewDateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (DateTime),
                        ColumnWidth = 22.0
                      }
                    },
                    {
                      GalleryResources.ReviewerText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 15.0
                      }
                    },
                    {
                      GalleryResources.RatingText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (int),
                        ColumnWidth = 8.0
                      }
                    },
                    {
                      GalleryResources.ReviewText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 60.0
                      }
                    },
                    {
                      GalleryResources.ResponseDateText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (DateTime),
                        ColumnWidth = 22.0
                      }
                    },
                    {
                      GalleryResources.ResponseText(),
                      new SheetColumnData()
                      {
                        ColumnType = typeof (string),
                        ColumnWidth = 60.0
                      }
                    }
                  };
                  goto label_26;
                }
                else
                  break;
            }
            break;
          case 24:
            if (tableName == "salesTransactionsDetails")
            {
              columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>()
              {
                {
                  GalleryResources.StatisticDateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 20.0
                  }
                },
                {
                  GalleryResources.AccountNameText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 40.0
                  }
                },
                {
                  GalleryResources.TypeText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 15.0
                  }
                },
                {
                  GalleryResources.StateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (string),
                    ColumnWidth = 15.0
                  }
                },
                {
                  GalleryResources.ChangedQuantityText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (int),
                    ColumnWidth = 20.0
                  }
                },
                {
                  GalleryResources.TotalQuantityText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (int),
                    ColumnWidth = 20.0
                  }
                },
                {
                  GalleryResources.TrialEndDateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 20.0
                  }
                },
                {
                  GalleryResources.LastContactedDateText(),
                  new SheetColumnData()
                  {
                    ColumnType = typeof (DateTime),
                    ColumnWidth = 30.0
                  }
                }
              };
              goto label_26;
            }
            else
              break;
        }
      }
      columnNamesAndType = (IDictionary<string, SheetColumnData>) new Dictionary<string, SheetColumnData>();
label_26:
      return columnNamesAndType;
    }
  }
}
