﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace gamelogic
{
    /// <summary>
    /// Класс проведения действий управляет свершениями различных игровых действий
    /// </summary>
    public class ProceedActions
    {
        /// <summary>
        /// Проводит атаку на базу
        /// </summary>
        /// <param name="attackerID"></param>
        /// <param name="victimID"></param>
        /// <returns></returns>
        public static string Battle(int attackerID, int victimID)
        {
            if (BaseManager.GetBaseUnitsCount(attackerID) == 0)
            {
                return "nounits";
            }
            var attacker = PlayerManager.GetPlayerByID(attackerID);
            var victim = PlayerManager.GetPlayerByID(victimID);
            var attackerUnits = BaseManager.GetBaseUnits(attackerID);
            var victimUnits = BaseManager.GetBaseUnits(victimID);

            Log("Event", $"Player {attacker.Playername} initiated a battle.");

            var attackerPower = 0;
            var victimPower = 0;
            foreach (var unit in attackerUnits)
            {
                attackerPower += unit.Count * ItemsVars.GetCost(unit.Type);
                Log("Battle", $"Player {attacker.Playername} has {unit.Count} units of {unit.Type} class.");
            }
            foreach (var unit in victimUnits)
            {
                Log("Battle", $"Player {victim.Playername} has {unit.Count} units of {unit.Type} class.");
                victimPower += unit.Count * ItemsVars.GetCost(unit.Type);
            }

            var result = "";
            if (attackerPower > victimPower)
            {
                var delta = (attackerPower > 0 && victimPower > 0 ? attackerPower / victimPower : 1);
                DoBattle(ref attacker, ref attackerUnits, ref victim, ref victimUnits, delta);
                result = "youwin";
            }
            else
            {
                var delta = (attackerPower > 0 && victimPower > 0 ? attackerPower / victimPower : 1);
                DoBattle(ref victim, ref victimUnits, ref attacker, ref attackerUnits, delta);
                result = "youlose";
            }
            Log("Event", $"Player {attacker.Playername} (units power is {attackerPower}) attacked {victim.Playername} " +
                $"(units power is {victimPower}) and {(attackerPower > victimPower ? "wins" : "loses")} that battle.");

            return result;
        }

        /// <summary>
        /// Реализует механизм обработки последствий битвы (занесение очков в стату, обнуление юнитов у проигравшего и вычитание части у победителя)
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="winnerUnits"></param>
        /// <param name="loser"></param>
        /// <param name="loserUnits"></param>
        /// <param name="delta"></param>
        private static void DoBattle(ref Player winner, ref IEnumerable<Unit> winnerUnits, ref Player loser, ref IEnumerable<Unit> loserUnits, double delta)
        {
            using (var db = new Entities())
            {
                winner.Wins++;

                foreach (var unit in winnerUnits)
                {
                    unit.Count = (int)(unit.Count / delta);
                }

                loser.Loses++;

                foreach (var unit in loserUnits)
                {
                    db.Entry(unit).State = EntityState.Deleted;
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        ///  Реализует механизм покупки
        /// </summary>
        /// <param name="baseid"></param>
        /// <param name="itemName"></param>
        /// <param name="level"></param>
        public static void DoBuyItem(int baseid, string itemName, int level = 0)
        {
            level++;
            using (var db = new Entities())
            {
                var resources = db.Resources.FirstOrDefault(o => o.Instance == "bas" + baseid.ToString());
                var cost = ItemsVars.GetCost(itemName);
                resources.Credits -= cost.Credits * level;
                resources.Energy -= cost.Energy * level;
                resources.Neutrino -= cost.Neutrino * level;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Реализует механизм логгирования
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        public async static void Log(string type, string text)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            using (var fstream = new FileStream($"{dir}/../csharpgame.log", FileMode.OpenOrCreate))
            {
                var array = System.Text.Encoding.Default.GetBytes($"{DateTime.Now.ToString("h:mm:ss tt")} [{type}] {text}{Environment.NewLine}");
                fstream.Seek(0, SeekOrigin.End);
                await fstream.WriteAsync(array, 0, array.Length);
            }
            // Вечная память нашему брату, с которым дебажили в консоли, имя ему - System.Diagnostics.Debug.WriteLine
        }
    }
}
