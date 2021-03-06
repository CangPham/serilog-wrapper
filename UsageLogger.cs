﻿using Logging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Logging
{
    public class UsageLogger : SeriLogger
    {
        private readonly IHttpContextAccessor contextAccessor;

        protected override string TableName => configuration[$"{configTable}:Usage"];

        public UsageLogger(IConfiguration configuration, IHttpContextAccessor contextAccessor)
            : base(configuration)
        {
            this.contextAccessor = contextAccessor;
        }

        public void Log(string activityName, Dictionary<string, object> additionalInfo)
        {
            if (!Convert.ToBoolean(configuration[$"{configModuleStatus}:Usage"]))
            {
                return;
            }
            
            var logDetail = new UsageLogDetail();
            logDetail.PopulateModel(activityName, contextAccessor.HttpContext, configuration, additionalInfo, false);

            logger.Information("{Timestamp}{Message}{Location}{Product}" +
                    "{Hostname}" +
                    "{UserId}{UserName}{SessionId}{CorrelationId}{AdditionalInfo}",
                    logDetail.Timestamp, logDetail.Message,
                    logDetail.Location,
                    logDetail.Product,
                    logDetail.Hostname, logDetail.UserId,
                    logDetail.UserName, logDetail.SessionId,
                    logDetail.CorrelationId,
                    logDetail.AdditionalInfoDictionary.JsonSerialize());
        }
    }
}
