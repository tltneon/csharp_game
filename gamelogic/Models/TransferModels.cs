﻿namespace gamelogic.Models
{
    public class WithToken
    {
        public string token { get; set; }
    }
    public class AuthData
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class StatsData
    {
        public string username { get; set; }
        public string wins { get; set; }
        public string loses { get; set; }
        public string basename { get; set; }
        public string baselevel { get; set; }
    }
    public class BaseAction : WithToken
    {
        public int baseid { get; set; }
        public string action { get; set; }
        public string result { get; set; }
    }
    public class SquadAction : WithToken
    {
        public string key { get; set; }
        public string action { get; set; }
        public int to { get; set; }
    }
}