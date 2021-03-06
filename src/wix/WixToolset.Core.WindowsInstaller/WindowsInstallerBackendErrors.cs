// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Core.WindowsInstaller
{
    using WixToolset.Data;

    internal static class WindowsInstallerBackendErrors
    {
        //public static Message ReplaceThisWithTheFirstError(SourceLineNumber sourceLineNumbers)
        //{
        //    return Message(sourceLineNumbers, Ids.ReplaceThisWithTheFirstError, "format string", arg1, arg2);
        //}

        private static Message Message(SourceLineNumber sourceLineNumber, Ids id, string format, params object[] args)
        {
            return new Message(sourceLineNumber, MessageLevel.Error, (int)id, format, args);
        }

        public enum Ids
        {
            // ReplaceThisWithTheFirstError = 7500,
        } // last available is 7999. 8000 is BurnBackendErrors.
    }
}
