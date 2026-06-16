using OrderAccumulator.Services;

Console.WriteLine("Iniciando OrderAccumulator (Acceptor FIX 4.4)...");

var service = new FixAcceptorService("Config/Acceptor.cfg");
service.Start();

Console.WriteLine("Pressione ENTER para encerrar.");
Console.ReadLine();

service.Stop();
