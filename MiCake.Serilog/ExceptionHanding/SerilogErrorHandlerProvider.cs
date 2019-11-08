﻿using MiCake.Core;
using MiCake.Core.Abstractions.ExceptionHandling;
using MiCake.Core.Abstractions.Log;
using MiCake.Core.ExceptionHandling;
using System;

namespace MiCake.Serilog.ExceptionHanding
{
    /// <summary>
    /// log error info with serilog
    /// </summary>
    public class SerilogErrorHandlerProvider : ILogErrorHandlerProvider
    {
        public Action<MiCakeErrorInfo> GetErrorHandler()
        {
            return serilogErrorHandler;

            void serilogErrorHandler(MiCakeErrorInfo miCakeError)
            {

            }
        }
    }
}
