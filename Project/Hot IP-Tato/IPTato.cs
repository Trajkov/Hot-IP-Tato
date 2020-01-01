using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot_IP_Tato.CS_Scripts
{
    [SerializableAttribute]
    public class IPTato : Object
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public IPTato(string name, int age)
        {
            Name = name;
            Age = age;

            /* * MVP Class
             *  
             * 
             * 
             */

            /* * Network variables
             * Origin - Which server/client created the potato (could be hostname or ipaddress)
             * Last Holder - The last non-server to hold the potato
             * Last Server(Could be same as origin) or last server to distribute the potato
             * 
             * * Potato Variables
             * DeathFlag - A flag that when set to true the potato explodes
             * Mutators - An array of arguments which change the way that the potato logic works.
             * 
             * * Antropomorphic Variables
             * PotatoSkin - which image the potato shows when it is passed to someone.
             * 
             */

        }
        new public string ToString()
        {
            string returnstring = "Name: " + Name + " Age: " + Age;
            return returnstring;
        }
    }
}
