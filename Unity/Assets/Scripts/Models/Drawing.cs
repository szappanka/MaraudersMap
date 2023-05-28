using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class Drawing
    {
        public List<Vector3> coordinates;

        public Drawing()
        {
            coordinates = new List<Vector3>();
        }

        public Drawing(List<Vector3> coordinates)
        {
            this.coordinates = coordinates;
        }

        public Drawing(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        }

        public override string ToString()
        {
            string jsonString = "{\"coordinates\":[";
            for (int i = 0; i < coordinates.Count; i++)
            {
                jsonString += Vector3ToString(coordinates[i]);
                if (i < coordinates.Count - 1)
                {
                    jsonString += ",";
                }
            }
            jsonString += "]}";

            return jsonString;
        }

        private string Vector3ToString(Vector3 v)
        {
            return "{\"x\":" + v.x.ToString("0.####").Replace(",", ".") + ",\"y\":" + v.y.ToString("0.####").Replace(",", ".") + ",\"z\":" + v.z.ToString("0.####").Replace(",", ".") + "}";
        }
    }
}
