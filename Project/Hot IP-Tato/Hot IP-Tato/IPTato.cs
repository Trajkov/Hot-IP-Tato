using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace IP_Tato
{
    [Serializable]
    public class IP_Tato
    {
        // Flavors of IPTato

        // TODO: Add in flag parameter constructor.
        // TODO: Add in a way to have a count of passes persist.
        // TODO: Actually use the targetClient Variable.

        // This is a nickname for the potato
        // This will also be used when costumes are added.
        public string Name { get; }
        // The state of the potato - alive, dead, exploded, etc.
        public bool Exploded { get; set; }
        // How many passes are left before the potato explodes.
        public int totalPasses { get; }
        public int passes { get; set; }
        // The target client which the potato will be sent to.
        public HelloPacket targetClient { get; set; }
        // The previous client which held the tater.
        public HelloPacket lastClient { get; set; }

        // The server which will moderate the game.
        // * Potatoes will be sent to the server to be passed around.
        public string server { get; }





        public IP_Tato()
        {
            this.Name = "Potato";
            this.totalPasses = 5;
            this.passes = 0;
        }
        public IP_Tato(string name)
        {
            this.Name = name;
        }
        // public IP_Tato(Byte[] serializedTater)
        // {
        // This constructor will eventually utilize the deserialize method
        //  and build a tater from the byte array
        // I don't know how to have it create itself without creating one,
        //  It might just create a tater and then assign it all of the things which
        //   The byte array contains. (but it would be nice if I could just deserialize here.
        // }

        // This constructor is for the server.
        public IP_Tato(string name, int totalPasses = 5)
        {
            this.Name = name;
            this.totalPasses = totalPasses;
            this.passes = 0;
            var flagList = new List<KeyValuePair<string, bool>>()
            {

            };
        }

        // This method checks a bunch of flags which can be applied to the object.
        public void CheckFlags()
        {
            // Return flags which are true
        }
        // This will do the things which the potato should do when it expires.
        public void Explode()
        {
            this.Exploded = true;
            Console.WriteLine(".\n.\n.\nKABOOOOOOOOOOOOOOOOOM!!!!!!\nIP_Tato {0} has exploded.", this.Name);
        }
        // Passes the potato to the origin 
        public void PassToServer()
        {

        }
        // Passes the potato to another member of the group
        public void PassOnInGroup()
        {

        }
        public void TestMethods()
        {
            Console.WriteLine("-- Testing Methods within IP_Tato {0} --", this.Name);

        }
        public override string ToString()
        {
            string toString = null;
            toString = base.ToString();
            foreach (var x in this.GetType().GetProperties())
            {
                if (x.GetValue(this) != null)
                {
                    // Add to ToString
                    toString += "\n " + x.Name + " "
                        + x.GetValue(this);
                }
            }
            return toString;
        }
        // **Manipulators for Objects**
        // Process for the process:
        // * 
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
}
