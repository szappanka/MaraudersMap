using UnityEngine;

namespace Assets.Scripts.Models
{

    [System.Serializable]
    public class User
    {
        public int id;
        public string name;
        public string coordinates;

        public User() { }

        public User(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        }

        public User(int i, string n, string c)
        {
            id = i;
            name = n;
            coordinates = c;
        }

        public void UpdateGame(string jsonString)
        {
            var user = JsonUtility.FromJson<User>(jsonString);
            id = user.id;
            name = user.name;
            coordinates = user.coordinates;
        }

        override public string ToString()
        {
            return "id: " + id + " name: " + name + " coordinates: " + coordinates;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetCoordinates(string coordinates)
        {
            this.coordinates = coordinates;
        }

        public string GetCoordinates()
        {
            return coordinates;
        }
    }
}