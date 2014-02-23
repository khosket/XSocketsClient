using System;

namespace XSocketsClient.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Connect to xsockets windows service (installed from https://chocolatey.org/packages/XSockets.Windows.Service/)
            var c = new XSocketClient("ws://127.0.0.1:4509/Generic", "http://*");

            c.Bind("foo", textArgs => Console.WriteLine("foo {0}", textArgs.data));
            c.OnOpen += (sender, eventArgs) => Console.WriteLine("OPEN");
            c.OnClose += (sender, eventArgs) => Console.WriteLine("CLOSED");
            c.Open();

            Console.ReadLine();
        }
    }
}
