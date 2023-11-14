using System.Reflection;
using System.Runtime.InteropServices;

namespace RI.System.WinAPI
{
    [ComVisible(false)]
    public class WinError
    {
        public const int ERROR_SUCCESS = 0;

        public const int ERROR_NO_MORE_ITEMS = 259;

        public const int ERROR_INVALID_FUNCTION = 1;

        public const int ERROR_FILE_NOT_FOUND = 2;

        public const int ERROR_PATH_NOT_FOUND = 3;

        public const int ERROR_TOO_MANY_OPEN_FILES = 4;

        public const int ERROR_ACCESS_DENIED = 5;

        public const int ERROR_INVALID_HANDLE = 6;

        public const int ERROR_ARENA_TRASHED = 7;

        public const int ERROR_NOT_ENOUGH_MEMORY = 8;

        public const int ERROR_INVALID_BLOCK = 9;

        public const int ERROR_BAD_ENVIRONMENT = 10;

        public const int ERROR_BAD_FORMAT = 11;

        public const int ERROR_INVALID_ACCESS = 12;

        public const int ERROR_INVALID_DATA = 13;

        public const int ERROR_OUTOFMEMORY = 14;

        public const int ERROR_INVALID_DRIVE = 15;

        public const int ERROR_CURRENT_DIRECTORY = 16;

        public const int ERROR_NOT_SAME_DEVICE = 17;

        public const int ERROR_NO_MORE_FILES = 18;

        public const int ERROR_WRITE_PROTECT = 19;

        public const int ERROR_BAD_UNIT = 20;

        public const int ERROR_NOT_READY = 21;

        public const int ERROR_BAD_COMMAND = 22;

        public const int ERROR_CRC = 23;

        public const int ERROR_BAD_LENGTH = 24;

        public const int ERROR_SEEK = 25;

        public const int ERROR_NOT_DOS_DISK = 26;

        public const int ERROR_SECTOR_NOT_FOUND = 27;

        public const int ERROR_OUT_OF_PAPER = 28;

        public const int ERROR_WRITE_FAULT = 29;

        public const int ERROR_READ_FAULT = 30;

        public const int ERROR_GEN_FAILURE = 31;

        public const int ERROR_INVALID_PASSWORD = 86;

        public const int ERROR_INVALID_NAME = 123;

        public const int ERROR_INVALID_LEVEL = 124;

        public const int ERROR_NO_VOLUME_LABEL = 125;

        public const int ERROR_MOD_NOT_FOUND = 126;

        public const int ERROR_PROC_NOT_FOUND = 127;

        public const int ERROR_WAIT_NO_CHILDREN = 128;

        public const int ERROR_FILEMARK_DETECTED = 1101;

        public const int DNS_ERROR_RESPONSE_CODES_BASE = 9000;

        public const int DNS_ERROR_RCODE_FORMAT_ERROR = 9001;

        public const int DNS_ERROR_RCODE_SERVER_FAILURE = 9002;

        public const int DNS_ERROR_RCODE_NAME_ERROR = 9003;

        public const int DNS_ERROR_RCODE_NOT_IMPLEMENTED = 9004;

        public const int DNS_ERROR_RCODE_REFUSED = 9005;

        public const int DNS_ERROR_RCODE_YXDOMAIN = 9006;

        public const int DNS_ERROR_RCODE_YXRRSET = 9007;

        public const int DNS_ERROR_RCODE_NXRRSET = 9008;

        public const int DNS_ERROR_RCODE_NOTAUTH = 9009;

        public const int DNS_ERROR_RCODE_NOTZONE = 9010;

        public const int DNS_ERROR_RCODE_BADSIG = 9016;

        public const int DNS_ERROR_RCODE_BADKEY = 9017;

        public const int DNS_ERROR_RCODE_BADTIME = 9018;

        public const int DNS_ERROR_PACKET_FMT_BASE = 9500;

        public const int DNS_INFO_NO_RECORDS = 9501;

        public const int DNS_ERROR_BAD_PACKET = 9502;

        public const int DNS_ERROR_NO_PACKET = 9503;

        public const int DNS_ERROR_RCODE = 9504;

