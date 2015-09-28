using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrawlerResultHandler.Implementation;
using CrawlerResultHandler.Interfaces;

namespace CrawlerResultHandler
{
    class Program
    {
        static void Main(string[] args)
        {

            int sum = 0;
            var tasks = new List<Task<int>>();
            using (FileStream stream = new FileStream("result_rus.txt", FileMode.Create))
            {

                //tasks.Add(new TengriNewsHandler().HandleAsync(@"C:\GIT\CrawlerResult\kaz.tengrinews.kz", stream));
                //tasks.Add(new NurKzHandler().HandleAsync(@"C:\GIT\CrawlerResult\www.nur.kz\kk", stream));
                tasks.Add(new XmlHandler().HandleAsync(@"C:\GIT\CrawlerResult\rus_ksu",stream));
                //tasks.Add(new TatInformhandler().HandleAsync(@"C:\GIT\CrawlerResult\tat.tatar-inform.ru", stream));
                //tasks.Add(new TNVhandler().HandleAsync(@"C:\GIT\CrawlerResult\tnv.ru\tat", stream));
                Task.WaitAll(tasks.ToArray());
                
            }
           

            Console.WriteLine(" All handlers finished. Files count: {0}",tasks.Sum(x => x.Result));
            Console.Read();

        }
    }
}
