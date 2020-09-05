using System;

namespace FootballLibrary
{
    public class Player
    {
        int id;
        public int Id 
        {
            get
            {
                return id;
            } 
            set
            {
                if (value < 1 || value > 999999)
                    throw new ArgumentOutOfRangeException();
                id = value;
            }
        }
        public string Url { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        int age;
        public int Age 
        {
            get
            {
                return age;
            }
            set
            {
                if (value < 1 || value > 100)
                    throw new ArgumentOutOfRangeException();
                age = value;
            }
        }
        public DateTime Birthday { get; set; }
        int height;
        public int Height 
        { 
            get
            {
                return height;
            }
            set
            {
                if (value < 1 || value > 250)
                    throw new ArgumentOutOfRangeException();
                height = value;
            }
        }
        int weight;
        public int Weight
        {
            get
            {
                return weight;
            }
            set
            {
                if (value < 1 || value > 103)
                    throw new ArgumentOutOfRangeException();
                weight = value;
            }
        }
        public string Nationality { get; set; }
        public string Club { get; set; }
        int overall;
        public int Overall 
        { 
            get
            {
                return overall;
            }
            set
            {
                if (value < 1 || value > 100)
                    throw new ArgumentOutOfRangeException();
                overall = value;
            }
        }
        int potential;
        public int Potential 
        { 
            get
            {
                return potential;
            }
            set
            {
                if (value < 1 || value>100)
                    throw new ArgumentOutOfRangeException();
                potential = value;
            }
        }
        public Player() { }
        public Player(int id, string u, string s, string l, int a, DateTime b, int h, int w, string n, string c, int o, int p)
        {
            Id = id;
            Url = u;
            ShortName = s;
            LongName = l;
            Age = a;
            Birthday = b;
            Height = h;
            Weight = w;
            Nationality = n;
            Club = c;
            Overall = o;
            Potential = p;
        }

        public override string ToString()
        {
            return $"{ShortName} Id: {Id}";
        }

    }
}
