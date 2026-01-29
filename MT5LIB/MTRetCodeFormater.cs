using MetaQuotes.MT5CommonAPI;

namespace MT5LIB;

internal class MTRetCodeFormater
{
    public static string Format(MTRetCode retcode)
    {
        switch (retcode)
        {
            case MTRetCode.MT_RET_OK:
                return "Done";
            case MTRetCode.MT_RET_OK_NONE:
                return "OK/None";
            case MTRetCode.MT_RET_ERROR:
                return "Common error";
            case MTRetCode.MT_RET_ERR_PARAMS:
                return "Invalid parameters";
            case MTRetCode.MT_RET_ERR_DATA:
                return "Invalid data";
            case MTRetCode.MT_RET_ERR_DISK:
                return "Disk error";
            case MTRetCode.MT_RET_ERR_MEM:
                return "Memory error";
            case MTRetCode.MT_RET_ERR_NETWORK:
                return "Network error";
            case MTRetCode.MT_RET_ERR_PERMISSIONS:
                return "Not enough permissions";
            case MTRetCode.MT_RET_ERR_TIMEOUT:
                return "Operation timeout";
            case MTRetCode.MT_RET_ERR_CONNECTION:
                return "No connection";
            case MTRetCode.MT_RET_ERR_NOSERVICE:
                return "Service is not available";
            case MTRetCode.MT_RET_ERR_FREQUENT:
                return "Too frequent requests";
            case MTRetCode.MT_RET_ERR_NOTFOUND:
                return "Not found";
            case MTRetCode.MT_RET_ERR_SHUTDOWN:
                return "Server shutdown in progress";
            case MTRetCode.MT_RET_ERR_CANCEL:
                return "Operation was canceled";
            case MTRetCode.MT_RET_ERR_DUPLICATE:
                return "Duplicate attempt";
            case MTRetCode.MT_RET_AUTH_CLIENT_INVALID:
                return "Invalid terminal type";
            case MTRetCode.MT_RET_AUTH_ACCOUNT_INVALID:
                return "Invalid account";
            case MTRetCode.MT_RET_AUTH_ACCOUNT_DISABLED:
                return "Account disabled";
            case MTRetCode.MT_RET_AUTH_ADVANCED:
                return "Advanced authorization";
            case MTRetCode.MT_RET_AUTH_CERTIFICATE:
                return "Certificate required";
            case MTRetCode.MT_RET_AUTH_CERTIFICATE_BAD:
                return "Invalid certificate";
            case MTRetCode.MT_RET_AUTH_NOTCONFIRMED:
                return "Certificate is not confirmed";
            case MTRetCode.MT_RET_AUTH_SERVER_INTERNAL:
                return "Attempt to connect to non-access server";
            case MTRetCode.MT_RET_AUTH_SERVER_BAD:
                return "Invalid or fake server";
            case MTRetCode.MT_RET_AUTH_UPDATE_ONLY:
                return "Only updates available";
            case MTRetCode.MT_RET_AUTH_CLIENT_OLD:
                return "Old version";
            case MTRetCode.MT_RET_AUTH_MANAGER_NOCONFIG:
                return "Account doesn't have manager config";
            case MTRetCode.MT_RET_AUTH_MANAGER_IPBLOCK:
                return "IP address unallowed for manager";
            case MTRetCode.MT_RET_AUTH_GROUP_INVALID:
                return "Group is not initialized, server restart needed";
            case MTRetCode.MT_RET_AUTH_CA_DISABLED:
                return "Certificate generation disabled";
            case MTRetCode.MT_RET_AUTH_INVALID_ID:
                return "Invalid or disabled server id [check server id]";
            case MTRetCode.MT_RET_AUTH_INVALID_IP:
                return "Unallowed address [check server ip address]";
            case MTRetCode.MT_RET_AUTH_INVALID_TYPE:
                return "Invalid server type [check server id and type]";
            case MTRetCode.MT_RET_AUTH_SERVER_BUSY:
                return "Server is busy";
            case MTRetCode.MT_RET_AUTH_SERVER_CERT:
                return "Invalid server certificate or invalid local time";
            case MTRetCode.MT_RET_AUTH_ACCOUNT_UNKNOWN:
                return "Unknown account";
            case MTRetCode.MT_RET_AUTH_SERVER_OLD:
                return "Old server version";
            case MTRetCode.MT_RET_AUTH_SERVER_LIMIT:
                return "Server cannot be connected due to license limitation";
            case MTRetCode.MT_RET_AUTH_MOBILE_DISABLED:
                return "Mobile terminal isn't allowed in license";
            case MTRetCode.MT_RET_AUTH_MANAGER_TYPE:
                return "Connection type is not permitted for manager";
            case MTRetCode.MT_RET_AUTH_DEMO_DISABLED:
                return "Demo allocation disabled";
            case MTRetCode.MT_RET_AUTH_RESET_PASSWORD:
                return "Master password must be changed";
            case MTRetCode.MT_RET_CFG_LAST_ADMIN:
                return "Last admin config cannot be deleted";
            case MTRetCode.MT_RET_CFG_LAST_ADMIN_GROUP:
                return "Last admin group cannot be deleted";
            case MTRetCode.MT_RET_CFG_NOT_EMPTY:
                return "Accounts or trades in group/symbo: return ";
            case MTRetCode.MT_RET_CFG_INVALID_RANGE:
                return "Invalid account or trade ranges";
            case MTRetCode.MT_RET_CFG_NOT_MANAGER_LOGIN:
                return "Account doesn't belong to manager group";
            case MTRetCode.MT_RET_CFG_BUILTIN:
                return "Built-in protected config";
            case MTRetCode.MT_RET_CFG_DUPLICATE:
                return "Configuration duplicate";
            case MTRetCode.MT_RET_CFG_LIMIT_REACHED:
                return "Configuration limit reached";
            case MTRetCode.MT_RET_CFG_NO_ACCESS_TO_MAIN:
                return "Invalid network configuration";
            case MTRetCode.MT_RET_CFG_DEALER_ID_EXIST:
                return "Dealer with same ID already exists";
            case MTRetCode.MT_RET_CFG_BIND_ADDR_EXIST:
                return "Binding address already exists";
            case MTRetCode.MT_RET_CFG_WORKING_TRADE:
                return "Attempt to delete working trade server";
            case MTRetCode.MT_RET_CFG_GATEWAY_NAME_EXIST:
                return "Gateway with same name already exists";
            case MTRetCode.MT_RET_CFG_SWITCH_TO_BACKUP:
                return "Server must be switched to backup mode";
            case MTRetCode.MT_RET_CFG_NO_BACKUP_MODULE:
                return "Backup server module is absent";
            case MTRetCode.MT_RET_CFG_NO_TRADE_MODULE:
                return "Trade server module is absent";
            case MTRetCode.MT_RET_CFG_NO_HISTORY_MODULE:
                return "History server module is absent";
            case MTRetCode.MT_RET_CFG_ANOTHER_SWITCH:
                return "Another switching process in progress";
            case MTRetCode.MT_RET_CFG_NO_LICENSE_FILE:
                return "License file is absent";
            case MTRetCode.MT_RET_CFG_GATEWAY_LOGIN_EXIST:
                return "Gateway with same login already exist";
            case MTRetCode.MT_RET_USR_LAST_ADMIN:
                return "Last admin account cannot be deleted";
            case MTRetCode.MT_RET_USR_LOGIN_EXHAUSTED:
                return "Login range exhausted";
            case MTRetCode.MT_RET_USR_LOGIN_PROHIBITED:
                return "Login reserved at another server";
            case MTRetCode.MT_RET_USR_LOGIN_EXIST:
                return "Account already exists";
            case MTRetCode.MT_RET_USR_SUICIDE:
                return "Attempt of self-deletion";
            case MTRetCode.MT_RET_USR_INVALID_PASSWORD:
                return "Invalid account password";
            case MTRetCode.MT_RET_USR_LIMIT_REACHED:
                return "User limit reached";
            case MTRetCode.MT_RET_USR_HAS_TRADES:
                return "Account has open trades";
            case MTRetCode.MT_RET_USR_DIFFERENT_SERVERS:
                return "Attempt to move account to different server";
            case MTRetCode.MT_RET_USR_DIFFERENT_CURRENCY:
                return "Attempt to move account to group with different currency";
            case MTRetCode.MT_RET_USR_IMPORT_BALANCE:
                return "Account balance import error";
            case MTRetCode.MT_RET_USR_IMPORT_GROUP:
                return "Imported account has invalid group";
            case MTRetCode.MT_RET_USR_ACCOUNT_EXIST:
                return "Account already exist";
            case MTRetCode.MT_RET_TRADE_LIMIT_REACHED:
                return "Order or deal limit reached";
            case MTRetCode.MT_RET_TRADE_ORDER_EXIST:
                return "Order already exists";
            case MTRetCode.MT_RET_TRADE_ORDER_EXHAUSTED:
                return "Order range exhausted";
            case MTRetCode.MT_RET_TRADE_DEAL_EXHAUSTED:
                return "Deal range exhausted";
            case MTRetCode.MT_RET_TRADE_MAX_MONEY:
                return "Money limit reached";
            case MTRetCode.MT_RET_TRADE_DEAL_EXIST:
                return "Deal already exists";
            case MTRetCode.MT_RET_TRADE_ORDER_PROHIBITED:
                return "Order ticket reserved at another server";
            case MTRetCode.MT_RET_TRADE_DEAL_PROHIBITED:
                return "Deal ticket reserved at another server";
            case MTRetCode.MT_RET_TRADE_SPLIT_VOLUME:
                return "Volume of the new position is less than the minimum allowed";
            case MTRetCode.MT_RET_REPORT_SNAPSHOT:
                return "Base snapshot error";
            case MTRetCode.MT_RET_REPORT_NOTSUPPORTED:
                return "Method is not supported by this report";
            case MTRetCode.MT_RET_REPORT_NODATA:
                return "No data for report";
            case MTRetCode.MT_RET_REPORT_TEMPLATE_BAD:
                return "Bad template";
            case MTRetCode.MT_RET_REPORT_TEMPLATE_END:
                return "End of template";
            case MTRetCode.MT_RET_REPORT_INVALID_ROW:
                return "Invalid row size";
            case MTRetCode.MT_RET_REPORT_LIMIT_REPEAT:
                return "Tag repeat limit reached ";
            case MTRetCode.MT_RET_REPORT_LIMIT_REPORT:
                return "Report size limit reached";
            case MTRetCode.MT_RET_HST_SYMBOL_NOTFOUND:
                return "Symbol not found, try to restart history server";
            case MTRetCode.MT_RET_REQUEST_INWAY:
                return "Request on the way";
            case MTRetCode.MT_RET_REQUEST_ACCEPTED:
                return "Request accepted";
            case MTRetCode.MT_RET_REQUEST_PROCESS:
                return "Request processed";
            case MTRetCode.MT_RET_REQUEST_REQUOTE:
                return "Requote";
            case MTRetCode.MT_RET_REQUEST_PRICES:
                return "Prices";
            case MTRetCode.MT_RET_REQUEST_REJECT:
                return "Request rejected";
            case MTRetCode.MT_RET_REQUEST_CANCEL:
                return "Request canceled";
            case MTRetCode.MT_RET_REQUEST_PLACED:
                return "Order placed";
            case MTRetCode.MT_RET_REQUEST_DONE:
                return "Request executed";
            case MTRetCode.MT_RET_REQUEST_DONE_PARTIAL:
                return "Request executed partially";
            case MTRetCode.MT_RET_REQUEST_ERROR:
                return "Request error";
            case MTRetCode.MT_RET_REQUEST_TIMEOUT:
                return "Request timeout";
            case MTRetCode.MT_RET_REQUEST_INVALID:
                return "Invalid request";
            case MTRetCode.MT_RET_REQUEST_INVALID_VOLUME:
                return "Invalid volume";
            case MTRetCode.MT_RET_REQUEST_INVALID_PRICE:
                return "Invalid price";
            case MTRetCode.MT_RET_REQUEST_INVALID_STOPS:
                return "Invalid stops";
            case MTRetCode.MT_RET_REQUEST_TRADE_DISABLED:
                return "Trade disabled";
            case MTRetCode.MT_RET_REQUEST_MARKET_CLOSED:
                return "Market closed";
            case MTRetCode.MT_RET_REQUEST_NO_MONEY:
                return "No money";
            case MTRetCode.MT_RET_REQUEST_PRICE_CHANGED:
                return "Price changed";
            case MTRetCode.MT_RET_REQUEST_PRICE_OFF:
                return "No prices";
            case MTRetCode.MT_RET_REQUEST_INVALID_EXP:
                return "Invalid expiration";
            case MTRetCode.MT_RET_REQUEST_ORDER_CHANGED:
                return "Order has been changed already";
            case MTRetCode.MT_RET_REQUEST_TOO_MANY:
                return "Too many trade requests";
            case MTRetCode.MT_RET_REQUEST_AT_DISABLED_SERVER:
                return "AutoTrading disabled by server";
            case MTRetCode.MT_RET_REQUEST_AT_DISABLED_CLIENT:
                return "AutoTrading disabled by client";
            case MTRetCode.MT_RET_REQUEST_LOCKED:
                return "Order locked by dealer";
            case MTRetCode.MT_RET_REQUEST_FROZEN:
                return "Modification failed due to order or position being close to market";
            case MTRetCode.MT_RET_REQUEST_INVALID_FILL:
                return "Unsupported filling mode";
            case MTRetCode.MT_RET_REQUEST_ONLY_REAL:
                return "Allowed for real accounts only";
            case MTRetCode.MT_RET_REQUEST_LIMIT_ORDERS:
                return "Order limit reached";
            case MTRetCode.MT_RET_REQUEST_LIMIT_VOLUME:
                return "Volume limit reached";
            case MTRetCode.MT_RET_REQUEST_POSITION_CLOSED:
                return "Position already closed";
            case MTRetCode.MT_RET_REQUEST_EXECUTION_SKIPPED:
                return "Execution doesn't belong to this server";
            case MTRetCode.MT_RET_REQUEST_INVALID_CLOSE_VOLUME:
                return "Volume to be closed exceeds the position volume";
            case MTRetCode.MT_RET_REQUEST_CLOSE_ORDER_EXIST:
                return "Order to close this position already exists";
            case MTRetCode.MT_RET_REQUEST_LIMIT_POSITIONS:
                return "Position limit reached";
            case MTRetCode.MT_RET_REQUEST_REJECT_CANCEL:
                return "Request rejected, order will be canceled";
            case MTRetCode.MT_RET_REQUEST_LONG_ONLY:
                return "Only long positions are allowed";
            case MTRetCode.MT_RET_REQUEST_SHORT_ONLY:
                return "Only short positions are allowed";
            case MTRetCode.MT_RET_REQUEST_CLOSE_ONLY:
                return "Only position closing is allowed";
            case MTRetCode.MT_RET_REQUEST_RETURN:
                return "Request returned in queue";
            case MTRetCode.MT_RET_REQUEST_DONE_CANCEL:
                return "Request executed partially";
            case MTRetCode.MT_RET_REQUEST_REQUOTE_RETURN:
                return "Requote";
            case MTRetCode.MT_RET_ERR_NOTIMPLEMENT:
                return "Not implemented";
            case MTRetCode.MT_RET_ERR_NOTMAIN:
                return "Operation must be performed on main server";
            case MTRetCode.MT_RET_ERR_NOTSUPPORTED:
                return "Command doesn't supported";
            case MTRetCode.MT_RET_ERR_DEADLOCK:
                return "Operation canceled due possible deadlock";
            case MTRetCode.MT_RET_ERR_LOCKED:
                return "Operation on locked entity";
            case MTRetCode.MT_RET_MESSENGER_INVALID_PHONE:
                return "Invalid phone number";
            case MTRetCode.MT_RET_MESSENGER_NOT_MOBILE:
                return "Phone number isn't mobile";
            case MTRetCode.MT_RET_SUBS_NOT_FOUND:
                return "Subscription is not found";
            case MTRetCode.MT_RET_SUBS_NOT_FOUND_CFG:
                return "Subscription config is not found";
            case MTRetCode.MT_RET_SUBS_NOT_FOUND_USER:
                return "User for subscription is not found";
            case MTRetCode.MT_RET_SUBS_DISABLED:
                return "Subscription is disabled";
            case MTRetCode.MT_RET_SUBS_PERMISSION_USER:
                return "Subscription is not allowed for user";
            case MTRetCode.MT_RET_SUBS_PERMISSION_SUBSCRIBE:
                return "Subscribe is not allowed";
            case MTRetCode.MT_RET_SUBS_PERMISSION_UNSUBSCRIBE:
                return "Unsubscribe is not allowed";
            case MTRetCode.MT_RET_SUBS_REAL_ONLY:
                return "Subscriptions are available for real users only";
            default:
                return string.Format("Unknown error {0}", (object)retcode);
        }
    }
}
