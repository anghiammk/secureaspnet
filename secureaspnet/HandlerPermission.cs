using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace secureaspnet
{
    public class HandlerPermission
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);


        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static SafeTokenHandle GetHandleUserLogon(
            string domainName, 
            string userName, 
            string password)
        {
            SafeTokenHandle safeTokenHandle = null;
           
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;

            bool returnValue = LogonUser(userName, domainName, password,
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeTokenHandle);

            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
            }
            return safeTokenHandle;
        }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}
