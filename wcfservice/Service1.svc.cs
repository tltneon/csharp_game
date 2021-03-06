﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using gamelogic;

namespace wcfservice
{
    /// <summary>
    /// Класс для обращения к сервису WCF
    /// </summary>
    public class Service1 : IService1
    {
        // >> auth-n-player section
        /// <summary>
        /// Управляет авторизацией и регистрацией пользователей
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ReturnAuthData SendAuthData(AuthData data)
        {
            var result = new ReturnAuthData { success = false };
            if (data == null)
            {
                result.message = "nodatareceived";
                return result;
            }
            if (data.username == null || data.password == null || data.username.Length < 3 || 
                data.password.Length < 3 || data.username.Length > 20 || data.password.Length > 20)
            {
                result.message = "wrongdatareceived";
                return result;
            }
            const string pattern = @"[^a-zA-ZА-Яа-я0-9!№%*@#$^]";
            if (Regex.IsMatch(data.username, pattern))
            {
                result.message = "wrongdatareceived";
                return result;
            }
            try
            {
                return Tools.SmartMapper<gamelogic.Models.ReturnAuthData, ReturnAuthData>(
                    AccountManager.AuthClient(
                        Tools.SmartMapper<AuthData, gamelogic.Models.AuthData>(data)
                        )
                    );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Метод: {ex.TargetSite}");
                System.Diagnostics.Debug.WriteLine($"Трассировка стека: {ex.StackTrace}");

                ProceedActions.Log("Exception", $"Исключение: {ex.Message}, функция Service1.SendAuthData");
                result.message = ex.Message;
                return result;
            }
        }

        public string SetAccountPassword(AuthData obj)
        {
            var result = Tools.CheckAuthedInput(obj);
            if (result != "passed")
            {
                return result;
            }
            var user = AccountManager.GetAccountByToken(obj.token);
            obj.username = obj.username.ToLower();
            if (user.Username == obj.username)
            {
                return AccountManager.ChangePassword(user.Username, obj.password);
            }
            return "wrongdata";
        }

        /// <summary>
        /// Возвращает список всех игроков
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PlayerData> GetPlayerList() => 
            Tools.EnumSmartMapper<gamelogic.Player, PlayerData>(PlayerManager.GetPlayerList());

        /// <summary>
        /// Возвращает статистику всех игроков
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StatsData> GetStats() =>
            Tools.EnumSmartMapper<gamelogic.Models.StatsData, StatsData>(PlayerManager.GetStats());

        // >> base section
        /// <summary>
        /// возвращает список всех баз
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseEntity> GetBaseList() => 
            Tools.EnumSmartMapper<gamelogic.Base, BaseEntity>(BaseManager.GetBaseList());

        /// <summary>
        /// Реализует управление базой
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string BaseAction(BaseAction obj)
        {
            var result = Tools.CheckAuthedInput(obj);
            if (result != "passed")
            {
                return result;
            }

            var mapobj = Tools.SmartMapper<BaseAction, gamelogic.Models.BaseAction>(obj);
            try
            {
                switch (mapobj.action) {
                    case "upgrade":
                        result = BaseManager.UpgradeBase(mapobj);
                        break;
                    case "repair":
                        result = BaseManager.RepairBase(mapobj);
                        break;
                    case "build":
                        result = BaseManager.BuildStructure(mapobj);
                        break;
                    case "upgradestructure":
                        result = BaseManager.UpgradeStructure(mapobj);
                        break;
                    case "makeunit":
                        result = BaseManager.MakeUnit(mapobj);
                        break;
                    default:
                        result = "wrongaction";
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Метод: {ex.TargetSite}");
                System.Diagnostics.Debug.WriteLine($"Трассировка стека: {ex.StackTrace}");
                result = ex.Message;
                ProceedActions.Log("Exception", $"Исключение: {ex.Message}, функция BaseAction");
            }
            return result;
        }

        /// <summary>
        /// Возвращает данные о базе
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public BaseEntity GetBaseInfo(BaseAction obj)
        {
            if (Tools.CheckAuthedInput(obj) != "passed")
            {
                return null;
            }

            var acc = AccountManager.GetAccountByToken(obj.token);

            var curbase = BaseManager.GetBaseInfo(acc);

            var result = Tools.SmartMapper<gamelogic.Base, BaseEntity>(curbase);

            result.Structures = Tools.EnumSmartMapper<gamelogic.Structure, StructureEntity>(BaseManager.GetBaseStructures(curbase.BaseID)); 
            result.Resources = Tools.SmartMapper<gamelogic.Resource, ResourcesData>(BaseManager.GetBaseResources(curbase.BaseID));
            result.Units = Tools.EnumSmartMapper<gamelogic.Unit, UnitsData>(BaseManager.GetBaseUnits(curbase.BaseID));

            return result;
        }

        /// <summary>
        /// Возвращает список всех строений на базе
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IEnumerable<StructureEntity> GetBaseStructures(BaseAction obj)
        {
            var result = Tools.CheckAuthedInput(obj);
            if (result != "passed")
            {
                return null;
            }

            return Tools.EnumSmartMapper<gamelogic.Structure, StructureEntity>(BaseManager.GetBaseStructures(obj.baseid));
        }

        /// <summary>
        /// Возвращает список всех отрядов на базе
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IEnumerable<UnitsData> GetBaseUnitsList(BaseAction obj)
        {
            var result = Tools.CheckAuthedInput(obj);
            if (result != "passed")
            {
                return null;
            }

            var acc = AccountManager.GetAccountByToken(obj.token);

            var curbase = BaseManager.GetBaseInfo(acc);

            return Tools.EnumSmartMapper<gamelogic.Unit, UnitsData>(BaseManager.GetBaseUnits(curbase.BaseID));
        }

        // >> squad section
        /// <summary>
        /// Возвращает список всех отрядов
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IEnumerable<SquadEntity> GetSquads(SquadAction obj)
        {
            var result = Tools.CheckAuthedInput(obj);
            if (result != "passed")
            {
                return null;
            }

            return Tools.EnumSmartMapper<gamelogic.Squad, SquadEntity>(SquadManager.GetSquads());
        }

        /// <summary>
        /// Реализует управление отрядами
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string SquadAction(SquadAction obj)
        {
            var result = Tools.CheckAuthedInput(obj);
            if (result != "passed")
            {
                return result;
            }

            try
            {
                var mapobj = Tools.SmartMapper<SquadAction, gamelogic.Models.SquadAction>(obj);
                switch (obj.action)
                {
                    case "attack":
                        result = SquadManager.SendAttackOrder(mapobj);
                        break;
                    case "return":
                        result = SquadManager.SendReturnOrder(mapobj);
                        break;
                    default:
                        result = "wrongaction";
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Метод: {ex.TargetSite}");
                System.Diagnostics.Debug.WriteLine($"Трассировка стека: {ex.StackTrace}");

                ProceedActions.Log("Exception", $"Исключение: {ex.Message}, функция SquadAction");
                result = "err";
            }
            return result;
        }

        /// <summary>
        ///  Это очень плохая реализация игрового лупа, очень плохая и ничем не защищена
        /// </summary>
        public void DbStatus(string msg)
        {
            if(msg == "e23je93jeyfg@_EO<Vdp-fo4-")
                BaseManager.BaseGatherResources();
        }
    }
}