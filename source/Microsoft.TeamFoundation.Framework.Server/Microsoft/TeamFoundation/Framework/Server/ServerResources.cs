// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServerResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServerResources), typeof (ServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServerResources.s_resMgr;

    private static string Get(string resourceName) => ServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServerResources.Get(resourceName) : ServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServerResources.GetInt(resourceName) : (int) ServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServerResources.GetBool(resourceName) : (bool) ServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServerResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string AUTHORIZATIONSERVICE_DATABASE_CONFIG_EXCEPTION() => ServerResources.Get("AUTHORIZATIONSERVICE.DATABASE-CONFIG-EXCEPTION");

    public static string AUTHORIZATIONSERVICE_DATABASE_CONFIG_EXCEPTION(CultureInfo culture) => ServerResources.Get("AUTHORIZATIONSERVICE.DATABASE-CONFIG-EXCEPTION", culture);

    public static string CONDITIONALCOMPONENT_X_NOTSPECIFIED() => ServerResources.Get("CONDITIONALCOMPONENT.X-NOTSPECIFIED");

    public static string CONDITIONALCOMPONENT_X_NOTSPECIFIED(CultureInfo culture) => ServerResources.Get("CONDITIONALCOMPONENT.X-NOTSPECIFIED", culture);

    public static string CONFIG_X_INTFORMAT(object arg0) => ServerResources.Format("CONFIG.X-INTFORMAT", arg0);

    public static string CONFIG_X_INTFORMAT(object arg0, CultureInfo culture) => ServerResources.Format("CONFIG.X-INTFORMAT", culture, arg0);

    public static string CONFIG_X_MISSINGSETTING(object arg0, object arg1) => ServerResources.Format("CONFIG.X-MISSINGSETTING", arg0, arg1);

    public static string CONFIG_X_MISSINGSETTING(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("CONFIG.X-MISSINGSETTING", culture, arg0, arg1);

    public static string CONFIG_X_SETTINGNAME() => ServerResources.Get("CONFIG.X-SETTINGNAME");

    public static string CONFIG_X_SETTINGNAME(CultureInfo culture) => ServerResources.Get("CONFIG.X-SETTINGNAME", culture);

    public static string EVENT_SUBSCRIPTION_GENERIC_READ() => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_GENERIC_READ));

    public static string EVENT_SUBSCRIPTION_GENERIC_READ(CultureInfo culture) => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_GENERIC_READ), culture);

    public static string EVENT_SUBSCRIPTION_GENERIC_WRITE() => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_GENERIC_WRITE));

    public static string EVENT_SUBSCRIPTION_GENERIC_WRITE(CultureInfo culture) => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_GENERIC_WRITE), culture);

    public static string EVENT_SUBSCRIPTION_UNSUBSCRIBE() => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_UNSUBSCRIBE));

    public static string EVENT_SUBSCRIPTION_UNSUBSCRIBE(CultureInfo culture) => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_UNSUBSCRIBE), culture);

    public static string EVENT_SUBSCRIPTION_CREATE_SOAP_SUBSCRIPTION() => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_CREATE_SOAP_SUBSCRIPTION));

    public static string EVENT_SUBSCRIPTION_CREATE_SOAP_SUBSCRIPTION(CultureInfo culture) => ServerResources.Get(nameof (EVENT_SUBSCRIPTION_CREATE_SOAP_SUBSCRIPTION), culture);

    public static string EVENTONSERVER_COULDNTPARSESTATUS(object arg0) => ServerResources.Format("EVENTONSERVER.COULDNTPARSESTATUS", arg0);

    public static string EVENTONSERVER_COULDNTPARSESTATUS(object arg0, CultureInfo culture) => ServerResources.Format("EVENTONSERVER.COULDNTPARSESTATUS", culture, arg0);

    public static string EVENTONSERVER_EVALITEMPATH(object arg0) => ServerResources.Format("EVENTONSERVER.EVALITEMPATH", arg0);

    public static string EVENTONSERVER_EVALITEMPATH(object arg0, CultureInfo culture) => ServerResources.Format("EVENTONSERVER.EVALITEMPATH", culture, arg0);

    public static string EVENTONSERVER_DONEWITHNOTIFICATION() => ServerResources.Get("EVENTONSERVER.DONEWITHNOTIFICATION");

    public static string EVENTONSERVER_DONEWITHNOTIFICATION(CultureInfo culture) => ServerResources.Get("EVENTONSERVER.DONEWITHNOTIFICATION", culture);

    public static string EVENTONSERVER_EVENTPROCESSED(object arg0) => ServerResources.Format("EVENTONSERVER.EVENTPROCESSED", arg0);

    public static string EVENTONSERVER_EVENTPROCESSED(object arg0, CultureInfo culture) => ServerResources.Format("EVENTONSERVER.EVENTPROCESSED", culture, arg0);

    public static string EVENTONSERVER_DELETEDNONEXISTINGEVENT(object arg0) => ServerResources.Format("EVENTONSERVER.DELETEDNONEXISTINGEVENT", arg0);

    public static string EVENTONSERVER_DELETEDNONEXISTINGEVENT(object arg0, CultureInfo culture) => ServerResources.Format("EVENTONSERVER.DELETEDNONEXISTINGEVENT", culture, arg0);

    public static string EVENTMETA_X_FIELDNAME() => ServerResources.Get("EVENTMETA.X-FIELDNAME");

    public static string EVENTMETA_X_FIELDNAME(CultureInfo culture) => ServerResources.Get("EVENTMETA.X-FIELDNAME", culture);

    public static string EVENTMETA_X_UNRECOGNIZEDTYPE(object arg0) => ServerResources.Format("EVENTMETA.X-UNRECOGNIZEDTYPE", arg0);

    public static string EVENTMETA_X_UNRECOGNIZEDTYPE(object arg0, CultureInfo culture) => ServerResources.Format("EVENTMETA.X-UNRECOGNIZEDTYPE", culture, arg0);

    public static string EVENTSERVICE_ASMX_ASYNCEVENTRECEIVED(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.ASYNCEVENTRECEIVED", arg0);

    public static string EVENTSERVICE_ASMX_ASYNCEVENTRECEIVED(object arg0, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.ASYNCEVENTRECEIVED", culture, arg0);

    public static string EVENTSERVICE_ASMX_CREATINGNEWSUBSCRIPTION(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.CREATINGNEWSUBSCRIPTION", arg0);

    public static string EVENTSERVICE_ASMX_CREATINGNEWSUBSCRIPTION(object arg0, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.CREATINGNEWSUBSCRIPTION", culture, arg0);

    public static string EVENTSERVICE_ASMX_RETURNING(object arg0, object arg1) => ServerResources.Format("EVENTSERVICE.ASMX.RETURNING", arg0, arg1);

    public static string EVENTSERVICE_ASMX_RETURNING(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.RETURNING", culture, arg0, arg1);

    public static string EVENTSERVICE_ASMX_RETURNINGALLSUBSCRIPTIONS(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.RETURNINGALLSUBSCRIPTIONS", arg0);

    public static string EVENTSERVICE_ASMX_RETURNINGALLSUBSCRIPTIONS(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format("EVENTSERVICE.ASMX.RETURNINGALLSUBSCRIPTIONS", culture, arg0);
    }

    public static string EVENTSERVICE_ASMX_RETURNINGQUEUENAME(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.RETURNINGQUEUENAME", arg0);

    public static string EVENTSERVICE_ASMX_RETURNINGQUEUENAME(object arg0, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.RETURNINGQUEUENAME", culture, arg0);

    public static string EVENTSERVICE_ASMX_SYNCEVENTRECEIVED(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.SYNCEVENTRECEIVED", arg0);

    public static string EVENTSERVICE_ASMX_SYNCEVENTRECEIVED(object arg0, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.SYNCEVENTRECEIVED", culture, arg0);

    public static string EVENTSERVICE_ASMX_UNSUBSCRIBINGUSER(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.UNSUBSCRIBINGUSER", arg0);

    public static string EVENTSERVICE_ASMX_UNSUBSCRIBINGUSER(object arg0, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.UNSUBSCRIBINGUSER", culture, arg0);

    public static string EVENTSERVICE_ASMX_X_EMPTYUSERID() => ServerResources.Get("EVENTSERVICE.ASMX.X-EMPTYUSERID");

    public static string EVENTSERVICE_ASMX_X_EMPTYUSERID(CultureInfo culture) => ServerResources.Get("EVENTSERVICE.ASMX.X-EMPTYUSERID", culture);

    public static string EVENTSERVICE_ASMX_X_INVALIDEMAIL() => ServerResources.Get("EVENTSERVICE.ASMX.X-INVALIDEMAIL");

    public static string EVENTSERVICE_ASMX_X_INVALIDEMAIL(CultureInfo culture) => ServerResources.Get("EVENTSERVICE.ASMX.X-INVALIDEMAIL", culture);

    public static string EVENTSERVICE_ASMX_INVALIDURI() => ServerResources.Get("EVENTSERVICE.ASMX.INVALIDURI");

    public static string EVENTSERVICE_ASMX_INVALIDURI(CultureInfo culture) => ServerResources.Get("EVENTSERVICE.ASMX.INVALIDURI", culture);

    public static string EVENTSERVICE_ASMX_X_MISSINGSCHEMA(object arg0) => ServerResources.Format("EVENTSERVICE.ASMX.X-MISSINGSCHEMA", arg0);

    public static string EVENTSERVICE_ASMX_X_MISSINGSCHEMA(object arg0, CultureInfo culture) => ServerResources.Format("EVENTSERVICE.ASMX.X-MISSINGSCHEMA", culture, arg0);

    public static string EVENTSERVICE_ASMX_X_SUBSCRIPTIONNOTFOUND() => ServerResources.Get("EVENTSERVICE.ASMX.X-SUBSCRIPTIONNOTFOUND");

    public static string EVENTSERVICE_ASMX_X_SUBSCRIPTIONNOTFOUND(CultureInfo culture) => ServerResources.Get("EVENTSERVICE.ASMX.X-SUBSCRIPTIONNOTFOUND", culture);

    public static string GAP_ASMX_ARTIFACT() => ServerResources.Get("GAP.ASMX.ARTIFACT");

    public static string GAP_ASMX_ARTIFACT(CultureInfo culture) => ServerResources.Get("GAP.ASMX.ARTIFACT", culture);

    public static string GAP_ASMX_ENTERED() => ServerResources.Get("GAP.ASMX.ENTERED");

    public static string GAP_ASMX_ENTERED(CultureInfo culture) => ServerResources.Get("GAP.ASMX.ENTERED", culture);

    public static string GAP_ASMX_EXITING() => ServerResources.Get("GAP.ASMX.EXITING");

    public static string GAP_ASMX_EXITING(CultureInfo culture) => ServerResources.Get("GAP.ASMX.EXITING", culture);

    public static string GAP_ASMX_GETARTIFACTS() => ServerResources.Get("GAP.ASMX.GETARTIFACTS");

    public static string GAP_ASMX_GETARTIFACTS(CultureInfo culture) => ServerResources.Get("GAP.ASMX.GETARTIFACTS", culture);

    public static string GAP_ASMX_GETARTIFACTSBYEXTERNALID() => ServerResources.Get("GAP.ASMX.GETARTIFACTSBYEXTERNALID");

    public static string GAP_ASMX_GETARTIFACTSBYEXTERNALID(CultureInfo culture) => ServerResources.Get("GAP.ASMX.GETARTIFACTSBYEXTERNALID", culture);

    public static string GAP_ASMX_GETREFERENCINGARTIFACTS() => ServerResources.Get("GAP.ASMX.GETREFERENCINGARTIFACTS");

    public static string GAP_ASMX_GETREFERENCINGARTIFACTS(CultureInfo culture) => ServerResources.Get("GAP.ASMX.GETREFERENCINGARTIFACTS", culture);

    public static string GAP_ASMX_GETREFERENCINGARTIFACTS2() => ServerResources.Get("GAP.ASMX.GETREFERENCINGARTIFACTS2");

    public static string GAP_ASMX_GETREFERENCINGARTIFACTS2(CultureInfo culture) => ServerResources.Get("GAP.ASMX.GETREFERENCINGARTIFACTS2", culture);

    public static string GAP_ASMX_ISILLFORMED() => ServerResources.Get("GAP.ASMX.ISILLFORMED");

    public static string GAP_ASMX_ISILLFORMED(CultureInfo culture) => ServerResources.Get("GAP.ASMX.ISILLFORMED", culture);

    public static string GAP_ASMX_UPDATEARTIFACTDATA() => ServerResources.Get("GAP.ASMX.UPDATEARTIFACTDATA");

    public static string GAP_ASMX_UPDATEARTIFACTDATA(CultureInfo culture) => ServerResources.Get("GAP.ASMX.UPDATEARTIFACTDATA", culture);

    public static string GAP_ASMX_URI() => ServerResources.Get("GAP.ASMX.URI");

    public static string GAP_ASMX_URI(CultureInfo culture) => ServerResources.Get("GAP.ASMX.URI", culture);

    public static string GAP_ASMX_X_ARTIFACTISINGAP() => ServerResources.Get("GAP.ASMX.X-ARTIFACTISINGAP");

    public static string GAP_ASMX_X_ARTIFACTISINGAP(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-ARTIFACTISINGAP", culture);

    public static string GAP_ASMX_X_ARTIFACTISNOTINGAPCHANGE() => ServerResources.Get("GAP.ASMX.X-ARTIFACTISNOTINGAPCHANGE");

    public static string GAP_ASMX_X_ARTIFACTISNOTINGAPCHANGE(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-ARTIFACTISNOTINGAPCHANGE", culture);

    public static string GAP_ASMX_X_ARTIFACTISNOTINGAPDELETE() => ServerResources.Get("GAP.ASMX.X-ARTIFACTISNOTINGAPDELETE");

    public static string GAP_ASMX_X_ARTIFACTISNOTINGAPDELETE(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-ARTIFACTISNOTINGAPDELETE", culture);

    public static string GAP_ASMX_X_ILLFORMEDARTIFACT() => ServerResources.Get("GAP.ASMX.X-ILLFORMEDARTIFACT");

    public static string GAP_ASMX_X_ILLFORMEDARTIFACT(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-ILLFORMEDARTIFACT", culture);

    public static string GAP_ASMX_X_ILLFORMEDLINKFILTER() => ServerResources.Get("GAP.ASMX.X-ILLFORMEDLINKFILTER");

    public static string GAP_ASMX_X_ILLFORMEDLINKFILTER(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-ILLFORMEDLINKFILTER", culture);

    public static string GAP_ASMX_X_NULLARTIFACT() => ServerResources.Get("GAP.ASMX.X-NULLARTIFACT");

    public static string GAP_ASMX_X_NULLARTIFACT(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-NULLARTIFACT", culture);

    public static string GAP_ASMX_X_NULLARTIFACTARRAY() => ServerResources.Get("GAP.ASMX.X-NULLARTIFACTARRAY");

    public static string GAP_ASMX_X_NULLARTIFACTARRAY(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-NULLARTIFACTARRAY", culture);

    public static string GAP_ASMX_X_NULLARTIFACTURI() => ServerResources.Get("GAP.ASMX.X-NULLARTIFACTURI");

    public static string GAP_ASMX_X_NULLARTIFACTURI(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-NULLARTIFACTURI", culture);

    public static string GAP_ASMX_X_NULLEXTERNALID() => ServerResources.Get("GAP.ASMX.X-NULLEXTERNALID");

    public static string GAP_ASMX_X_NULLEXTERNALID(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-NULLEXTERNALID", culture);

    public static string GAP_ASMX_X_NULLEXTERNALIDARRAY() => ServerResources.Get("GAP.ASMX.X-NULLEXTERNALIDARRAY");

    public static string GAP_ASMX_X_NULLEXTERNALIDARRAY(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-NULLEXTERNALIDARRAY", culture);

    public static string GAP_ASMX_X_NULLURILIST() => ServerResources.Get("GAP.ASMX.X-NULLURILIST");

    public static string GAP_ASMX_X_NULLURILIST(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-NULLURILIST", culture);

    public static string GAP_ASMX_X_UNKNOWNCHANGETYPE() => ServerResources.Get("GAP.ASMX.X-UNKNOWNCHANGETYPE");

    public static string GAP_ASMX_X_UNKNOWNCHANGETYPE(CultureInfo culture) => ServerResources.Get("GAP.ASMX.X-UNKNOWNCHANGETYPE", culture);

    public static string GAPDB_DELETEARTIFACT() => ServerResources.Get("GAPDB.DELETEARTIFACT");

    public static string GAPDB_DELETEARTIFACT(CultureInfo culture) => ServerResources.Get("GAPDB.DELETEARTIFACT", culture);

    public static string GAPDB_ENTERED() => ServerResources.Get("GAPDB.ENTERED");

    public static string GAPDB_ENTERED(CultureInfo culture) => ServerResources.Get("GAPDB.ENTERED", culture);

    public static string GAPDB_EXITING() => ServerResources.Get("GAPDB.EXITING");

    public static string GAPDB_EXITING(CultureInfo culture) => ServerResources.Get("GAPDB.EXITING", culture);

    public static string GAPDB_GETARTIFACTBYEXTERNALID() => ServerResources.Get("GAPDB.GETARTIFACTBYEXTERNALID");

    public static string GAPDB_GETARTIFACTBYEXTERNALID(CultureInfo culture) => ServerResources.Get("GAPDB.GETARTIFACTBYEXTERNALID", culture);

    public static string GAPDB_GETARTIFACTBYURI() => ServerResources.Get("GAPDB.GETARTIFACTBYURI");

    public static string GAPDB_GETARTIFACTBYURI(CultureInfo culture) => ServerResources.Get("GAPDB.GETARTIFACTBYURI", culture);

    public static string GAPDB_GETREFERENCINGARTIFACTLINKS() => ServerResources.Get("GAPDB.GETREFERENCINGARTIFACTLINKS");

    public static string GAPDB_GETREFERENCINGARTIFACTLINKS(CultureInfo culture) => ServerResources.Get("GAPDB.GETREFERENCINGARTIFACTLINKS", culture);

    public static string GAPDB_UPDATEARTIFACT() => ServerResources.Get("GAPDB.UPDATEARTIFACT");

    public static string GAPDB_UPDATEARTIFACT(CultureInfo culture) => ServerResources.Get("GAPDB.UPDATEARTIFACT", culture);

    public static string GAPDB_X_CANNOTFIREARTIFACTCHANGEDEVENT() => ServerResources.Get("GAPDB.X-CANNOTFIREARTIFACTCHANGEDEVENT");

    public static string GAPDB_X_CANNOTFIREARTIFACTCHANGEDEVENT(CultureInfo culture) => ServerResources.Get("GAPDB.X-CANNOTFIREARTIFACTCHANGEDEVENT", culture);

    public static string GLOBAL_ASAX_APPSTARTING() => ServerResources.Get("GLOBAL.ASAX.APPSTARTING");

    public static string GLOBAL_ASAX_APPSTARTING(CultureInfo culture) => ServerResources.Get("GLOBAL.ASAX.APPSTARTING", culture);

    public static string GLOBAL_ASAX_CURRENTDIRECTORY(object arg0) => ServerResources.Format("GLOBAL.ASAX.CURRENTDIRECTORY", arg0);

    public static string GLOBAL_ASAX_CURRENTDIRECTORY(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.CURRENTDIRECTORY", culture, arg0);

    public static string GLOBAL_ASAX_DOMAIN(object arg0) => ServerResources.Format("GLOBAL.ASAX.DOMAIN", arg0);

    public static string GLOBAL_ASAX_DOMAIN(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.DOMAIN", culture, arg0);

    public static string GLOBAL_ASAX_INITCOMPLETE() => ServerResources.Get("GLOBAL.ASAX.INITCOMPLETE");

    public static string GLOBAL_ASAX_INITCOMPLETE(CultureInfo culture) => ServerResources.Get("GLOBAL.ASAX.INITCOMPLETE", culture);

    public static string GLOBAL_ASAX_MACHINENAME(object arg0) => ServerResources.Format("GLOBAL.ASAX.MACHINENAME", arg0);

    public static string GLOBAL_ASAX_MACHINENAME(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.MACHINENAME", culture, arg0);

    public static string GLOBAL_ASAX_MISSINGSETTING(object arg0) => ServerResources.Format("GLOBAL.ASAX.MISSINGSETTING", arg0);

    public static string GLOBAL_ASAX_MISSINGSETTING(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.MISSINGSETTING", culture, arg0);

    public static string GLOBAL_ASAX_OSVERSION(object arg0) => ServerResources.Format("GLOBAL.ASAX.OSVERSION", arg0);

    public static string GLOBAL_ASAX_OSVERSION(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.OSVERSION", culture, arg0);

    public static string GLOBAL_ASAX_SYSTEMSTARTMESSAGE() => ServerResources.Get("GLOBAL.ASAX.SYSTEMSTARTMESSAGE");

    public static string GLOBAL_ASAX_SYSTEMSTARTMESSAGE(CultureInfo culture) => ServerResources.Get("GLOBAL.ASAX.SYSTEMSTARTMESSAGE", culture);

    public static string GLOBAL_ASAX_SYSTEMSTOPMESSAGE() => ServerResources.Get("GLOBAL.ASAX.SYSTEMSTOPMESSAGE");

    public static string GLOBAL_ASAX_SYSTEMSTOPMESSAGE(CultureInfo culture) => ServerResources.Get("GLOBAL.ASAX.SYSTEMSTOPMESSAGE", culture);

    public static string GLOBAL_ASAX_UNHANDLEDAPPERROR(object arg0) => ServerResources.Format("GLOBAL.ASAX.UNHANDLEDAPPERROR", arg0);

    public static string GLOBAL_ASAX_UNHANDLEDAPPERROR(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.UNHANDLEDAPPERROR", culture, arg0);

    public static string GLOBAL_ASAX_USER(object arg0) => ServerResources.Format("GLOBAL.ASAX.USER", arg0);

    public static string GLOBAL_ASAX_USER(object arg0, CultureInfo culture) => ServerResources.Format("GLOBAL.ASAX.USER", culture, arg0);

    public static string GSS_PROJECT_ADMINISTRATORS() => ServerResources.Get("GSS.PROJECT-ADMINISTRATORS");

    public static string GSS_PROJECT_ADMINISTRATORS(CultureInfo culture) => ServerResources.Get("GSS.PROJECT-ADMINISTRATORS", culture);

    public static string LINKING_ASMX_DECODEURI() => ServerResources.Get("LINKING.ASMX.DECODEURI");

    public static string LINKING_ASMX_DECODEURI(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.DECODEURI", culture);

    public static string LINKING_ASMX_ENCODEURI() => ServerResources.Get("LINKING.ASMX.ENCODEURI");

    public static string LINKING_ASMX_ENCODEURI(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.ENCODEURI", culture);

    public static string LINKING_ASMX_ENTERED(object arg0) => ServerResources.Format("LINKING.ASMX.ENTERED", arg0);

    public static string LINKING_ASMX_ENTERED(object arg0, CultureInfo culture) => ServerResources.Format("LINKING.ASMX.ENTERED", culture, arg0);

    public static string LINKING_ASMX_EXITING(object arg0) => ServerResources.Format("LINKING.ASMX.EXITING", arg0);

    public static string LINKING_ASMX_EXITING(object arg0, CultureInfo culture) => ServerResources.Format("LINKING.ASMX.EXITING", culture, arg0);

    public static string LINKING_ASMX_EXTRACTLINKS() => ServerResources.Get("LINKING.ASMX.EXTRACTLINKS");

    public static string LINKING_ASMX_EXTRACTLINKS(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.EXTRACTLINKS", culture);

    public static string LINKING_ASMX_EXTRACTLINKS2() => ServerResources.Get("LINKING.ASMX.EXTRACTLINKS2");

    public static string LINKING_ASMX_EXTRACTLINKS2(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.EXTRACTLINKS2", culture);

    public static string LINKING_ASMX_EXTRACTLINKS3() => ServerResources.Get("LINKING.ASMX.EXTRACTLINKS3");

    public static string LINKING_ASMX_EXTRACTLINKS3(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.EXTRACTLINKS3", culture);

    public static string LINKING_ASMX_EXTRACTLINKSERROR(object arg0, object arg1) => ServerResources.Format("LINKING.ASMX.EXTRACTLINKSERROR", arg0, arg1);

    public static string LINKING_ASMX_EXTRACTLINKSERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("LINKING.ASMX.EXTRACTLINKSERROR", culture, arg0, arg1);
    }

    public static string LINKING_ASMX_GETARTIFACTS() => ServerResources.Get("LINKING.ASMX.GETARTIFACTS");

    public static string LINKING_ASMX_GETARTIFACTS(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETARTIFACTS", culture);

    public static string LINKING_ASMX_GETARTIFACTS2() => ServerResources.Get("LINKING.ASMX.GETARTIFACTS2");

    public static string LINKING_ASMX_GETARTIFACTS2(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETARTIFACTS2", culture);

    public static string LINKING_ASMX_GETARTIFACTURI() => ServerResources.Get("LINKING.ASMX.GETARTIFACTURI");

    public static string LINKING_ASMX_GETARTIFACTURI(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETARTIFACTURI", culture);

    public static string LINKING_ASMX_GETARTIFACTURL() => ServerResources.Get("LINKING.ASMX.GETARTIFACTURL");

    public static string LINKING_ASMX_GETARTIFACTURL(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETARTIFACTURL", culture);

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS() => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS");

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS", culture);

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS2() => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS2");

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS2(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS2", culture);

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS3() => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS3");

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS3(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS3", culture);

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS4() => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS4");

    public static string LINKING_ASMX_GETREFERENCINGARTIFACTS4(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETREFERENCINGARTIFACTS4", culture);

    public static string LINKING_ASMX_GETSERVICEINTERFACEURL() => ServerResources.Get("LINKING.ASMX.GETSERVICEINTERFACEURL");

    public static string LINKING_ASMX_GETSERVICEINTERFACEURL(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.GETSERVICEINTERFACEURL", culture);

    public static string LINKING_ASMX_X_ARTIFACTURIROOTNOTSET() => ServerResources.Get("LINKING.ASMX.X-ARTIFACTURIROOTNOTSET");

    public static string LINKING_ASMX_X_ARTIFACTURIROOTNOTSET(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.X-ARTIFACTURIROOTNOTSET", culture);

    public static string LINKING_ASMX_X_ILLFORMEDFILTER(object arg0) => ServerResources.Format("LINKING.ASMX.X-ILLFORMEDFILTER", arg0);

    public static string LINKING_ASMX_X_ILLFORMEDFILTER(object arg0, CultureInfo culture) => ServerResources.Format("LINKING.ASMX.X-ILLFORMEDFILTER", culture, arg0);

    public static string LINKING_ASMX_X_ILLFORMEDURI(object arg0) => ServerResources.Format("LINKING.ASMX.X-ILLFORMEDURI", arg0);

    public static string LINKING_ASMX_X_ILLFORMEDURI(object arg0, CultureInfo culture) => ServerResources.Format("LINKING.ASMX.X-ILLFORMEDURI", culture, arg0);

    public static string LINKING_ASMX_X_ILLFORMEDURI2(object arg0) => ServerResources.Format("LINKING.ASMX.X-ILLFORMEDURI2", arg0);

    public static string LINKING_ASMX_X_ILLFORMEDURI2(object arg0, CultureInfo culture) => ServerResources.Format("LINKING.ASMX.X-ILLFORMEDURI2", culture, arg0);

    public static string LINKING_ASMX_X_NULLURILIST() => ServerResources.Get("LINKING.ASMX.X-NULLURILIST");

    public static string LINKING_ASMX_X_NULLURILIST(CultureInfo culture) => ServerResources.Get("LINKING.ASMX.X-NULLURILIST", culture);

    public static string MethodologyUploadHander_TooLarge() => ServerResources.Get("MethodologyUploadHander.TooLarge");

    public static string MethodologyUploadHander_TooLarge(CultureInfo culture) => ServerResources.Get("MethodologyUploadHander.TooLarge", culture);

    public static string ONLINEF1HELP_ASMX_ENTERED() => ServerResources.Get("ONLINEF1HELP.ASMX.ENTERED");

    public static string ONLINEF1HELP_ASMX_ENTERED(CultureInfo culture) => ServerResources.Get("ONLINEF1HELP.ASMX.ENTERED", culture);

    public static string ONLINEF1HELP_ASMX_EXITING() => ServerResources.Get("ONLINEF1HELP.ASMX.EXITING");

    public static string ONLINEF1HELP_ASMX_EXITING(CultureInfo culture) => ServerResources.Get("ONLINEF1HELP.ASMX.EXITING", culture);

    public static string PARSER_ADDINGTAG(object arg0, object arg1) => ServerResources.Format("PARSER.ADDINGTAG", arg0, arg1);

    public static string PARSER_ADDINGTAG(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("PARSER.ADDINGTAG", culture, arg0, arg1);

    public static string PARSER_POPPINGCOMPONENT(object arg0) => ServerResources.Format("PARSER.POPPINGCOMPONENT", arg0);

    public static string PARSER_POPPINGCOMPONENT(object arg0, CultureInfo culture) => ServerResources.Format("PARSER.POPPINGCOMPONENT", culture, arg0);

    public static string PARSER_PUSHINGCONDITIONAL() => ServerResources.Get("PARSER.PUSHINGCONDITIONAL");

    public static string PARSER_PUSHINGCONDITIONAL(CultureInfo culture) => ServerResources.Get("PARSER.PUSHINGCONDITIONAL", culture);

    public static string PARSER_PUSHINGREPETITION() => ServerResources.Get("PARSER.PUSHINGREPETITION");

    public static string PARSER_PUSHINGREPETITION(CultureInfo culture) => ServerResources.Get("PARSER.PUSHINGREPETITION", culture);

    public static string PARSER_X_MISSINGOPENINGBRACE(object arg0) => ServerResources.Format("PARSER.X-MISSINGOPENINGBRACE", arg0);

    public static string PARSER_X_MISSINGOPENINGBRACE(object arg0, CultureInfo culture) => ServerResources.Format("PARSER.X-MISSINGOPENINGBRACE", culture, arg0);

    public static string PARSER_X_NULLTAG() => ServerResources.Get("PARSER.X-NULLTAG");

    public static string PARSER_X_NULLTAG(CultureInfo culture) => ServerResources.Get("PARSER.X-NULLTAG", culture);

    public static string PARSER_X_UNBALANCEDTAG(object arg0, object arg1) => ServerResources.Format("PARSER.X-UNBALANCEDTAG", arg0, arg1);

    public static string PARSER_X_UNBALANCEDTAG(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("PARSER.X-UNBALANCEDTAG", culture, arg0, arg1);

    public static string PARSER_X_UNBALANCEDTAG2(object arg0) => ServerResources.Format("PARSER.X-UNBALANCEDTAG2", arg0);

    public static string PARSER_X_UNBALANCEDTAG2(object arg0, CultureInfo culture) => ServerResources.Format("PARSER.X-UNBALANCEDTAG2", culture, arg0);

    public static string PARSER_EXPECTED_INT() => ServerResources.Get(nameof (PARSER_EXPECTED_INT));

    public static string PARSER_EXPECTED_INT(CultureInfo culture) => ServerResources.Get(nameof (PARSER_EXPECTED_INT), culture);

    public static string PARSER_SUB_SHOULDNOTBEHERE() => ServerResources.Get("PARSER_SUB.SHOULDNOTBEHERE");

    public static string PARSER_SUB_SHOULDNOTBEHERE(CultureInfo culture) => ServerResources.Get("PARSER_SUB.SHOULDNOTBEHERE", culture);

    public static string PARSER_SUB_X_EXPECTED(object arg0) => ServerResources.Format("PARSER_SUB.X-EXPECTED", arg0);

    public static string PARSER_SUB_X_EXPECTED(object arg0, CultureInfo culture) => ServerResources.Format("PARSER_SUB.X-EXPECTED", culture, arg0);

    public static string PARSER_SUB_X_EXPECTEDBOOL(object arg0) => ServerResources.Format("PARSER_SUB.X-EXPECTEDBOOL", arg0);

    public static string PARSER_SUB_X_EXPECTEDBOOL(object arg0, CultureInfo culture) => ServerResources.Format("PARSER_SUB.X-EXPECTEDBOOL", culture, arg0);

    public static string PARSER_SUB_X_SYNTAXERROR(object arg0) => ServerResources.Format("PARSER_SUB.X-SYNTAXERROR", arg0);

    public static string PARSER_SUB_X_SYNTAXERROR(object arg0, CultureInfo culture) => ServerResources.Format("PARSER_SUB.X-SYNTAXERROR", culture, arg0);

    public static string PERFORMANCECOUNTERS_ASYNCEVENTSPERSEC() => ServerResources.Get("PERFORMANCECOUNTERS.ASYNCEVENTSPERSEC");

    public static string PERFORMANCECOUNTERS_ASYNCEVENTSPERSEC(CultureInfo culture) => ServerResources.Get("PERFORMANCECOUNTERS.ASYNCEVENTSPERSEC", culture);

    public static string PERFORMANCECOUNTERS_AVGBASE() => ServerResources.Get("PERFORMANCECOUNTERS.AVGBASE");

    public static string PERFORMANCECOUNTERS_AVGBASE(CultureInfo culture) => ServerResources.Get("PERFORMANCECOUNTERS.AVGBASE", culture);

    public static string PERFORMANCECOUNTERS_AVGEVENTPROCTIME() => ServerResources.Get("PERFORMANCECOUNTERS.AVGEVENTPROCTIME");

    public static string PERFORMANCECOUNTERS_AVGEVENTPROCTIME(CultureInfo culture) => ServerResources.Get("PERFORMANCECOUNTERS.AVGEVENTPROCTIME", culture);

    public static string PERFORMANCECOUNTERS_SUBSCRIPTIONS() => ServerResources.Get("PERFORMANCECOUNTERS.SUBSCRIPTIONS");

    public static string PERFORMANCECOUNTERS_SUBSCRIPTIONS(CultureInfo culture) => ServerResources.Get("PERFORMANCECOUNTERS.SUBSCRIPTIONS", culture);

    public static string PERFORMANCECOUNTERS_SYNCEVENTSPERSEC() => ServerResources.Get("PERFORMANCECOUNTERS.SYNCEVENTSPERSEC");

    public static string PERFORMANCECOUNTERS_SYNCEVENTSPERSEC(CultureInfo culture) => ServerResources.Get("PERFORMANCECOUNTERS.SYNCEVENTSPERSEC", culture);

    public static string PGIINIT_ADDPG() => ServerResources.Get("PGIINIT.ADDPG");

    public static string PGIINIT_ADDPG(CultureInfo culture) => ServerResources.Get("PGIINIT.ADDPG", culture);

    public static string PGIINIT_ENTER() => ServerResources.Get("PGIINIT.ENTER");

    public static string PGIINIT_ENTER(CultureInfo culture) => ServerResources.Get("PGIINIT.ENTER", culture);

    public static string PGIINIT_EXIT() => ServerResources.Get("PGIINIT.EXIT");

    public static string PGIINIT_EXIT(CultureInfo culture) => ServerResources.Get("PGIINIT.EXIT", culture);

    public static string PGIINIT_NOPG() => ServerResources.Get("PGIINIT.NOPG");

    public static string PGIINIT_NOPG(CultureInfo culture) => ServerResources.Get("PGIINIT.NOPG", culture);

    public static string PLUGIN_INITSECURITYPLUGINS() => ServerResources.Get("PLUGIN.INITSECURITYPLUGINS");

    public static string PLUGIN_INITSECURITYPLUGINS(CultureInfo culture) => ServerResources.Get("PLUGIN.INITSECURITYPLUGINS", culture);

    public static string PLUGIN_INITSECURITYPLUGINSCOMPLETE() => ServerResources.Get("PLUGIN.INITSECURITYPLUGINSCOMPLETE");

    public static string PLUGIN_INITSECURITYPLUGINSCOMPLETE(CultureInfo culture) => ServerResources.Get("PLUGIN.INITSECURITYPLUGINSCOMPLETE", culture);

    public static string PLUGIN_LOADINGPLUGININSTANCE() => ServerResources.Get("PLUGIN.LOADINGPLUGININSTANCE");

    public static string PLUGIN_LOADINGPLUGININSTANCE(CultureInfo culture) => ServerResources.Get("PLUGIN.LOADINGPLUGININSTANCE", culture);

    public static string REGISTRATION_ASMX_GETREGISTRATIONENTRIES(object arg0) => ServerResources.Format("REGISTRATION.ASMX.GETREGISTRATIONENTRIES", arg0);

    public static string REGISTRATION_ASMX_GETREGISTRATIONENTRIES(object arg0, CultureInfo culture) => ServerResources.Format("REGISTRATION.ASMX.GETREGISTRATIONENTRIES", culture, arg0);

    public static string REGISTRATIONEVENTSCHEMAPROVIDER_DIDNOTFINDEXPECTEDENTRY() => ServerResources.Get("REGISTRATIONEVENTSCHEMAPROVIDER.DIDNOTFINDEXPECTEDENTRY");

    public static string REGISTRATIONEVENTSCHEMAPROVIDER_DIDNOTFINDEXPECTEDENTRY(CultureInfo culture) => ServerResources.Get("REGISTRATIONEVENTSCHEMAPROVIDER.DIDNOTFINDEXPECTEDENTRY", culture);

    public static string REGISTRATIONEVENTSCHEMAPROVIDER_LOOKINGFOREVENTSCHEMAS() => ServerResources.Get("REGISTRATIONEVENTSCHEMAPROVIDER.LOOKINGFOREVENTSCHEMAS");

    public static string REGISTRATIONEVENTSCHEMAPROVIDER_LOOKINGFOREVENTSCHEMAS(CultureInfo culture) => ServerResources.Get("REGISTRATIONEVENTSCHEMAPROVIDER.LOOKINGFOREVENTSCHEMAS", culture);

    public static string REGISTRATIONEVENTSCHEMAPROVIDER_X_EVENTTYPE() => ServerResources.Get("REGISTRATIONEVENTSCHEMAPROVIDER.X-EVENTTYPE");

    public static string REGISTRATIONEVENTSCHEMAPROVIDER_X_EVENTTYPE(CultureInfo culture) => ServerResources.Get("REGISTRATIONEVENTSCHEMAPROVIDER.X-EVENTTYPE", culture);

    public static string REPETITIONCOMPONENT_ITERATINGOVER(object arg0) => ServerResources.Format("REPETITIONCOMPONENT.ITERATINGOVER", arg0);

    public static string REPETITIONCOMPONENT_ITERATINGOVER(object arg0, CultureInfo culture) => ServerResources.Format("REPETITIONCOMPONENT.ITERATINGOVER", culture, arg0);

    public static string SCANNER_X_EXPECTEDGT() => ServerResources.Get("SCANNER.X-EXPECTEDGT");

    public static string SCANNER_X_EXPECTEDGT(CultureInfo culture) => ServerResources.Get("SCANNER.X-EXPECTEDGT", culture);

    public static string SCANNER_SUB_X_SYNTAXERROR(object arg0) => ServerResources.Format("SCANNER_SUB.X-SYNTAXERROR", arg0);

    public static string SCANNER_SUB_X_SYNTAXERROR(object arg0, CultureInfo culture) => ServerResources.Format("SCANNER_SUB.X-SYNTAXERROR", culture, arg0);

    public static string SCANNER_UNEXPECTED_NEWLINE() => ServerResources.Get(nameof (SCANNER_UNEXPECTED_NEWLINE));

    public static string SCANNER_UNEXPECTED_NEWLINE(CultureInfo culture) => ServerResources.Get(nameof (SCANNER_UNEXPECTED_NEWLINE), culture);

    public static string SCANNER_UNEXPECTED_EOF() => ServerResources.Get(nameof (SCANNER_UNEXPECTED_EOF));

    public static string SCANNER_UNEXPECTED_EOF(CultureInfo culture) => ServerResources.Get(nameof (SCANNER_UNEXPECTED_EOF), culture);

    public static string SEARCH_ASMX_ENTERED() => ServerResources.Get("SEARCH.ASMX.ENTERED");

    public static string SEARCH_ASMX_ENTERED(CultureInfo culture) => ServerResources.Get("SEARCH.ASMX.ENTERED", culture);

    public static string SEARCH_ASMX_EXITING() => ServerResources.Get("SEARCH.ASMX.EXITING");

    public static string SEARCH_ASMX_EXITING(CultureInfo culture) => ServerResources.Get("SEARCH.ASMX.EXITING", culture);

    public static string SERVERSTATUS_ASMX_CHECKAUTHENTICATION_IN() => ServerResources.Get("SERVERSTATUS.ASMX.CHECKAUTHENTICATION-IN");

    public static string SERVERSTATUS_ASMX_CHECKAUTHENTICATION_IN(CultureInfo culture) => ServerResources.Get("SERVERSTATUS.ASMX.CHECKAUTHENTICATION-IN", culture);

    public static string SERVERSTATUS_ASMX_CHECKAUTHENTICATION_OUT() => ServerResources.Get("SERVERSTATUS.ASMX.CHECKAUTHENTICATION-OUT");

    public static string SERVERSTATUS_ASMX_CHECKAUTHENTICATION_OUT(CultureInfo culture) => ServerResources.Get("SERVERSTATUS.ASMX.CHECKAUTHENTICATION-OUT", culture);

    public static string SERVERSTATUS_ASMX_GETSERVERSTATUS_IN() => ServerResources.Get("SERVERSTATUS.ASMX.GETSERVERSTATUS-IN");

    public static string SERVERSTATUS_ASMX_GETSERVERSTATUS_IN(CultureInfo culture) => ServerResources.Get("SERVERSTATUS.ASMX.GETSERVERSTATUS-IN", culture);

    public static string SERVERSTATUS_ASMX_GETSERVERSTATUS_OUT() => ServerResources.Get("SERVERSTATUS.ASMX.GETSERVERSTATUS-OUT");

    public static string SERVERSTATUS_ASMX_GETSERVERSTATUS_OUT(CultureInfo culture) => ServerResources.Get("SERVERSTATUS.ASMX.GETSERVERSTATUS-OUT", culture);

    public static string SERVERSTATUS_ASMX_GETSUPPORTEDCONTRACTVERSION_IN() => ServerResources.Get("SERVERSTATUS.ASMX.GETSUPPORTEDCONTRACTVERSION-IN");

    public static string SERVERSTATUS_ASMX_GETSUPPORTEDCONTRACTVERSION_IN(CultureInfo culture) => ServerResources.Get("SERVERSTATUS.ASMX.GETSUPPORTEDCONTRACTVERSION-IN", culture);

    public static string SERVERSTATUS_ASMX_GETSUPPORTEDCONTRACTVERSION_OUT() => ServerResources.Get("SERVERSTATUS.ASMX.GETSUPPORTEDCONTRACTVERSION-OUT");

    public static string SERVERSTATUS_ASMX_GETSUPPORTEDCONTRACTVERSION_OUT(CultureInfo culture) => ServerResources.Get("SERVERSTATUS.ASMX.GETSUPPORTEDCONTRACTVERSION-OUT", culture);

    public static string SIMPLEEVENTSCHEMAPROVIDER_X_NOFORMATTER() => ServerResources.Get("SIMPLEEVENTSCHEMAPROVIDER.X-NOFORMATTER");

    public static string SIMPLEEVENTSCHEMAPROVIDER_X_NOFORMATTER(CultureInfo culture) => ServerResources.Get("SIMPLEEVENTSCHEMAPROVIDER.X-NOFORMATTER", culture);

    public static string STRINGFIELDCONDITION_X_VALUENOTFOUND(object arg0) => ServerResources.Format("STRINGFIELDCONDITION.X-VALUENOTFOUND", arg0);

    public static string STRINGFIELDCONDITION_X_VALUENOTFOUND(object arg0, CultureInfo culture) => ServerResources.Format("STRINGFIELDCONDITION.X-VALUENOTFOUND", culture, arg0);

    public static string SUBSCRIBER_ACTIVATINGTHREAD() => ServerResources.Get("SUBSCRIBER.ACTIVATINGTHREAD");

    public static string SUBSCRIBER_ACTIVATINGTHREAD(CultureInfo culture) => ServerResources.Get("SUBSCRIBER.ACTIVATINGTHREAD", culture);

    public static string SUBSCRIBER_ENTERINGSENDLOOP() => ServerResources.Get("SUBSCRIBER.ENTERINGSENDLOOP");

    public static string SUBSCRIBER_ENTERINGSENDLOOP(CultureInfo culture) => ServerResources.Get("SUBSCRIBER.ENTERINGSENDLOOP", culture);

    public static string SUBSCRIBER_EXITINGSENDLOOP() => ServerResources.Get("SUBSCRIBER.EXITINGSENDLOOP");

    public static string SUBSCRIBER_EXITINGSENDLOOP(CultureInfo culture) => ServerResources.Get("SUBSCRIBER.EXITINGSENDLOOP", culture);

    public static string SUBSCRIBER_SENDLOOPEXCEPTION(object arg0) => ServerResources.Format("SUBSCRIBER.SENDLOOPEXCEPTION", arg0);

    public static string SUBSCRIBER_SENDLOOPEXCEPTION(object arg0, CultureInfo culture) => ServerResources.Format("SUBSCRIBER.SENDLOOPEXCEPTION", culture, arg0);

    public static string SUBSCRIBER_WAITINGFORAPRIOREVENT() => ServerResources.Get("SUBSCRIBER.WAITINGFORAPRIOREVENT");

    public static string SUBSCRIBER_WAITINGFORAPRIOREVENT(CultureInfo culture) => ServerResources.Get("SUBSCRIBER.WAITINGFORAPRIOREVENT", culture);

    public static string SUBSCRIPTION_CANNOTADDORUPDATE(object arg0) => ServerResources.Format("SUBSCRIPTION.CANNOTADDORUPDATE", arg0);

    public static string SUBSCRIPTION_CANNOTADDORUPDATE(object arg0, CultureInfo culture) => ServerResources.Format("SUBSCRIPTION.CANNOTADDORUPDATE", culture, arg0);

    public static string SUBSCRIPTION_CREATEDSUBSCRIPTION(object arg0) => ServerResources.Format("SUBSCRIPTION.CREATEDSUBSCRIPTION", arg0);

    public static string SUBSCRIPTION_CREATEDSUBSCRIPTION(object arg0, CultureInfo culture) => ServerResources.Format("SUBSCRIPTION.CREATEDSUBSCRIPTION", culture, arg0);

    public static string SUBSCRIPTION_INVALIDSUBSCRIPTION(object arg0) => ServerResources.Format("SUBSCRIPTION.INVALIDSUBSCRIPTION", arg0);

    public static string SUBSCRIPTION_INVALIDSUBSCRIPTION(object arg0, CultureInfo culture) => ServerResources.Format("SUBSCRIPTION.INVALIDSUBSCRIPTION", culture, arg0);

    public static string SUBSCRIPTION_PROBLEMSAVINGSUBSCRIPTION(object arg0) => ServerResources.Format("SUBSCRIPTION.PROBLEMSAVINGSUBSCRIPTION", arg0);

    public static string SUBSCRIPTION_PROBLEMSAVINGSUBSCRIPTION(object arg0, CultureInfo culture) => ServerResources.Format("SUBSCRIPTION.PROBLEMSAVINGSUBSCRIPTION", culture, arg0);

    public static string SUBSCRIPTION_SAVINGSUBSCRIPTION(object arg0) => ServerResources.Format("SUBSCRIPTION.SAVINGSUBSCRIPTION", arg0);

    public static string SUBSCRIPTION_SAVINGSUBSCRIPTION(object arg0, CultureInfo culture) => ServerResources.Format("SUBSCRIPTION.SAVINGSUBSCRIPTION", culture, arg0);

    public static string SUBSCRIPTION_X_SYNTAXERROR() => ServerResources.Get("SUBSCRIPTION.X-SYNTAXERROR");

    public static string SUBSCRIPTION_X_SYNTAXERROR(CultureInfo culture) => ServerResources.Get("SUBSCRIPTION.X-SYNTAXERROR", culture);

    public static string System_Security_SecurityException(object arg0) => ServerResources.Format("System.Security.SecurityException", arg0);

    public static string System_Security_SecurityException(object arg0, CultureInfo culture) => ServerResources.Format("System.Security.SecurityException", culture, arg0);

    public static string Microsoft_VisualStudio_Bis_Services_GroupSecuritySubsystemServiceException(
      object arg0)
    {
      return ServerResources.Format("Microsoft.VisualStudio.Bis.Services.GroupSecuritySubsystemServiceException", arg0);
    }

    public static string Microsoft_VisualStudio_Bis_Services_GroupSecuritySubsystemServiceException(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format("Microsoft.VisualStudio.Bis.Services.GroupSecuritySubsystemServiceException", culture, arg0);
    }

    public static string System_ArgumentException() => ServerResources.Get("System.ArgumentException");

    public static string System_ArgumentException(CultureInfo culture) => ServerResources.Get("System.ArgumentException", culture);

    public static string System_Data_SqlClient_SqlException(object arg0) => ServerResources.Format("System.Data.SqlClient.SqlException", arg0);

    public static string System_Data_SqlClient_SqlException(object arg0, CultureInfo culture) => ServerResources.Format("System.Data.SqlClient.SqlException", culture, arg0);

    public static string VARIABLECOMPONENT_EVALUATINGVARIABLE(object arg0, object arg1) => ServerResources.Format("VARIABLECOMPONENT.EVALUATINGVARIABLE", arg0, arg1);

    public static string VARIABLECOMPONENT_EVALUATINGVARIABLE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("VARIABLECOMPONENT.EVALUATINGVARIABLE", culture, arg0, arg1);
    }

    public static string VARIABLECOMPONENT_X_NONAMEATTRIBUTE() => ServerResources.Get("VARIABLECOMPONENT.X-NONAMEATTRIBUTE");

    public static string VARIABLECOMPONENT_X_NONAMEATTRIBUTE(CultureInfo culture) => ServerResources.Get("VARIABLECOMPONENT.X-NONAMEATTRIBUTE", culture);

    public static string XMLDOCUMENTFIELDCONTAINER_UNKNOWNFIELDTYPE(object arg0) => ServerResources.Format("XMLDOCUMENTFIELDCONTAINER.UNKNOWNFIELDTYPE", arg0);

    public static string XMLDOCUMENTFIELDCONTAINER_UNKNOWNFIELDTYPE(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format("XMLDOCUMENTFIELDCONTAINER.UNKNOWNFIELDTYPE", culture, arg0);
    }

    public static string XMLDOCUMENTFIELDCONTAINER_X_FIELDNAME() => ServerResources.Get("XMLDOCUMENTFIELDCONTAINER.X-FIELDNAME");

    public static string XMLDOCUMENTFIELDCONTAINER_X_FIELDNAME(CultureInfo culture) => ServerResources.Get("XMLDOCUMENTFIELDCONTAINER.X-FIELDNAME", culture);

    public static string UnauthorizedAccessNeedOnePermission(object arg0, object arg1) => ServerResources.Format(nameof (UnauthorizedAccessNeedOnePermission), arg0, arg1);

    public static string UnauthorizedAccessNeedOnePermission(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (UnauthorizedAccessNeedOnePermission), culture, arg0, arg1);
    }
  }
}