        public const int DNS_ERROR_UNSECURE_PACKET = 9505;

        public const int DNS_ERROR_NO_MEMORY = 14;

        public const int DNS_ERROR_INVALID_NAME = 123;

        public const int DNS_ERROR_INVALID_DATA = 13;

        public const int DNS_ERROR_GENERAL_API_BASE = 9550;

        public const int DNS_ERROR_INVALID_TYPE = 9551;

        public const int DNS_ERROR_INVALID_IP_ADDRESS = 9552;

        public const int DNS_ERROR_INVALID_PROPERTY = 9553;

        public const int DNS_ERROR_TRY_AGAIN_LATER = 9554;

        public const int DNS_ERROR_NOT_UNIQUE = 9555;

        public const int DNS_ERROR_NON_RFC_NAME = 9556;

        public const int DNS_STATUS_FQDN = 9557;

        public const int DNS_STATUS_DOTTED_NAME = 9558;

        public const int DNS_STATUS_SINGLE_PART_NAME = 9559;

        public const int DNS_ERROR_INVALID_NAME_CHAR = 9560;

        public const int DNS_ERROR_NUMERIC_NAME = 9561;

        public const int DNS_ERROR_NOT_ALLOWED_ON_ROOT_SERVER = 9562;

        public const int DNS_ERROR_NOT_ALLOWED_UNDER_DELEGATION = 9563;

        public const int DNS_ERROR_CANNOT_FIND_ROOT_HINTS = 9564;

        public const int DNS_ERROR_INCONSISTENT_ROOT_HINTS = 9565;

        public const int DNS_ERROR_ZONE_BASE = 9600;

        public const int DNS_ERROR_ZONE_DOES_NOT_EXIST = 9601;

        public const int DNS_ERROR_NO_ZONE_INFO = 9602;

        public const int DNS_ERROR_INVALID_ZONE_OPERATION = 9603;

        public const int DNS_ERROR_ZONE_CONFIGURATION_ERROR = 9604;

        public const int DNS_ERROR_ZONE_HAS_NO_SOA_RECORD = 9605;

        public const int DNS_ERROR_ZONE_HAS_NO_NS_RECORDS = 9606;

        public const int DNS_ERROR_ZONE_LOCKED = 9607;

        public const int DNS_ERROR_ZONE_CREATION_FAILED = 9608;

        public const int DNS_ERROR_ZONE_ALREADY_EXISTS = 9609;

        public const int DNS_ERROR_AUTOZONE_ALREADY_EXISTS = 9610;

        public const int DNS_ERROR_INVALID_ZONE_TYPE = 9611;

        public const int DNS_ERROR_SECONDARY_REQUIRES_MASTER_IP = 9612;

        public const int DNS_ERROR_ZONE_NOT_SECONDARY = 9613;

        public const int DNS_ERROR_NEED_SECONDARY_ADDRESSES = 9614;

        public const int DNS_ERROR_WINS_INIT_FAILED = 9615;

        public const int DNS_ERROR_NEED_WINS_SERVERS = 9616;

        public const int DNS_ERROR_NBSTAT_INIT_FAILED = 9617;

        public const int DNS_ERROR_SOA_DELETE_INVALID = 9618;

        public const int DNS_ERROR_FORWARDER_ALREADY_EXISTS = 9619;

        public const int DNS_ERROR_ZONE_REQUIRES_MASTER_IP = 9620;

        public const int DNS_ERROR_ZONE_IS_SHUTDOWN = 9621;

        public const int DNS_ERROR_DATAFILE_BASE = 9650;

        public const int DNS_ERROR_PRIMARY_REQUIRES_DATAFILE = 9651;

        public const int DNS_ERROR_INVALID_DATAFILE_NAME = 9652;

        public const int DNS_ERROR_DATAFILE_OPEN_FAILURE = 9653;

        public const int DNS_ERROR_FILE_WRITEBACK_FAILED = 9654;

        public const int DNS_ERROR_DATAFILE_PARSING = 9655;

        public const int DNS_ERROR_DATABASE_BASE = 9700;

        public const int DNS_ERROR_RECORD_DOES_NOT_EXIST = 9701;

        public const int DNS_ERROR_RECORD_FORMAT = 9702;

