using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class Information
    {
        public int port;
        public string address;
        public List<string> walls;

        public Information() { }

        public Information(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        }

        public Information(int port, string address)
        {
            this.port = port;
            this.address = address;
        }

        public override string ToString()
        {
            string walls = "";
            foreach (var wall in this.walls)
            {
                walls += wall + "\n";
            }
            return $"Address: {address}, port: {port}, walls: {walls}";
        }

    }
}
