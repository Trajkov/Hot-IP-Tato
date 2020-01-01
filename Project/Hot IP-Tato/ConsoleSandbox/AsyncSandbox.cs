using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSandbox
{
    class AsyncSandbox
    {
        class Egg
        {
            public string value = "";
        }
        public static async Task Test()
        {
            String cup = "Juice";
            Console.WriteLine("{0} is ready", cup);
            var eggTask = FryEggsAsync(2);
            var eggs = await eggTask;
            Console.WriteLine("{0} are ready", eggs);
        }

        static async Task<Egg> FryEggsAsync(int totalEggs)
        {
            // Fry x eggs
            // Do Stuff
            //Task<String> eggs = new Task<String>(egg, "alpha");
            try
            {
               var egg = await Task.Run(() =>
              {
                  Egg fryEgg = new Egg();
                  for (int x = 0; x < totalEggs; x++)
                  {
                      if (x > 0) { fryEgg.value += " "; }
                      fryEgg.value += "Egg";
                  }
                  return fryEgg;
              }
                );
                return egg;
            }
            catch (Exception e) {
                Console.WriteLine("Something went wrong with frying the eggs. {0}", e);
                return null;
            }
            
        }

    }
}