        public const int DNS_ERROR_NODE_CREATION_FAILED = 9703;

        public const int DNS_ERROR_UNKNOWN_RECORD_TYPE = 9704;

        public const int DNS_ERROR_RECORD_TIMED_OUT = 9705;

        public const int DNS_ERROR_NAME_NOT_IN_ZONE = 9706;

        public const int DNS_ERROR_CNAME_LOOP = 9707;

        public const int DNS_ERROR_NODE_IS_CNAME = 9708;

        public const int DNS_ERROR_CNAME_COLLISION = 9709;

        public const int DNS_ERROR_RECORD_ONLY_AT_ZONE_ROOT = 9710;

        public const int DNS_ERROR_RECORD_ALREADY_EXISTS = 9711;

        public const int DNS_ERROR_SECONDARY_DATA = 9712;

        public const int DNS_ERROR_NO_CREATE_CACHE_DATA = 9713;

        public const int DNS_ERROR_NAME_DOES_NOT_EXIST = 9714;

        public const int DNS_WARNING_PTR_CREATE_FAILED = 9715;

        public const int DNS_WARNING_DOMAIN_UNDELETED = 9716;

        public const int DNS_ERROR_DS_UNAVAILABLE = 9717;

        public const int DNS_ERROR_DS_ZONE_ALREADY_EXISTS = 9718;

        public const int DNS_ERROR_NO_BOOTFILE_IF_DS_ZONE = 9719;

        public const int DNS_ERROR_OPERATION_BASE = 9750;

        public const int DNS_INFO_AXFR_COMPLETE = 9751;

        public const int DNS_ERROR_AXFR = 9752;

        public const int DNS_INFO_ADDED_LOCAL_WINS = 9753;

        public const int DNS_ERROR_SECURE_BASE = 9800;

        public const int DNS_STATUS_CONTINUE_NEEDED = 9801;

        public const int DNS_ERROR_SETUP_BASE = 9850;

        public const int DNS_ERROR_NO_TCPIP = 9851;

        public const int DNS_ERROR_NO_DNS_SERVERS = 9852;

        public const int DNS_ERROR_DP_BASE = 9900;

        public const int DNS_ERROR_DP_DOES_NOT_EXIST = 9901;

        public const int DNS_ERROR_DP_ALREADY_EXISTS = 9902;

        public const int DNS_ERROR_DP_NOT_ENLISTED = 9903;

        public const int DNS_ERROR_DP_ALREADY_ENLISTED = 9904;

        public const int DNS_ERROR_DP_NOT_AVAILABLE = 9905;

        public const int WSABASEERR = 10000;

        public const int WSAEINTR = 10004;

        public const int WSAEBADF = 10009;

        public const int WSAEACCES = 10013;

        public const int WSAEFAULT = 10014;

        public const int WSAEINVAL = 10022;

        public const int WSAEMFILE = 10024;

        public const int WSAEWOULDBLOCK = 10035;

        public const int WSAEINPROGRESS = 10036;

        public const int WSAEALREADY = 10037;

        public const int WSAENOTSOCK = 10038;

        public const int WSAEDESTADDRREQ = 10039;

        public const int WSAEMSGSIZE = 10040;

        public const int WSAEPROTOTYPE = 10041;

        public const int WSAENOPROTOOPT = 10042;

        public const int WSAEPROTONOSUPPORT = 10043;

        public const int WSAESOCKTNOSUPPORT = 10044;

        public const int WSAEOPNOTSUPP = 10045;

        public const int WSAEPFNOSUPPORT = 10046;

        public const int WSAEAFNOSUPPORT = 10047;

        public const int WSAEADDRINUSE = 10048;

        public const int WSAEADDRNOTAVAIL = 10049;

        public const int WSAENETDOWN = 10050;

        public const int WSAENETUNREACH = 10051;

        public const int WSAENETRESET = 10052;

        public const int WSAECONNABORTED = 10053;

        public const int WSAECONNRESET = 10054;

        public const int WSAENOBUFS = 10055;

        public const int WSAEISCONN = 10056;

        public const int WSAENOTCONN = 10057;

        public const int WSAESHUTDOWN = 10058;

