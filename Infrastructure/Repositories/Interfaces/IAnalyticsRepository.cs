using Infrastructure.DTOs.Analytics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAnalyticsRepository
    {
        public Task<ActionResult<AdminAnalyticsDto>> GetAdminAnalytics();
        public Task<ActionResult<ExaminerAnalyticsDto>> GetExaminerAnalytics(int examinerId);
    }
}
