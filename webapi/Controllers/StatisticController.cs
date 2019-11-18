﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace webapi.Controllers
{
    /// <summary>
    /// Контроллер, получающий данные о статистике
    /// </summary>
    public class StatisticController : ApiController
    {
        /// <summary>
        /// Возвращает данные статистики
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<wcfservice.StatEntity>> GetPlayerList()
        {
            var client = new Service1Client();
            var entities = await client.GetPlayerListAsync();
            client.Close();
            return entities;
        }

        /// <summary>
        /// Возвращает данные статистики
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<wcfservice.StatEntity>> GetUnitsList(wcfservice.BaseEntity q)
        {
            var client = new Service1Client();
            var entities = await client.GetPlayerListAsync();
            client.Close();
            return entities;
        }
    }
}