        public const int WSAETOOMANYREFS = 10059;

        public const int WSAETIMEDOUT = 10060;

        public const int WSAECONNREFUSED = 10061;

        public const int WSAELOOP = 10062;

        public const int WSAENAMETOOInt32 = 10063;

        public const int WSAEHOSTDOWN = 10064;

        public const int WSAEHOSTUNREACH = 10065;

        public const int WSAENOTEMPTY = 10066;

        public const int WSAEPROCLIM = 10067;

        public const int WSAEUSERS = 10068;

        public const int WSAEDQUOT = 10069;

        public const int WSAESTALE = 10070;

        public const int WSAEREMOTE = 10071;

        public const int WSASYSNOTREADY = 10091;

        public const int WSAVERNOTSUPPORTED = 10092;

        public const int WSANOTINITIALISED = 10093;

        public const int WSAEDISCON = 10101;

        public const int WSAENOMORE = 10102;

        public const int WSAECANCELLED = 10103;

        public const int WSAEINVALIDPROCTABLE = 10104;

        public const int WSAEINVALIDPROVIDER = 10105;

        public const int WSAEPROVIDERFAILEDINIT = 10106;

        public const int WSASYSCALLFAILURE = 10107;

        public const int WSASERVICE_NOT_FOUND = 10108;

        public const int WSATYPE_NOT_FOUND = 10109;

        public const int WSA_E_NO_MORE = 10110;

        public const int WSA_E_CANCELLED = 10111;

        public const int WSAEREFUSED = 10112;

        public const int WSAHOST_NOT_FOUND = 11001;

        public const int WSATRY_AGAIN = 11002;

        public const int WSANO_RECOVERY = 11003;

        public const int WSANO_DATA = 11004;

        public const int WSA_QOS_RECEIVERS = 11005;

        public const int WSA_QOS_SENDERS = 11006;

        public const int WSA_QOS_NO_SENDERS = 11007;

        public const int WSA_QOS_NO_RECEIVERS = 11008;

        public const int WSA_QOS_REQUEST_CONFIRMED = 11009;

        public const int WSA_QOS_ADMISSION_FAILURE = 11010;

        public const int WSA_QOS_POLICY_FAILURE = 11011;

        public const int WSA_QOS_BAD_STYLE = 11012;

        public const int WSA_QOS_BAD_OBJECT = 11013;

        public const int WSA_QOS_TRAFFIC_CTRL_ERROR = 11014;

        public const int WSA_QOS_GENERIC_ERROR = 11015;

        public const int WSA_QOS_ESERVICETYPE = 11016;

        public const int WSA_QOS_EFLOWSPEC = 11017;

        public const int WSA_QOS_EPROVSPECBUF = 11018;

        public const int WSA_QOS_EFILTERSTYLE = 11019;

        public const int WSA_QOS_EFILTERTYPE = 11020;

        public const int WSA_QOS_EFILTERCOUNT = 11021;

        public const int WSA_QOS_EOBJLENGTH = 11022;

        public const int WSA_QOS_EFLOWCOUNT = 11023;

        public const int WSA_QOS_EUNKOWNPSOBJ = 11024;

        public const int WSA_QOS_EPOLICYOBJ = 11025;

        public const int WSA_QOS_EFLOWDESC = 11026;

        public const int WSA_QOS_EPSFLOWSPEC = 11027;

        public const int WSA_QOS_EPSFILTERSPEC = 11028;

        public const int WSA_QOS_ESDMODEOBJ = 11029;

        public const int WSA_QOS_ESHAPERATEOBJ = 11030;

        public const int WSA_QOS_RESERVED_PETYPE = 11031;

        public static string GetErrorName(int result)
        {
            FieldInfo[] fields = typeof(WinError).GetFields();
            FieldInfo[] array = fields;
            foreach (FieldInfo fieldInfo in array)
            {
                if ((int)fieldInfo.GetValue(null) == result)
                {
                    return fieldInfo.Name;
                }
            }

            return string.Empty;
        }

        public static bool Succeeded(int result)
        {
            return result == 0;
        }

        public static bool Failed(int result)
        {
            return !Succeeded(result);
        }
    }
}